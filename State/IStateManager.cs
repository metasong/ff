namespace ff.State;

public interface IStateManager
{
    public event Action<IContainer, IContainer> StateChanged;
    IContainer CurrentState { get; }
    bool Push(IContainer container);
    bool CanUp();
    bool CanForward();
    bool CanBack();
    bool Up();
    bool Forward();
    bool Back();
}