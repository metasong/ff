using ff.Views.Preview.Config.Image;

namespace ff.Views.Preview.Config;

public class PreviewConfig
{
    public ImageConfig Image { get; set; } = new ImageConfig();
    public TextConfig Text { get; set; } = new TextConfig();
}