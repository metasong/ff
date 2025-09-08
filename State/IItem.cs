namespace ff.State;

/// <summary>
/// can be container or leaf
/// </summary>
public interface IItem
{
    string Name { get; }
    string FullName { get; } // with parent path
    bool IsLeaf { get; }// container or leaf
    IDataSystem DataSystem { get; }
    bool IsSelected { get; set; }
    IDictionary<string, string> Properties { get;}
}
