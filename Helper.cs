using System.Text.RegularExpressions;

namespace WebVideoDownloader.App
{
    public static class Helper
    {
        public static bool ValidateYouTubeUrl(Dictionary<string, string> requestData)
        {
            var videoUrl = GetValueOrDefault(requestData, "VideoUrl");

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

        private static string? GetValueOrDefault(Dictionary<string, string> dictionary, string key)
        {
            if (dictionary != null && dictionary.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
            {
                return value;
            }
            return null;
        }

        private static bool IsValidYouTubeUrl(string url)
        {
            return Regex.IsMatch(url, @"^(https?://)?(www\.)?(youtube\.com|youtu\.be)/.*$");
        }
    }
}