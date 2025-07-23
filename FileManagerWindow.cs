namespace TerminalFileManager;

public class FileManagerWindow : Window
{
    private TextView previewPane;


    public FileManagerWindow()
    {


        InitializeComponents();
        SetupKeyBindings();
    }

    private void InitializeComponents()
    {
        BorderStyle = LineStyle.None;


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