using ff.State;

namespace ff.Views.NavigationBar;

/// <summary>
/// the subview when user want to see/modify the path of current folder
/// </summary>
internal sealed class NavigationBarTextView: View
{
    private readonly IFileSystem _fileSystem;
    private readonly TextField _tbPath;
    private string _feedback;

    /// <summary>
    ///     Characters to prevent entry into <see cref="_tbPath"/>. Note that this is not using
    ///     <see cref="System.IO.Path.GetInvalidFileNameChars"/> because we do want to allow directory separators, arrow keys
    ///     etc.
    /// </summary>
    private static readonly char[] BadChars = ['"', '<', '>', '|', '*', '?'];

    public NavigationBarTextView(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
        BorderStyle = LineStyle.None;
        _tbPath = new() { Width = Dim.Fill(), CaptionColor = new(Color.Black) };
        _tbPath.KeyDown += (s, k) =>
        {
            ClearFeedback();

            AcceptIf(k, KeyCode.Enter);

            SuppressIfBadChar(k);
        };
        _tbPath.Autocomplete = new AppendAutocomplete(_tbPath);
        _tbPath.Autocomplete.SuggestionGenerator = new FilepathSuggestionGenerator();

        _tbPath.TextChanged += (s, e) => PathChanged();
        Add(_tbPath);
    }

    public void OnLoaded()
    {
        SetStyle();
        _tbPath.Autocomplete.Scheme = new(_tbPath.GetScheme())
        {
            Normal = new(Color.Black, _tbPath.GetAttributeForRole(VisualRole.Normal).Background)
        };

        if (_tbPath.Text.Length <= 0)
        {
            Path = _fileSystem.Directory.GetCurrentDirectory();
        }
    }
    private void ClearFeedback() { _feedback = null; }

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
        // avoid re-entry
        if (_pushingState)
        {
            return;
        }

        var path = _tbPath.Text;

        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var dir = _fileSystem.ToDirectoryInfo(path);

        if (dir.Exists)
        {
            PushState(dir, true, false);
        }
        else if (dir.Parent?.Exists ?? false)
        {
            PushState(dir.Parent, true, false);
        }

        _tbPath.Autocomplete.GenerateSuggestions(
            new AutocompleteFilepathContext(_tbPath.Text, _tbPath.CursorPosition, StateManager)
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
        get => _tbPath.Text;
        set
        {
            _tbPath.Text = value;
            _tbPath.MoveEnd();
        }
    }
}