using ff.Views.Preview.Config.Image;

namespace ff.Views.Preview;

public class TextItemView : TextView, IPreviewer
{
    private readonly TextConfig config;
    private readonly ILogger logger;
    //private readonly ScrollBar scrollBar;

    public TextItemView(TextConfig config, ILogger logger)
    {
        this.config = config;
        this.logger = logger;
        ReadOnly = true;
        //WordWrap = true; // if enable word wrapper exception will throw because of a bug: i.e. view this ff.sln
        Width = Dim.Fill();
        Height = Dim.Fill();
        //VerticalScrollBar.AutoShow = true;
        //VerticalScrollBar.Visible = true;
        //scrollBar = new ScrollBar
        //{
        //    X = Pos.AnchorEnd(),
        //    //AutoShow = true,
        //    //ScrollableContentSize = 100,
        //    Height = Dim.Fill()
        //};
        //Add(scrollBar);
        //ViewportChanged += (_, e) =>
        //{
        //    //ScrollTo(e.NewViewport.Y,true);
        //};
        //scrollBar.PositionChanged += (s, e) =>
        //{
        //    //Viewport = Viewport with { Y = e.Value };
        //    ScrollTo(e.Value, true);
        //};
    }

    public bool CanView(IItem item)
        => config.Exts.Any(ext => item.Name.EndsWith($".{ext}"));

    public static string ReadText(string path, int MaxLines = 100)
    {
        var lines = new List<string>(MaxLines);
        using (var reader = new StreamReader(path))
        {
            string? line;
            int count = 0;
            while (count < MaxLines && (line = reader.ReadLine()) != null)
            {
                lines.Add(line);
                count++;
            }
            if (count == MaxLines)
            {
                // too slow for large file
                lines.Add($"---- Max {MaxLines} lines for preview ---- ");
            }
        }

        return string.Join('\n', lines);
    }

    public void View(IItem item)
    {
        Text = ReadText(item.FullName);
        // Optionally, you can split lines if you need the count for scrollbars, etc.
        // var lines = Text.Split('\n');
        ////SetContentSize(new(Frame.Width,lines.Length));
        //scrollBar.Position = 0;
        //scrollBar.ScrollableContentSize = lines.Length;
        //scrollBar.VisibleContentSize = Frame.Height;
    }

    // then scroll a large file, ti become slow, so we will try to filler out event continuously event
    // not work!
    // because it's the event firing are slow, not showing the UI, the event firing are delayed after mouse scrolling: scrolling action then => delay 1-300msfire ->  delay 1-300msfire -> delay 1-300ms fire
    //protected override bool OnMouseEvent(MouseEventArgs ev)
    //{
    //    
    //    if (ev.Flags == MouseFlags.WheeledDown || ev.Flags == MouseFlags.WheeledUp|| ev.Flags == MouseFlags.WheeledRight|| ev.Flags == MouseFlags.WheeledLeft)
    //    {
    //        debouncer.Debounce(() => Application.Invoke(()=>
    //        {
    //            //logger.LogDebug($"");
    //            base.OnMouseEvent(ev);
    //        }), logger);
    //        return true;
    //    }

    //    return base.OnMouseEvent(ev);
    //}
}
