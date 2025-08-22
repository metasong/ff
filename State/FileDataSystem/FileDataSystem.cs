using ff.State.FileDataSystem;

namespace ff.State.FileSystem;

public class FileDataSystem() : IDataSystem
{
    public static IDataSystem Instance = new FileDataSystem();

    public ITableSource GetTableSource(IContainer container, int sortColumn = 0,
        bool sortIsAsc = true)
    {
        if (container is not FileSystemState state)
        {
            throw new ArgumentException($"the container must be a FileSystemState!");
        }

        return new CurrentFolderTableSource(state, sortColumn, sortIsAsc);
    }
}