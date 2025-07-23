namespace ff.State;

internal class StateManager
{
    internal ff.State.State State { get; private set; }
    /// <summary>Changes the file manager such that <paramref name="d"/> is being explored.</summary>
    /// <param name="d"></param>
    /// <param name="addCurrentStateToHistory"></param>
    /// <param name="setPathText"></param>
    /// <param name="clearForward"></param>
    /// <param name="pathText">Optional alternate string to set path to.</param>
    internal void PushState(
        IDirectoryInfo d,
        bool addCurrentStateToHistory,
        bool setPathText = true,
        bool clearForward = true,
        string? pathText = null
    )
    {
        // no change of state
        if (d == State?.Directory)
        {
            return;
        }

        if (d.FullName == State?.Directory.FullName)
        {
            return;
        }

        PushState(
            new ff.State.State(d, this),
            addCurrentStateToHistory,
            setPathText,
            clearForward,
            pathText
        );
    }
}