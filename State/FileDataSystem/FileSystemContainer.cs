namespace ff.State.FileDataSystem;

using System.IO.Abstractions;
using TerminalFileManager;

public class FileSystemContainer(IDirectoryInfo dir, FileSystemItem[]? children = null, CultureInfo? culture = null)
    : FileSystemItem(dir, culture), IContainer
{
    private IItem[]? children = children?.OfType<IItem>().ToArray();

    /// <summary>
    /// return the parent container with the file (path) as active child
    /// </summary>
    /// <param name="path"></param>
    /// <param name="children"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static  FileSystemContainer New(string path, FileSystemItem[]? children = null, CultureInfo? culture = null)
    {
        path = Path.GetFullPath(path);
        if (File.Exists(path))
        {
            var dirPath = Path.GetDirectoryName(path)!;
            var fileName = Path.GetFileName(path);
            var dirInfoFactory = new FileSystem().DirectoryInfo;
            var container = new FileSystemContainer(dirInfoFactory.New(dirPath), children, culture);
            container.ActiveChild = container.Children.First(c => c.Name == fileName);
            return container;
        }
        else if (System.IO.Directory.Exists(path))
        {
            var dirInfoFactory = new FileSystem().DirectoryInfo;
            return new FileSystemContainer(dirInfoFactory.New(path), children, culture);
        }

        throw new ArgumentException($"Path: '{path}' is not exist.");
    }

    private IEnumerable<IItem> GetChildren()
    {
        try
        {
            if (FileSystemInfo is IDirectoryInfo dir)
            {
                var items = dir.GetFileSystemInfos()
                    .Select(e =>
                    {
                        if (e is IDirectoryInfo d)
                        {
                            return new FileSystemContainer(d);
                        }

                        return new FileSystemItem(e);
                    });

                return items;
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
        get => children??=GetChildren().ToArray();
        internal set => children = value;
    }

    public event Action? ChildrenUpdated;
    public IItem? ActiveChild { get; set; }
    public IDirectoryInfo Directory { get; } = dir;

    public IContainer? GetParent()
    {
        var parent = Directory.Parent;
        return parent == null ? null : new FileSystemContainer(parent);
    }
    protected bool Equals(FileSystemContainer other)
    {
        return Directory.FullName.Equals(other.Directory.FullName);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FileSystemContainer other)
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
