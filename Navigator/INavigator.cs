namespace ff.Navigator;

public interface INavigator
{
    Task GoToAsync(IDirectoryInfo dir);
    Task BackAsync();
    Task ForwardAsync();
    Task UpAsync();
}