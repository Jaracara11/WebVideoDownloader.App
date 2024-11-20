using Photino.NET;

namespace WebVideoDownloader.App.Classes
{
  public static class PhotinoWindowManager
  {
    private static PhotinoWindow? _window;

    public static PhotinoWindow GetInstance()
    {
      _window ??= new PhotinoWindow()
            .SetTitle("Web Video Downloader")
            .SetIconFile("wwwroot/assets/images/logo.ico")
            .SetUseOsDefaultSize(false)
            .SetSize(800, 400)
            .SetResizable(false)
            .SetUseOsDefaultLocation(false)
            .Center()
            .RegisterWebMessageReceivedHandler(RequestHandler)
            .Load("wwwroot/index.html");

      return _window;
    }

    private static async void RequestHandler(object? sender, string message)
    {
      await VideoDownloader.HandleUIRequest(message);
    }
  }
}
