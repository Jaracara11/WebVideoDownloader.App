using Photino.NET;

namespace WebVideoDownloader.App;

public class Program
{
  [STAThread]
  public static void Main(string[] args)
  {
    var window = new PhotinoWindow().SetTitle("WebVideoDownloader.App").Load("wwwroot/index.html");

    window.WaitForClose();
  }
}