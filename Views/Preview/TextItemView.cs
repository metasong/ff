using System.Collections.ObjectModel;
using ff.Views.Preview.Config.Image;
using Terminal.Gui.Views;

namespace ff.Views.Preview;

public class TextItemView : TextView, IPreviewer
{
    private readonly TextConfig config;

    public TextItemView(TextConfig config)
    {
        this.config = config;
        ReadOnly = true;
        WordWrap = true;
        Width = Dim.Fill();
        Height = Dim.Fill();
        VerticalScrollBar.AutoShow = true;
        VerticalScrollBar.Visible = true;
    }

    public bool CanView(IItem item)
        => config.Exts.Any(ext => item.Name.EndsWith($".{ext}"));

    public void View(IItem item)
    {
        var content = File.ReadAllText(item.FullName);
        Text = content;
    }
}

public class CommonItemView : View, IPreviewer
{
    private readonly TableView table;

    public CommonItemView()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        VerticalScrollBar.AutoShow = true;
        VerticalScrollBar.Visible = true;

        table = new TableView(){Width = Dim.Fill(), Height = Dim.Fill()};

        VerticalScrollBar.AutoShow = true;
        HorizontalScrollBar.AutoShow = true;
        table.MultiSelect = false;

        var style = table.Style;
        style.ShowHeaders = false;
        style.AlwaysShowHeaders = true;

        style.ShowHorizontalHeaderOverline = false;
        style.ShowHorizontalBottomline = false;
        style.ShowVerticalCellLines = false;
        style.ShowVerticalHeaderLines = false;
        style.ShowHorizontalHeaderUnderline = false;
        Add(table);
    }


    public bool CanView(IItem item)
        => true;

    public void View(IItem item)
    {
        var properties = item.Properties;
        table.Title = item.Name;
        var source = new IDictionaryTableSource<string, string>(properties);
        table.Table = source;
    }
}

public class IDictionaryTableSource<K, V>(IDictionary<K, V> dictionary) : ITableSource
{
    private readonly KeyValuePair<K, V>[] keyValuePairs = dictionary.ToArray();

    public string[] ColumnNames => ["Key", "Value"];
    public int Columns => 2;

    public object this[int row, int col]
    {
        get
        {
            var v = col == 0 ? keyValuePairs[row].Key.ToString() : keyValuePairs[row].Value.ToString();
            return v;
        }
    }

    public int Rows => keyValuePairs.Length;
}