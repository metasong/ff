using ff.State.FileDataSystem;
using ff.State.FileSystem;

namespace ff.State;

/// <summary>
/// c:  - <char>: or .\ or ./ - file system 
/// hk: - hive key - registry: hk:\
/// ftp:\\
/// </summary>
public static class SystemSwitch
{
    public static IContainer GetState(string path)
    {
        path = path.Trim();
        if (path.StartsWith("./") || path.StartsWith(@".\") || path == "."|| path[1] == ':' && char.IsAsciiLetter(path[0]))
        {
            return FileSystemContainer.New(path);
        }

        throw new ArgumentException("can not file system from path!");
    }
}