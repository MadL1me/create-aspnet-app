﻿using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CliWrap;
using CliWrap.Buffered;
using Create_aspnet_app.Utils;
using Sharprompt;
using Sharprompt.Prompts;

namespace Create_aspnet_app.Commands;

[Command]
public class CreateWebAppCommand : ICommand
{
    private const string AppVersion = "0.0.2";
    private const string AspAwesomeTemplateRequiredVersion = "0.0.2";
    
    private const string DotnetCommandName = "dotnet";
    private const string DotnetNewCommandName = "new";
    
    private const string TemplateShortName = "asp-awesome-main-cli";
    private const string TemplatePackageId = "Asp.AwesomeTemplates.Main.Cli";
    
    private readonly BuilderData _builderData = new () { CliArguments = { DotnetNewCommandName, TemplateShortName } };
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var prompt = Prompt.PromptRealisation;

        await console.Output.WriteLineAsyncWithColors("Welcome To Create-aspnet-app!", ConsoleColor.Cyan);

        await HandleTemplateVersions(console); 

        await ConfigureName(prompt, _builderData);
        
        await RunDotnetNewCommand(console);

        await console.Output.WriteLineAsync($"Your project {_builderData.ProjectName} was successfully created!");
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
        
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments($"{DotnetNewCommandName} {install} {TemplatePackageId}::{AspAwesomeTemplateRequiredVersion}")
            .ExecuteBufferedAsync();

        await console.Output.WriteLineAsync($"Successfully installed: {TemplatePackageId}::{AspAwesomeTemplateRequiredVersion}");
    }
    
    private ValueTask ConfigureName(IPrompt prompt, BuilderData data)
    {
        const string optionName = "-o";
        var projectName = prompt.Input<string>("Choose name for your project");
        data.CliArguments.Add(optionName);
        data.CliArguments.Add(projectName);
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
            throw new KeyNotFoundException("Founded value is not found");
        
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
            .WithArguments(_builderData.CliArguments)
            .ExecuteBufferedAsync();
        
        await console.Output.WriteAsync(commandResult.StandardOutput);
    }
    
    private class BuilderData
    {
        public string ProjectName { get; init; }
    
        public readonly List<string> CliArguments = new();
    }
}