namespace ff.Views.CurrentFolder;

internal sealed class CurrentFolder : FrameView
{
    private readonly TableView _itemsList = new()
    {
        Width = Dim.Fill(),
        Height = Dim.Fill(),
        MultiSelect = true,
        BorderStyle = LineStyle.None
    };

    public CurrentFolder()
    {
        BorderStyle = LineStyle.None;

        Add(_itemsList);
        _itemsList.SetFocus();
    }

}