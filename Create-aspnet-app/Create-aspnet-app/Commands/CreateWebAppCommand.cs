using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Exceptions;
using Create_aspnet_app.Utils;
using Sharprompt;
using Sharprompt.Prompts;

namespace Create_aspnet_app.Commands;

public class BuilderData
{
    public string ProjectName { get; init; }
    
    public readonly List<string> CliArguments = new();
}

[Command]
public class CreateWebAppCommand : ICommand
{
    public const string AppVersion = "0.0.2";
    public const string AspAwesomeTemplateRequiredVersion = "0.0.2";
    
    public const string DotnetCommandName = "dotnet";
    public const string DotnetNewCommandName = "new";
    public const string TemplateShortName = "asp-awesome-main-cli";
    public const string TemplatePackageId = "Asp.AwesomeTemplates.Main.Cli";
    
    private readonly BuilderData _builderData = new () { CliArguments = { DotnetNewCommandName, TemplateShortName } };
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var prompt = Prompt.PromptRealisation;

        await console.Output.WriteLineAsyncWithColors("Welcome To Create-aspnet-app!", ConsoleColor.DarkCyan);

        if (await CheckIfTemplateExists(console))
        { 
            if (await GetTemplateVersion(console) != AspAwesomeTemplateRequiredVersion)
                await InstallRequiredTemplateVersion(console);
        }
        else await InstallRequiredTemplateVersion(console);

        //await ConfigureName(prompt, _builderData);
        //await RunDotnetNewCommand(console);

        await console.Output.WriteLineAsync($"Your project {_builderData.ProjectName} was successfully created!");
    }

    private async ValueTask InstallRequiredTemplateVersion(IConsole console)
    {
        const string install = "--install";
        
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(new [] {DotnetNewCommandName, install, TemplatePackageId + "::" + AspAwesomeTemplateRequiredVersion})
            .ExecuteBufferedAsync();

        await console.Output.WriteAsync(commandResult.StandardOutput);
    }
    
    private ValueTask ConfigureName(IPrompt prompt, BuilderData data)
    {
        const string optionName = "-o";
        var projectName = prompt.Input<string>("Choose name for your project");
        data.CliArguments.Add(optionName);
        data.CliArguments.Add(projectName);
        return default;
    }
    
    private async ValueTask<string> GetTemplateVersion(IConsole console)
    {
        const string helpArg = "-u";
        
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(new [] {DotnetNewCommandName, helpArg})
            .ExecuteBufferedAsync();
        
        var lines = commandResult.StandardOutput.Split("\n");
        var nameNumber = lines.IndexOfOrNull(s => s.Contains(TemplatePackageId));

        if (nameNumber is null)
            throw new KeyNotFoundException("Founded value is not found");
        
        var version = lines[nameNumber.Value + 1]
            .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries)[1];

        return version;
    }
    
    private async ValueTask<bool> CheckIfTemplateExists(IConsole console)
    {
        const string helpArg = "-h";
        
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(new [] {DotnetNewCommandName, TemplateShortName, helpArg})
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();
        
        await console.Output.WriteAsync(commandResult.StandardOutput);
        await console.Output.WriteAsync(commandResult.StandardError);

        return commandResult.StandardError.Length != 0;
    }

    private async ValueTask RunDotnetNewCommand(IConsole console)
    {
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(_builderData.CliArguments)
            .ExecuteBufferedAsync();
        
        await console.Output.WriteAsync(commandResult.StandardOutput);
    }
}