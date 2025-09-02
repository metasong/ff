namespace ff.Views;

public sealed class Splitter : View
{
    private  bool folderHeaderShown;
    private  bool statusBarShown;
    private readonly TileView tileView = new TileView() { Width = Dim.Fill(), Height = Dim.Fill() };
    private readonly Button showHideFolderPanelHeader;
    private readonly Button showHideStatusPanelHeader;
    public Splitter(bool folderHeaderShown = true, bool statusBarShown = true)
    {
        this.folderHeaderShown = folderHeaderShown;
        this.statusBarShown = statusBarShown;
        CanFocus = true;
        Width = Dim.Fill();
        Height = Dim.Fill(); //Dim.Fill(Dim.Func(() => IsInitialized ? _btnOk.Frame.Height : 1))
        Add(tileView);

        showHideFolderPanelHeader = new Button() { Text = $"{getButtonText(folderHeaderShown)}", NoDecorations = true, Arrangement = ViewArrangement.Overlapped, ShadowStyle = ShadowStyle.None, Visible = false};
        showHideFolderPanelHeader.MouseClick += ShowHideFolderPanelHeader_MouseClick;

        showHideStatusPanelHeader = new Button() { Text = $"{getButtonText(folderHeaderShown)}", NoDecorations = true, Arrangement = ViewArrangement.Overlapped, ShadowStyle = ShadowStyle.None, Visible = false, Y = Pos.Bottom(tileView) -1 };
        showHideStatusPanelHeader.MouseClick += ShowHideStatusBar_MouseClick;

        Add(showHideFolderPanelHeader);
        Add(showHideStatusPanelHeader);

    }

    private string getButtonText(bool shown) => shown ? $"{Glyphs.Collapse}" : $"{Glyphs.Expand}";

    private void ShowHideFolderPanelHeader_MouseClick(object? sender, MouseEventArgs e)
    {
        folderHeaderShown = !folderHeaderShown;
        showHideFolderPanelHeader.Text = getButtonText(folderHeaderShown);
        ShowHideFolderHeader?.Invoke(folderHeaderShown);
    }

    private void ShowHideStatusBar_MouseClick(object? sender, MouseEventArgs e)
    {
        statusBarShown = !statusBarShown;
        showHideStatusPanelHeader.Text = getButtonText(statusBarShown);
        ShowHideStatusBar?.Invoke(statusBarShown);
    }

    public event Action<bool> ShowHideFolderHeader;
    public event Action<bool> ShowHideStatusBar;


    public void AddViews(View leftView, View rightView)
    {
        var tiles = tileView.Tiles.ToArray()!;

        tiles[0].ContentView?.Add(leftView);
        tiles[1].ContentView?.Add(rightView);

        var obj = tileView.SubViews.ToArray()[1] as LineView;
        obj.MouseEnter += (sender, e) =>
        {
            showHideFolderPanelHeader.X = tileView.SplitterDistances.ToArray()[0];
            showHideFolderPanelHeader.Visible = true;

            showHideStatusPanelHeader.X = tileView.SplitterDistances.ToArray()[0];
            showHideStatusPanelHeader.Visible = true;
        };

        obj.MouseLeave += (sender, e) =>
        {
            showHideFolderPanelHeader.Visible = false;
            showHideStatusPanelHeader.Visible = false;
        };

    }

}