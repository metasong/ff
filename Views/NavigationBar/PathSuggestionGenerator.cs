using System.Runtime.InteropServices;

namespace ff.Views.NavigationBar;

internal class PathSuggestionGenerator : ISuggestionGenerator
{
    private IContainer? CurrentContainer;

    public IEnumerable<Suggestion> GenerateSuggestions(AutocompleteContext context)
    {
        return [];
        if (context is not AutocompleteFilePathContext fileState)
        {
            return [];
        }

        CurrentContainer = fileState.CurrentContainer;

        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        var path = Cell.ToString(context.CurrentLine);
        if (string.IsNullOrWhiteSpace(path) || path[0] == '.')
        {
            path = Path.Combine(CurrentContainer.FullName, path);
        }
        //to here:the Path.IsPathRooted(path), full path
        var pathParts = path.Split(NavigationBarPanel.DirectorySeparators, StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();
        var termToComplete = "";
        for (int i = 0; i < pathParts.Length; i++)
        {
            var part = pathParts[i];
            if (Path.Exists($"{sb}"))
            {
                sb.Append(part);
                continue;
            }
            else
            {
                termToComplete = part;
            }
        }

        var availablePath = $"{sb}";

        //var last = path.LastIndexOfAny(NavigationBarPanel.DirectorySeparators);
        //var term = path.Substring(last + 1);

        // If path is /tmp/ then don't just list everything in it
        if (string.IsNullOrWhiteSpace(termToComplete))
        {
            return [];
        }


        var isCurrentContainerPath = isWindows
            ? string.Equals(path, CurrentContainer.FullName, StringComparison.InvariantCultureIgnoreCase)
            : string.Equals(path, CurrentContainer.FullName, StringComparison.InvariantCulture);

        if (isCurrentContainerPath)
        {
            // Clear suggestions
            return [];
        }

        // scan path one by one seperated by seperator, until the one not exist, which is the term to complete
        string[] suggestions = Directory.EnumerateFileSystemEntries(availablePath)
            .OrderBy(entry => !Directory.Exists(entry)) // directories first, then files
            .ThenBy(entry => Path.GetFileName(entry), isWindows ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture)
            .Select(entry =>
                Directory.Exists(entry)
                    ? Path.GetFileName(entry) + Path.DirectorySeparatorChar
                    : Path.GetFileName(entry)
            )
            .ToArray();

        string[] validSuggestions = suggestions
            .Where(
                s => s.StartsWith(
                    termToComplete,
                    isWindows
                        ? StringComparison.InvariantCultureIgnoreCase
                        : StringComparison.InvariantCulture
                )
            )
            .OrderBy(m => m.Length)
            .ToArray();

        // nothing to suggest
        if (validSuggestions.Length == 0 || validSuggestions[0].Length == termToComplete.Length)
        {
            return [];
        }

        return validSuggestions.Select(
                f => new Suggestion(termToComplete.Length, f, f)
            )
            .ToList();
    }

    public bool IsWordChar(Rune rune)
    {
        if (rune.Value == '\n')
        {
            return false;
        }

        return true;
    }
}