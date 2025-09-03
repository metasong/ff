namespace ff.State.TableSource;

internal class SelectableSortableTableSource(TableView tableView, ISortableTableSource toWrap)
    : CheckBoxTableSourceWrapperByIndex(tableView, toWrap), ISortableTableSource
{
    public IItem GetChild(int row)
    {
        return toWrap.GetChild(row);
    }

    public Task Sort(int column, bool asc)
    {
        return toWrap.Sort(column, asc);
    }
}