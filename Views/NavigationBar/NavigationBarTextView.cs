namespace ff.Views.NavigationBar;

/// <summary>
/// the subview when user want to see/modify the path of current folder, without the selected item part.
/// </summary>
internal sealed class NavigationBarTextView : View
{
    private readonly IStateManager stateManager;
    public IFileSystem FileSystem { get; }

    private TextField tbPath;
    private string feedback;

    /// <summary>
    ///     Characters to prevent entry into <see cref="tbPath"/>. Note that this is not using
    ///     <see cref="System.IO.Path.GetInvalidFileNameChars"/> because we do want to allow directory separators, arrow keys
    ///     etc.
    /// </summary>
    private static readonly char[] BadChars = ['"', '<', '>', '|', '*', '?'];

    public NavigationBarTextView(IStateManager stateManager)
    {
        CanFocus = true;
        this.stateManager = stateManager;
        InitializeComponents();
        this.stateManager.ContainerChanged += (oldState, newState) =>
        {
            tbPath!.Autocomplete.ClearSuggestions();
            Path = newState.FullName;

            tbPath.Autocomplete.GenerateSuggestions(
                new AutocompleteFilePathContext(tbPath.Text, tbPath.CursorPosition, newState)
            );
        };

    }

    public void InitializeComponents()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();

        BorderStyle = LineStyle.None;
        tbPath = new() { Width = Dim.Fill(), Height = Dim.Fill(), CaptionColor = new(Color.Black) };
        Add(tbPath);

        tbPath.KeyDown += (s, k) =>
        {
            ClearFeedback();
            AcceptIf(k, KeyCode.Enter);
            SuppressIfBadChar(k);
        };
        tbPath.Autocomplete = new AppendAutocomplete(tbPath);
        tbPath.Autocomplete.SuggestionGenerator = new PathSuggestionGenerator();

        tbPath.TextChanged += (s, e) => PathChanged();

        SetStyle();
        tbPath.Autocomplete.Scheme = new(tbPath.GetScheme())
        {
            Normal = new(Color.Black, tbPath.GetAttributeForRole(VisualRole.Normal).Background)
        };


        Path = stateManager.CurrentContainer.FullName;
    }
    private void ClearFeedback() { feedback = null; }

    private void AcceptIf(Key key, KeyCode isKey)
    {
        if (!key.Handled && key.KeyCode == isKey)
        {
            key.Handled = true;

            stateManager.GoTo(SystemSwitch.GetState(tbPath.Text));
        }
    }
    private void SetStyle()
    {

    }

    private void PathChanged()
    {
        var path = tbPath.Text;

        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        tbPath.Autocomplete.GenerateSuggestions(
            new AutocompleteFilePathContext(tbPath.Text, tbPath.CursorPosition, stateManager.CurrentContainer)
        );
    }

    private void SuppressIfBadChar(Key k)
    {
        // Only check printable keys, otherwise the End key would be converted to ", which is BadChars, so can not move cursor to end
        if (k.KeyCode is >= KeyCode.Space and <= KeyCode.Z) 
        {
            var ch = (char)k;
            if (BadChars.Contains(ch))
            {
                k.Handled = true;
            }
        }
    }


    public string Path
    {
        get => tbPath.Text;
        set
        {
            tbPath.Text = value;
            tbPath.MoveEnd();
        }
    }
}