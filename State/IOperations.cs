namespace ff.State;

/// <summary>
///     Interface for defining how to handle file/directory deletion, rename and newing attempts in
///     <see cref="FileDialog"/>.
/// </summary>
public interface IOperations
{
    /// <summary>Specifies how to handle file/directory deletion attempts in <see cref="FileDialog"/>.</summary>
    /// <param name="toDelete"></param>
    /// <returns><see langword="true"/> if operation was completed or <see langword="false"/> if cancelled</returns>
    /// <remarks>
    ///     Ensure you use a try/catch block with appropriate error handling (e.g. showing a <see cref="MessageBox"/>
    /// </remarks>
    bool Delete (IEnumerable<IItem> toDelete);

    //IItem New (IFileSystem fileSystem, IDirectoryInfo inDirectory);

    IItem Rename(IItem item, string newName);
}
