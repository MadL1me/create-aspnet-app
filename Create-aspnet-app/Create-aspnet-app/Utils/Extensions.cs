using CliFx.Infrastructure;

namespace Create_aspnet_app.Utils;

public static class Extensions
{
    public static string WrapInQuotes(this string str) => $"\"{str}\"";

    /// <summary>
    /// Writes console with color of background and text
    /// </summary>
    /// <param name="writer">source</param>
    /// <param name="value">what to write to console</param>
    /// <param name="foregroundColor">text color</param>
    /// <param name="backgroundColor">background color</param>
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

    /// <summary>
    /// Returns index of first successful predicate or null if not found any
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static int? IndexOfOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var index = 0;

        foreach (var element in source)
        {
            if (predicate(element))
                return index;

            index++;
        }

        return null;
    }
}