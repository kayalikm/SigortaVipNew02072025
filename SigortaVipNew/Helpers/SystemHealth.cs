using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SigortaVipNew.Configuration;

namespace SigortaVipNew.Helpers
{
    public static class SystemHealth
    {
        public static HealthReport GetHealthReport()
        {
            var report = new HealthReport
            {
                Timestamp = DateTime.Now,
                ApplicationUptime = GetApplicationUptime(),
                MemoryUsage = GetMemoryUsage(),
                DiskSpace = GetDiskSpace(),
                LogFileStatus = GetLogFileStatus(),
                ConfigurationStatus = GetConfigurationStatus(),
                ServicesStatus = GetServicesStatus()
            };

            // Overall health skoru hesapla
            report.OverallHealthScore = CalculateHealthScore(report);

            return report;
        }

        public static void LogHealthReport()
        {
            try
            {
                var report = GetHealthReport();

                AdvancedLogger.LogInfo("=== SYSTEM HEALTH REPORT ===", "Health");
                AdvancedLogger.LogInfo($"Overall Health Score: {report.OverallHealthScore}/100", "Health");
                AdvancedLogger.LogInfo($"Uptime: {report.ApplicationUptime.TotalMinutes:F1} minutes", "Health");
                AdvancedLogger.LogInfo($"Memory Usage: {report.MemoryUsage.UsedMB}MB / {report.MemoryUsage.TotalMB}MB ({report.MemoryUsage.UsagePercentage:F1}%)", "Health");
                AdvancedLogger.LogInfo($"Disk Space: {report.DiskSpace.FreeMB}MB free / {report.DiskSpace.TotalMB}MB total", "Health");
                AdvancedLogger.LogInfo($"Log Files: {report.LogFileStatus.TotalFiles} files, {report.LogFileStatus.TotalSizeMB}MB", "Health");
                AdvancedLogger.LogInfo($"Configuration: {(report.ConfigurationStatus.IsValid ? "OK" : "ERROR")}", "Health");
                AdvancedLogger.LogInfo($"Services: {report.ServicesStatus.ActiveServices}/{report.ServicesStatus.TotalServices} active", "Health");

                // Uyarılar
                if (report.OverallHealthScore < 70)
                {
                    AdvancedLogger.LogWarning($"UYARI: Sistem sağlığı düşük! Skor: {report.OverallHealthScore}", "Health");
                }

                if (report.MemoryUsage.UsagePercentage > 80)
                {
                    AdvancedLogger.LogWarning("UYARI: Yüksek memory kullanımı!", "Health");
                }
            }
            catch (Exception ex)
            {
                AdvancedLogger.LogError(ex, "Health report oluşturma hatası", "Health");
            }
        }

        private static TimeSpan GetApplicationUptime()
        {
            try
            {
                using (var process = Process.GetCurrentProcess())
                {
                    return DateTime.Now - process.StartTime;
                }
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        private static MemoryInfo GetMemoryUsage()
        {
            try
            {
                using (var process = Process.GetCurrentProcess())
                {
                    var usedBytes = process.WorkingSet64;
                    var totalBytes = GC.GetTotalMemory(false);

                    return new MemoryInfo
                    {
                        UsedMB = usedBytes / (1024 * 1024),
                        TotalMB = totalBytes / (1024 * 1024),
                        UsagePercentage = (double)usedBytes / (totalBytes + usedBytes) * 100
                    };
                }
            }
            catch
            {
                return new MemoryInfo();
            }
        }

        private static DiskInfo GetDiskSpace()
        {
            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory));

                return new DiskInfo
                {
                    FreeMB = drive.AvailableFreeSpace / (1024 * 1024),
                    TotalMB = drive.TotalSize / (1024 * 1024),
                    UsagePercentage = (double)(drive.TotalSize - drive.AvailableFreeSpace) / drive.TotalSize * 100
                };
            }
            catch
            {
                return new DiskInfo();
            }
        }

