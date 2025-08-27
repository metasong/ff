namespace ff.Views.NavigationBar;

/// <summary>
/// the subview when user want to see/modify the path of current folder, without the selected item part.
/// </summary>
internal sealed class NavigationBarTextView: View
{
    private readonly IStateManager stateManager;
    public IFileSystem FileSystem { get; }

    private readonly TextField tbPath;
    private string feedback;

    /// <summary>
    ///     Characters to prevent entry into <see cref="tbPath"/>. Note that this is not using
    ///     <see cref="System.IO.Path.GetInvalidFileNameChars"/> because we do want to allow directory separators, arrow keys
    ///     etc.
    /// </summary>
    private static readonly char[] BadChars = ['"', '<', '>', '|', '*', '?'];

    public NavigationBarTextView(IStateManager stateManager)
    {
        this.stateManager = stateManager;

        BorderStyle = LineStyle.None;
        tbPath = new() { Width = Dim.Fill(), CaptionColor = new(Color.Black) };
        tbPath.KeyDown += (s, k) =>
        {
            ClearFeedback();
            AcceptIf(k, KeyCode.Enter);
            SuppressIfBadChar(k);
        };
        tbPath.Autocomplete = new AppendAutocomplete(tbPath);
        tbPath.Autocomplete.SuggestionGenerator = new FilepathSuggestionGenerator();

        tbPath.TextChanged += (s, e) => PathChanged();
        Add(tbPath);

        this.stateManager.StateChanged += (oldState, newState) =>
        {
            tbPath.Autocomplete.ClearSuggestions();
            Path = newState.FullName;

            tbPath.Autocomplete.GenerateSuggestions(
                new AutocompleteFilepathContext(tbPath.Text, tbPath.CursorPosition, newState)
            );
        };
    }

    public void OnLoaded()
    {
        SetStyle();
        tbPath.Autocomplete.Scheme = new(tbPath.GetScheme())
        {
            Normal = new(Color.Black, tbPath.GetAttributeForRole(VisualRole.Normal).Background)
        };

        if (tbPath.Text.Length <= 0)
        {
            Path = FileSystem.Directory.GetCurrentDirectory();
        }
    }
    private void ClearFeedback() { feedback = null; }

    private void AcceptIf(Key key, KeyCode isKey)
    {
        if (!key.Handled && key.KeyCode == isKey)
        {
            key.Handled = true;

            // User hit Enter in text box so probably wants the
            // contents of the text box as their selection not
            // whatever lingering selection is in TableView
            //Accept(false);
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

        var dir = FileSystem.ToDirectoryInfo(path);

        if (dir.Exists)
        {
            //stateManager.Push();
        }
        else if (dir.Parent?.Exists ?? false)
        {
            //PushState(dir.Parent, true, false);
        }

        tbPath.Autocomplete.GenerateSuggestions(
            new AutocompleteFilepathContext(tbPath.Text, tbPath.CursorPosition, stateManager.CurrentState)
        );
    }
    
    private void SuppressIfBadChar(Key k)
    {
        // don't let user type bad letters
        var ch = (char)k;

        if (BadChars.Contains(ch))
        {
            k.Handled = true;
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