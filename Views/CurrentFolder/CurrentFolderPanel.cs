namespace ff.Views.CurrentFolder;

public sealed class CurrentFolderPanel : View
{
    private readonly IStateManager stateManager;
    private readonly ILogger<CurrentFolderPanel> logger;
    private readonly ItemTable itemListTable = new(true) { ShowSelectionBox = true };

    public CurrentFolderPanel(IStateManager stateManager, ILogger<CurrentFolderPanel> logger)
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        CanFocus = true;
        //VerticalScrollBar.Visible = true;
        //var sch = GetScheme();
        //SetScheme(new() { Normal = new(sch.Normal.Foreground, ColorName16.Blue) });
        //BorderStyle = LineStyle.Dotted;

        this.stateManager = stateManager;
        this.logger = logger;
        Add(itemListTable);
        itemListTable.SetFocus();
        itemListTable.KeyDownHandler = ProcessKeyDown;
        itemListTable.SelectedCellChanged += SelectionChanged;
        itemListTable.CellActivated += ItemListTable_CellActivated;

        this.stateManager.StateChanged += (oldState, newState) => {ShowData(newState); };
        ShowData(stateManager.CurrentState);
        logger.LogInformation("ItemTableView initial");

    }

    public bool ShowHeader
    {
        get => itemListTable.ShowHeader;
        set => itemListTable.ShowHeader = value;
    }

    private void ShowData(IContainer state)
    {
        itemListTable.ShowData(state);
        PreviewItem(-1, itemListTable.SelectedRow);
    }

    private void SelectionChanged(object? sender, SelectedCellChangedEventArgs e)
    {
        PreviewItem(e.OldRow, e.NewRow);
    }

    private void PreviewItem(int old, int row)
    {
        //var selectedItem = itemListTable.TableSource.GetChild(row);
        stateManager.ChangeActiveItem(old,row, itemListTable.TableSource.Container.Children);

    }

    private void ItemListTable_CellActivated(object? sender, CellActivatedEventArgs e)
    {
        var item = itemListTable.TableSource.GetChild(e.Row);
        Open(item);
    }

    private bool ProcessKeyDown(Key keyNoAlt)
    {
        if (keyNoAlt == Key.CursorRight)
        {
            var item = itemListTable.TableSource.GetChild(itemListTable.SelectedRow);
            return Open(item);
        }
        if (keyNoAlt == Key.CursorLeft)
        {

            stateManager.Up();
            return true;
        }

        if (keyNoAlt == Key.CursorUp)
        {

            return false; // let base table to do 
        }

        if (keyNoAlt.IsAlt)
        {
            keyNoAlt = keyNoAlt.NoAlt;
            if (keyNoAlt == Key.CursorRight)
            {
                stateManager.Forward();
                return true;
            }

            if (keyNoAlt == Key.CursorLeft)
            {
                stateManager.Back();
                return true;
            }
        }
        return false;
    }

    private bool Open(IItem item)
    {
        if (item is IContainer container)
        {
           stateManager.Push(container);
            return true;
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = item.FullName,
                UseShellExecute = true // Important: allows Windows to use file associations
            };

            Process.Start(startInfo);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"error when start '{item.FullName}'");
        }

        return false;
    }
}