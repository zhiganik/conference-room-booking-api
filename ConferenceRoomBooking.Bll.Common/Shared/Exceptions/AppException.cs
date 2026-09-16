using System.Collections;
using System.Text.RegularExpressions;

namespace ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

/// <summary>
/// Base type for exceptions that represent an expected business-rule failure rather than a bug.
/// The Web layer's global exception handler maps every subclass to a specific HTTP status, surfaces
/// <see cref="Exception.Message"/> to the client, and logs a Warning using <see cref="LogTemplate"/>
/// and <see cref="LogArgs"/> - the same template/args pair that built the message, so it's authored
/// once and reused for both the response and the log.
/// </summary>
/// <param name="template">
/// Message template with named {Placeholder} tokens, e.g. "Room {RoomId} not found." Client-safe:
/// don't put anything here that shouldn't be echoed back in the HTTP response.
/// </param>
/// <param name="args">Values substituted into <paramref name="template"/>'s placeholders, in order.</param>
public abstract class AppException(string template, params object?[] args) : Exception(AppException.Format(template, args))
{
    public string LogTemplate { get; } = template;

    public object?[] LogArgs { get; } = args;

    private static readonly Regex PlaceholderPattern = new(@"\{[^{}]+\}", RegexOptions.Compiled);

    private static string Format(string template, object?[] args)
    {
        var index = 0;
        return PlaceholderPattern.Replace(template, _ => index < args.Length ? FormatValue(args[index++]) : "?");
    }

    private static string FormatValue(object? value) => value switch
    {
        null => "null",
        string text => text,
        IEnumerable sequence => string.Join(", ", sequence.Cast<object?>().Select(FormatValue)),
        _ => value.ToString() ?? "null"
    };
}
