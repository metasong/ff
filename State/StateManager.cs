namespace ff.State;

internal class StateManager: IStateManager
{
    public StateManager(IContainer currentContainer)
    {
        CurrentState = currentContainer;
    }

    private readonly Stack<IContainer> backStack = new();
    private readonly Stack<IContainer> forwardStack = new();
    private IContainer currentContainer;

    public IContainer CurrentState
    {
        get => currentContainer;
        private set
        {
            var last = currentContainer;
            currentContainer = value;
            StateChanged?.Invoke(last, currentContainer);
        }
    }

    public event Action<IContainer, IContainer>? StateChanged;

    public IContainer? LastState
    {
        get
        {
            if (backStack.TryPeek(out var last))
                return last;
            return null;
        }
    }

    public bool Back()
    {
        if (backStack.Count == 0) return false;

        var last = backStack.Pop();
        forwardStack.Push(CurrentState);
        CurrentState = last;
        return true;
    }


    public bool Forward()
    {
        if (forwardStack.Count == 0) return false;

        backStack.Push(CurrentState);
        var current = forwardStack.Pop();
        CurrentState = current;
        return true;
    }


    public bool Up()
    {
        var parent = currentContainer.GetParent();

        if (parent is { })
        {
            backStack.Push(currentContainer);
            CurrentState = parent;
            return true;
        }

        return false;
    }

    public bool CanBack() { return backStack.Count > 0; }
    public bool CanForward() { return  forwardStack.Count > 0; }

    public bool CanUp() => CurrentState.GetParent() != null;

    public void ClearForward() {  forwardStack.Clear(); }

    public bool Push(IContainer container)
    {
        if (CurrentState.Equals(LastState))
            return false;

        backStack.Push(CurrentState);
        CurrentState = container;
        return true;
    }
}