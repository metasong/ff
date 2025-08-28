namespace ff.Views.Preview.Sixel;

public class CIE76ColorDistance : LabColorDistance
{
    public override double CalculateDistance(Color c1, Color c2)
    {
        var lab1 = RgbToLab(c1);
        var lab2 = RgbToLab(c2);

        // Euclidean distance in Lab color space
        return Math.Sqrt(Math.Pow(lab1.L - lab2.L, 2) + Math.Pow(lab1.A - lab2.A, 2) + Math.Pow(lab1.B - lab2.B, 2));
    }
}