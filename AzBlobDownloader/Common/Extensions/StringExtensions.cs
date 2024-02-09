using System.Text.RegularExpressions;

namespace AzBlobDownloader.Common.Extensions;

public static class StringExtensions
{
    public static bool IsMatch(this string name, string? pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return true;

        if (pattern.Contains('*'))
            pattern = pattern.Replace("*", ".*");

        return Regex.IsMatch(name, pattern);
    }
}