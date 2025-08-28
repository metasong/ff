using ff.Views.Preview.Config;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using ff.Views.Preview.Sixel;

namespace ff.Views.Preview;

public class ImageView : View
{
    private SixelGenerator sixelGenerator;
    private SixelToRender _sixelImage;

    public ImageView(ImageConfig config)
    {
        sixelGenerator = new SixelGenerator(config.Sixel);
    }

    public void ShowImage(Image<Rgba32> image)
    {
        var screenLocationForSixel = FrameToScreen().Location;
        var encodedSixelData = sixelGenerator.GenerateSixelData(
            image,
            Frame.Size);

        if (_sixelImage == null)
        {
            _sixelImage = new()
            {
                SixelData = encodedSixelData,
                ScreenPosition = screenLocationForSixel
            };

            Application.Sixel.Add(_sixelImage);
        }
        else
        {
            _sixelImage.ScreenPosition = screenLocationForSixel;
            _sixelImage.SixelData = encodedSixelData;
        }

        SetNeedsDraw();
    }
}