using SigortaVipNew.Configuration; // Bu satırı ekleyin

namespace SigortaVipNew.Configuration
{
    public static class AppConstants
    {
        // Loader Messages
        public const string LoaderLoginMessage = "Giriş yapılıyor...";
        public const string LoaderCompaniesMessage = "Şirketler yükleniyor...";
        public const string LoaderUIMessage = "Arayüz hazırlanıyor...";
        public const string LoaderOpeningCompany = "açılıyor...";
        public const string LoaderRefreshing = "yenileniyor...";
        public const string LoaderGettingToken = "için token alınıyor...";
        public const string LoaderPriceQuery = "fiyat sorgulanıyor...";

        // Error Messages
        public const string ErrorCompaniesLoad = "Şirketler yüklenemedi";
        public const string ErrorMainFormInit = "Ana form başlatılamadı";
        public const string ErrorCompanyOpen = "Şirket açılamadı";
        public const string ErrorPriceQuery = "Fiyat sorgulanamadı";
        public const string ErrorCookieAdd = "Cookie eklenemedi";
        public const string ErrorTokenGet = "Token alınamadı";

        // Success Messages
        public const string SuccessCompaniesLoaded = "Şirketler başarıyla yüklendi";
        public const string SuccessCompanyOpened = "Şirket başarıyla açıldı";
        public const string SuccessPriceQueried = "Fiyat sorgulama tamamlandı";

        // UI Messages
        public const string ConfirmCloseApp = "Uygulamayı kapatmak istiyor musunuz?";
        public const string ConfirmOpenAllCompanies = "Tüm sigorta şirketleri açılacak. Devam etmek istiyor musunuz?";

        // File Names
        public const string ErrorLogFile = "errors.log";
        public const string ConfigFile = "app.config";

        // Config'den gelen değerler - artık dinamik
        public static string DefaultPhone => AppSettings.DefaultPhone;
        public static string DefaultEmail => AppSettings.DefaultEmail;
        public static int DefaultTimeout => AppSettings.ApiTimeout;
        public static int DefaultDelay => 1000; // Bu sabit kalabilir
        public static int NavPaneWidth => AppSettings.NavPaneWidth;
    }
}