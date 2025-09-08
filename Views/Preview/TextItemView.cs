using ff.Views.Preview.Config.Image;

namespace ff.Views.Preview;

public class TextItemView : TextView, IPreviewer
{
    private readonly TextConfig config;

    public TextItemView(TextConfig config)
    {
        this.config = config;
        ReadOnly = true;
        WordWrap = true;
        Width = Dim.Fill();
        Height = Dim.Fill();
        VerticalScrollBar.AutoShow = true;
        VerticalScrollBar.Visible = true;
    }

    public bool CanView(IItem item)
        => config.Exts.Any(ext => item.Name.EndsWith($".{ext}"));

    public void View(IItem item)
    {
        var content = File.ReadAllText(item.FullName);
        Text = content;
    }
}