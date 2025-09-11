namespace ff.State.FileDataSystem;

/// <summary>Default file operation handlers using modal dialogs.</summary>
public class DefaultFileOperations : IOperations
{
    /// <inheritdoc/>
    public bool Delete(IEnumerable<IFileSystemInfo> toDelete)
    {
        // Default implementation does not allow deleting multiple files
        //if (toDelete.Count () != 1)
        //{
        //    return false;
        //}

        //IFileSystemInfo d = toDelete.Single ();
        //string adjective = d.Name;

        //int result = MessageBox.Query (
        //                               string.Format (Strings.fdDeleteTitle, adjective),
        //                               string.Format (Strings.fdDeleteBody, adjective),
        //                               Strings.btnYes,
        //                               Strings.btnNo
        //                              );

        //try
        //{
        //    if (result == 0)
        //    {
        //        if (d is IFileInfo)
        //        {
        //            d.Delete ();
        //        }
        //        else
        //        {
        //            ((IDirectoryInfo)d).Delete (true);
        //        }

        //        return true;
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.ErrorQuery (Strings.fdDeleteFailedTitle, ex.Message, Strings.btnOk);
        //}

        return false;
    }

    public bool Delete(IEnumerable<IItem> toDelete)
    {
        throw new NotImplementedException();
    }

    public IItem Rename(IItem item, string newName)
    {
        var it = (FileSystemItem)item;
        // Don't allow renaming C: or D: or / (on linux) etc
        if (it is IContainer dir && dir.GetParent() is null)
        {
            throw new ArgumentException("can not rename root disk");
        }

        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("to rename we need a none empty new name");

        var parentFullPath = Directory.GetParent(it.FullName).FullName;
        var fullNewName = Path.Combine(parentFullPath, newName);

        try
        {
            if (it.FileSystemInfo is IFileInfo fi)
            {
                var newFi = it.FileSystemInfo.FileSystem.FileInfo.New(fullNewName);
                fi.MoveTo(newFi.FullName);
                return new FileSystemItem(newFi);
            }
            else
            {
                var newDi = it.FileSystemInfo.FileSystem.DirectoryInfo.New(fullNewName);
                var di = (IDirectoryInfo)it.FileSystemInfo;
                di.MoveTo(newDi.FullName);
                return new FileSystemContainer(newDi);
            }
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Can not rename", ex.Message, "Ok");
            throw;
        }
    }

    /// <inheritdoc/>
    //public IFileSystemInfo New(IFileSystem fileSystem, IDirectoryInfo inDirectory)
    //{
    //    //if (Prompt (Strings.fdNewTitle, "", out string named))
    //    //{
    //    //    if (!string.IsNullOrWhiteSpace (named))
    //    //    {
    //    //        try
    //    //        {
    //    //            IDirectoryInfo newDir =
    //    //                fileSystem.DirectoryInfo.New (
    //    //                                              Path.Combine (inDirectory.FullName, named)
    //    //                                             );
    //    //            newDir.Create ();

    //    //            return newDir;
    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            MessageBox.ErrorQuery (Strings.fdNewFailed, ex.Message, "Ok");
    //    //        }
    //    //    }
    //    //}

    //    return null;
    //}

    //private bool Prompt(string title, string defaultText, out string result)
    //{
    //    var confirm = false;
    //    //var btnOk = new Button { IsDefault = true, Text = Strings.btnOk };

    //    //btnOk.Accepting += (s, e) =>
    //    //                 {
    //    //                     confirm = true;
    //    //                     Application.RequestStop ();
    //    //                     // When Accepting is handled, set e.Handled to true to prevent further processing.
    //    //                     e.Handled = true;
    //    //                 };
    //    //var btnCancel = new Button { Text = Strings.btnCancel };

    //    //btnCancel.Accepting += (s, e) =>
    //    //                     {
    //    //                         confirm = false;
    //    //                         Application.RequestStop ();
    //    //                         // When Accepting is handled, set e.Handled to true to prevent further processing.
    //    //                         e.Handled = true;
    //    //                     };

    //    //var lbl = new Label { Text = Strings.fdRenamePrompt };
    //    //var tf = new TextField { X = Pos.Right (lbl), Width = Dim.Fill (), Text = defaultText };
    //    //tf.SelectAll ();

    //    //var dlg = new Dialog { Title = title, Width = Dim.Percent (50), Height = 4 };
    //    //dlg.Add (lbl);
    //    //dlg.Add (tf);

    //    //// Add buttons last so tab order is friendly
    //    //// and TextField gets focus
    //    //dlg.AddButton (btnOk);
    //    //dlg.AddButton (btnCancel);

    //    //Application.Run (dlg);
    //    //dlg.Dispose ();

    //    //result = tf.Text;

    //    return confirm;
    //}
}
