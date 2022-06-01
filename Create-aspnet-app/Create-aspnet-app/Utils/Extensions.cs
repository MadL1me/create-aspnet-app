using CliFx.Infrastructure;

namespace Create_aspnet_app.Utils;

public static class Extensions
{
    public static string WrapInQuotes(this string str) => $"\"{str}\"";

    public static async Task WriteLineAsyncWithColors(this ConsoleWriter writer, string value, 
        ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        var originalBackgroundColor = writer.Console.BackgroundColor;
        var originalForegroundColor = writer.Console.ForegroundColor;

        writer.Console.BackgroundColor = backgroundColor;
        writer.Console.ForegroundColor = foregroundColor;
        
        await writer.Console.Output.WriteLineAsync(value);

        writer.Console.BackgroundColor = originalBackgroundColor;
        writer.Console.ForegroundColor = originalForegroundColor;
    }
}