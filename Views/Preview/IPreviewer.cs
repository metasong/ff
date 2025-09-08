namespace ff.Views.Preview;

public interface IPreviewer
{
    bool CanView(IItem item);
    void View(IItem item);
}