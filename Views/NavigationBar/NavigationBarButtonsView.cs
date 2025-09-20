namespace ff.Views.NavigationBar;

public class NavigationBarButtonsView : View
{
    public Button BtnHome { get; }
    public Button BtnUp { get; }
    public Button BtnForward { get; }
    public Button BtnBack { get; }

    public NavigationBarButtonsView(IStateManager state)
    {
        BtnHome = CreateButton(0, 0, $"{Glyphs.IdenticalTo}", (s, e) => { 
            e.Handled = true;
        });

        BtnUp = CreateButton(Pos.Right(BtnHome), 0, "▲", (s, e) => { 
            state.Up(); 
            e.Handled = true; 
        });
        BtnBack = CreateButton(Pos.Right(BtnUp), 0, $"{Glyphs.LeftArrow}", (s, e) => { 
            state.Back(); 
            e.Handled = true; 
        });
        BtnForward = CreateButton(Pos.Right(BtnBack), 0, $"{Glyphs.RightArrow}", (s, e) => {
            state.Forward(); 
            e.Handled = true; 
        });

        Add(BtnHome);
        Add(BtnUp);
        Add(BtnBack);
        Add(BtnForward);

        Height = Dim.Fill();
        Width = Dim.Auto();
    }

    private static Button CreateButton(Pos x, Pos y, string text, EventHandler<CommandEventArgs> onAccept)
    {
        var btn = new Button
        {
            X = x,
            Y = y,
            NoPadding = true,
            ShadowStyle = ShadowStyle.None,
            Text = text
        };

        btn.Accepting += onAccept;
        return btn;
    }
}