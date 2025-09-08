namespace ff.Views.Preview.Config.Image;

public class ImageConfig
{
    public string[] Exts { get; set; } = ["png", "jpg", "jpeg", "bmp", "gif", "pbm", "tga", "tiff", "webp", "qoi"];
    public SixelConfig Sixel { get; set; } = new SixelConfig();
}

public class TextConfig
{
    public string[] Exts { get; set; } = ["md","txt", "json", "ini", "yml", "xml", "xaml", "cs", "sln", "csproj"];

}