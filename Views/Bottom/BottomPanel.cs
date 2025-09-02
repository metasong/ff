namespace ff.Views.Bottom;

public class BottomPanel: View
{
    public BottomPanel(CommandView commandView, StatusView statusView)
    {
        Width = Dim.Fill();
        CanFocus = true;
        Height = 1;
        commandView.Visible = false;
        Add(commandView);
        Add(statusView);
    }
}