using System;
using System.Windows.Forms;
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
                // Global exception handling başlat
                GlobalExceptionHandler.Initialize();

                // Service container'ı başlat
                ServiceContainer = new SimpleServiceContainer();
                RegisterServices();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Ana formu başlat
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılamadı: {ex.Message}",
                    "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void RegisterServices()
        {
            // Servisleri kaydet
            ServiceContainer.RegisterInstance<SimpleCache>(new SimpleCache());
            ServiceContainer.RegisterInstance<ResourceManager>(new ResourceManager());

            // Test için - Cache'e bir test değeri ekle
            var cache = ServiceContainer.Resolve<SimpleCache>();
            cache.Set("test_key", "DI sistemi çalışıyor!", TimeSpan.FromMinutes(1));

            ErrorLogger.LogError("Services registered successfully");
            ErrorLogger.LogError("DI sistemi başarıyla başlatıldı");
        }
    }
}