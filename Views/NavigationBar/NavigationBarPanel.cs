namespace ff.Views.NavigationBar;

public sealed class NavigationBarPanel : View
{
    private readonly IStateManager state;
    private readonly NavigationBarTextView textView;
    private readonly Button _btnHome;
    private readonly Button _btnUp;
    private readonly Button _btnForward;
    private readonly Button _btnBack;
    public NavigationBarPanel(IStateManager state, ILogger<NavigationBarPanel> logger)
    {
        this.state = state;
        Width = Dim.Fill();
        Height = Dim.Fill();

        _btnHome = new() { X = 0, Y = 0, NoPadding = true };
        _btnHome.Text = $"{Glyphs.IdenticalTo}";
        _btnHome.Accepting += (s, e) =>
        {
            
            e.Handled = true;
        };

        _btnUp = new() { X = Pos.Right(_btnHome), Y = 0, NoPadding = true };
        _btnUp.Text = "▲";
        _btnUp.Accepting += (s, e) =>
        {
            state.Up();
            e.Handled = true;
        };

        _btnBack = new() { X = Pos.Right(_btnUp), Y = 0, NoPadding = true };
        _btnBack.Text = $"{Glyphs.LeftArrow}";// + "-";
        _btnBack.Accepting += (s, e) =>
        {
            state.Back();
            e.Handled = true;
        };

        _btnForward = new() { X = Pos.Right(_btnBack) , Y = 0, NoPadding = true };
        _btnForward.Text = $"{Glyphs.RightArrow}";
        _btnForward.Accepting += (s, e) =>
        {
            state.Forward();
            e.Handled = true;
        };

        state.ContainerChanged += ContainerContainerChanged;


        textView = new NavigationBarTextView(state){X = Pos.Right(_btnForward)};
        UpdateButtonStatus();
        Add(_btnHome);
        Add(_btnForward);
        Add(_btnBack);
        Add(_btnUp);
        Add(textView);
    }

    private void ContainerContainerChanged(IContainer arg1, IContainer arg2)
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