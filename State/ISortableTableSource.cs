namespace ff.State;

public interface ISortableTableSource: ITableSource
{
    Task Sort(int column, bool asc);
}