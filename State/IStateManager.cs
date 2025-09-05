namespace ff.State;

public interface IStateManager
{
    public event Action<IContainer, IContainer> StateChanged;
    event Action<int, int, IItem[]> ActiveItemChanged;
    IContainer CurrentState { get; }
    bool Push(IContainer container);
    bool CanUp();
    bool CanForward();
    bool CanBack();
    bool Up();
    bool Forward();
    bool Back();
    void ChangeActiveItem(int oldIndex, int newIndex, IItem[] children);
}