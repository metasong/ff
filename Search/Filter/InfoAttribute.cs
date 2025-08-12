using Attribute = System.Attribute;

namespace ff.Search.Filter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field)]
public class InfoAttribute(string name, string description) : Attribute
{
    public string Value { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public string Description { get; set; } = description;
}
