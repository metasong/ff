using ff.Navigator;

namespace ff.Views.CurrentFolder;

public sealed class CurrentFolderPanel : View
{
    private int _currentSortColumn;
    private bool _currentSortIsAsc = true;

    private readonly IStateManager stateManager;
    private readonly INavigator navigator;
    private readonly ILogger<CurrentFolderPanel> logger;

    private readonly ItemTable itemListTable = new(true);

    public CurrentFolderPanel(IStateManager stateManager, INavigator navigator, ILogger<CurrentFolderPanel> logger)
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        CanFocus = true;
        //VerticalScrollBar.Visible = true;
        //var sch = GetScheme();
        //SetScheme(new() { Normal = new(sch.Normal.Foreground, ColorName16.Blue) });
        //BorderStyle = LineStyle.Dotted;

        this.stateManager = stateManager;
        this.navigator = navigator;
        this.logger = logger;
        Add(itemListTable);

        itemListTable.SetFocus();
        itemListTable.KeyDownHandler = ProcessKeyDown;
        itemListTable.SelectedCellChanged += SelectionChanged;

        this.stateManager.StateChanged += (oldState, newState) => {ShowData(newState); };
        ShowData(stateManager.CurrentState);
        logger.LogInformation("ItemTableView initial");
    }

    private void ShowData(IContainer state)
    {
        itemListTable.ShowData(state, _currentSortColumn, _currentSortIsAsc);
        PreviewItem(itemListTable.SelectedRow);
    }

    private void SelectionChanged(object? sender, SelectedCellChangedEventArgs e)
    {
        PreviewItem(e.NewRow);
    }

    private void PreviewItem(int row)
    {
        var selectedItem = itemListTable.TableSource.GetItem(row);
        navigator.SelectItem(selectedItem);

    }

    private bool ProcessKeyDown(Key keyNoAlt)
    {
        if (keyNoAlt == Key.CursorRight)
        {
            var item = itemListTable.TableSource.GetItem(itemListTable.SelectedRow);
            if (item is IContainer container)
            {
                navigator.GoToAsync(container);
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
        if (keyNoAlt == Key.CursorLeft)
        {

            navigator.GoToParentAsync();
            return true;
        }

        if (keyNoAlt.IsAlt)
        {
            keyNoAlt = keyNoAlt.NoAlt;
            if (keyNoAlt == Key.CursorRight)
            {
                navigator.ForwardAsync();
                return true;
            }

            if (keyNoAlt == Key.CursorLeft)
            {
                navigator.BackAsync();
                return true;
            }
        }
        return false;
    }


}