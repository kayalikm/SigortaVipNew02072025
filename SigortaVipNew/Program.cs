using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using SigortaVipNew.Helpers;
using SigortaVipNew.Servicess;

namespace SigortaVipNew
{
    static class Program
    {
        public static IServiceContainer ServiceContainer { get; private set; }

        [STAThread]
        static void Main()
        {
            try
            {
                // 🔧 ADIM 1: Thread Pool Optimizasyonu (Basit Versiyon)
                OptimizeThreadPool();
                LogThreadCount("Başlangıç");

                GlobalExceptionHandler.Initialize();
                ServiceContainer = new SimpleServiceContainer();
                InitializeAllServices();

                LogThreadCount("Servisler Sonrası");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılamadı: {ex.Message}",
                    "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void InitializeAllServices()
        {
            AdvancedLogger.CleanOldLogs(7);
            AdvancedLogger.LogInfo("Uygulama başlatılıyor", "System");
            ServiceContainer.RegisterInstance<SimpleCache>(new SimpleCache());
            ServiceContainer.RegisterInstance<ResourceManager>(new ResourceManager());
            ServiceContainer.RegisterInstance<BackgroundTaskManager>(new BackgroundTaskManager());
            ServiceContainer.RegisterInstance<PerformanceMonitor>(new PerformanceMonitor());
            ServiceContainer.RegisterInstance<UserActivityTracker>(new UserActivityTracker());
            var cache = ServiceContainer.Resolve<SimpleCache>();
            cache.Set("test_key", "DI sistemi çalışıyor!", TimeSpan.FromMinutes(1));
            AdvancedLogger.LogInfo("Tüm servisler başarıyla kaydedildi", "DI");
            SystemHealth.LogHealthReport();
        }

        // 🔧 Thread Pool Optimizasyonu - Basit Versiyon
        private static void OptimizeThreadPool()
        {
            try
            {
                int cores = Environment.ProcessorCount;

                // Mevcut ayarları al
                ThreadPool.GetMinThreads(out int minW, out int minIO);
                ThreadPool.GetMaxThreads(out int maxW, out int maxIO);

                Console.WriteLine("=== THREAD POOL OPTİMİZASYONU ===");
                Console.WriteLine($"CPU Cores: {cores}");
                Console.WriteLine($"ÖNCE - Worker: {minW}-{maxW}, IO: {minIO}-{maxIO}");

                // Yeni ayarlar (konservatif)
                int newMinWorker = Math.Max(cores, 4);
                int newMaxWorker = Math.Min(cores * 2, 16);
                int newMinIO = Math.Max(cores, 4);
                int newMaxIO = Math.Min(cores * 3, 24);

                // Uygula
                ThreadPool.SetMinThreads(newMinWorker, newMinIO);
                ThreadPool.SetMaxThreads(newMaxWorker, newMaxIO);

                // Doğrula
                ThreadPool.GetMinThreads(out int vMinW, out int vMinIO);
                ThreadPool.GetMaxThreads(out int vMaxW, out int vMaxIO);

                Console.WriteLine($"SONRA - Worker: {vMinW}-{vMaxW}, IO: {vMinIO}-{vMaxIO}");
                Console.WriteLine("================================");

                AdvancedLogger.LogInfo($"ThreadPool optimize edildi: Worker {vMinW}-{vMaxW}, IO {vMinIO}-{vMaxIO}", "Performance");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ThreadPool hatası: {ex.Message}");
                AdvancedLogger.LogError(ex, "ThreadPool optimizasyon hatası", "Performance");
            }
        }

        // 🔧 Thread sayısını basit loglama
        private static void LogThreadCount(string context)
        {
            try
            {
                var process = Process.GetCurrentProcess();
                int threadCount = process.Threads.Count;
                long memoryMB = process.WorkingSet64 / 1024 / 1024;

                string message = $"{context} - Threads: {threadCount}, Memory: {memoryMB}MB";
                Console.WriteLine(message);
                AdvancedLogger.LogInfo(message, "Performance");

                if (threadCount > 50)
                {
                    Console.WriteLine("⚠️ YÜKSEK THREAD SAYISI!");
                    AdvancedLogger.LogWarning($"Yüksek thread sayısı: {threadCount}", "Performance");
                }
                else if (threadCount <= 25)
                {
                    Console.WriteLine("✅ Normal thread sayısı");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Thread info hatası: {ex.Message}");
            }
        }
    }
}