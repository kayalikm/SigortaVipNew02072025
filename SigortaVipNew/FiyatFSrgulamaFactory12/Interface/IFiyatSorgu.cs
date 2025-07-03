using CefSharp.WinForms;
using SigortaVip.Business;
using SigortaVip.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SigortaVip.FiyatSorgulamaFactory.Interface
{
    public interface IFiyatSorgu
    {
        Task<FiyatBilgisi> TrafikSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken = default, IProgress<int> progress = null);

       // Task<FiyatBilgisi> KaskoSorgula(KullaniciBilgileri info, ChromiumWebBrowser browser, CancellationToken cancellationToken, IProgress<ProgressReportModel> progress = null, ProgressReportModel progressReportModel = null);
    }
}
