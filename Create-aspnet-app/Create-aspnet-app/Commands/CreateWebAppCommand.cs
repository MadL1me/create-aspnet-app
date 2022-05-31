using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Sharprompt;

namespace Create_aspnet_app.Commands;

[Command]
public class CreateWebAppCommand : ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        var prompt = Prompt.PromptRealisation;
        var name = prompt.Input<string>("What's your name?");
        console.Output.WriteLine($"Hello, {name}!");

        /*// Password input
        var secret = Prompt.Password("Type new password", validators: new[] { Validators.Required(), Validators.MinLength(8) });
        console.Output.WriteLine("Password OK");

        // Confirmation
        var answer = Prompt.Confirm("Are you ready?", defaultValue: true);
        console.Output.WriteLine($"Your answer is {answer}");*/

        return default;
    }
}