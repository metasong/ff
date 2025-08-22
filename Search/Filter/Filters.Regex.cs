using ff.State;
using System.Text.RegularExpressions;

namespace ff.Search.Filter;

internal partial class Filters
{
    [Info("regex", "search files or dirs of name with optional regex string")]
    public static IEnumerable<IItem> Regex(
        this IEnumerable<IItem> items,
        string? regex,
        bool matchFullPath = false,
        RegexOptions regexOptions = RegexOptions.IgnoreCase)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (string.IsNullOrEmpty(regex))
            return items;

        var compiledRegex = new Regex(regex, regexOptions);

        return items.Where(item =>
        {
            var target = matchFullPath ? item.FullName : item.Name;
            return compiledRegex.IsMatch(target);
        });
    }

}