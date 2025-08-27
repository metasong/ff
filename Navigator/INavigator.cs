namespace ff.Navigator;

public interface INavigator
{
    Task GoToAsync(IContainer dir);
    Task BackAsync();
    Task ForwardAsync();
    Task GoToParentAsync();
    Task SelectItem(IItem item);
}