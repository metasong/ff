namespace ff.Views.CurrentFolder;

public partial class ItemTable
{
    private TextField? textField;
    private void RenameCommand(int x, int y, IItem item)
    {
        if (textField == null)
        {
            textField = new TextField() { Arrangement = ViewArrangement.Movable | ViewArrangement.Overlapped };
            textField.KeyUp += TextField_KeyUp;
            textField.KeyDown += TextField_KeyDown;
            Add(textField);
        }
        textField.X = x;

        textField.Y = y;
        textField.Width = item.Name.Length;
        textField.Text = item.Name;
        textField.Visible = true;
    }

    private void TextField_KeyDown(object? sender, Key e)
    {
        if (e.KeyCode == KeyCode.Enter)
        {
            Console.WriteLine("dd");
            textField.Visible = false;
            
            e.Handled = true;
        }
    }

    private void TextField_KeyUp(object? sender, Key e)
    {
        
    }

    private void HideRename()
    {
        if (textField is { Visible: true }) textField.Visible = false;
    }
}