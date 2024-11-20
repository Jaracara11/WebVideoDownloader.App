using System.Text.RegularExpressions;

namespace WebVideoDownloader.App.Classes
{
    public static partial class StringValidator
    {
        public static bool ValidateYouTubeUrl(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
            {
                ErrorHandler.HandleError("Invalid or missing Video URL.");
                return false;
            }

            if (!IsValidYouTubeUrl(videoUrl))
            {
                ErrorHandler.HandleError("Invalid YouTube URL.");
                return false;
            }

            return true;
        }

        [GeneratedRegex(@"^(https?://)?(www\.)?(youtube\.com(/watch\?v=[\w-]+|/embed/[\w-]+)?|youtu\.be/[\w-]+).*", RegexOptions.IgnoreCase)]
        private static partial Regex YouTubeUrlRegex();
        private static bool IsValidYouTubeUrl(string url) => YouTubeUrlRegex().IsMatch(url);
    }
}