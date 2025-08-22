using ff.State;

namespace ff.Navigator;

public class Navigator(IStateManager stateManager): INavigator
{
    public Task GoToAsync(IDirectoryInfo dir)
    {
        throw new NotImplementedException();
    }

    public Task BackAsync()
    {
        throw new NotImplementedException();
    }

    public Task ForwardAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpAsync()
    {
        throw new NotImplementedException();
    }
}