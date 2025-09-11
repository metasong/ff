using ff.State.FileDataSystem;
using DefaultFileOperations = ff.State.FileDataSystem.DefaultFileOperations;

namespace ff.State.FileSystem;

public class FileDataSystem() : IDataSystem
{
    public static IDataSystem Instance = new FileDataSystem();
    public static ColorProvider ColorProvider = new();

    public ISortableTableSource GetTableSource(IContainer container, int sortColumn = 0,
        bool sortIsAsc = true)
    {
        if (container is not FileSystemContainer state)
        {
            throw new ArgumentException($"the container must be a FileSystemState!");
        }

        return new CurrentFolderTableSource(state, sortColumn, sortIsAsc);
    }

    public Scheme GetColor(IItem item, Scheme current)
    {
        if (item is not FileSystemItem state)
        {
            throw new ArgumentException($"the container must be a FileSystemState!");
        }

        return ColorProvider.GetColorScheme(state, current);
    }

    public IOperations Operations { get; private set; } = new DefaultFileOperations();
}

