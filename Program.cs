using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using WebVideoDownloader.App.Classes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapPost("/api/download", static async (HttpContext context) =>
{
  try
  {
    using var reader = new StreamReader(context.Request.Body);

    string videoUrl = await reader.ReadToEndAsync();

    if (string.IsNullOrEmpty(videoUrl))
    {
      var errorMessage = "Invalid video URL.";
      ErrorHandler.HandleError(errorMessage);
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      await context.Response.WriteAsJsonAsync(new { message = errorMessage });
      return;
    }

    var outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
    var downloadedFilePath = await VideoDownloader.DownloadYoutubeVideoAsync(videoUrl, outputDirectory);

    if (!string.IsNullOrEmpty(downloadedFilePath) && File.Exists(downloadedFilePath))
    {
      context.Response.ContentType = "application/octet-stream";
      context.Response.Headers["Content-Disposition"] = "attachment; filename=\"video.mp4\"";
      await context.Response.SendFileAsync(downloadedFilePath);
    }
    else
    {
      string errorMessage = "Failed to download the video.";
      ErrorHandler.HandleError(errorMessage);
      context.Response.StatusCode = StatusCodes.Status500InternalServerError;

      await context.Response.WriteAsJsonAsync(new { message = errorMessage });
    }
  }
  catch (Exception ex)
  {
    ErrorHandler.HandleError(ex, "An unexpected error occurred while processing the request.");
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

    await context.Response.WriteAsJsonAsync(new { message = "An error occurred.", error = ex.Message });
  }
});

app.MapFallbackToFile("index.html");
app.Run();
