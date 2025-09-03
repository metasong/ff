namespace ff.State;

public interface ISortableTableSource: ITableSource
{
    IItem GetChild(int row);
    Task Sort(int column, bool asc);
}