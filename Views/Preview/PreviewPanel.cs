using ff.Views.Preview.Config;

namespace ff.Views.Preview;

public class PreviewPanel : View, IPreviewPanel
{
    private readonly ILogger<PreviewPanel> logger;
    private readonly ItemTable itemListTable = new() { ShowSelectionBox = false};
    private readonly ImageView imageView;
    private readonly PreviewConfig config = new ();

    private readonly IPreviewer[] Previewers; 

    public PreviewPanel(IStateManager stateManager, ILogger<PreviewPanel> logger)
    {
        this.logger = logger;
        Width = Dim.Fill();
        Height = Dim.Fill();
        //BorderStyle = LineStyle.Dashed;
        //this.SetBackgroundColor(ColorName16.Blue);
        imageView = new(config.Image);
        var textItemView = new TextItemView(config.Text, logger);
        stateManager.ActiveItemChanged += Navigator_ActiveItemChanged;
        Previewers = [itemListTable, imageView, textItemView, new CommonItemView()];
    }


    private void Navigator_ActiveItemChanged(int old, int newIndex, IItem[] children)
    {
        if(newIndex > -1 && newIndex < children.Length)
            Preview(children[newIndex]);
    }

    public bool ShowHeader
    {
        get => itemListTable.ShowHeader;
        set => itemListTable.ShowHeader = value;
    }

    public void Preview(IItem item)
    {
        foreach (var previewer in Previewers)
        {
            if (previewer.CanView(item))
            {
                try
                {
                    SwitchToSubview((View)previewer);
                    previewer.View(item);
                    return;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Preview Error");
                    MessageBox.ErrorQuery("Error", e.Message, "OK");
                    return;
                }
            }
        }
    }

    private void SwitchToSubview(View view)
    {
        if (SubViews.FirstOrDefault() == imageView) imageView.HideImage();
        RemoveAll();
        Add(view);
    }
    
}