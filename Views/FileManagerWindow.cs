using ff.State;
using ff.Views.CurrentFolder;
using ff.Views.Preview;

namespace ff.Views;

public class FileManagerWindow : Window
{
    private readonly IStateManager state;
    private readonly ItemTableView itemTableViewPanel;
    private readonly PreviewPanel previewPane;
    private readonly Spinner spinnerView;

    public FileManagerWindow(IStateManager state, ItemTableView itemTableViewPanel, PreviewPanel previewPane, Spinner spinnerView)
    {
        this.state = state;
        this.itemTableViewPanel = itemTableViewPanel;
        this.previewPane = previewPane;
        this.spinnerView = spinnerView;
        InitializeComponents();
        SetupKeyBindings();
    }
    private void Quit() { Application.RequestStop(); }

    private void InitializeComponents()
    {
        BorderStyle = LineStyle.None;
        Add(itemTableViewPanel);
        //Add(previewPane);
        Add(spinnerView);
        var statusBar = new StatusBar(new Shortcut[] { new(Application.QuitKey, "Quit", Quit) });

        Add(statusBar);
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
        KeyBindings.Add(Key.Q, Command.Quit);

    }

}