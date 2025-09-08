using ff.Views.Preview.Config.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Terminal.Gui.Drawing.Color;
using Size = System.Drawing.Size;

namespace ff.Views.Preview.Sixel;

public class SixelGenerator(SixelConfig sixelConfig)
{
    public string GenerateSixelData(
        Image<Rgba32> fullResImage,
        Size maxSize
    )
    {
        var encoder = new SixelEncoder();
        encoder.Quantizer.MaxColors = Math.Min(encoder.Quantizer.MaxColors, sixelConfig.MaxPaletteColors);
        encoder.Quantizer.PaletteBuildingAlgorithm = GetPaletteBuilder();
        encoder.Quantizer.DistanceAlgorithm = GetDistanceAlgorithm();

        // Calculate the target size in pixels based on console units
        var targetWidthInPixels = maxSize.Width * sixelConfig.PixelsPerColumn ;
        var targetHeightInPixels = maxSize.Height * sixelConfig.PixelsPerRow;

        // Get the original image dimensions
        var originalWidth = fullResImage.Width;
        var originalHeight = fullResImage.Height;

        // Use the helper function to get the resized dimensions while maintaining the aspect ratio
        var newSize = CalculateAspectRatioFit(originalWidth, originalHeight, targetWidthInPixels, targetHeightInPixels);

        // Resize the image to match the console size
        var resizedImage = fullResImage.Clone(x => x.Resize(newSize.Width, newSize.Height));

        var encoded = encoder.EncodeSixel(ConvertToColorArray(resizedImage));


        return encoded;
    }

    private IPaletteBuilder GetPaletteBuilder()
    {
        switch (sixelConfig.PaletteType)
        {
            case PaletteType.Popularity: return new PopularityPaletteWithThreshold(GetDistanceAlgorithm(), sixelConfig.PopularityThreshold);
            case PaletteType.MedianCut: return new MedianCutPaletteBuilder(GetDistanceAlgorithm());
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IColorDistance GetDistanceAlgorithm()
    {
        switch (sixelConfig.ColorDistanceAlgorithm)
        {
            case ColorDistanceAlgorithm.Euclidian: return new EuclideanColorDistance();
            case ColorDistanceAlgorithm.CIE76: return new CIE76ColorDistance();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Size CalculateAspectRatioFit(int originalWidth, int originalHeight, int targetWidth, int targetHeight)
    {
        // Calculate the scaling factor for width and height
        var widthScale = (double)targetWidth / originalWidth;
        var heightScale = (double)targetHeight / originalHeight;

        // Use the smaller scaling factor to maintain the aspect ratio
        var scale = Math.Min(widthScale, heightScale);

        // Calculate the new width and height while keeping the aspect ratio
        var newWidth = (int)(originalWidth * scale);
        var newHeight = (int)(originalHeight * scale);

        // Return the new size as a Size object
        return new(newWidth, newHeight);
    }
    public static Color[,] ConvertToColorArray(Image<Rgba32> image)
    {
        var width = image.Width;
        var height = image.Height;
        var colors = new Color[width, height];

        // Loop through each pixel and convert Rgba32 to Terminal.Gui color
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var pixel = image[x, y];
                colors[x, y] = new(pixel.R, pixel.G, pixel.B); // Convert Rgba32 to Terminal.Gui color
            }
        }

        return colors;
    }
}