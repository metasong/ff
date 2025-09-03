using ff.Views.Bottom;
using ff.Views.CurrentFolder;
using ff.Views.NavigationBar;
using ff.Views.Preview;
using Terminal.Gui.Configuration;

namespace ff.Views;

public class FileManagerWindow : Window
{
    private readonly IStateManager state;
    private readonly CurrentFolderPanel currentFolderPanelPanel;
    private readonly PreviewPanel previewPane;
    private readonly NavigationBarPanel navigationBar;
    private readonly BottomPanel bottomPanel;
    private readonly Spinner spinnerView;
    private Splitter _splitContainer;

    public FileManagerWindow(IStateManager state, CurrentFolderPanel currentFolderPanelPanel, PreviewPanel previewPane,NavigationBarPanel navigationBar, BottomPanel bottomPanel, Spinner spinnerView)
    {
        this.state = state;
        CanFocus = true;
        this.spinnerView = spinnerView;
        this.previewPane = previewPane;
        this.navigationBar = navigationBar;
        this.bottomPanel = bottomPanel;
        this.currentFolderPanelPanel = currentFolderPanelPanel;
        InitializeComponents();
        SetupKeyBindings();
    }

    private void InitializeComponents()
    {
        BorderStyle = LineStyle.None;
        navigationBar.Height = 1;
        
        _splitContainer = new()
        {
            Y = Pos.Bottom(navigationBar),
        };

        _splitContainer.AddViews(currentFolderPanelPanel, previewPane);
        _splitContainer.ShowHideFolderHeader += shown =>
        {
            currentFolderPanelPanel.ShowHeader = shown;
            previewPane.ShowHeader = shown;
        };
        _splitContainer.ShowHideStatusBar += shown =>
        {
            bottomPanel.Visible = shown;
        };

        _splitContainer.Height = Dim.Fill(
            Dim.Func(
                _ =>
                {
                    if (bottomPanel!.NeedsLayout)
                    {
                        throw new LayoutException("DimFunc.Fn aborted because dependent View needs layout.");
                    }

                    return bottomPanel.Visible ? bottomPanel.Frame.Height : 0;
                }));

        Initialized += (s, e) =>
        {
            //_splitContainer.SetSplitterPos(0, Pos.Percent(50));
            //_splitContainer.Tiles.ElementAt(0).ContentView.Visible = true;
        };
       

        bottomPanel.Y = Pos.Bottom(_splitContainer);

        Add(navigationBar, _splitContainer, bottomPanel);

        Add(spinnerView);
        var theme = ThemeManager.Theme;
        var t = ThemeManager.GetCurrentTheme();
        var dd = ThemeManager.GetCurrentThemeName();
        var e = t["Schemes"];
    }


    private void SetupKeyBindings()
    {
        // Add key bindings using the V2 approach
        AddCommand(Command.Quit, () =>
        {
            Application.RequestStop();
            return true;
        });

        // Custom key bindings
        Application.KeyBindings.Add(Key.Q.WithAlt, this,Command.Quit);

    }
}