        private static LogInfo GetLogFileStatus()
        {
            try
            {
                var logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");

                if (!Directory.Exists(logDirectory))
                    return new LogInfo();

                var logFiles = Directory.GetFiles(logDirectory, "*.log");
                var totalSize = logFiles.Sum(f => new FileInfo(f).Length);

                return new LogInfo
                {
                    TotalFiles = logFiles.Length,
                    TotalSizeMB = totalSize / (1024 * 1024),
                    OldestFile = logFiles.Any() ? logFiles.Min(f => new FileInfo(f).CreationTime) : (DateTime?)null,
                    NewestFile = logFiles.Any() ? logFiles.Max(f => new FileInfo(f).CreationTime) : (DateTime?)null
                };
            }
            catch
            {
                return new LogInfo();
            }
        }

        private static ConfigInfo GetConfigurationStatus()
        {
            try
            {
                // Temel config değerlerini test et
                var navPaneWidth = AppSettings.NavPaneWidth;
                var apiTimeout = AppSettings.ApiTimeout;
                var cacheMinutes = AppSettings.CompanyCacheMinutes;

                return new ConfigInfo
                {
                    IsValid = navPaneWidth > 0 && apiTimeout > 0 && cacheMinutes > 0,
                    LoadedSettings = 3
                };
            }
            catch
            {
                return new ConfigInfo { IsValid = false, LoadedSettings = 0 };
            }
        }

        private static ServiceInfo GetServicesStatus()
        {
            try
            {
                var serviceContainer = Program.ServiceContainer;

                var services = new[] {
                    typeof(SimpleCache),
                    typeof(ResourceManager),
                    typeof(BackgroundTaskManager),
                    typeof(PerformanceMonitor),
                    typeof(UserActivityTracker)
                };

                int activeServices = 0;
                foreach (var serviceType in services)
                {
                    try
                    {
                        var service = serviceContainer.Resolve(serviceType);
                        if (service != null) activeServices++;
                    }
                    catch
                    {
                        // Service active değil
                    }
                }

                return new ServiceInfo
                {
                    TotalServices = services.Length,
                    ActiveServices = activeServices
                };
            }
            catch
            {
                return new ServiceInfo();
            }
        }

        private static int CalculateHealthScore(HealthReport report)
        {
            int score = 100;

            // Memory kullanımı penaltısı
            if (report.MemoryUsage.UsagePercentage > 90) score -= 30;
            else if (report.MemoryUsage.UsagePercentage > 70) score -= 15;

            // Disk alanı penaltısı
            if (report.DiskSpace.UsagePercentage > 95) score -= 20;
            else if (report.DiskSpace.UsagePercentage > 85) score -= 10;

            // Servis durumu penaltısı
            var serviceRatio = (double)report.ServicesStatus.ActiveServices / report.ServicesStatus.TotalServices;
            if (serviceRatio < 0.8) score -= 25;
            else if (serviceRatio < 1.0) score -= 10;

            // Configuration penaltısı
            if (!report.ConfigurationStatus.IsValid) score -= 15;

            return Math.Max(0, Math.Min(100, score));
        }
    }

    public class HealthReport
    {
        public DateTime Timestamp { get; set; }
        public TimeSpan ApplicationUptime { get; set; }
        public MemoryInfo MemoryUsage { get; set; }
        public DiskInfo DiskSpace { get; set; }
        public LogInfo LogFileStatus { get; set; }
        public ConfigInfo ConfigurationStatus { get; set; }
        public ServiceInfo ServicesStatus { get; set; }
        public int OverallHealthScore { get; set; }
    }

    public class MemoryInfo
    {
        public long UsedMB { get; set; }
        public long TotalMB { get; set; }
        public double UsagePercentage { get; set; }
    }

    public class DiskInfo
    {
        public long FreeMB { get; set; }
        public long TotalMB { get; set; }
        public double UsagePercentage { get; set; }
    }

    public class LogInfo
    {
        public int TotalFiles { get; set; }
        public long TotalSizeMB { get; set; }
        public DateTime? OldestFile { get; set; }
        public DateTime? NewestFile { get; set; }
    }

    public class ConfigInfo
    {
        public bool IsValid { get; set; }
        public int LoadedSettings { get; set; }
    }

    public class ServiceInfo
    {
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
    }
}