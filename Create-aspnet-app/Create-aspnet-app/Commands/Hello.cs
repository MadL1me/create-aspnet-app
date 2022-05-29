using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Create_aspnet_app.Commands;

[Command]
public class ConcatCommand : ICommand
{
    [CommandOption("left")]
    public string Left { get; init; } = "Hello";

    [CommandOption("right")]
    public string Right { get; init; } = "world";

    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.Write(Left);
        console.Output.Write(' ');
        console.Output.Write(Right);

        return default;
    }
}