using ff.Navigator;

namespace ff.Views.NavigationBar;

public sealed class NavigationBarPanel : View
{
    private readonly INavigator navigator;
    private readonly NavigationBarTextView textView;
    private readonly Button _btnUp;
    private readonly Button _btnForward;
    private readonly Button _btnBack;
    public NavigationBarPanel(IStateManager state, INavigator navigator, ILogger<NavigationBarPanel> logger)
    {
        this.navigator = navigator;
        Width = Dim.Fill();
        Height = Dim.Fill();

        _btnUp = new() { X = 0, Y = 0, NoPadding = true };
        _btnUp.Text = "▲";
        _btnUp.Accepting += (s, e) =>
        {
            navigator.GoToParentAsync();
            e.Handled = true;
        };

        _btnBack = new() { X = Pos.Right(_btnUp), Y = 0, NoPadding = true };
        _btnBack.Text = Glyphs.LeftArrow + "-";
        _btnBack.Accepting += (s, e) =>
        {
            navigator.BackAsync();
            e.Handled = true;
        };

        _btnForward = new() { X = Pos.Right(_btnBack) , Y = 0, NoPadding = true };
        _btnForward.Text = "-" + Glyphs.RightArrow;
        _btnForward.Accepting += (s, e) =>
        {
            this.navigator.ForwardAsync();
            e.Handled = true;
        };

        textView = new NavigationBarTextView(state){X = Pos.Right(_btnForward)};

        Add(_btnForward);
        Add(_btnBack);
        Add(_btnUp);
        Add(textView);
    }

    internal static char[] DirectorySeparators =
    [
        Path.AltDirectorySeparatorChar,
        Path.DirectorySeparatorChar
    ];

}