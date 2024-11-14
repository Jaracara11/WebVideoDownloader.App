using System.Diagnostics;
using Photino.NET;

namespace WebVideoDownloader.App
{
    public static class VideoDownloader
    {
        private static readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), "WebVideoDownloader");

        static VideoDownloader()
        {
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
        }

        public static async Task DownloadYoutubeVideoAsync(string videoUrl, PhotinoWindow window)
        {
            try
            {
                if (string.IsNullOrEmpty(videoUrl) || !StringValidator.ValidateYouTubeUrl(videoUrl))
                {
                    window.SendWebMessage("Invalid or missing Video URL.");
                    return;
                }

                var ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "yt-dlp.exe");
                var ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binaries", "ffmpeg.exe");
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var outputFilePath = Path.Combine(_tempDirectory, $"downloaded_video_{timestamp}.%(ext)s");
                var arguments = $"-f b -o \"{outputFilePath}\" --ffmpeg-location \"{ffmpegPath}\" \"{videoUrl}\"";

                var errorMessages = await ExecuteProcessAsync(ytDlpPath, arguments);

                if (errorMessages.Any())
                {
                    var errorMessage = string.Join("; ", errorMessages);
                    Console.WriteLine($"Error: yt-dlp process failed: {errorMessage}");
                    window.SendWebMessage($"Error: yt-dlp process failed: {errorMessage}");
                    return;
                }

                var downloadedFile = GetDownloadedFile(outputFilePath);

                if (!string.IsNullOrEmpty(downloadedFile) && File.Exists(downloadedFile))
                {
                    Console.WriteLine($"File ready for streaming: {downloadedFile}");
                    window.SendWebMessage($"File ready for download: {downloadedFile}");

                    SendFilePathToFrontendAsync(downloadedFile, window);

                    DeleteFileAfterDelay(downloadedFile, TimeSpan.FromMinutes(5));
                }
                else
                {
                    Console.WriteLine("Failed to download video.");
                    window.SendWebMessage("Failed to download video.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                window.SendWebMessage($"Error: {ex.Message}");
            }
        }

        private static async Task<List<string>> ExecuteProcessAsync(string ytDlpPath, string arguments)
        {
            var errorMessages = new List<string>();
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var processStartInfo = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorMessages.Add(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var processExitTask = process.WaitForExitAsync(cancellationTokenSource.Token);
                var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5), cancellationTokenSource.Token);

                if (await Task.WhenAny(processExitTask, timeoutTask) == timeoutTask)
                {
                    process.Kill();
                    errorMessages.Add("yt-dlp process timed out.");
                }

                if (process.ExitCode != 0 || errorMessages.Any())
                {
                    errorMessages.Add($"yt-dlp process failed with exit code {process.ExitCode}");
                }
            }

            return errorMessages;
        }

        private static string? GetDownloadedFile(string outputFilePath)
        {
            var directory = Path.GetDirectoryName(outputFilePath);

            if (directory == null)
            {
                Console.WriteLine("Error: Unable to determine the directory for the output file.");
                return null;
            }

            var allowedExtensions = new HashSet<string> { ".mp4", ".mkv", ".webm" };
            var files = Directory.GetFiles(directory, $"{Path.GetFileNameWithoutExtension(outputFilePath)}.*")
                                 .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                                 .ToList();

            return files.FirstOrDefault();
        }

        private static void SendFilePathToFrontendAsync(string outputFile, PhotinoWindow window)
        {
            window.SendWebMessage($"File ready for download: {outputFile}");
            Console.WriteLine($"File path sent to frontend: {outputFile}");
        }

        private static void DeleteFileAfterDelay(string filePath, TimeSpan delay)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            Task.Delay(delay, token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    Console.WriteLine("File deletion was canceled.");
                    return;
                }

                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"Deleted file after {delay.TotalMinutes} minutes: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
            }, TaskScheduler.Default);
        }
    }
}