namespace ff.State;

public interface ISortableTableSource: ITableSource, IDisposable
{
    IContainer Container { get; }
    IItem GetChild(int row);
    Task Sort(int column, bool asc);
}