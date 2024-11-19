using Photino.NET;
using WebVideoDownloader.App.Classes;

namespace WebVideoDownloader.App
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var window = new PhotinoWindow()
                .SetTitle("Web Video Downloader")
                .SetUseOsDefaultSize(false)
                .SetSize(800, 400)
                .SetUseOsDefaultLocation(false)
                .Center()
                .RegisterWebMessageReceivedHandler(RequestHandler)
                .Load("wwwroot/index.html");

            window.WaitForClose();
        }

        private static async void RequestHandler(object? sender, string message)
        {
            var window = (PhotinoWindow)sender!;

            await VideoDownloader.HandleDownloadRequest(message, window);
        }
    }
}