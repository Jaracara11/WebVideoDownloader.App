using Photino.NET;

namespace WebVideoDownloader.App.Classes
{
    public static class ErrorHandler
    {
        public static void HandleError(object error, PhotinoWindow? window = null, string? customMessage = null)
        {
            string errorMessage = error switch
            {
                Exception ex => HandleException(ex, customMessage),
                string message => HandleStringError(message),
                _ => "An unknown error occurred."
            };

            window?.SendWebMessage($"Error: {errorMessage}");
        }

        private static string HandleException(Exception ex, string? customMessage)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");

            return customMessage ?? "An unexpected error occurred. Please try again later.";
        }

        private static string HandleStringError(string message)
        {
            Console.WriteLine($"Error: {message}");

            return message;
        }
    }
}