namespace ff.State;

public interface IDataSystem
{
    ISortableTableSource GetTableSource(IContainer container, int sortColumn,
        bool sortIsAsc);
    Scheme GetColor(IItem item, Scheme current);

    IOperations Operations { get; }
}