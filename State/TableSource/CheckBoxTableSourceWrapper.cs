
namespace ff.State.TableSource;


public class CheckBoxTableSourceWrapper : ITableSource
{
    private readonly ItemTable tableView;

    public CheckBoxTableSourceWrapper(ItemTable tableView, ISortableTableSource toWrap)
    {
        WrappedTableSource = toWrap;
        this.tableView = tableView;
        tableView.KeyBindings.ReplaceCommands(Key.Space, Command.Select);

        tableView.MouseClick += TableView_MouseClick;
        //tableView.CellToggled += TableView_CellToggled;

        for (var i = 0; i < WrappedTableSource.Container.Children.Length; i++)
        {
            var item = WrappedTableSource.Container.Children[i];
            if (item.IsSelected)
            {
                tableView.SetSelectionInToggleableState(i);
            }
        }

        selectedAll = toWrap.Container.Children.All(it => it.IsSelected);
    }

    public Rune CheckedRune { get; set; } = Glyphs.CheckStateChecked;

    public Rune RadioCheckedRune { get; set; } = Glyphs.Selected;

    public Rune RadioUnCheckedRune { get; set; } = Glyphs.UnSelected;

    public Rune UnCheckedRune { get; set; } = Glyphs.CheckStateUnChecked;

    public bool UseRadioButtons { get; set; }

    private ISortableTableSource WrappedTableSource { get; }

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

            return WrappedTableSource[row, col - 1];
        }
    }

    public int Rows => WrappedTableSource.Rows;

    public int Columns => WrappedTableSource.Columns + 1;

    private bool selectedAll;

    public string[] ColumnNames
    {
        get
        {
            var toReturn = WrappedTableSource.ColumnNames.ToList();
            if (UseRadioButtons)
            {
                toReturn.Insert(0, " ");
            }
            else
            {
                toReturn.Insert(0, selectedAll ? $"{CheckedRune}" : $"{UnCheckedRune}");
            }

            return toReturn.ToArray();
        }
    }

    protected virtual void ClearAllToggles() { }

    bool IsChecked(int row) => tableView.IsPureSelected(row);

    /// <summary>
    ///     Called when the 'toggled all' action is performed. This should change state from 'some selected' to 'all
    ///     selected' or clear selection if all area already selected.
    /// </summary>
    protected virtual void ToggleAllRows()
    {
        if (selectedAll)
        {
            tableView.SelectAllInToggleableState();
            foreach (var child in WrappedTableSource.Container.Children)
            {
                child.IsSelected = true;
            }
        }
        else
        {
            tableView.MultiSelectedRegions.Clear();
            foreach (var child in WrappedTableSource.Container.Children)
            {
                child.IsSelected = false;
            }
        }
    }

    protected virtual void ToggleRow(int row)
    {
        //var item = WrappedTableSource.GetChild(row);
        //item.IsSelected = !item.IsSelected;
    }

    protected virtual void ToggleRows(int[] range) { }

    /// <summary>
    /// triggered by mouse click or 'space' bar
    /// </summary>
    //private void TableView_CellToggled(object sender, CellToggledEventArgs e)
    //{
    //    var range = tableView.GetAllSelectedCells().Select(c => c.Y).Distinct().ToArray();

    //    if (UseRadioButtons)
    //    {
    //        // multi selection makes it unclear what to toggle in this situation
    //        if (range.Length != 1)
    //        {
    //            // Suppress default toggle behavior when using checkboxes
    //            // and instead handle ourselves
    //            e.Cancel = true;
    //            return;
    //        }

    //        ClearAllToggles();
    //        ToggleRow(range.Single());
    //    }
    //    else
    //    {
    //        ToggleRows(range); // no action now
    //    }

    //    tableView.SetNeedsDraw();
    //}

    private void TableView_MouseClick(object sender, MouseEventArgs e)
    {
        // we only care about left button clicks (not movements)
        if (!e.Flags.HasFlag(MouseFlags.Button1Clicked))
        {
            return;
        }

        var hit = tableView.ScreenToCell(e.Position.X, e.Position.Y, out var headerIndex);

        if (headerIndex is 0)
        {
            if (UseRadioButtons)
            {
                // clicking in header with radio buttons does nothing
                return;
            }

            // otherwise it ticks all rows
            selectedAll = !selectedAll;
            ToggleAllRows();

            e.Handled = true; // so the sorting from wrapped table source is prevented.
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

            //e.Handled = true; // we still need to use default row click behavior for showing the visual of selection and active row
            tableView.SetNeedsDraw();
        }
    }

    public void Dispose()
    {
        // remove event handlers so that this obj can be gc.
        tableView.MouseClick -= TableView_MouseClick;
        //tableView.CellToggled -= TableView_CellToggled;
    }

}
