using ff.Views.Preview.Config;

namespace ff.Views.Preview;

public class PreviewPanel : View, IPreviewPanel
{
    private readonly ItemTable itemListTable = new();
    private readonly ImageView imageView;
    private readonly PreviewConfig previewConfig = new PreviewConfig();
    public PreviewPanel()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        //BorderStyle = LineStyle.Dashed;
        //this.SetBackgroundColor(ColorName16.Blue);
        imageView = new(previewConfig.ImageConfig);
    }

    public bool ShowHeader
    {
        get => itemListTable.ShowHeader;
        set => itemListTable.ShowHeader = value;
    }

    public void Preview(IItem item)
    {
        if (item is IContainer container)
        {
            //if(!SubViews.Contains(itemListTable))
            //    Add(itemListTable);
            SwitchToSubview(itemListTable);
            itemListTable.ShowData(container);
            return;
        }

        if (item.Name.EndsWith(".png"))
        {
            SwitchToSubview(imageView);
            imageView.ShowImage(item.FullName);
            return;
        }
    }

    private void SwitchToSubview(View view)
    {
        if (SubViews.FirstOrDefault() == imageView) imageView.HideImage();
        RemoveAll();
        Add(view);
    }
    
}