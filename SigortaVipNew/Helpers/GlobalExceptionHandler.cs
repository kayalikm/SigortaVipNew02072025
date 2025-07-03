using System;
using System.Threading;
using System.Windows.Forms;

namespace SigortaVipNew.Helpers
{
    public static class GlobalExceptionHandler
    {
        public static void Initialize()
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ErrorLogger.LogError(e.Exception, "Application thread exception");
            MessageBox.Show("Bir hata oluştu. Lütfen uygulamayı yeniden başlatın.",
                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorLogger.LogError(e.ExceptionObject as Exception, "Unhandled domain exception");
        }
    }
}