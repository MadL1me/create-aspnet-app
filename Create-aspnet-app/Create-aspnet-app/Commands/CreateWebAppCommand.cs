using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CliWrap;
using CliWrap.Buffered;
using CreateAspnetApp.Utils;
using Sharprompt;
using Sharprompt.Prompts;

namespace CreateAspnetApp.Commands;

[Command]
public class CreateWebAppCommand : ICommand
{
    private const string AppVersion = "0.2.0";
    private const string AspAwesomeTemplateRequiredVersion = "0.2.0";
    
    private const string DotnetCommandName = "dotnet";
    private const string DotnetNewCommandName = "new";
    
    private const string TemplateShortName = "asp-awesome-main-cli";
    private const string TemplatePackageId = "Asp.AwesomeTemplates.Main.Cli";
    
    private readonly BuilderData _builderData = new ();
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var prompt = Prompt.PromptRealisation;

        await console.Output.WriteLineAsyncWithColors($"Welcome To Create-aspnet-app! Version: {AppVersion}", ConsoleColor.Cyan);

        await HandleTemplateVersions(console); 

        await ConfigureName(prompt, _builderData);
        await ConfigureSwagger(prompt, _builderData);
        await ConfiguteSpaFramework(prompt, _builderData);
        
        await RunDotnetNewCommand(console);

        await console.Output.WriteLineAsync($"Your project {_builderData.ProjectName.Value} was successfully created!");
    }
    
    private async Task HandleTemplateVersions(IConsole console)
    {
        if (await CheckIfTemplateExists(console))
        {
            var installedVersion = await GetTemplateVersion(console);
            await console.Output.WriteLineAsync($@"Found installed template with version: {installedVersion}");
            
            if (installedVersion != AspAwesomeTemplateRequiredVersion)
            {
                await console.Output.WriteLineAsync(
                    $"Installed wrong template version, installing required version: {AspAwesomeTemplateRequiredVersion}");
                await InstallRequiredTemplateVersion(console);
            }
            
            return;
        }

        await console.Output.WriteLineAsync($"Template nuget package is not found, installing required version: {AspAwesomeTemplateRequiredVersion}");
        await InstallRequiredTemplateVersion(console);
    }
    
    private async Task InstallRequiredTemplateVersion(IConsole console)
    {
        const string install = "--install";
        
        await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments($"{DotnetNewCommandName} {install} {TemplatePackageId}::{AspAwesomeTemplateRequiredVersion}")
            .ExecuteBufferedAsync();

        await console.Output.WriteLineAsync($"Successfully installed: {TemplatePackageId}::{AspAwesomeTemplateRequiredVersion}");
    }
    
    private ValueTask ConfigureName(IPrompt prompt, BuilderData data)
    {
        var projectName = prompt.Input<string>(
            "Choose name for your project", 
            "MyAwesomeProject", 
            "Type your project name");
        
        data.ProjectName = data.ProjectName.WithValue(projectName);
        return default;
    }

    private ValueTask ConfigureSwagger(IPrompt prompt, BuilderData data)
    {
        var useSwagger = prompt.Select(
            "Use swagger for back-end API?", 
            new [] { YesNoAnswer.Yes, YesNoAnswer.No }, 
            defaultValue: YesNoAnswer.Yes);
        
        data.UseSwagger = data.UseSwagger.WithValue(useSwagger == YesNoAnswer.Yes);
        return default;
    }

    private ValueTask ConfiguteSpaFramework(IPrompt prompt, BuilderData data)
    {
        var spaType = prompt.Select(
            "Select your front-end SPA framework, or NONE option if you don't want to use any",
            new [] {SpaFrameworkType.React, SpaFrameworkType.None }, 
            defaultValue: SpaFrameworkType.None);

        data.SpaType = data.SpaType.WithValue(spaType);
        return default;
    }
    
    private async Task<string> GetTemplateVersion(IConsole console)
    {
        const string helpArg = "-u";
        
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(new [] {DotnetNewCommandName, helpArg})
            .ExecuteBufferedAsync();
        
        var lines = commandResult.StandardOutput.Split("\n");
        var nameNumber = lines.IndexOfOrNull(s => s.Contains(TemplatePackageId));

        if (nameNumber is null)
            throw new KeyNotFoundException("Found value is not found");
        
        var version = lines[nameNumber.Value + 1]
            .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries)[1];

        return version;
    }
    
    private async Task<bool> CheckIfTemplateExists(IConsole console)
    {
        const string helpArg = "-h";
        
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(new [] {DotnetNewCommandName, TemplateShortName, helpArg})
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();
        
        return commandResult.ExitCode is 0;
    }

    private async Task RunDotnetNewCommand(IConsole console)
    {
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(new [] { DotnetNewCommandName, TemplateShortName, _builderData.ToCliArguments().ToStringValues() }, false)
            .ExecuteBufferedAsync();
        
        await console.Output.WriteAsync(commandResult.StandardOutput);
    }
    
    private class BuilderData
    {
        public CliOption<string> ProjectName { get; set; } = ("-o", "MyAwesomeProject");
       
        public CliOption<SpaFrameworkType> SpaType { get; set; } = ("-U", SpaFrameworkType.None);

        public CliOption<bool> UseSwagger { get; set; } = ("-E", true);

        public string[] ToCliArguments()
        {
            var cliArguments = new List<string>
            {
                ProjectName.ToCliParamsString(),
                SpaType.ToCliParamsString()
            };

            return cliArguments.ToArray();
        }
    }
    
    private record struct CliOption<TValue> 
    {
        public string OptionName { get; set; }
        
        public TValue Value { get; set; }
        
        public string ToCliParamsString() => $"{OptionName} {Value}";

        public CliOption<TValue> WithValue(TValue value) => this with { Value = value };
        
        public static implicit operator TValue(CliOption<TValue> option) => option.Value;

        public static implicit operator CliOption<TValue>(TValue value) => new () { Value = value };
        
        public static implicit operator CliOption<TValue>((string optionName, TValue value) valueTuple) =>
            new () { Value = valueTuple.value, OptionName = valueTuple.optionName};
    }

    private enum SpaFrameworkType
    {
        React,
        None
    }

    private enum YesNoAnswer
    {
        Yes,
        No
    }
}