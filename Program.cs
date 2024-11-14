using Photino.NET;

namespace WebVideoDownloader.App
{
  public class Program
  {
    [STAThread]
    public static void Main(string[] args)
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
      string response = $"Received URL: \"{message}\"";
      window.SendWebMessage(response);

      var requestData = new Dictionary<string, string> { { "VideoUrl", message } };

      await VideoDownloader.DownloadYoutubeVideoAsync(requestData, window);
    }
  }
}