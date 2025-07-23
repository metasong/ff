using System.Text.RegularExpressions;

namespace ff;

internal static class Helper
{
    public static IDirectoryInfo ToDirectoryInfo(this IFileSystem fileSystem ,string path)
    {
        // if you pass new DirectoryInfo("C:") you get a weird object
        // where the FullName is in fact the current working directory.
        // really not what most users would expect
        if (Regex.IsMatch(path, "^\\w:$"))
        {
            return fileSystem.DirectoryInfo.New(path + fileSystem.Path.DirectorySeparatorChar);
        }

        return fileSystem.DirectoryInfo.New(path);
    }
}