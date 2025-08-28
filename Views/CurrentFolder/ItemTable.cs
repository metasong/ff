using Microsoft.VisualBasic;
using System.Drawing;
using Terminal.Gui.FileServices;
using Terminal.Gui.Views;

namespace ff.Views.CurrentFolder;

public record ShortcutConfig(string command, string condition = "");

public class ItemTable : TableView
{
    private int _currentSortColumn;
    private bool _currentSortIsAsc = true;

    public FileSystemColorProvider ColorProvider { get; set; } = new();
    private Scheme OldScheme;

    public ItemTable(bool canFocus = false)
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        MultiSelect = false;
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
            var item = TableSource.GetItem(args.RowIndex);
            var stats = TableSource.DataSystem.GetColor(item, OldScheme);
            return stats;
        };

    }

    internal Func<Key, bool>? KeyDownHandler;

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
    {
        if (KeyDownHandler?.Invoke(key) ?? false)
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
                _currentSortIsAsc = _currentSortColumn != clickedHeaderCol || !_currentSortIsAsc;
                _currentSortColumn = clickedHeaderCol.Value;
                ApplySort(_currentSortColumn, _currentSortIsAsc);
                return true; // stop further process
            }
        }

        if (e.Flags.HasFlag(MouseFlags.Button1DoubleClicked)) // left double click
        {

        }

        return base.OnMouseClick(e);
    }


    public void ShowData(IContainer container)
    {
        Table = container.DataSystem.GetTableSource(container, _currentSortColumn, _currentSortIsAsc);
        ApplySort(_currentSortColumn, _currentSortIsAsc);

    }

    internal void ApplySort( int sortColumn, bool isSortAsc)
    {
        TableSource.Sort(sortColumn, isSortAsc);
        //itemsListTable.RowOffset = 0;
        //itemsListTable.SelectedRow = 0;
        Update();
    }
}