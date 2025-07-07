using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVipNew.Helpers
{
    public class PerformanceMonitor : IDisposable
    {
        private readonly Dictionary<string, PerformanceCounter> _performanceMetrics;
        private readonly Timer _monitoringTimer;
        private readonly Process _currentProcess;
        private bool _disposed = false;

        public PerformanceMonitor()
        {
            _performanceMetrics = new Dictionary<string, PerformanceCounter>();
            _currentProcess = Process.GetCurrentProcess();

            // Her 30 saniyede bir performans metrikleri kaydet
            _monitoringTimer = new Timer(LogPerformanceMetrics, null,
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

            ErrorLogger.LogError("PerformanceMonitor başlatıldı");
        }

        public PerformanceMetrics GetCurrentMetrics()
        {
            try
            {
                _currentProcess.Refresh();

                return new PerformanceMetrics
                {
                    Timestamp = DateTime.Now,
                    CpuUsage = GetCpuUsage(),
                    MemoryUsageMB = _currentProcess.WorkingSet64 / (1024 * 1024),
                    ThreadCount = _currentProcess.Threads.Count,
                    HandleCount = _currentProcess.HandleCount,
                    GCMemoryKB = GC.GetTotalMemory(false) / 1024,
                    Gen0Collections = GC.CollectionCount(0),
                    Gen1Collections = GC.CollectionCount(1),
                    Gen2Collections = GC.CollectionCount(2)
                };
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Performance metrics alma hatası");
                return new PerformanceMetrics { Timestamp = DateTime.Now };
            }
        }

        public async Task<TimeSpan> MeasureExecutionTimeAsync(Func<Task> operation, string operationName = "Unknown")
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await operation();
                return stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Measured operation failed: {operationName}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                ErrorLogger.LogError($"Operation '{operationName}' took {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        public TimeSpan MeasureExecutionTime(Action operation, string operationName = "Unknown")
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                operation();
                return stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Measured operation failed: {operationName}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                ErrorLogger.LogError($"Sync operation '{operationName}' took {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        public T MeasureExecutionTime<T>(Func<T> operation, string operationName = "Unknown")
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = operation();
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Measured operation failed: {operationName}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                ErrorLogger.LogError($"Sync operation '{operationName}' took {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private void LogPerformanceMetrics(object state)
        {
            try
            {
                var metrics = GetCurrentMetrics();

                ErrorLogger.LogError($"PERFORMANCE METRICS:");
                ErrorLogger.LogError($"  Memory: {metrics.MemoryUsageMB}MB");
                ErrorLogger.LogError($"  GC Memory: {metrics.GCMemoryKB}KB");
                ErrorLogger.LogError($"  Threads: {metrics.ThreadCount}");
                ErrorLogger.LogError($"  Handles: {metrics.HandleCount}");
                ErrorLogger.LogError($"  GC Collections - Gen0: {metrics.Gen0Collections}, Gen1: {metrics.Gen1Collections}, Gen2: {metrics.Gen2Collections}");

                // Memory uyarıları
                if (metrics.MemoryUsageMB > 500) // 500MB üzeri
                {
                    ErrorLogger.LogError($"UYARI: Yüksek memory kullanımı: {metrics.MemoryUsageMB}MB");

                    if (metrics.MemoryUsageMB > 1000) // 1GB üzeri
                    {
                        ErrorLogger.LogError("KRİTİK: Çok yüksek memory kullanımı! GC zorlanıyor...");
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }

                // Thread sayısı uyarısı
                if (metrics.ThreadCount > 50)
                {
                    ErrorLogger.LogError($"UYARI: Yüksek thread sayısı: {metrics.ThreadCount}");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Performance monitoring hatası");
            }
        }

        private double GetCpuUsage()
        {
            try
            {
                return _currentProcess.TotalProcessorTime.TotalMilliseconds;
            }
            catch
            {
                return 0;
            }
        }

        public void ForceGarbageCollection()
        {
            try
            {
                var beforeMemory = GC.GetTotalMemory(false);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var afterMemory = GC.GetTotalMemory(false);
                var freedMemory = (beforeMemory - afterMemory) / 1024;

                ErrorLogger.LogError($"Manuel GC tamamlandı. Serbest bırakılan: {freedMemory}KB");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Manuel GC hatası");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _monitoringTimer?.Dispose();

                foreach (var counter in _performanceMetrics.Values)
                {
                    try
                    {
                        counter.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError(ex, "Performance counter dispose hatası");
                    }
                }

                _performanceMetrics.Clear();
                _currentProcess?.Dispose();

                ErrorLogger.LogError("PerformanceMonitor dispose edildi");
                _disposed = true;
            }
        }
    }

    public class PerformanceMetrics
    {
        public DateTime Timestamp { get; set; }
        public double CpuUsage { get; set; }
        public long MemoryUsageMB { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public long GCMemoryKB { get; set; }
        public int Gen0Collections { get; set; }
        public int Gen1Collections { get; set; }
        public int Gen2Collections { get; set; }

        public override string ToString()
        {
            return $"Memory: {MemoryUsageMB}MB, Threads: {ThreadCount}, GC: {GCMemoryKB}KB";
        }
    }
}