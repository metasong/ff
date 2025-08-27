namespace ff.Views.Preview;

public static class ViewHelper
{
    /// <summary>
    /// ColorName16.Blue
    /// </summary>
    /// <param name="color"></param>
    public static void SetBackgroundColor(this View view, Color color)
    {
        var sch = view.GetScheme();
        view.SetScheme(new() { Normal = new(sch.Normal.Foreground, color) });
    }
}
