using Photino.NET;
using System.Diagnostics;

namespace WebVideoDownloader.App.Classes
{
    public static class VideoDownloader
    {
        private static readonly string _ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "yt-dlp.exe");
        private static readonly string _ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "ffmpeg.exe");

        public static async Task DownloadYoutubeVideoAsync(string videoUrl, PhotinoWindow window)
        {
            try
            {
                if (string.IsNullOrEmpty(videoUrl) || !StringValidator.ValidateYouTubeUrl(videoUrl))
                {
                    ErrorHandler.HandleError("Invalid or missing Video URL.", window);
                    return;
                }

                var saveFilePath = ShowSaveFileDialog();

                if (string.IsNullOrEmpty(saveFilePath))
                {
                    ErrorHandler.HandleError("No save location selected.", window);
                    return;
                }

                var arguments = $"-f b -o \"{saveFilePath}.%(ext)s\" --ffmpeg-location \"{_ffmpegPath}\" \"{videoUrl}\"";
                bool ytDlpProcess = await ExecuteYtDlpProcessAsync(_ytDlpPath, arguments, window);

                if (!ytDlpProcess)
                {
                    return;
                }

                var downloadedFile = GetDownloadedFile(saveFilePath);

                if (!string.IsNullOrEmpty(downloadedFile) && File.Exists(downloadedFile))
                {
                    SendFileReadyMessage(window, downloadedFile);
                }
                else
                {
                    ErrorHandler.HandleError("Failed to download video.", window);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, window);
            }
        }

        private static string? ShowSaveFileDialog()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Select Download Location",
                Filter = "MP4 files|*.mp4|All files|*.*"
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }

        private static async Task<bool> ExecuteYtDlpProcessAsync(string ytDlpPath, string arguments, PhotinoWindow window)
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

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                ErrorHandler.HandleError("yt-dlp process failed.", window);
                return false;
            }

            return true;
        }

        private static string? GetDownloadedFile(string saveFilePath)
        {
            var directory = Path.GetDirectoryName(saveFilePath);
            if (directory == null) return null;

            var allowedExtensions = new HashSet<string> { ".mp4", ".mkv", ".webm" };
            var files = Directory.GetFiles(directory, $"{Path.GetFileNameWithoutExtension(saveFilePath)}.*")
                .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            return files.FirstOrDefault();
        }

        private static void SendFileReadyMessage(PhotinoWindow window, string filePath)
        {
            window.SendWebMessage($"FileReady: {filePath}");
        }
    }
}