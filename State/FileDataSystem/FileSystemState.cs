namespace ff.State.FileDataSystem;

using System.IO.Abstractions;
using TerminalFileManager;

public class FileSystemState : FileSystemItem, IContainer
{
    private IItem[]? children1;

    public static  FileSystemState New(string path, FileSystemItem[]? children = null, CultureInfo? culture = null)
    {
        path = Path.GetFullPath(path);
        if (File.Exists(path))
        {
            var dirPath = Path.GetDirectoryName(path)!;
            var fileName = Path.GetFileName(path);
            var dirInfoFactory = new FileSystem().DirectoryInfo;
            var state = new FileSystemState(dirInfoFactory.New(dirPath), children, culture);
            state.SelectedChild = state.Children.First(c => c.Name == fileName);
            return state;
        }
        else if (System.IO.Directory.Exists(path))
        {
            var dirInfoFactory = new FileSystem().DirectoryInfo;
            return new FileSystemState(dirInfoFactory.New(path), children, culture);
        }

        throw new ArgumentException($"Path: '{path}' is not exist.");
    }

    public FileSystemState(IDirectoryInfo dir, FileSystemItem[]? children = null, CultureInfo? culture = null) : base(dir, culture)
    {
        Directory = dir;
        children1 = children?.OfType<IItem>().ToArray();
    }

    private IEnumerable<IItem> GetChildren()
    {
        try
        {
            if (FileSystemInfo is IDirectoryInfo dir)
            {
                var children = dir.GetFileSystemInfos()
                    .Select(e =>
                    {
                        if (e is IDirectoryInfo d)
                        {
                            return new FileSystemState(d);
                        }

                        return new FileSystemItem(e);
                    });

                return children;
            }

            return [];
        }
        catch (Exception ex)
        {
            var logger = DI.GetLogger<ILogger<FileSystemItem>>();
            logger.LogError(ex, "Access permissions Exceptions, Dir not exists etc");
            return [];
        }
    }

    public IItem[] Children
    {
        get => children1??=GetChildren().ToArray();
        internal set => children1 = value;
    }

    public event Action? ChildrenUpdated;
    public IItem? SelectedChild { get; set; }
    public IDirectoryInfo Directory { get; }

    public IContainer? GetParent()
    {
        var parent = Directory.Parent;
        return parent == null ? null : new FileSystemState(parent);
    }
    protected bool Equals(FileSystemState other)
    {
        return Directory.FullName.Equals(other.Directory.FullName);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FileSystemState other)
        {
            return false;
        }

        return Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Directory.FullName);
    }
}
