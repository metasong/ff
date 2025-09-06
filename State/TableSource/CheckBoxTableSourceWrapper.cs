
namespace ff.State.TableSource;


public class CheckBoxTableSourceWrapper : ITableSource
{
    private readonly ItemTable tableView;

    public CheckBoxTableSourceWrapper(ItemTable tableView, ISortableTableSource toWrap)
    {
        Wrapping = toWrap;
        this.tableView = tableView;
        tableView.KeyBindings.ReplaceCommands(Key.Space, Command.Select);

        tableView.MouseClick += TableView_MouseClick;
        tableView.CellToggled += TableView_CellToggled;

        for (var i = 0; i < Wrapping.Container.Children.Length; i++)
        {
            var item = Wrapping.Container.Children[i];
            if (item.IsSelected)
            {
                tableView.SetSelectionInToggleableState(i);
            }
        }
    }

    public Rune CheckedRune { get; set; } = Glyphs.CheckStateChecked;

    public Rune RadioCheckedRune { get; set; } = Glyphs.Selected;

    public Rune RadioUnCheckedRune { get; set; } = Glyphs.UnSelected;

    public Rune UnCheckedRune { get; set; } = Glyphs.CheckStateUnChecked;

    public bool UseRadioButtons { get; set; }

    public ISortableTableSource Wrapping { get; }

    public object this[int row, int col]
    {
        get
        {
            if (col == 0)
            {
                if (UseRadioButtons)
                {
                    return IsChecked(row) ? RadioCheckedRune : RadioUnCheckedRune;
                }

                return IsChecked(row) ? CheckedRune : UnCheckedRune;
            }

            return Wrapping[row, col - 1];
        }
    }

    public int Rows => Wrapping.Rows;

    public int Columns => Wrapping.Columns + 1;

    private bool seletedAll;

    public string[] ColumnNames
    {
        get
        {
            var toReturn = Wrapping.ColumnNames.ToList();
            if (UseRadioButtons)
            {
                toReturn.Insert(0, " ");
            }
            else
            {
                toReturn.Insert(0, seletedAll ? $"{CheckedRune}" : $"{UnCheckedRune}");
            }

            return toReturn.ToArray();
        }
    }

    /// <summary>Clears the toggled state of all rows.</summary>
    protected virtual void ClearAllToggles() { }

    /// <summary>Returns true if <paramref name="row"/> is checked.</summary>
    /// <param name="row"></param>
    /// <returns></returns>
    bool IsChecked(int row) => tableView.IsPureSelected(row);

    /// <summary>
    ///     Called when the 'toggled all' action is performed. This should change state from 'some selected' to 'all
    ///     selected' or clear selection if all area already selected.
    /// </summary>
    protected virtual void ToggleAllRows() { }

    /// <summary>Flips the checked state of the given <paramref name="row"/>/</summary>
    /// <param name="row"></param>
    protected virtual void ToggleRow(int row)
    {
        try
        {
            var item = Wrapping.GetChild(row);
            item.IsSelected = !item.IsSelected;
        }
        catch (Exception e)
        {

        }
    }

    protected virtual void ToggleRows(int[] range) { }

    /// <summary>
    /// triggered by mouse click or 'space' bar
    /// </summary>
    private void TableView_CellToggled(object sender, CellToggledEventArgs e)
    {
        // Suppress default toggle behavior when using checkboxes
        // and instead handle ourselves
        var range = tableView.GetAllSelectedCells().Select(c => c.Y).Distinct().ToArray();

        if (UseRadioButtons)
        {
            // multi selection makes it unclear what to toggle in this situation
            if (range.Length != 1)
            {
                e.Cancel = true;

                return;
            }

            ClearAllToggles();
            ToggleRow(range.Single());
        }
        else
        {
            ToggleRows(range);
        }


        tableView.SetNeedsDraw();
    }


    private void TableView_MouseClick(object sender, MouseEventArgs e)
    {
        // we only care about clicks (not movements)
        if (!e.Flags.HasFlag(MouseFlags.Button1Clicked))
        {
            return;
        }

        var hit = tableView.ScreenToCell(e.Position.X, e.Position.Y, out var headerIfAny);

        if (headerIfAny is 0)
        {
            // clicking in header with radio buttons does nothing
            if (UseRadioButtons)
            {
                return;
            }
            seletedAll = !seletedAll;

            // otherwise it ticks all rows
            ToggleAllRows();
            if (seletedAll)
                tableView.SelectAllInToggleableState();
            else
                tableView.MultiSelectedRegions.Clear();

            e.Handled = true;
            tableView.SetNeedsDraw();
        }
        else if (hit is { X: 0 })
        {
            if (UseRadioButtons)
            {
                ClearAllToggles();
                ToggleRow(hit.Value.Y);
            }
            else
            {
                ToggleRow(hit.Value.Y);
            }

            //e.Handled = true;
            tableView.SetNeedsDraw();
        }
    }

    public void Dispose()
    {
        tableView.MouseClick -= TableView_MouseClick;
        tableView.CellToggled -= TableView_CellToggled;
    }

}
