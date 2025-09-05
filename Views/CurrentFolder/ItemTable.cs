using ff.State.TableSource;

namespace ff.Views.CurrentFolder;

public record ShortcutConfig(string command, string condition = "");

public static class ExtTableView
{
    public static bool IsPureSelected(this TableView tableView, int row) =>
        tableView.MultiSelectedRegions.Any(r => r.Rectangle.Bottom > row && r.Rectangle.Top <= row);
}

public class ItemTable : TableView
{
    private int _currentSortColumn;
    private bool _currentSortIsAsc = true;

    private Scheme OldScheme;

    public ItemTable(bool canFocus = false)
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        MultiSelect = true;
        CanFocus = canFocus;
        //Enabled = canFocus;
        //BorderStyle = LineStyle.Dashed,
        FullRowSelect = true;
        // scroll bar can not be used.
        VerticalScrollBar.AutoShow = true;
        HorizontalScrollBar.AutoShow = true;
        Style.AlwaysShowHeaders = true;

        Style.ShowHorizontalHeaderOverline = false;
        Style.ShowHorizontalBottomline = false;
        Style.ShowVerticalCellLines = false;
        Style.ShowVerticalHeaderLines = false;
        RowOffset = 0;
        ColumnOffset = 0;
        EnsureValidScrollOffsets();

        KeyBindings.ReplaceCommands(Key.Home, Command.Start);//LeftStart
        KeyBindings.ReplaceCommands(Key.End, Command.End);//RightEnd

        //ColumnStyle nameStyle = Style.GetOrCreateColumnStyle(0);
        //nameStyle.MinWidth = 10;
        //nameStyle.ColorGetter = ColorGetter;

        //ColumnStyle sizeStyle = Style.GetOrCreateColumnStyle(1);
        //sizeStyle.MinWidth = 10;
        //sizeStyle.ColorGetter = ColorGetter;

        //ColumnStyle dateModifiedStyle = Style.GetOrCreateColumnStyle(2);
        //dateModifiedStyle.MinWidth = 30;
        //dateModifiedStyle.ColorGetter = ColorGetter;
        OldScheme = GetScheme();
        Style.RowColorGetter = args =>
        {
            var item = TableSource.GetChild(args.RowIndex);
            var selected = this.IsPureSelected(args.RowIndex); // current row is selected or active
            var active = this.SelectedRow == args.RowIndex; //current is active

            var stats = item.DataSystem.GetColor(item, OldScheme);
            if (selected)
            {
                if (active) // selected but also active
                {
                    stats = stats with { Focus = stats.Focus with { Style = TextStyle.Underline } };
                }
                else // selected but not active
                {
                    stats = stats with { Focus = stats.Normal with { Style = TextStyle.Underline } };
                }
            }

            return stats;
        };

    }
    // note: IsSelected(0, args.RowIndex) will return selected or active

    internal Func<Key, bool>? KeyDownHandler;
    private bool showHeader;
    private bool showSelectionBox;
    private IContainer? currentContainer;

    internal ISortableTableSource TableSource => (ISortableTableSource)Table;
    //private Dictionary<string, ShortcutConfig> ShortcutMaps = new Dictionary<string, ShortcutConfig>
    //{
    //    // key, command name, condition
    //    {"d", new("d", "d")}
    //};

    //private Dictionary<string, Func<Task>> Commands = new Dictionary<string, Func<Task>>
    //{
    //    // command name, action
    //    {"d", ()=>Task.CompletedTask}
    //};

    protected override bool OnKeyDown(Key key)
    { if (KeyDownHandler?.Invoke(key) ?? false)
        {
            return true;
        }

        return base.OnKeyDown(key);
    }

    protected override bool OnMouseClick(MouseEventArgs e)
    {
        var clickedCell = ScreenToCell(e.Position.X, e.Position.Y, out var clickedHeaderCol);
        if (e.Flags.HasFlag(MouseFlags.Button1Clicked)) // left click
        {
            if (clickedHeaderCol != null)
            {
                clickedHeaderCol = ShowSelectionBox ? clickedHeaderCol - 1 : clickedHeaderCol;
                if (clickedHeaderCol >= 0)
                {
                    _currentSortIsAsc = _currentSortColumn != clickedHeaderCol || !_currentSortIsAsc;
                    _currentSortColumn = clickedHeaderCol.Value;
                    ApplySort(_currentSortColumn, _currentSortIsAsc);
                    return true; // stop further process
                }
            }
        }

        if (e.Flags.HasFlag(MouseFlags.Button1DoubleClicked)) // left double click
        {

        }

        return base.OnMouseClick(e);
    }


    public void ShowData(IContainer container)
    {
        currentContainer = container;
        TableSource?.Dispose();
        var source = container.DataSystem.GetTableSource(container, _currentSortColumn, _currentSortIsAsc);
        if (ShowSelectionBox)
            source = new SelectableSortableTableSource(this, source);
        Table = source;

    }

    //public void SelectRowInToggleState()
    public void SelectAllInToggleableState(bool toggle = true)
    {
        if (!toggle)
        {
            base.SelectAll();
            return;
        }

        if (!MultiSelect || Table.Rows == 0)
        {
            return;
        }

        MultiSelectedRegions.Clear();

        // Create a single region over entire table, set the origin of the selection to the active cell so that a followup spread selection e.g. shift-right behaves properly
        MultiSelectedRegions.Push(
            new(
                new(SelectedColumn, SelectedRow),
                new(0, 0, Table.Columns, Table.Rows)
            ){IsToggled = true}
        );
        Update();
    }

    public void SetSelectionInToggleableState(int row, bool toggle = true)
    {
        if (!toggle)
        {
            base.SetSelection(0,row,false);
            return;
        }

        var newRect = new TableSelection(new(0, row), new(0, row, 1, 1) ){IsToggled = true};
        MultiSelectedRegions.Push(newRect);
    }

    public bool ShowSelectionBox
    {
        get => showSelectionBox;
        set
        {
            showSelectionBox = value;
            if (currentContainer != null) ShowData(currentContainer);
        }
    }

    internal void ApplySort(int sortColumn, bool isSortAsc)
    {
        TableSource.Sort(sortColumn, isSortAsc);
        //itemsListTable.RowOffset = 0;
        //itemsListTable.SelectedRow = 0;
        Update();
    }

    public bool ShowHeader
    {
        get => showHeader;
        set
        {
            showHeader = value;
            if (value)
            {
                Style.ShowHeaders = true;
                Style.ShowHorizontalHeaderUnderline = true;
            }
            else
            {
                Style.ShowHeaders = false;
                Style.ShowHorizontalHeaderUnderline = false;
            }
            SetNeedsDraw();
        }
    }

}