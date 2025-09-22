namespace ff.Views.NavigationBar;

internal class AutocompleteFilePathContext (string currentLine, int cursorPosition, IContainer CurrentContainer)
    : AutocompleteContext (Cell.ToCellList (currentLine), cursorPosition)
{
    public IContainer CurrentContainer { get; set; } = CurrentContainer;
}