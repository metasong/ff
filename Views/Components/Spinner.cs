namespace ff.Views.Components;

public class Spinner:SpinnerView
{
    public Spinner()
    {
        X = Pos.Align(Alignment.Start, AlignmentModes.AddSpaceBetweenItems, 32);
        Y = Pos.AnchorEnd(1);
        Visible = false;
    }

    public void Show()
    {
        //Application.Invoke();
        Visible = true;
        NeedsDraw = true;
    }

    public void Hide()
    {
        Visible = false;
    }
}