namespace ff.State;

public interface IContainer: IItem
{
    IItem[] Children { get; }
    event System.Action ChildrenUpdated;
    IItem? ActiveChild { get; set; }
    IContainer? GetParent();
    new bool IsLeaf => false;
}

public static class ContainerExt
{
    public static IItem[] SelectedItems(this IContainer container)
    {
        var items = container.Children.Where(i => i.IsSelected).ToArray();
        return items;
    }
}