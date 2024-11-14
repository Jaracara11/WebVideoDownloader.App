using Photino.NET;

namespace WebVideoDownloader.App;

public class Program
{
  [STAThread]
  public static void Main(string[] args)
  {
    var window = new PhotinoWindow()
    .SetTitle("WebVideoDownloader.App")
    .Load("wwwroot/index.html")
    .SetUseOsDefaultSize(false)
    .SetSize(800, 400)
    .SetUseOsDefaultLocation(false)
    .Center();

    window.RegisterWebMessageReceivedHandler(RequestHandler);
    window.WaitForClose();
  }

  private static void RequestHandler(object? sender, string message)
  {
    var window = (PhotinoWindow)sender!;
    window.SendWebMessage($"Received message: {message}");
  }
}