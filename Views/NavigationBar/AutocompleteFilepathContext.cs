using System.Runtime.InteropServices;
using ff.State;
using ff.State.FileDataSystem;
using ff.State.FileSystem;

namespace ff.Views.NavigationBar;

internal class AutocompleteFilepathContext (string currentLine, int cursorPosition, IContainer state)
    : AutocompleteContext (Cell.ToCellList (currentLine), cursorPosition)
{
    public FileSystemState State { get; set; } = (FileSystemState)state;
}

internal class FilepathSuggestionGenerator : ISuggestionGenerator
{
    private FileSystemState? state;

    public IEnumerable<Suggestion> GenerateSuggestions (AutocompleteContext context)
    {
        if (context is AutocompleteFilepathContext fileState)
        {
            state = fileState.State;
        }

        if (state is null)
        {
            return [];
        }

        var path = Cell.ToString (context.CurrentLine);
        var last = path.LastIndexOfAny (NavigationBar.DirectorySeparators);

        if (string.IsNullOrWhiteSpace (path) || !Path.IsPathRooted (path))
        {
            return [];
        }

        var term = path.Substring (last + 1);

        // If path is /tmp/ then don't just list everything in it
        if (string.IsNullOrWhiteSpace (term))
        {
            return [];
        }

        if (term.Equals (state?.Directory?.Name))
        {
            // Clear suggestions
            return [];
        }

        var isWindows = RuntimeInformation.IsOSPlatform (OSPlatform.Windows);

        string [] suggestions = state.Children
                                     .Select (
                                              e => e.IsLeaf
                                                       ? e.Name
                                                       : e.Name + Path.DirectorySeparatorChar
                                             )
                                     .ToArray ();

        string [] validSuggestions = suggestions
                                     .Where (
                                             s => s.StartsWith (
                                                                term,
                                                                isWindows
                                                                    ? StringComparison.InvariantCultureIgnoreCase
                                                                    : StringComparison.InvariantCulture
                                                               )
                                            )
                                     .OrderBy (m => m.Length)
                                     .ToArray ();

        // nothing to suggest
        if (validSuggestions.Length == 0 || validSuggestions [0].Length == term.Length)
        {
            return [];
        }

        return validSuggestions.Select (
                                        f => new Suggestion (term.Length, f, f)
                                       )
                               .ToList ();
    }

    public bool IsWordChar (Rune rune)
    {
        if (rune.Value == '\n')
        {
            return false;
        }

        return true;
    }
}
