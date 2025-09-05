namespace ff.State;

internal class StateManager: IStateManager
{
    public StateManager(IContainer currentContainer)
    {
        CurrentContainer = currentContainer;
    }

    private readonly Stack<IContainer> backStack = new();
    private readonly Stack<IContainer> forwardStack = new();
    private IContainer currentContainer;

    public IContainer CurrentContainer
    {
        get => currentContainer;
        private set
        {
            var last = currentContainer;
            currentContainer = value;
            ContainerChanged?.Invoke(last, currentContainer);
        }
    }

    public event Action<IContainer, IContainer>? ContainerChanged;

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
        forwardStack.Push(CurrentContainer);
        CurrentContainer = last;
        return true;
    }


    public bool Forward()
    {
        if (forwardStack.Count == 0) return false;

        backStack.Push(CurrentContainer);
        var current = forwardStack.Pop();
        CurrentContainer = current;
        return true;
    }


    public bool Up()
    {
        var parent = currentContainer.GetParent();

        if (parent is { })
        {
            backStack.Push(currentContainer);
            CurrentContainer = parent;
            return true;
        }

        return false;
    }

    public bool CanBack() { return backStack.Count > 0; }
    public bool CanForward() { return  forwardStack.Count > 0; }

    public bool CanUp() => CurrentContainer.GetParent() != null;

    public void ClearForward() {  forwardStack.Clear(); }

    public bool GoTo(IContainer container)
    {
        if (CurrentContainer.Equals(LastState))
            return false;

        backStack.Push(CurrentContainer);
        CurrentContainer = container;
        return true;
    }
    public event Action<int, int, IItem[]>? ActiveItemChanged;

    public void ChangeActiveItem(int oldIndex, int newIndex, IItem[] children)
    {
        ActiveItemChanged?.Invoke(oldIndex, newIndex, children);
    }
}