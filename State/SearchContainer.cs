using ff.State.FileDataSystem;

namespace ff.State;

internal class SearchContainer(IDirectoryInfo dir, string searchTerms) : FileSystemContainer(dir)
{
    public string SearchTerms { get; } = searchTerms;

    public override bool Equals(object? obj)
    {
        if (obj is not SearchContainer other)
        {
            return false;
        }

        if (SearchTerms != other.SearchTerms)
            return false;

        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Directory.FullName, SearchTerms);
    }
}