namespace ff.Views.Bottom;

public class StatusView : StatusBar
{
    private readonly StatusBar statusBar;

    public StatusView(IStateManager stateManager)
    {
        Width = Dim.Fill();
        AlignmentModes = AlignmentModes.IgnoreFirstOrLast;
        Height = Dim.Auto(
            minimumContentDim: Dim.Func(_ => Visible ? 1 : 0),
            maximumContentDim: Dim.Func(_ => Visible ? 1 : 0)
            );
        CanFocus = false;
        var selectedCellLabel = new Label
        {
            Text = "0/0"
        };
        statusBar = new(new Shortcut[]
        {
            new(Application.QuitKey, "Quit", () => Application.RequestStop()), new ()
            {
                CommandView = selectedCellLabel
            }
        })
        {
            AlignmentModes = AlignmentModes.IgnoreFirstOrLast
        };

        stateManager.ActiveItemChanged += ((oldIndex, newIndex, children) =>
        {
            selectedCellLabel.Text = $"{newIndex+1}/{children.Length}";
        });

        Add(statusBar);

    }

}