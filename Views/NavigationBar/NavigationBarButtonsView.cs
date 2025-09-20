using Terminal.Gui;

namespace ff.Views.NavigationBar;

public class NavigationBarButtonsView : View
{
    public Button BtnHome { get; }
    public Button BtnUp { get; }
    public Button BtnForward { get; }
    public Button BtnBack { get; }

    public NavigationBarButtonsView(IStateManager state)
    {
        X = 0;
        Y = 0;

        BtnHome = new() { X = 0, Y = 0, NoPadding = true, Text = $"{Glyphs.IdenticalTo}", ShadowStyle = ShadowStyle.None, };
        BtnHome.Accepting += (s, e) => { e.Handled = true; };

        BtnUp = new() { X = Pos.Right(BtnHome), Y = 0, NoPadding = true, Text = "▲",ShadowStyle = ShadowStyle.None, };
        BtnUp.Accepting += (s, e) => { state.Up(); e.Handled = true; };

        BtnBack = new() { X = Pos.Right(BtnUp), Y = 0, NoPadding = true, Text = $"{Glyphs.LeftArrow}", ShadowStyle = ShadowStyle.None, };
        BtnBack.Accepting += (s, e) => { state.Back(); e.Handled = true; };

        BtnForward = new() { X = Pos.Right(BtnBack), Y = 0, NoPadding = true, Text = $"{Glyphs.RightArrow}" , ShadowStyle = ShadowStyle.None, };
        BtnForward.Accepting += (s, e) => { state.Forward(); e.Handled = true; };

        Add(BtnHome);
        Add(BtnUp);
        Add(BtnBack);
        Add(BtnForward);

        Height = Dim.Fill();
        Width = Dim.Auto();
    }
}