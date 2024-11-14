using System.Text.RegularExpressions;

namespace WebVideoDownloader.App
{
    public static class StringValidator
    {
        public static bool ValidateYouTubeUrl(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
            {
                Console.WriteLine("Invalid or missing Video URL.");
                return false;
            }

            if (!IsValidYouTubeUrl(videoUrl))
            {
                Console.WriteLine("Invalid YouTube URL.");
                return false;
            }

            return true;
        }

        private static bool IsValidYouTubeUrl(string url)
        {
            return Regex.IsMatch(url, @"^(https?://)?(www\.)?(youtube\.com|youtu\.be)/.*$");
        }
    }
}