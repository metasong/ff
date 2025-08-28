using ff.Views.CurrentFolder;
using Terminal.Gui.Drawing;

namespace ff.Views.Preview;

public class PreviewPanel : View, IPreviewPanel
{
    private readonly ItemTable itemListTable = new();

    public PreviewPanel()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        //BorderStyle = LineStyle.Dashed;
        //this.SetBackgroundColor(ColorName16.Blue);
    }


    public void Preview(IItem item)
    {
        if (item is IContainer container)
        {
            if(!SubViews.Contains(itemListTable))
                Add(itemListTable);
            itemListTable.ShowData(container);
        }
    }
}