namespace ff.State;

public interface IContainer: IItem
{
    IItem[] Children { get; }
    event Action ChildrenUpdated;
    IItem? ActiveChild { get; set; }
    IContainer? GetParent();
    new bool IsLeaf => false;
}