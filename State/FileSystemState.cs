namespace ff.State;

/// <summary>
///     Wrapper for <see cref="FileSystemInfo"/> that contains additional information (e.g. <see cref="IsParent"/>)
///     and helper methods.
/// </summary>
internal class FileSystemState
{
    private readonly CultureInfo culture;

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
    private const long ByteConversion = 1024;
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

    private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    /// <summary>Initializes a new instance of the <see cref="FileSystemState"/> class.</summary>
    /// <param name="fileSystemInfo">The directory of path to wrap.</param>
    /// <param name="culture"></param>
    public FileSystemState(IFileSystemInfo fileSystemInfo, CultureInfo culture)
    {
        this.culture = culture;
        FileSystemInfo = fileSystemInfo;
    }

    /// <summary>Gets the wrapped <see cref="FileSystemInfo"/> (directory or file).</summary>
    public IFileSystemInfo FileSystemInfo { get; }

    public string HumanReadableLength => FileSystemInfo is IFileInfo fi
        ? GetHumanReadableFileSize(MachineReadableLength, culture)
        : string.Empty;

    public bool IsDir => !(FileSystemInfo is IFileInfo);

    /// <summary>Gets or Sets a value indicating whether this instance represents the parent of the current state (i.e. "..").</summary>
    public bool IsParent { get; internal set; }

    public DateTime? LastWriteTime => FileSystemInfo.LastWriteTime;
    public long MachineReadableLength => FileSystemInfo is IFileInfo fi ? fi.Length : 0;
    public string Name => IsParent ? ".." : FileSystemInfo.Name;
    public string Type => FileSystemInfo is IFileInfo fi ? fi.Extension : $"<{Strings.Directory}>";

    // TODO: handle linux executable status
    public bool IsExecutable
     => (FileSystemInfo is { } f) && ExecutableExtensions.Contains(
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

        var mag = (int)Math.Log(value, ByteConversion);
        var adjustedSize = value / Math.Pow(1000, mag);

        return string.Format(culture.NumberFormat, "{0:n2} {1}", adjustedSize, SizeSuffixes[mag]);
    }
}
