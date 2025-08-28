using Terminal.Gui.FileServices;
using TerminalFileManager;

namespace ff.State.FileDataSystem;

internal class CurrentFolderTableSource(
    FileSystemState state,
    int sortColumn = 0,
    bool sortIsAsc = true)
    : ISortableTableSource
{
    FileSystemIconProvider IconProvider { get; set; } = new(){UseNerdIcons = true};
    public IDataSystem DataSystem => FileSystem.FileDataSystem.Instance;

    public object this[int row, int col] => GetColumnValue(col, (FileSystemItem)state.Children[row]);
    public int Rows => state.Children.Count();
    public int Columns => 3;
    public IItem GetItem(int row) => state.Children[row];

    public string[] ColumnNames =>
    [
        MaybeAddSortArrows (DI.Localization.GetString("FilenameColumnName"), 0),
        MaybeAddSortArrows (DI.Localization.GetString("SizeColumnName"), 1),
        MaybeAddSortArrows (DI.Localization.GetString("ModifiedColumnName"), 2),
    ];
    private object GetSortableObject(IItem item, int col)
    {
        var fileItem = (FileSystemItem)item;
        return col switch
        {
            0 => fileItem.Name,
            1 => fileItem.SizeReadable,
            2 => fileItem.LastWriteTime,
            _ => throw new ArgumentOutOfRangeException(nameof(col))
        };
    }

    public Task Sort(int column, bool asc)
    {
        sortColumn = column;
        sortIsAsc = asc;
        // This portion on top (folders)
        var forcedOrder = state.Children
            .OrderBy(f => f.IsLeaf ? 100 : -1);

        var ordered =
            asc
                ? forcedOrder.ThenBy(
                    f =>
                        GetSortableObject(f, sortColumn)
                )
                : forcedOrder.ThenByDescending(
                    f =>
                        GetSortableObject(f, sortColumn)
                );

        state.Children = ordered.ToArray();
        return Task.CompletedTask;
    }

    private object GetColumnValue(int col, FileSystemItem item)
    {
        switch (col)
        {
            case 0:
                var icon = IconProvider.GetIconWithOptionalSpace(item.FileSystemInfo);
                return (icon + item.Name).Trim();
            case 1:
                return item.SizeReadable;
            case 2:
                return item.LastWriteTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
            default:
                throw new ArgumentOutOfRangeException(nameof(col));
        }
    }

    private string MaybeAddSortArrows(string name, int idx)
    {
        if (idx == sortColumn)
        {
            return name + (sortIsAsc ? " (▲)" : " (▼)");
        }

        return name;
    }

}
