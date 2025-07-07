using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVipNew.Helpers
{
    public class BackgroundTaskManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, BackgroundTask> _tasks;
        private readonly Timer _cleanupTimer;
        private readonly object _lockObject;
        private bool _disposed = false;

        public BackgroundTaskManager()
        {
            _tasks = new ConcurrentDictionary<string, BackgroundTask>();
            _lockObject = new object();

            // Her 30 saniyede bir tamamlanan görevleri temizle
            _cleanupTimer = new Timer(CleanupCompletedTasks, null,
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

            ErrorLogger.LogError("BackgroundTaskManager başlatıldı");
        }

        public string StartTask(Func<CancellationToken, Task> taskFunc, string taskName = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BackgroundTaskManager));

            var taskId = taskName ?? Guid.NewGuid().ToString("N").Substring(0, 8); // Range operator yerine Substring
            var cancellationTokenSource = new CancellationTokenSource();

            var backgroundTask = new BackgroundTask
            {
                Id = taskId,
                Name = taskName ?? "Anonymous Task",
                StartTime = DateTime.Now,
                CancellationTokenSource = cancellationTokenSource,
                Status = TaskStatus.Running
            };

            // Task'ı başlat
            backgroundTask.Task = Task.Run(async () =>
            {
                try
                {
                    ErrorLogger.LogError($"Background task başlatıldı: {backgroundTask.Name} ({taskId})");
                    await taskFunc(cancellationTokenSource.Token);

                    backgroundTask.Status = TaskStatus.RanToCompletion;
                    backgroundTask.CompletedTime = DateTime.Now;
                    ErrorLogger.LogError($"Background task tamamlandı: {backgroundTask.Name} ({taskId})");
                }
                catch (OperationCanceledException)
                {
                    backgroundTask.Status = TaskStatus.Canceled;
                    backgroundTask.CompletedTime = DateTime.Now;
                    ErrorLogger.LogError($"Background task iptal edildi: {backgroundTask.Name} ({taskId})");
                }
                catch (Exception ex)
                {
                    backgroundTask.Status = TaskStatus.Faulted;
                    backgroundTask.CompletedTime = DateTime.Now;
                    backgroundTask.Exception = ex;
                    ErrorLogger.LogError(ex, $"Background task hatası: {backgroundTask.Name} ({taskId})");
                }
            });

            _tasks.TryAdd(taskId, backgroundTask);
            return taskId;
        }

        public bool CancelTask(string taskId)
        {
            if (_tasks.TryGetValue(taskId, out var task))
            {
                try
                {
                    task.CancellationTokenSource.Cancel();
                    ErrorLogger.LogError($"Background task iptal istendi: {task.Name} ({taskId})");
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, $"Task iptal hatası: {taskId}");
                }
            }
            return false;
        }

        public BackgroundTaskInfo GetTaskInfo(string taskId)
        {
            if (_tasks.TryGetValue(taskId, out var task))
            {
                return new BackgroundTaskInfo
                {
                    Id = task.Id,
                    Name = task.Name,
                    Status = task.Status,
                    StartTime = task.StartTime,
                    CompletedTime = task.CompletedTime,
                    Duration = task.CompletedTime?.Subtract(task.StartTime) ??
                              DateTime.Now.Subtract(task.StartTime),
                    HasError = task.Exception != null,
                    ErrorMessage = task.Exception?.Message
                };
            }
            return null;
        }

        public List<BackgroundTaskInfo> GetAllTasks()
        {
            var result = new List<BackgroundTaskInfo>();

            foreach (var kvp in _tasks)
            {
                var taskInfo = GetTaskInfo(kvp.Key);
                if (taskInfo != null)
                    result.Add(taskInfo);
            }

            return result;
        }

        public int GetActiveTaskCount()
        {
            int count = 0;
            foreach (var task in _tasks.Values)
            {
                if (task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingToRun)
                    count++;
            }
            return count;
        }

        private void CleanupCompletedTasks(object state)
        {
            try
            {
                var tasksToRemove = new List<string>();
                var cutoffTime = DateTime.Now.AddMinutes(-5); // 5 dakikadan eski tamamlanmış görevleri sil

                foreach (var kvp in _tasks)
                {
                    var task = kvp.Value;
                    if (task.CompletedTime.HasValue && task.CompletedTime.Value < cutoffTime)
                    {
                        tasksToRemove.Add(kvp.Key);
                    }
                }

                foreach (var taskId in tasksToRemove)
                {
                    if (_tasks.TryRemove(taskId, out var removedTask))
                    {
                        removedTask.CancellationTokenSource?.Dispose();
                    }
                }

                if (tasksToRemove.Count > 0)
                {
                    ErrorLogger.LogError($"Background task temizleme: {tasksToRemove.Count} eski görev silindi");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Background task temizleme hatası");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _cleanupTimer?.Dispose();

                // Tüm aktif görevleri iptal et
                foreach (var task in _tasks.Values)
                {
                    try
                    {
                        task.CancellationTokenSource?.Cancel();
                        task.CancellationTokenSource?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError(ex, "Task dispose hatası");
                    }
                }

                _tasks.Clear();
                ErrorLogger.LogError("BackgroundTaskManager dispose edildi");
                _disposed = true;
            }
        }

        private class BackgroundTask
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? CompletedTime { get; set; }
            public TaskStatus Status { get; set; }
            public Task Task { get; set; }
            public CancellationTokenSource CancellationTokenSource { get; set; }
            public Exception Exception { get; set; }
        }
    }


    public class BackgroundTaskInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id}) - {Status} - {Duration.TotalSeconds:F1}s";
        }
    }
}