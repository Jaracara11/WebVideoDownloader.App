using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVideoDownloader.App.Classes
{
    public static class VideoDownloader
    {
        private static readonly string _ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "yt-dlp.exe");
        private static readonly string _ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "ffmpeg.exe");

        public static async Task<string?> DownloadYoutubeVideoAsync(string videoUrl, string outputDirectory)
        {
            try
            {
                if (string.IsNullOrEmpty(videoUrl))
                {
                    throw new ArgumentException("Video URL cannot be null or empty.");
                }

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                var saveFilePath = Path.Combine(outputDirectory, "video.mp4");
                var arguments = $"-o \"{saveFilePath}\" --ffmpeg-location \"{_ffmpegPath}\" \"{videoUrl}\"";
                var ytDlpSuccess = await ExecuteYtDlpProcessAsync(_ytDlpPath, arguments);

                if (!ytDlpSuccess)
                {
                    return null;
                }

                var downloadedFile = GetDownloadedFile(saveFilePath);

                if (!string.IsNullOrEmpty(downloadedFile) && File.Exists(downloadedFile))
                {
                    return downloadedFile;
                }

                throw new Exception("Failed to locate the downloaded video file.");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "An error occurred while downloading the video.");
                return null;
            }
        }

        private static async Task<bool> ExecuteYtDlpProcessAsync(string ytDlpPath, string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };

            var errorOutput = new StringBuilder();

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorOutput.AppendLine(e.Data);
                }
            };

            try
            {
                process.Start();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    var errorMessage = $"yt-dlp exited with code {process.ExitCode}. Details: {errorOutput}";
                    ErrorHandler.HandleError(errorMessage);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Failed to execute yt-dlp. Please check your setup.");
                return false;
            }
        }

        private static string? GetDownloadedFile(string saveFilePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(saveFilePath);
                if (directory == null) return null;

                var allowedExtensions = new HashSet<string> { ".mp4", ".mkv", ".webm" };
                var baseFileName = Path.GetFileNameWithoutExtension(saveFilePath);
                var files = Directory.GetFiles(directory, $"{baseFileName}.*")
                    .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .ToList();

                return files.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Error occurred while locating the downloaded file.");
                return null;
            }
        }
    }
}
