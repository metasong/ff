
namespace ff.State.TableSource;

/// <summary>
///     <see cref="ITableSource"/> for a <see cref="TableView"/> which adds a checkbox column as an additional column
///     in the table.
/// </summary>
/// <remarks>
///     This class wraps another <see cref="ITableSource"/> and dynamically serves its rows/cols plus an extra column.
///     Data in the wrapped source can be dynamic (change over time).
/// </remarks>
public class CheckBoxTableSourceWrapper : ITableSource
{
    private readonly ItemTable tableView;

    /// <summary>
    ///     Creates a new instance of the class presenting the data in <paramref name="toWrap"/> plus an additional
    ///     checkbox column.
    /// </summary>
    /// <param name="tableView">
    ///     The <see cref="TableView"/> this source will be used with. This is required for event
    ///     registration.
    /// </param>
    /// <param name="toWrap">The original data source of the <see cref="TableView"/> that you want to add checkboxes to.</param>
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

    /// <summary>
    ///     Gets or sets the character to use for checked entries. Defaults to <see cref="Glyphs.CheckStateChecked"/>
    /// </summary>
    public Rune CheckedRune { get; set; } = Glyphs.CheckStateChecked;

    /// <summary>
    ///     Gets or sets the character to use for checked entry when <see cref="UseRadioButtons"/> is true. Defaults to
    ///     <see cref="Glyphs.Selected"/>
    /// </summary>
    public Rune RadioCheckedRune { get; set; } = Glyphs.Selected;

    /// <summary>
    ///     Gets or sets the character to use for unchecked entries when <see cref="UseRadioButtons"/> is true. Defaults
    ///     to <see cref="Glyphs.UnSelected"/>
    /// </summary>
    public Rune RadioUnCheckedRune { get; set; } = Glyphs.UnSelected;

    /// <summary>
    ///     Gets or sets the character to use for UnChecked entries. Defaults to <see cref="Glyphs.CheckStateUnChecked"/>
    /// </summary>
    public Rune UnCheckedRune { get; set; } = Glyphs.CheckStateUnChecked;

    /// <summary>Gets or sets whether to only allow a single row to be toggled at once (Radio button).</summary>
    public bool UseRadioButtons { get; set; }

    /// <summary>Gets the <see cref="ITableSource"/> that this instance is wrapping.</summary>
    public ISortableTableSource Wrapping { get; }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public int Rows => Wrapping.Rows;

    /// <inheritdoc/>
    public int Columns => Wrapping.Columns + 1;

    private bool seletedAll;

    /// <inheritdoc/>
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

    /// <summary>
    ///     Flips the checked state for a collection of rows. If some (but not all) are selected they should flip to all
    ///     selected.
    /// </summary>
    /// <param name="range"></param>
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

        // if click first column, we let table to draw its selection visual: e.Cancel = false
        if (e.Col != 0)
        {
            //e.Cancel = true; // we can not select by click or with 'space' key
            if (IsCommandTriggerFromMouseClick())
            {
                e.Cancel = true;// enable 'space' key
            }
        }
        tableView.SetNeedsDraw();
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
