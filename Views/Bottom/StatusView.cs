namespace ff.Views.Bottom;

public class StatusView : StatusBar
{
    public StatusView()
    {
        Width = Dim.Fill();
        AlignmentModes = AlignmentModes.IgnoreFirstOrLast;
        Height = Dim.Auto(
            DimAutoStyle.Auto,
            minimumContentDim: Dim.Func(() => Visible ? 1 : 0),
            maximumContentDim: Dim.Func(() => Visible ? 1 : 0));
        CanFocus = false;
        // statusBar = new(new Shortcut[] { new(Application.QuitKey, "Quit", () => Application.RequestStop()) });
    }
}