namespace ff.State.TableSource;

internal class SelectableSortableTableSource(ItemTable tableView, ISortableTableSource toWrap)
    : CheckBoxTableSourceWrapper(tableView, toWrap), ISortableTableSource
{
    public IContainer Container => toWrap.Container;

    public IItem GetChild(int row)
    {
        return toWrap.GetChild(row);
    }

    public Task Sort(int column, bool asc)
    {
        return toWrap.Sort(column, asc);
    }

}