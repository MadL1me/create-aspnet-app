using System.Text;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CliWrap;
using CliWrap.Buffered;
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
    public const string DotnetCommandName = "dotnet";
    public const string DotnetNewCommandName = "new";
    public const string TemplateShortName = "ez-webapp-main";
    
    private readonly BuilderData _builderData = new () { CliArguments = { DotnetNewCommandName, TemplateShortName } };
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var prompt = Prompt.PromptRealisation;
        await console.Output.WriteLineAsync("Welcome To Create-aspnet-app!");

        await ConfigureName(prompt, _builderData);
        await RunDotnetNewCommand(console);

        await console.Output.WriteLineAsync($"Your project {_builderData.ProjectName} was successfully created!");
    }

    private ValueTask ConfigureName(IPrompt prompt, BuilderData data)
    {
        const string optionName = "-o";
        var projectName = prompt.Input<string>("Choose name for your project");
        data.CliArguments.Add(optionName);
        data.CliArguments.Add("aboba");
        return default;
    }

    private async ValueTask RunDotnetNewCommand(IConsole console)
    {
        var commandResult = await Cli.Wrap($"{DotnetCommandName}")
            .WithArguments(_builderData.CliArguments)
            .WithWorkingDirectory(Directory.GetCurrentDirectory() + "/SomeDirectory")
            .ExecuteBufferedAsync();
        
        await console.Output.WriteAsync(commandResult.StandardOutput);
    }
}