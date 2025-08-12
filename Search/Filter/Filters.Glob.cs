using ff.State;

namespace ff.Search.Filter;

internal partial class Filters
{
    [Info("glob", "search files or dirs of name with optional glob string(?*)")]
    public static IEnumerable<FileSystemState> Glob(
        this IEnumerable<FileSystemState> items,
        string? pattern,
        bool matchFullPath = false)
    {
        if(pattern == null) return items;

        var regexPattern = WildcardToRegex(pattern);

        return items.Regex(regexPattern, matchFullPath);
    }

    private static string WildcardToRegex(string pattern)
    {
        // Escape special regex characters
        var escaped = System.Text.RegularExpressions.Regex.Escape(pattern);

        // Replace escaped wildcards with regex equivalents
        escaped = escaped.Replace(@"\*", ".*"); // * matches any sequence
        escaped = escaped.Replace(@"\?", "."); // ? matches any single character

        // Anchor the pattern to match the entire string
        return $"^{escaped}$";
    }
}