using ff.Views.Preview;

namespace ff.Navigator;

public class Navigator(IStateManager stateManager, IPreviewPanel previewPanel): INavigator
{
    public Task GoToAsync(IContainer dir)
    {
        stateManager.Push(dir);
        return Task.CompletedTask;
    }

    public Task BackAsync()
    {
        stateManager.Back();
        return Task.CompletedTask;

    }

    public Task ForwardAsync()
    {
        stateManager.Forward();
        return Task.CompletedTask;

    }

    public Task GoToParentAsync()
    {
        stateManager.Up();
        return Task.CompletedTask;

    }

    public Task SelectItem(IItem item)
    {
        previewPanel.Preview(item);
        return Task.CompletedTask;
    }
}