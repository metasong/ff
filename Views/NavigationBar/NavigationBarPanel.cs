namespace ff.Views.NavigationBar;

public sealed class NavigationBarPanel : View
{
    private readonly IStateManager state;
    private readonly NavigationBarTextView textView;
    private readonly Button _btnUp;
    private readonly Button _btnForward;
    private readonly Button _btnBack;
    public NavigationBarPanel(IStateManager state, ILogger<NavigationBarPanel> logger)
    {
        this.state = state;
        Width = Dim.Fill();
        Height = Dim.Fill();

        _btnUp = new() { X = 0, Y = 0, NoPadding = true };
        _btnUp.Text = "▲";
        _btnUp.Accepting += (s, e) =>
        {
            state.Up();
            e.Handled = true;
        };

        _btnBack = new() { X = Pos.Right(_btnUp), Y = 0, NoPadding = true };
        _btnBack.Text = Glyphs.LeftArrow + "-";
        _btnBack.Accepting += (s, e) =>
        {
            state.Back();
            e.Handled = true;
        };

        _btnForward = new() { X = Pos.Right(_btnBack) , Y = 0, NoPadding = true };
        _btnForward.Text = "-" + Glyphs.RightArrow;
        _btnForward.Accepting += (s, e) =>
        {
            state.Forward();
            e.Handled = true;
        };

        state.StateChanged += State_StateChanged;


        textView = new NavigationBarTextView(state){X = Pos.Right(_btnForward)};
        UpdateButtonStatus();
        Add(_btnForward);
        Add(_btnBack);
        Add(_btnUp);
        Add(textView);
    }

    private void State_StateChanged(IContainer arg1, IContainer arg2)
    {
        UpdateButtonStatus();
    }

    private void UpdateButtonStatus()
    {
        _btnUp.Enabled = state.CanUp();
        _btnBack.Enabled = state.CanBack();
        _btnForward.Enabled = state.CanForward();
    }

    internal static char[] DirectorySeparators =
    [
        Path.AltDirectorySeparatorChar,
        Path.DirectorySeparatorChar
    ];

}