using ff.State;

namespace ff.Search.Filter;

internal static partial class Filters
{
    [Info("dir", "search directories of name with optional glob string(?*)")]
    public static IEnumerable<FileSystemState> Directory(this IEnumerable<FileSystemState> items, string? globStr = null) => items
            .Where(it => it.IsDir)
            .Glob(globStr);

    [Info("file", "search files of name with optional glob string(?*)")]
    public static IEnumerable<FileSystemState> File(this IEnumerable<FileSystemState> items, string? globStr = null) => items
        .Where(it => !it.IsDir)
        .Glob(globStr);

    [Info("type", "search files of name with ext strings, i.e. png|jpg")]
    public static IEnumerable<FileSystemState> Type(IEnumerable<FileSystemState> items, IEnumerable<string> allowedTypes) => items
        .Where(
        it =>
        {
            if (!it.IsDir && allowedTypes.Any(ty => it.Name.EndsWith($".{ty}")))
            {
                return true;
            }
        });
}
