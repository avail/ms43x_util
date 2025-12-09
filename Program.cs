namespace ms43x_util;

static class G
{
    public static Form MainForm;

    public static Config Config;

    public static XdfFile Xdf;
    public static XdfBinaryReader MapFile;

    public static void CloseApp()
    {
        MainForm.Close();
    }
}

static class Program
{
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        G.MainForm = new MainForm();
        Application.Run(G.MainForm);
    }    
}