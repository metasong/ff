namespace ff.Views.NavigationBar;

internal sealed class NavigationBar(IFileSystem fileSystem) : View
{
    private readonly NavigationBarTextView textView = new(fileSystem);

    public void OnLoaded()
    {
        textView.Onloaded();
    }
}