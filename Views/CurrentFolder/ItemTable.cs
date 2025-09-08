using ff.State.TableSource;
using Terminal.Gui.Views;

namespace ff.Views.CurrentFolder;

public record ShortcutConfig(string Command, string Condition = "");

public static class ExtTableView
{
    public static bool IsPureSelected(this TableView tableView, int row) =>
        tableView.MultiSelectedRegions.Any(r => r.Rectangle.Bottom > row && r.Rectangle.Top <= row);
}

public class ItemTable : TableView, IPreviewer
{
    private int currentSortColumn;
    private bool currentSortIsAsc = true;

    private readonly Scheme oldScheme;
    private bool showHeader;
    private bool showSelectionBox;
    private IContainer? currentContainer;
    private PopoverMenu contextMenu = null!;

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
        oldScheme = GetScheme();

        Style.RowColorGetter = RowColorGetter;

        CreateContextMenu();

    }

    private Scheme RowColorGetter(RowColorGetterArgs args)
    {
        var item = TableSource.GetChild(args.RowIndex);
        // note: IsSelected(0, args.RowIndex) will return selected or active
        var selected = this.IsPureSelected(args.RowIndex); // current row is pure selected
        var active = SelectedRow == args.RowIndex; //current is active

        var scheme = item.DataSystem.GetColor(item, oldScheme);
        if (selected)
        {
            if (active) // selected but also active
            {
                scheme = scheme with { Focus = scheme.Focus with { Style = TextStyle.Underline } };
            }
            else // selected but not active
            {
                scheme = scheme with { Focus = scheme.Normal with { Style = TextStyle.Underline } };
            }
        }

        return scheme;
    }

    public event Func<Key, bool>? KeyDownHandler;


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
                    currentSortIsAsc = currentSortColumn != clickedHeaderCol || !currentSortIsAsc;
                    currentSortColumn = clickedHeaderCol.Value;
                    ApplySort(currentSortColumn, currentSortIsAsc);
                    return true; // stop further process
                }
            }
        }

        if (e.Flags.HasFlag(MouseFlags.Button1DoubleClicked)) // left double click
        {


        } else if (e.Flags.HasFlag((MouseFlags.Button3Clicked))) // right button click
        {
            contextMenu.MakeVisible(e.ScreenPosition);
            UpdateContextMenu();
        }


        return base.OnMouseClick(e);
    }

    protected override void OnCellToggled(CellToggledEventArgs args)
    {
        // if click first column, we let table to draw its selection visual: e.Cancel = false
        if (!(ShowSelectionBox && args.Col == 0))
        {
            //e.Cancel = true; // we can not select by click or with 'space' key
            if (IsCommandTriggerFromMouseClick())
            {
                args.Cancel = true;// cancel the mouse click, only enable 'space' key
                return;
            }
        }

        base.OnCellToggled(args);
    }

    private bool IsCommandTriggerFromMouseClick()
    {
        var stackTrace = new StackTrace();
        for (var i = 3; i < 12; i++)
        {
            var frame = stackTrace.GetFrame(i);
            if (frame?.GetMethod()?.Name == "RaiseMouseClickEvent")
                return true; // i = 5
        }

        return false;
    }

    public bool CanView(IItem item) => item is IContainer;

    public void View(IItem container)
    {
        currentContainer = (IContainer)container;
        TableSource?.Dispose();
        var source = container.DataSystem.GetTableSource(currentContainer, currentSortColumn, currentSortIsAsc);
        if (ShowSelectionBox)
            source = new SelectableSortableTableSource(this, source);
        Table = source;

    }

    public void SelectAllInToggleableState(bool toggle = true)
    {
        if (!toggle)
        {
            SelectAll();
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
            SetSelection(0,row,false);
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
            if (currentContainer != null) View(currentContainer);
        }
    }

    private void CreateContextMenu()
    {
        var showSelection = new MenuItemv2()
        {
            Title = "Show Selection Box",
            //Key = Key.S.WithCtrl,
            
            CommandView = new CheckBox() { Title = "Show Selection Box", CanFocus = false,CheckedState = ShowSelectionBox ? CheckState.Checked: CheckState.UnChecked}
        };

        showSelection.Action = () =>
        {
            var checkBox = ((CheckBox)(showSelection!.CommandView));
            var check = checkBox.CheckedState == CheckState.Checked;
            ShowSelectionBox = check;
        };
        contextMenu = new PopoverMenu([
            showSelection
        ]);
        
    }

    private void UpdateContextMenu()
    {
        var menuItems = contextMenu.Root.SubViews.Cast<MenuItemv2>().ToArray();
        var showSelectionCheckbox = (CheckBox)(menuItems[0].CommandView);
        showSelectionCheckbox. CheckedState = ShowSelectionBox ? CheckState.Checked : CheckState.UnChecked;
    }


    internal void ApplySort(int sortColumn, bool isSortAsc)
    {
        TableSource.Sort(sortColumn, isSortAsc);
        //itemsListTable.RowOffset = 0;
        //itemsListTable.SelectedRow = 0;
        SetNeedsDraw();
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