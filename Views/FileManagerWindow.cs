using ff.Views.Components;

namespace ff.Views;

public class FileManagerWindow : Window
{
    private readonly IStateManager stateManager;
    private readonly CurrentFolderPanel currentFolderPanelPanel;
    private readonly PreviewPanel previewPane;
    private readonly NavigationBarPanel navigationBar;
    private readonly BottomPanel bottomPanel;
    private readonly Spinner spinnerView;
    private Splitter splitContainer = null!;

    public FileManagerWindow(IStateManager stateManager, BottomPanel bottomPanel,PreviewPanel previewPane, NavigationBarPanel navigationBar, Spinner spinnerView, CurrentFolderPanel currentFolderPanelPanel)
    {
        this.stateManager = stateManager;
        this.spinnerView = spinnerView;
        this.previewPane = previewPane;
        this.navigationBar = navigationBar;
        this.bottomPanel = bottomPanel;
        this.currentFolderPanelPanel = currentFolderPanelPanel;
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        CanFocus = true;
        BorderStyle = LineStyle.None;

        navigationBar.Height = 1;


        splitContainer = new()
        {
            Y = Pos.Bottom(navigationBar),
        };
        splitContainer.AddViews(currentFolderPanelPanel, previewPane);
        splitContainer.ShowHideFolderHeader += shown =>
        {
            currentFolderPanelPanel.ShowHeader = shown;
            previewPane.ShowHeader = shown;
        };
        splitContainer.ShowHideStatusBar += shown =>
        {
            bottomPanel.Visible = shown;
        };

        splitContainer.Height = Dim.Fill(
            Dim.Func(
                _ =>
                {
                    if (bottomPanel!.NeedsLayout)
                    {
                        throw new LayoutException("DimFunc.Fn aborted because dependent View needs layout.");
                    }

                    return bottomPanel.Visible ? bottomPanel.Frame.Height : 0;
                }));

        //Initialized += (s, e) =>
        //{
        //    //_splitContainer.SetSplitterPos(0, Pos.Percent(50));
        //    //_splitContainer.Tiles.ElementAt(0).ContentView.Visible = true;
        //};
       

        bottomPanel.Y = Pos.Bottom(splitContainer);

        Add(navigationBar, splitContainer, bottomPanel);
        Add(spinnerView);

        //var theme = ThemeManager.Theme;
        //var t = ThemeManager.GetCurrentTheme();
        //var dd = ThemeManager.GetCurrentThemeName();
        //var e = t["Schemes"];
        SetupKeyBindings();
    }

    private void SetupKeyBindings()
    {
        AddCommand(Command.Quit, () =>
        {
            Application.RequestStop();
            return true;
        });

        Application.KeyBindings.Add(Key.Q.WithAlt, this,Command.Quit);

    }
}