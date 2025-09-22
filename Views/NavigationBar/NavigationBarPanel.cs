namespace ff.Views.NavigationBar;

public sealed class NavigationBarPanel : View
{
    private readonly IStateManager state;
    private readonly NavigationBarTextView textView;
    private readonly NavigationBarButtonsView buttonsView;

    public NavigationBarPanel(IStateManager state, ILogger<NavigationBarPanel> logger)
    {
        this.state = state;
        CanFocus = true;
        Width = Dim.Fill();
        Height = Dim.Fill();

        buttonsView = new NavigationBarButtonsView(state);
        textView = new NavigationBarTextView(state) { X = Pos.Right(buttonsView) + 1};

        state.ContainerChanged += ContainerContainerChanged;

        UpdateButtonStatus();
        Add(buttonsView);
        Add(textView);
    }

    private void ContainerContainerChanged(IContainer arg1, IContainer arg2)
    {
        UpdateButtonStatus();
    }

    private void UpdateButtonStatus()
    {
        buttonsView.BtnUp.Enabled = state.CanUp();
        buttonsView.BtnBack.Enabled = state.CanBack();
        buttonsView.BtnForward.Enabled = state.CanForward();
    }

    internal static char[] DirectorySeparators =
    [
        Path.AltDirectorySeparatorChar,
        Path.DirectorySeparatorChar
    ];
}