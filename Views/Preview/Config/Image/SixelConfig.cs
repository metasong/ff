namespace ff.Views.Preview.Config.Image;
public enum PaletteType { Popularity, MedianCut }
public enum ColorDistanceAlgorithm { Euclidian, CIE76 }
public class SixelConfig
{
    public int MaxPaletteColors { get; set; } = 256;
    public int PixelsPerColumn { get; set; } = 10;
    public int PixelsPerRow { get; set; } = 20;
    public PaletteType PaletteType { get; set; } = PaletteType.Popularity;
    public int PopularityThreshold { get; set; } = 8;
    public ColorDistanceAlgorithm ColorDistanceAlgorithm { get; set; } = ColorDistanceAlgorithm.Euclidian;
}