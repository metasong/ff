using ff.Views.Preview.Config;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using ff.Views.Preview.Sixel;

namespace ff.Views.Preview;

public class ImageView : View
{
    private SixelGenerator sixelGenerator;
    private SixelToRender sixelToRender = new SixelToRender();

    public ImageView(ImageConfig config)
    {
        sixelGenerator = new SixelGenerator(config.Sixel);
        Width = Dim.Fill();
        Height = Dim.Fill();
    }

    public void ShowImage(string path)
    {
        //Application.Force16Colors = false;
        Image<Rgba32> image;

        try
        {
            image = Image.Load<Rgba32>(File.ReadAllBytes(path));
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Could not open file", ex.Message, "Ok");

            return;
        }

        var screenLocationForSixel = FrameToScreen().Location;
        var encodedSixelData = sixelGenerator.GenerateSixelData(
            image,
            Frame.Size);

        sixelToRender.SixelData = encodedSixelData;
        sixelToRender.ScreenPosition = screenLocationForSixel;
        
        if(!Application.Sixel.Contains(sixelToRender))
            Application.Sixel.Add(sixelToRender);

        //SetNeedsDraw();
        //Application.LayoutAndDraw();

    }

    public void HideImage()
    {
        //Application.Force16Colors = true;

        if (Application.Sixel.Contains(sixelToRender))
            Application.Sixel.Remove(sixelToRender);
    }


}