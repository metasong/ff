namespace ff.Views.Preview.Config;

public class ImageConfig
{
    public SixelConfig Sixel { get; set; }= new SixelConfig();
}

public class PreviewConfig
{
    public ImageConfig ImageConfig { get; set; } = new ImageConfig();
}