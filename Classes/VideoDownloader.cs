using System.Diagnostics;
using System.Text;

namespace WebVideoDownloader.App.Classes
{
    public static class VideoDownloader
    {
        private static readonly string _ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "yt-dlp.exe");
        private static readonly string _ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "ffmpeg.exe");

        public static async Task HandleUIRequest(string message)
        {
            if (message.StartsWith("OpenFolder:"))
            {
                var filePath = message.Replace("OpenFolder:", "");
                OpenContainingFolder(filePath);
            }
            else
            {
                await DownloadYoutubeVideoAsync(message);
            }
        }

        private static async Task DownloadYoutubeVideoAsync(string videoUrl)
        {
            try
            {
                var window = PhotinoWindowManager.GetInstance();

                if (!StringValidator.ValidateYouTubeUrl(videoUrl))
                {
                    return;
                }

                var saveFilePath = ShowSaveFileDialog();

                if (string.IsNullOrEmpty(saveFilePath))
                {
                    ErrorHandler.HandleError("No save location selected.");
                    return;
                }

                var arguments = $"-f b -o \"{saveFilePath}\" --ffmpeg-location \"{_ffmpegPath}\" \"{videoUrl}\"";
                bool ytDlpProcess = await ExecuteYtDlpProcessAsync(_ytDlpPath, arguments);

                if (!ytDlpProcess)
                {
                    return;
                }

                var downloadedFile = GetDownloadedFile(saveFilePath);

                if (!string.IsNullOrEmpty(downloadedFile) && File.Exists(downloadedFile))
                {
                    window.SendWebMessage($"FileReady: {downloadedFile}");
                }
                else
                {
                    ErrorHandler.HandleError("Failed to download video.");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
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
            var directory = Path.GetDirectoryName(saveFilePath);

            if (directory == null) return null;

            var allowedExtensions = new HashSet<string> { ".mp4", ".mkv", ".webm" };
            var baseFileName = Path.GetFileNameWithoutExtension(saveFilePath);
            var files = Directory.GetFiles(directory, $"{baseFileName}.*")
                .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            return files.FirstOrDefault();
        }

        private static void OpenContainingFolder(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    ErrorHandler.HandleError("File path is null or empty.");
                    return;
                }

                var directory = Path.GetDirectoryName(filePath);
                directory = directory?.Trim();

                if (string.IsNullOrEmpty(directory))
                {
                    ErrorHandler.HandleError("Failed to extract directory from file path.");
                    return;
                }

                if (Directory.Exists(directory))
                {
                    Process.Start("explorer.exe", directory);
                }
                else
                {
                    ErrorHandler.HandleError($"The directory does not exist: {directory}");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError($"Failed to open folder: {ex.Message}");
            }
        }
    }
}