using WebVideoDownloader.App.Classes;

namespace WebVideoDownloader.App
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var window = PhotinoWindowManager.GetInstance();

            window.WaitForClose();
        }
    }
}