namespace TerminalFileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();

            try
            {
                var fileManager = new FileManagerWindow();
                Application.Run(fileManager);
                fileManager.Dispose();
            }
            finally
            {
                Application.Shutdown();
            }
        }
    }
}