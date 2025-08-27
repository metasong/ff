using ff.State.FileDataSystem;

namespace ff.Views.NavigationBar;

internal class AutocompleteFilePathContext (string currentLine, int cursorPosition, IContainer state)
    : AutocompleteContext (Cell.ToCellList (currentLine), cursorPosition)
{
    public IContainer State { get; set; } = state;
}