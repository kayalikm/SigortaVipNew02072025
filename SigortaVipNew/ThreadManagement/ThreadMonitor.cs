// ===== .NET Framework 4.8 Uyumlu ThreadMonitor.cs =====
// Bu dosyayı projenize ekleyin: SigortaVipNew/ThreadManagement/ThreadMonitor.cs

using System;
using System.Diagnostics;
using System.Threading;
using SigortaVip.Utility;
using SigortaVipNew.Helpers;

namespace SigortaVipNew.ThreadManagement
{
    public class ThreadMonitor : IDisposable
    {
        private readonly System.Threading.Timer _monitorTimer; // ✅ Explicit namespace
        private int _lastThreadCount = 0;
        private long _lastMemoryUsage = 0;
        private bool _disposed = false;

        public event Action<ThreadInfo> ThreadCountChanged;

        public ThreadMonitor()
        {
            // Her 10 saniyede bir thread sayısını kontrol et
            _monitorTimer = new System.Threading.Timer(CheckThreadCount, null,
                TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

            ErrorLogger.LogError("ThreadMonitor started");
        }

        private void CheckThreadCount(object state)
        {
            if (_disposed) return;

            try
            {
                var process = Process.GetCurrentProcess();
                var currentThreadCount = process.Threads.Count;
                var memoryUsage = process.WorkingSet64 / (1024 * 1024); // MB

                // Thread sayısında 2'den fazla değişiklik varsa bildir
                if (Math.Abs(currentThreadCount - _lastThreadCount) > 2 ||
                    Math.Abs(memoryUsage - _lastMemoryUsage) > 10)
                {
                    var info = new ThreadInfo
                    {
                        ThreadCount = currentThreadCount,
                        MemoryUsageMB = memoryUsage,
                        Timestamp = DateTime.Now,
                        ThreadChange = currentThreadCount - _lastThreadCount,
                        MemoryChangeMB = memoryUsage - _lastMemoryUsage
                    };

                    // Event'i güvenli şekilde trigger et
                    var handler = ThreadCountChanged;
                    if (handler != null)
                    {
                        try
                        {
                            handler(info);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.LogError(ex, "ThreadCountChanged event handler error");
                        }
                    }

                    // Log yaz
                    var logMessage = string.Format(
                        "📊 Thread Monitor - Threads: {0} ({1:+#;-#;0}), Memory: {2}MB ({3:+#;-#;0}MB)",
                        currentThreadCount, info.ThreadChange, memoryUsage, info.MemoryChangeMB
                    );

                    ErrorLogger.LogError(logMessage);

                    // Kritik durum uyarısı
                    if (currentThreadCount > 50)
                    {
                        ErrorLogger.LogError(string.Format("⚠️ HIGH THREAD COUNT WARNING: {0} threads!", currentThreadCount));
                    }

                    if (memoryUsage > 400)
                    {
                        ErrorLogger.LogError(string.Format("⚠️ HIGH MEMORY USAGE WARNING: {0}MB!", memoryUsage));
                    }

                    _lastThreadCount = currentThreadCount;
                    _lastMemoryUsage = memoryUsage;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Thread monitoring error");
            }
        }

        public ThreadInfo GetCurrentInfo()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                return new ThreadInfo
                {
                    ThreadCount = process.Threads.Count,
                    MemoryUsageMB = process.WorkingSet64 / (1024 * 1024),
                    Timestamp = DateTime.Now,
                    ThreadChange = 0,
                    MemoryChangeMB = 0
                };
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "GetCurrentInfo error");
                return new ThreadInfo(); // ✅ Default constructor
            }
        }

        public void ForceCheck()
        {
            CheckThreadCount(null);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                if (_monitorTimer != null)
                {
                    _monitorTimer.Dispose();
                }
                ErrorLogger.LogError("ThreadMonitor disposed");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "ThreadMonitor dispose error");
            }
        }
    }

    public class ThreadInfo
    {
        public int ThreadCount { get; set; }
        public long MemoryUsageMB { get; set; }
        public DateTime Timestamp { get; set; }
        public int ThreadChange { get; set; }
        public long MemoryChangeMB { get; set; }

        // ✅ Default constructor for .NET Framework 4.8
        public ThreadInfo()
        {
            ThreadCount = 0;
            MemoryUsageMB = 0;
            Timestamp = DateTime.Now;
            ThreadChange = 0;
            MemoryChangeMB = 0;
        }

        public override string ToString()
        {
            return string.Format(
                "Threads: {0} ({1:+#;-#;0}), Memory: {2}MB ({3:+#;-#;0}MB) at {4:HH:mm:ss}",
                ThreadCount, ThreadChange, MemoryUsageMB, MemoryChangeMB, Timestamp
            );
        }
    }

    // ✅ Extension metodlar için static class
    public static class ThreadMonitorExtensions
    {
        public static void LogThreadInfo(this ThreadInfo info, string context = "")
        {
            try
            {
                var message = string.IsNullOrEmpty(context)
                    ? info.ToString()
                    : string.Format("[{0}] {1}", context, info.ToString());

                ErrorLogger.LogError(message);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "LogThreadInfo extension error");
            }
        }

        public static bool IsCritical(this ThreadInfo info)
        {
            return info.ThreadCount > 50 || info.MemoryUsageMB > 500;
        }

        public static bool IsHighUsage(this ThreadInfo info)
        {
            return info.ThreadCount > 30 || info.MemoryUsageMB > 300;
        }
    }
}