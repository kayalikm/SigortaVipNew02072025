using System;
using System.Configuration;

namespace SigortaVipNew.Configuration
{
    public static class AppSettings
    {
        // API Settings
        public static string ApiBaseUrl => GetSetting("ApiBaseUrl", "https://api.kayaliksigorta.com");
        public static int ApiTimeout => GetIntSetting("ApiTimeout", 30000);
        public static string TokenEndpoint => GetSetting("TokenEndpoint", "/auth/login");

        // Cache Settings
        public static int CompanyCacheMinutes => GetIntSetting("CompanyCacheMinutes", 5);
        public static int TokenCacheMinutes => GetIntSetting("TokenCacheMinutes", 30);

        // UI Settings
        public static string DefaultTheme => GetSetting("DefaultTheme", "Office2019Colorful");
        public static int NavPaneWidth => GetIntSetting("NavPaneWidth", 220);
        public static bool ShowSplashScreen => GetBoolSetting("ShowSplashScreen", true);

        // Default Values
        public static string DefaultPhone => GetSetting("DefaultPhone", "5435467543");
        public static string DefaultEmail => GetSetting("DefaultEmail", "test@example.com");
        public static int MaxConcurrentRequests => GetIntSetting("MaxConcurrentRequests", 5);

        // Error Handling
        public static string LogLevel => GetSetting("LogLevel", "All");
        public static int LogMaxFileSizeMB => GetIntSetting("LogMaxFileSizeMB", 10);
        public static bool ShowDetailedErrors => GetBoolSetting("ShowDetailedErrors", true);

        // Browser Settings
        public static int BrowserTimeout => GetIntSetting("BrowserTimeout", 30000);
        public static int MaxBrowserTabs => GetIntSetting("MaxBrowserTabs", 20);
        public static bool EnableCaching => GetBoolSetting("EnableCaching", true);

        // Helper methods
        private static string GetSetting(string key, string defaultValue = "")
        {
            try
            {
                return ConfigurationManager.AppSettings[key] ?? defaultValue;
            }
            catch (Exception ex)
            {
                SigortaVipNew.Helpers.ErrorLogger.LogError(ex, $"Config okuma hatası: {key}");
                return defaultValue;
            }
        }

        private static int GetIntSetting(string key, int defaultValue = 0)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return int.TryParse(value, out int result) ? result : defaultValue;
            }
            catch (Exception ex)
            {
                SigortaVipNew.Helpers.ErrorLogger.LogError(ex, $"Config int okuma hatası: {key}");
                return defaultValue;
            }
        }

        private static bool GetBoolSetting(string key, bool defaultValue = false)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return bool.TryParse(value, out bool result) ? result : defaultValue;
            }
            catch (Exception ex)
            {
                SigortaVipNew.Helpers.ErrorLogger.LogError(ex, $"Config bool okuma hatası: {key}");
                return defaultValue;
            }
        }
    }
}