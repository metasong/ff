using ff.State;

namespace ff.Views.NavigationBar;

internal sealed class NavigationBar(IStateManager state) : View
{
    private readonly NavigationBarTextView textView = new(state);
    internal static char[] DirectorySeparators =
    [
        Path.AltDirectorySeparatorChar,
        Path.DirectorySeparatorChar
    ];
    public void OnLoaded()
    {
        textView.OnLoaded();
    }
}