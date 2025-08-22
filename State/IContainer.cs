namespace ff.State;

public interface IContainer: IItem
{
    IItem[] Children { get; }
    event Action ChildrenUpdated;
    IItem? SelectedChild { get; set; }
    IContainer? GetParent();
    new bool IsLeaf => false;
}