using Photino.NET;
using System.Text.Json;
using WebVideoDownloader.App.Classes;

namespace WebVideoDownloader.App
{
    public record DownloadRequest(string VideoUrl, string DownloadPath);

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
            var requestData = JsonSerializer.Deserialize<DownloadRequest>(message);

            if (requestData != null)
            {
                await VideoDownloader.DownloadYoutubeVideoAsync(requestData, window);
            }
            else
            {
                ErrorHandler.HandleError("Invalid request data.", window);
            }
        }
    }
}