using ff.Views.CurrentFolder;
using ff.Views.Preview;

namespace ff.Views;

public class FileManagerWindow : Window
{
    private readonly IStateManager state;
    private readonly CurrentFolderPanel currentFolderPanelPanel;
    private readonly PreviewPanel previewPane;
    private readonly Spinner spinnerView;

    public FileManagerWindow(IStateManager state, CurrentFolderPanel currentFolderPanelPanel, PreviewPanel previewPane, Spinner spinnerView)
    {
        this.state = state;
        CanFocus = true;
        this.currentFolderPanelPanel = currentFolderPanelPanel;
        currentFolderPanelPanel.Width = Dim.Percent(50);
        this.previewPane = previewPane;
        previewPane.X = Pos.Percent(50);
        previewPane.Width = Dim.Percent(50);
        this.spinnerView = spinnerView;
        InitializeComponents();
        SetupKeyBindings();
    }
    private void Quit() { Application.RequestStop(); }

    private void InitializeComponents()
    {
        BorderStyle = LineStyle.None;
        Add(currentFolderPanelPanel);
        Add(previewPane);
        Add(spinnerView);
        //var statusBar = new StatusBar(new Shortcut[] { new(Application.QuitKey, "Quit", Quit) });

        //Add(statusBar);

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