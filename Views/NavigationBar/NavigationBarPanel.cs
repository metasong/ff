namespace ff.Views.NavigationBar;

public sealed class NavigationBarPanel : View
{
    private readonly NavigationBarTextView textView;

    public NavigationBarPanel(IStateManager state)
    {
        textView = new NavigationBarTextView(state);
        Width = Dim.Fill();
        Height = Dim.Fill();
        Add(textView);
    }

    internal static char[] DirectorySeparators =
    [
        Path.AltDirectorySeparatorChar,
        Path.DirectorySeparatorChar
    ];
    //public void OnLoaded()
    //{
    //    textView.OnLoaded();
    //}
}