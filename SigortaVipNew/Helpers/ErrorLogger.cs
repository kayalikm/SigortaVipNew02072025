using System;
using System.IO;
using System.Windows.Forms; // Bu satırı ekleyin

namespace SigortaVipNew.Helpers
{
    public static class ErrorLogger
    {
        public static void LogError(string message)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                string logPath = Path.Combine(Application.StartupPath, "errors.log");
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // Eğer log yazamazsa sessiz kal
            }
        }

        public static void LogError(Exception ex, string additionalMessage = "")
        {
            try
            {
                string message = string.IsNullOrEmpty(additionalMessage)
                    ? ex.Message
                    : $"{additionalMessage}: {ex.Message}";

                LogError(message);
            }
            catch
            {
                // Eğer log yazamazsa sessiz kal
            }
        }
    }
}