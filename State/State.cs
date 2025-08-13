using System.IO;

namespace ff.State;

internal class State
{
    public State(IDirectoryInfo dir, string path)
    {
        Directory = dir;
        Path =path;

        RefreshChildren();
    }

    public FileSystemState[] Children { get; internal set; }
    public IDirectoryInfo Directory { get; }

    /// <summary>Gets what was entered in the path text box of the dialog when the state was active.</summary>
    public string Path { get; }

    public FileSystemState Selected { get; set; }

    protected virtual IEnumerable<FileSystemState> GetChildren(IDirectoryInfo dir)
    {
        try
        {

            var children = dir.GetFileSystemInfos()
                .Select(e => new FileSystemState(e, Parent.Style.Culture));

            return children;
        }
        catch (Exception)
        {
            // Access permissions Exceptions, Dir not exists etc
            return [];
        }
    }

    internal virtual void RefreshChildren()
    {
        IDirectoryInfo dir = Directory;
        Children = GetChildren(dir).ToArray();
    }
}
