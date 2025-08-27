namespace ff.Search.Filter;

internal static partial class Filters
{
    [Info("dir", "search directories of name with optional glob string(?*)")]
    public static IEnumerable<IItem> Container(this IEnumerable<IItem> items, string? globStr = null) => items
            .Where(it => !it.IsLeaf)
            .Glob(globStr);

    [Info("file", "search leafs(i.e. files) of name with optional glob string(?*)")]
    public static IEnumerable<IItem> Leaf(this IEnumerable<IItem> items, string? globStr = null) => items
        .Where(it => it.IsLeaf)
        .Glob(globStr);

    [Info("type", "search files of name with ext strings, i.e. png|jpg")]
    public static IEnumerable<IItem> Type(IEnumerable<IItem> items, IEnumerable<string> allowedTypes) => items
        .Where(
        it =>
        {
            if (it.IsLeaf && allowedTypes.Any(ty => it.Name.EndsWith($".{ty}")))
            {
                return true;
            }
            return false;
        });
}
