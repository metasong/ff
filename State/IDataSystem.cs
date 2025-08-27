namespace ff.State;

public interface IDataSystem
{
    ITableSource GetTableSource(IContainer container, int sortColumn,
        bool sortIsAsc);
    Scheme? GetColor(IItem item, Scheme current);
}