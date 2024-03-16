namespace TwitchWidgetsApp.Library;

public static class StringExtensions
{
    public static string EnsureEndsWith(this string str, string endsWith) => str.EndsWith(endsWith) ? str : $"{str}{endsWith}";
}