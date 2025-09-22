using System.Drawing;
using TerminalFileManager;
using IFileInfo = System.IO.Abstractions.IFileInfo;

namespace ff.State.Implements.FileDataSystem;

/// <summary>
///     Wrapper for <see cref="FileSystemInfo"/> that contains additional information (e.g. <see cref="IsParent"/>)
///     and helper methods.
/// </summary>
public class FileSystemItem: IItem
{
    /// <summary>
    /// Name with extension
    /// </summary>
    public string Name => FileSystemInfo.Name;

    public string FullName => FileSystemInfo.FullName;

    public bool IsLeaf => !IsDir;
    public IDataSystem DataSystem => FileDataSystem.Instance;
    public bool IsSelected { get; set; }
    public IDictionary<string, string> Properties
    {
        get
        {
            var dict = new Dictionary<string, string>
            {
                {"Creation", $"{FileSystemInfo.CreationTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern)} (UTC: {FileSystemInfo.CreationTimeUtc.ToString(CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern)})"},
                {"Modified", $"{FileSystemInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern)} (UTC: {FileSystemInfo.LastWriteTimeUtc.ToString(CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern)})"},
                {"Accessed", $"{FileSystemInfo.LastAccessTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.UniversalSortableDateTimePattern)} (UTC: {FileSystemInfo.LastAccessTimeUtc.ToString(CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern)})"},
                {"Size",$"{Size:N0}({SizeReadable})"},
                {"Attributes", $"{FileSystemInfo.Attributes}"},
            };

            if (IsImage && FileSystemInfo is IFileInfo fi)
            {
                try
                {
                    using var img = new Bitmap(fi.FullName);
                    dict["Image Size"] = $"{img.Width} x {img.Height}";
                    dict["Bit Depth"] = $"{Image.GetPixelFormatSize(img.PixelFormat)}";
                }
                catch
                {
                    // Optionally log or ignore image loading errors
                }
            }

            return dict;
        }
    }

    public string TypeString => FileSystemInfo is IFileInfo fi ? fi.Extension : $"<{DI.Localization.GetString("Directory")}>";

    public IItem? GetParentItem()
    {
        var parent = FileSystemInfo switch
        {
            IFileInfo fi => fi.Directory,
            IDirectoryInfo di => di.Parent,
            _ => throw new Exception("the FileSystemInfo is not IFileInfo or IDirectoryInfo")
        };
        return parent == null ? null : new FileSystemItem(parent, Culture);
    }

    public readonly CultureInfo Culture;

    /* ---- Colors used by the ls command line tool ----
     *
     * Blue: Directory
     * Green: Executable or recognized data file
     * Cyan (Sky Blue): Symbolic link file
     * Yellow with black background: Device
     * Magenta (Pink): Graphic image file
     * Red: Archive file
     * Red with black background: Broken link
     */
    private const long OneK = 1024;
    private static readonly List<string> ExecutableExtensions = new() { ".EXE", ".BAT" };

    private static readonly List<string> ImageExtensions = new()
    {
        ".JPG",
        ".JPEG",
        ".JPE",
        ".BMP",
        ".GIF",
        ".PNG"
    };

    private static readonly string[] SizeSuffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

    /// <param name="fileSystemInfo">The directory/file path to wrap.</param>
    /// <param name="culture"></param>
    public FileSystemItem(IFileSystemInfo fileSystemInfo, CultureInfo? culture = null)
    {
        Culture = culture ?? CultureInfo.CurrentCulture;
        FileSystemInfo = fileSystemInfo;
    }

    public IFileSystemInfo FileSystemInfo { get; }

    public string SizeReadable => FileSystemInfo is IFileInfo fi
        ? GetHumanReadableFileSize(Size, Culture)
        : string.Empty;

    public DateTime LastWriteTime => FileSystemInfo.LastWriteTime;
    public long Size => FileSystemInfo is IFileInfo fi ? fi.Length : 0;
    public bool IsDir => FileSystemInfo is IDirectoryInfo;


    // TODO: handle linux executable status
    public bool IsExecutable
     => FileSystemInfo is { } f && ExecutableExtensions.Contains(
                                                 f.Extension,
                                                 StringComparer.InvariantCultureIgnoreCase
                                                );


    public bool IsImage
    => FileSystemInfo is { } f && ImageExtensions.Contains(
                                            f.Extension,
                                            StringComparer.InvariantCultureIgnoreCase
                                           );


    private static string GetHumanReadableFileSize(long value, CultureInfo culture)
    {
        if (value < 0)
        {
            return "-" + GetHumanReadableFileSize(-value, culture);
        }

        if (value == 0)
        {
            return "0.0 B";
        }

        var mag = (int)Math.Log(value, OneK);
        var adjustedSize = value / Math.Pow(1000, mag);

        return string.Format(culture.NumberFormat, "{0:n2} {1}", adjustedSize, SizeSuffixes[mag]);
    }
}
