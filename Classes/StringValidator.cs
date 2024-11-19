using System.Text.RegularExpressions;
using Photino.NET;

namespace WebVideoDownloader.App.Classes
{
    public static partial class StringValidator
    {
        public static bool ValidateYouTubeUrl(string videoUrl, PhotinoWindow? window = null)
        {
            if (string.IsNullOrEmpty(videoUrl))
            {
                ErrorHandler.HandleError("Invalid or missing Video URL.", window);
                return false;
            }

            if (!IsValidYouTubeUrl(videoUrl))
            {
                ErrorHandler.HandleError("Invalid YouTube URL.", window);
                return false;
            }

            return true;
        }

        [GeneratedRegex(@"^(https?://)?(www\.)?(youtube\.com(/watch\?v=[\w-]+|/embed/[\w-]+)?|youtu\.be/[\w-]+).*", RegexOptions.IgnoreCase)]
        private static partial Regex YouTubeUrlRegex();
        private static bool IsValidYouTubeUrl(string url) => YouTubeUrlRegex().IsMatch(url);
    }
}