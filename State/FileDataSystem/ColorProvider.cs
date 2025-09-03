using Terminal.Gui.FileServices;

namespace ff.State.FileDataSystem;

public class ColorProvider : FileSystemColorProvider
{
    public Scheme GetColorScheme(IItem item, Scheme current)
    {
        var fileItem = (FileSystemItem)item;
        var scheme = current;
        var color = GetColor(fileItem.FileSystemInfo);
        if (color != null)
            scheme = scheme with
            {
                Normal = new(color!.Value, current.Normal.Background),
                HotNormal = new(color!.Value, current.HotNormal.Background, TextStyle.Bold),
                Focus = new(current.Focus.Foreground, color!.Value),
                HotFocus = new(current.HotFocus.Foreground, color!.Value, TextStyle.Bold)
            };

        return scheme;
    }
}