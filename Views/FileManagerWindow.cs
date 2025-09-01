using ff.Views.CurrentFolder;
using ff.Views.NavigationBar;
using ff.Views.Preview;

namespace ff.Views;

public class FileManagerWindow : Window
{
    private readonly IStateManager state;
    private readonly CurrentFolderPanel currentFolderPanelPanel;
    private readonly PreviewPanel previewPane;
    private readonly NavigationBarPanel navigationBar;
    private readonly Spinner spinnerView;
    private Spliter _splitContainer;

    public FileManagerWindow(IStateManager state, CurrentFolderPanel currentFolderPanelPanel, PreviewPanel previewPane,NavigationBarPanel navigationBar, Spinner spinnerView)
    {
        this.state = state;
        CanFocus = true;
        this.spinnerView = spinnerView;
        this.previewPane = previewPane;
        this.navigationBar = navigationBar;
        this.currentFolderPanelPanel = currentFolderPanelPanel;

        InitializeComponents();
        SetupKeyBindings();
    }
    private void Quit() { Application.RequestStop(); }

    private void InitializeComponents()
    {
        navigationBar.Width = Dim.Fill();
        navigationBar.Height = 1;
        previewPane.Width = Dim.Fill();//Dim.Percent(50);
        BorderStyle = LineStyle.None;
        Add(navigationBar);
        //Add(currentFolderPanelPanel);
        //Add(previewPane);
        Add(spinnerView);
        //var statusBar = new StatusBar(new Shortcut[] { new(Application.QuitKey, "Quit", Quit) });

        //Add(statusBar);
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
            
        };

        Initialized += (s, e) =>
        {
            //_splitContainer.SetSplitterPos(0, Pos.Percent(50));
            //_splitContainer.Tiles.ElementAt(0).ContentView.Visible = true;
        };
        Add(_splitContainer);
        //var a = GetScheme();
        //this.SetBackgroundColor(Color.Black);
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