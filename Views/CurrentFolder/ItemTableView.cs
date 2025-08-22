using ff.State;

namespace ff.Views.CurrentFolder;

public sealed class ItemTableView : View
{
    private int _currentSortColumn;
    private bool _currentSortIsAsc = true;

    private readonly IStateManager stateManager;

    private readonly TableView itemsListTable = new()
    {
        Width = Dim.Fill(),
        Height = Dim.Fill(),
        MultiSelect = false,
        // BorderStyle = LineStyle.None,
    };

    public ItemTableView(IStateManager stateManager)
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        this.stateManager = stateManager;
        BorderStyle = LineStyle.None;

        Add(itemsListTable);
        itemsListTable.Style.AlwaysShowHeaders = true;
        itemsListTable.Style.ShowHorizontalHeaderOverline = false;
        itemsListTable.Style.ShowHorizontalBottomline = false;
        itemsListTable.Style.ShowVerticalCellLines = false;
        itemsListTable.Style.ShowVerticalHeaderLines = false;
        itemsListTable.SetFocus();

        this.stateManager.StateChanged += (oldState, newState) => { ShowData(newState); };
        ShowData(stateManager.CurrentState);
    }
    void ShowData(IContainer container)
    {
        itemsListTable.Table = container.DataSystem.GetTableSource(container, _currentSortColumn, _currentSortIsAsc);
        ApplySort(container);
    }
    internal void ApplySort(IContainer state)
    {
        var tableSource = (ISortableTableSource)itemsListTable.Table;

        tableSource.Sort(_currentSortColumn, _currentSortIsAsc);
        //itemsListTable.RowOffset = 0;
        //itemsListTable.SelectedRow = 0;
        itemsListTable.Update();
    }
}