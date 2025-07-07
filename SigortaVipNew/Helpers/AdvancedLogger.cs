using System;
using System.IO;
using System.Text;
using SigortaVipNew.Configuration;

namespace SigortaVipNew.Helpers
{
    public static class AdvancedLogger
    {
        private static readonly object _lockObject = new object();
        private static readonly string _logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");

        static AdvancedLogger()
        {
            // Log klasörünü oluştur
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public static void LogInfo(string message, string category = "General")
        {
            WriteLog(LogLevel.Info, message, category);
        }

        public static void LogWarning(string message, string category = "General")
        {
            WriteLog(LogLevel.Warning, message, category);
        }

        public static void LogError(string message, string category = "General")
        {
            WriteLog(LogLevel.Error, message, category);
        }

        public static void LogError(Exception ex, string message = "", string category = "Exception")
        {
            var fullMessage = string.IsNullOrEmpty(message)
                ? ex.Message
                : $"{message} - {ex.Message}";

            WriteLog(LogLevel.Error, fullMessage, category, ex);
        }

        public static void LogDebug(string message, string category = "Debug")
        {
            if (AppSettings.ShowDetailedErrors) // Config'den kontrol et
            {
                WriteLog(LogLevel.Debug, message, category);
            }
        }

        public static void LogPerformance(string operation, TimeSpan duration, string category = "Performance")
        {
            var message = $"Operation '{operation}' completed in {duration.TotalMilliseconds:F2}ms";
            WriteLog(LogLevel.Performance, message, category);
        }

        public static void LogUserAction(string action, string details = "", string category = "UserAction")
        {
            var message = string.IsNullOrEmpty(details) ? action : $"{action} - {details}";
            WriteLog(LogLevel.UserAction, message, category);
        }

        public static void LogApiCall(string endpoint, string method, TimeSpan? duration = null, string category = "API")
        {
            var message = $"{method} {endpoint}";
            if (duration.HasValue)
            {
                message += $" ({duration.Value.TotalMilliseconds:F0}ms)";
            }
            WriteLog(LogLevel.Api, message, category);
        }

        private static void WriteLog(LogLevel level, string message, string category, Exception exception = null)
        {
            try
            {
                lock (_lockObject)
                {
                    var timestamp = DateTime.Now;
                    var logEntry = FormatLogEntry(timestamp, level, category, message, exception);

                    // Ana log dosyasına yaz
                    WriteToFile(GetLogFileName("application"), logEntry);

                    // Kategori bazlı log dosyasına yaz
                    if (category != "General")
                    {
                        WriteToFile(GetLogFileName(category.ToLower()), logEntry);
                    }

                    // Error level ise ayrı error dosyasına da yaz
                    if (level == LogLevel.Error)
                    {
                        WriteToFile(GetLogFileName("errors"), logEntry);
                    }

                    // Console'a da yazdır (Debug için)
                    if (AppSettings.ShowDetailedErrors)
                    {
                        Console.WriteLine(logEntry);
                    }
                }
            }
            catch
            {
                // Logging başarısız olursa sessiz kal
            }
        }

        private static string FormatLogEntry(DateTime timestamp, LogLevel level, string category, string message, Exception exception = null)
        {
            var sb = new StringBuilder();

            sb.Append($"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] ");
            sb.Append($"[{level.ToString().ToUpper()}] ");
            sb.Append($"[{category}] ");
            sb.Append(message);

            if (exception != null)
            {
                sb.AppendLine();
                sb.Append($"Exception: {exception.GetType().Name}: {exception.Message}");
                sb.AppendLine();
                sb.Append($"StackTrace: {exception.StackTrace}");

                if (exception.InnerException != null)
                {
                    sb.AppendLine();
                    sb.Append($"InnerException: {exception.InnerException.GetType().Name}: {exception.InnerException.Message}");
                }
            }

            return sb.ToString();
        }

        private static string GetLogFileName(string category)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            return Path.Combine(_logDirectory, $"{category}_{today}.log");
        }

        private static void WriteToFile(string filePath, string content)
        {
            try
            {
                // Dosya boyutu kontrolü
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    var maxSizeMB = AppSettings.LogMaxFileSizeMB;

                    if (fileInfo.Length > maxSizeMB * 1024 * 1024)
                    {
                        // Dosya çok büyük, archive et
                        ArchiveLogFile(filePath);
                    }
                }

                File.AppendAllText(filePath, content + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // Dosya yazma hatası olursa sessiz kal
            }
        }

        private static void ArchiveLogFile(string filePath)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("HHmmss");
                var archivePath = filePath.Replace(".log", $"_archived_{timestamp}.log");
                File.Move(filePath, archivePath);
            }
            catch
            {
                // Archive başarısız olursa dosyayı sil
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // Silme de başarısız olursa sessiz kal
                }
            }
        }

        public static void CleanOldLogs(int daysToKeep = 7)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var logFiles = Directory.GetFiles(_logDirectory, "*.log");

                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, "Log temizleme hatası", "System", ex);
            }
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Performance,
        UserAction,
        Api
    }
}