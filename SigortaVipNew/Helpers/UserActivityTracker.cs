using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SigortaVipNew.Helpers
{
    public class UserActivityTracker : IDisposable
    {
        private readonly List<UserActivity> _activities;
        private readonly object _lockObject;
        private readonly System.Threading.Timer _statsTimer;
        private bool _disposed = false;

        public UserActivityTracker()
        {
            _activities = new List<UserActivity>();
            _lockObject = new object();

            // Her 5 dakikada bir istatistikleri logla
            _statsTimer = new System.Threading.Timer(LogActivityStats, null,
                TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

            AdvancedLogger.LogInfo("UserActivityTracker başlatıldı", "Activity");
        }

        public void TrackFormOpen(string formName)
        {
            TrackActivity("FormOpen", formName, $"Form açıldı: {formName}");
        }

        public void TrackFormClose(string formName)
        {
            TrackActivity("FormClose", formName, $"Form kapatıldı: {formName}");
        }

        public void TrackButtonClick(string buttonName, string formName = "")
        {
            var details = string.IsNullOrEmpty(formName)
                ? $"Buton tıklandı: {buttonName}"
                : $"Buton tıklandı: {buttonName} ({formName})";
            TrackActivity("ButtonClick", buttonName, details);
        }

        public void TrackApiCall(string endpoint, string method = "GET")
        {
            TrackActivity("ApiCall", endpoint, $"API çağrısı: {method} {endpoint}");
        }

        public void TrackError(string errorType, string errorMessage)
        {
            TrackActivity("Error", errorType, $"Hata: {errorType} - {errorMessage}");
        }

        public void TrackCompanyOpen(string companyName)
        {
            TrackActivity("CompanyOpen", companyName, $"Şirket açıldı: {companyName}");
        }

        public void TrackPriceQuery(string companyName, string customerInfo = "")
        {
            var details = string.IsNullOrEmpty(customerInfo)
                ? $"Fiyat sorgusu: {companyName}"
                : $"Fiyat sorgusu: {companyName} - {customerInfo}";
            TrackActivity("PriceQuery", companyName, details);
        }

        public void TrackDataEntry(string fieldName, string value = "")
        {
            // Hassas veriyi loglama, sadece field adını kaydet
            var details = string.IsNullOrEmpty(value)
                ? $"Veri girişi: {fieldName}"
                : $"Veri girişi: {fieldName} (Uzunluk: {value.Length})";
            TrackActivity("DataEntry", fieldName, details);
        }

        public void TrackSystemEvent(string eventType, string details)
        {
            TrackActivity("SystemEvent", eventType, details);
        }

        private void TrackActivity(string activityType, string target, string details)
        {
            try
            {
                lock (_lockObject)
                {
                    var activity = new UserActivity
                    {
                        Id = Guid.NewGuid().ToString("N").Substring(0, 8),
                        Timestamp = DateTime.Now,
                        ActivityType = activityType,
                        Target = target,
                        Details = details,
                        SessionId = GetSessionId()
                    };

                    _activities.Add(activity);

                    // Log'a yazdır
                    AdvancedLogger.LogUserAction($"{activityType}: {target}", details, "Activity");

                    // Memory kontrolü - 1000'den fazla activity varsa eski olanları sil
                    if (_activities.Count > 1000)
                    {
                        var toRemove = _activities.Count - 800; // 200 tane bırak
                        _activities.RemoveRange(0, toRemove);
                        AdvancedLogger.LogInfo($"Activity temizleme: {toRemove} eski kayıt silindi", "Activity");
                    }
                }
            }
            catch (Exception ex)
            {
                AdvancedLogger.LogError(ex, "Activity tracking hatası", "Activity");
            }
        }

        public ActivityStats GetActivityStats(TimeSpan? period = null)
        {
            lock (_lockObject)
            {
                var cutoffTime = period.HasValue
                    ? DateTime.Now.Subtract(period.Value)
                    : DateTime.Today;

                var relevantActivities = _activities
                    .Where(a => a.Timestamp >= cutoffTime)
                    .ToList();

                return new ActivityStats
                {
                    Period = period ?? TimeSpan.FromHours(DateTime.Now.Hour + 1),
                    TotalActivities = relevantActivities.Count,
                    MostActiveHour = GetMostActiveHour(relevantActivities),
                    TopActivityTypes = GetTopActivityTypes(relevantActivities),
                    TopTargets = GetTopTargets(relevantActivities),
                    FirstActivity = relevantActivities.FirstOrDefault()?.Timestamp,
                    LastActivity = relevantActivities.LastOrDefault()?.Timestamp,
                    UniqueTargets = relevantActivities.Select(a => a.Target).Distinct().Count(),
                    ErrorCount = relevantActivities.Count(a => a.ActivityType == "Error")
                };
            }
        }

        private int GetMostActiveHour(List<UserActivity> activities)
        {
            if (!activities.Any()) return DateTime.Now.Hour;

            return activities
                .GroupBy(a => a.Timestamp.Hour)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }

        private Dictionary<string, int> GetTopActivityTypes(List<UserActivity> activities)
        {
            return activities
                .GroupBy(a => a.ActivityType)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<string, int> GetTopTargets(List<UserActivity> activities)
        {
            return activities
                .GroupBy(a => a.Target)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private void LogActivityStats(object state)
        {
            try
            {
                var stats = GetActivityStats(TimeSpan.FromMinutes(5));

                AdvancedLogger.LogInfo($"ACTIVITY STATS (Son 5 dakika):", "Activity");
                AdvancedLogger.LogInfo($"  Toplam aktivite: {stats.TotalActivities}", "Activity");
                AdvancedLogger.LogInfo($"  Hata sayısı: {stats.ErrorCount}", "Activity");
                AdvancedLogger.LogInfo($"  Benzersiz hedef: {stats.UniqueTargets}", "Activity");

                if (stats.TopActivityTypes.Any())
                {
                    var topActivity = stats.TopActivityTypes.First();
                    AdvancedLogger.LogInfo($"  En çok yapılan: {topActivity.Key} ({topActivity.Value}x)", "Activity");
                }
            }
            catch (Exception ex)
            {
                AdvancedLogger.LogError(ex, "Activity stats hatası", "Activity");
            }
        }

        private string GetSessionId()
        {
            // Basit session ID - uygulama başlangıcından beri geçen dakika
            return $"SES_{DateTime.Now:yyyyMMdd}_{Environment.TickCount / 60000}";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _statsTimer?.Dispose();

                lock (_lockObject)
                {
                    var finalStats = GetActivityStats();
                    AdvancedLogger.LogInfo($"Son aktivite istatistikleri: {finalStats.TotalActivities} aktivite", "Activity");
                    _activities.Clear();
                }

                AdvancedLogger.LogInfo("UserActivityTracker dispose edildi", "Activity");
                _disposed = true;
            }
        }
    }

    public class UserActivity
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; }
        public string Target { get; set; }
        public string Details { get; set; }
        public string SessionId { get; set; }
    }

    public class ActivityStats
    {
        public TimeSpan Period { get; set; }
        public int TotalActivities { get; set; }
        public int MostActiveHour { get; set; }
        public Dictionary<string, int> TopActivityTypes { get; set; }
        public Dictionary<string, int> TopTargets { get; set; }
        public DateTime? FirstActivity { get; set; }
        public DateTime? LastActivity { get; set; }
        public int UniqueTargets { get; set; }
        public int ErrorCount { get; set; }

        public override string ToString()
        {
            return $"Activities: {TotalActivities}, Errors: {ErrorCount}, Unique Targets: {UniqueTargets}";
        }
    }
}