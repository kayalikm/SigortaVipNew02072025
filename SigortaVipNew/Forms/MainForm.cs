using CefSharp;
using CefSharp.DevTools.Network;
using CefSharp.WinForms;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;
using DevExpress.XtraTab;
using DevExpress.XtraWaitForm;
using Newtonsoft.Json;
using SigortaVip.Business;
using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory.Concrete;
using SigortaVip.Forms;
using SigortaVip.Helpers;
using SigortaVip.Helpers.TrafikFiyatSorgulamaFactory;
using SigortaVip.List;
using SigortaVip.Models;
using SigortaVip.Models;
using SigortaVip.Utility;
using SigortaVip.Utility.ScheduledJobs;
using SigortaVipNew.Api;
using SigortaVipNew.Handler;
using SigortaVipNew.Models;
using SigortaYazilim.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Cookie = CefSharp.Cookie;
using SigortaVipNew.Helpers;
using SigortaVipNew.Configuration;
namespace SigortaVipNew
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private static readonly object lockObj = new object();
        private static bool cefInitialized = false;
        private Dictionary<string, ChromiumWebBrowser> browserTabs = new Dictionary<string, ChromiumWebBrowser>();
        private List<InsuranceCompanyItem> insuranceCompanies;
        private DevExpress.XtraBars.BarButtonItem btnEditCompany;
        private DevExpress.XtraBars.BarButtonItem btnOpenAll;
        private DevExpress.XtraBars.PopupMenu contextMenu;
        private NavBarGroup companiesGroup;
        private ResourceManager _resourceManager;
        private SimpleCache _cache;



        private bool isClosingTab = false;
        public static string activeTabPage = "";
        public static string _token = string.Empty;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption,
            IntPtr lpBuffer, int lpdwBufferLength);
        private hizliTeklifFormNew _hizliTeklifForm;
        private KullaniciBilgileri _aktifMusteriBilgileri;

        public MainForm()
        {
            try
            {
                InitializeComponent();

                // Service container'dan servisleri al
                _resourceManager = Program.ServiceContainer.Resolve<ResourceManager>();
                _cache = Program.ServiceContainer.Resolve<SimpleCache>();

                // Exception handling'i başlat
                this.SetupExceptionHandling();

                //InitializeCef();
                // Browser dictionary'sini başlangıçta temizle
                browserTabs = new Dictionary<string, ChromiumWebBrowser>();

                // Config test logları
                ErrorLogger.LogError($"Config Test - NavPaneWidth: {AppSettings.NavPaneWidth}");
                ErrorLogger.LogError($"Config Test - DefaultPhone: {AppSettings.DefaultPhone}");
                ErrorLogger.LogError($"Config Test - CompanyCacheMinutes: {AppSettings.CompanyCacheMinutes}");

                // DI test kodu
                try
                {
                    var testValue = _cache.Get<string>("test_key");
                    if (testValue != null)
                    {
                        ErrorLogger.LogError($"DI Cache Test Başarılı: {testValue}");
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, "DI test hatası");
                }

                // Temalar ve görsel ayarlarını uygula
                ApplyVisualStyle();

                SetupUI();
                SetupRibbonItems();

                // Form kapanmasını engelle (X butonuna basılsa bile)
                this.FormClosing += MainForm_FormClosing;

                // Asenkron yükleme için Load eventını kullanın
                this.Load += async (sender, e) => await LoadCompanies();

                ErrorLogger.LogError("MainForm başarıyla başlatıldı - DI sistemi aktif");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "MainForm constructor hatası");
                MessageBox.Show($"Ana form başlatılamadı: {ex.Message}", "Kritik Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapatma olayından geldiyse ve bir sekme kapatma işlemi değilse
            if (e.CloseReason == CloseReason.UserClosing && !isClosingTab)
            {
                // Normal kapanma onayı
                if (MessageBox.Show("Uygulamayı kapatmak istiyor musunuz?", "Çıkış Onayı",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
        private void LoadCompaniesToNavBar(List<InsuranceCompanyItem> companies)
        {
            try
            {
                // UI thread'de olduğumuzdan emin olalım
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => LoadCompaniesToNavBar(companies)));
                    return;
                }

                if (companies == null || companies.Count == 0)
                {
                    ErrorLogger.LogError("Şirket listesi boş");
                    return;
                }

                // NavPane'in hazır olduğundan emin olalım
                if (navPane == null || navPane.IsDisposed)
                {
                    ErrorLogger.LogError("NavPane hazır değil");
                    return;
                }

                // companiesGroup'u kontrol et ve oluştur
                if (companiesGroup == null)
                {
                    companiesGroup = new DevExpress.XtraNavBar.NavBarGroup("Sigorta Firmaları");
                    companiesGroup.Expanded = true;

                    // Grubu NavPane'e ekle
                    if (navPane.Groups != null)
                    {
                        navPane.Groups.Add(companiesGroup);
                    }
                    else
                    {
                        ErrorLogger.LogError("NavPane.Groups null");
                        return;
                    }
                }

                // Mevcut öğeleri güvenli şekilde temizle
                if (companiesGroup.ItemLinks != null)
                {
                    companiesGroup.ItemLinks.Clear();
                }

                // Şirketleri ekle
                foreach (var company in companies)
                {
                    try
                    {
                        if (company?.InsuranceCompany?.Name != null)
                        {
                            var navItem = new DevExpress.XtraNavBar.NavBarItem();
                            navItem.Caption = company.InsuranceCompany.Name;
                            navItem.Tag = company;

                            // Icon eklemeyi geçici olarak skip edelim
                            // navItem.SmallImage = GetCompanyIcon(company.InsuranceCompany.Name);

                            // Event handler ekle
                            navItem.LinkClicked += NavItem_LinkClicked;

                            // ItemLinks'e ekle
                            if (companiesGroup.ItemLinks != null)
                            {
                                companiesGroup.ItemLinks.Add(navItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError(ex, $"Şirket ekleme hatası: {company?.InsuranceCompany?.Name}");
                    }
                }

                ErrorLogger.LogError($"NavBar'a {companies.Count} şirket eklendi");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "NavBar şirket yükleme hatası");
            }
        }

        private async Task LoadCompanies()
        {
            try
            {
                ShowLoader(AppConstants.LoaderLoginMessage);
                await LoginUserAsync();
                UpdateLoader(AppConstants.LoaderCompaniesMessage);
                insuranceCompanies = await GetInsuranceCompanies();
                UpdateLoader(AppConstants.LoaderUIMessage);
                LoadCompaniesToNavBar(insuranceCompanies);  // Şimdi bu çalışacak
                HideLoader();
                ErrorLogger.LogError(AppConstants.SuccessCompaniesLoaded);
            }
            catch (Exception ex)
            {
                HideLoader();
                ErrorLogger.LogError(ex, "Şirketler yüklenirken hata");
                MessageBox.Show($"Şirketler yüklenemedi: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyVisualStyle()
        {
            // Modern tema uygula
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(DevExpress.LookAndFeel.SkinStyle.Office2019Colorful);
            
            // Form özellikleri
            this.Text = "Sigorta VIP";
            // İkon ayarlamasını geçici olarak kaldırıldı - hata veriyor
            // this.Icon = DevExpress.Utils.ResourceImageHelper.CreateIconFromResources("DevExpress.Images.Office2013.About.png", typeof(DevExpress.Utils.ResourceImageHelper).Assembly);
            
            // Ribbon renk ayarları
            ribbon.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2019;
            ribbon.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
        }
        private void SetupExceptionHandling()
        {
            try
            {
                // Tüm async operasyonlar için exception handling
                System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) =>
                {
                    HandleException(e.Exception, "Unobserved task exception");
                    e.SetObserved();
                };

                ErrorLogger.LogError("Exception handling kuruldu");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Exception handling kurulum hatası");
            }
        }

        private void HandleException(Exception ex, string context = "")
        {
            try
            {
                var errorMessage = string.IsNullOrEmpty(context)
                    ? $"Hata: {ex.Message}"
                    : $"Hata ({context}): {ex.Message}";

                ErrorLogger.LogError(ex, context);

                // Kullanıcıya basit mesaj göster
                MessageBox.Show(errorMessage, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                // Hata handling'de bile hata olursa sessiz kal
            }
        }
        private void SetupUI()
        {
            // NavBarControl'ün görünümünü ayarla - sabit genişlik ve düğmesiz
            navPane.OptionsNavPane.ExpandedWidth = 220;
            navPane.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            navPane.OptionsNavPane.ShowExpandButton = false; // Genişletme düğmesi kaldırıldı
            navPane.OptionsNavPane.ShowOverflowButton = false; // Taşma düğmesi kaldırıldı
            navPane.OptionsNavPane.ShowOverflowPanel = false; // Taşma paneli kaldırıldı
            navPane.OptionsNavPane.ShowSplitter = false; // Bölücü çizgiyi kaldır
            navPane.PaintStyleKind = NavBarViewKind.NavigationPane;
            navPane.StoreDefaultPaintStyleName = true;
            navPane.Dock = DockStyle.Left;

            // Görsel stil ayarları
            navPane.Appearance.Background.BackColor = Color.FromArgb(240, 240, 240);
            navPane.Appearance.GroupHeader.Font = new Font("Segoe UI Semibold", 10F);
            navPane.Appearance.GroupHeader.Options.UseFont = true;
            navPane.Appearance.Item.Font = new Font("Segoe UI", 9.75F);
            navPane.Appearance.Item.Options.UseFont = true;

            // Mevcut grupları temizle
            navPane.Groups.Clear();

            // Şirketler grubu
            companiesGroup = new NavBarGroup("Sigorta Firmaları");
            companiesGroup.Expanded = true;
            companiesGroup.SmallImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocontact2_16x16.png");
            companiesGroup.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocontact2_32x32.png");
            navPane.Groups.Add(companiesGroup);

            // Şirketleri NavBar'a ekle
            LoadCompaniesToNavBar(insuranceCompanies);

            // Sekme kontrolü kurulumu
            tabControl.CloseButtonClick += TabControl_CloseButtonClick;
            tabControl.AppearancePage.Header.Font = new Font("Segoe UI", 9.75F);
            tabControl.AppearancePage.Header.Options.UseFont = true;
            
            // Anasayfa sekmesinin kapatma butonunu gizle
            tabMainPage.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
            tabMainPage.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/navigation/home_16x16.png");

            // Ana sayfaya hoşgeldiniz içeriği ekle
            CreateWelcomePage(tabMainPage);

            // NavBar öğeleri için sağ tık menüsü
            contextMenu = new DevExpress.XtraBars.PopupMenu(components);
            btnEditCompany = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Düzenle");
            btnEditCompany.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_16x16.png");
            contextMenu.AddItem(btnEditCompany);

            navPane.MouseUp += NavPane_MouseUp;
        }

        private void CreateWelcomePage(XtraTabPage page)
        {
            // Clear existing controls
            page.Controls.Clear();

            // Initialize CEF if not already initialized
            lock (lockObj)
            {
                if (!cefInitialized)
                {
                    string sessionId = Guid.NewGuid().ToString();
                    string rootCachePath = Path.Combine(Path.GetTempPath(), $"cef_root_{sessionId}");
                    string cachePath = Path.Combine(rootCachePath, "cache");

                    var settings = new CefSettings
                    {
                        RootCachePath = rootCachePath,
                        CachePath = cachePath
                    };

                    Cef.Initialize(settings);
                    cefInitialized = true;
                }
            }

            // Create browser control burada ? token gönderilecek ona göre kullanıcı otomatik login olacak

            var browser = new ChromiumWebBrowser("https://api.kayaliksigorta.com?token=sadjhkdjsahdj")
            {
                Dock = DockStyle.Fill
            };

            // Add event handlers
            browser.LoadError += Browser_LoadError;
            browser.ConsoleMessage += Browser_ConsoleMessage;

            // Add to page
            page.Controls.Add(browser);

            // Add to browser dictionary with special key
            browserTabs["home_tab"] = browser;
        }

        //Şirketleri NavBar'a yükler
        private async Task<List<InsuranceCompanyItem>> GetInsuranceCompanies()
        {
            const string cacheKey = "insurance_companies";

            // Önce cache'den kontrol et
            var cachedCompanies = _cache.Get<List<InsuranceCompanyItem>>(cacheKey);
            if (cachedCompanies != null)
            {
                ErrorLogger.LogError("Şirketler cache'den alındı");
                return cachedCompanies;
            }

            // Cache'de yoksa API'den al
            var api = new InsuranceCompanyApiService();
            try
            {
                var response = await api.GetAllAsync();
                var companies = response ?? new List<InsuranceCompanyItem>();

                // Cache'e kaydet (5 dakika süreyle)
                _cache.Set(cacheKey, companies, TimeSpan.FromMinutes(5));

                ErrorLogger.LogError($"Şirketler API'den alındı ve cache'lendi: {companies.Count} şirket");
                return companies;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Şirketler yüklenirken hata");
                return new List<InsuranceCompanyItem>();
            }
        }
        private Image GetCompanyIcon(string companyName)
        {
            // Şirket adı bazı kelimeler içeriyorsa özel ikonlar göster
            string nameLower = companyName.ToLower();
            
            if (nameLower.Contains("AkSigorta") || nameLower.Contains("ak sigorta"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocustomer_16x16.png");
            else if (nameLower.Contains("allianz"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bosale_16x16.png");
            else if (nameLower.Contains("anadolu"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/boresource_16x16.png");
            else if (nameLower.Contains("mapfre"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/boproduct_16x16.png");
            else
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocontact_16x16.png");
        }

        private void NavPane_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                NavBarHitInfo hitInfo = navPane.CalcHitInfo(e.Location);
                if (hitInfo.InLink)
                {
                    navPane.SelectedLink = hitInfo.Link;
                    contextMenu.ShowPopup(navPane.PointToScreen(e.Location));
                }
            }
        }

        private void SetupRibbonItems()
        {
            //// Ana sayfa ribbon grubuna "Toplu Aç" butonu ekle
            //var operationsGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup("İşlemler");
            //ribbonPage1.Groups.Add(operationsGroup);
            
            //btnOpenAll = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Tüm Şirketleri Aç");
            //btnOpenAll.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/open_16x16.png");
            //btnOpenAll.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/open_32x32.png");
            //btnOpenAll.ItemClick += BtnOpenAll_ItemClick;
            //ribbon.Items.Add(btnOpenAll);
            //operationsGroup.ItemLinks.Add(btnOpenAll);

           

            //// Sayfa yenileme butonu
            //var refreshBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Sayfayı Yenile");
            //refreshBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/refresh_16x16.png");
            //refreshBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/refresh_32x32.png");
            //refreshBtn.ItemShortcut = new DevExpress.XtraBars.BarShortcut(Keys.Control | Keys.R);
            //refreshBtn.ItemClick += (s, e) => RefreshCurrentTab();
            //ribbon.Items.Add(refreshBtn);
            //operationsGroup.ItemLinks.Add(refreshBtn);

            //// Sekme kapatma butonu
            //var closeTabBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Sekmeyi Kapat");
            //closeTabBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/close_16x16.png");
            //closeTabBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/close_32x32.png");
            //closeTabBtn.ItemShortcut = new DevExpress.XtraBars.BarShortcut(Keys.Control | Keys.W);
            //closeTabBtn.ItemClick += (s, e) => CloseCurrentTab();
            //ribbon.Items.Add(closeTabBtn);
            //operationsGroup.ItemLinks.Add(closeTabBtn);

            //// Fiyat toplama butonu
            //var getPricesBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Fiyat Topla");
            //getPricesBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocustomer_16x16.png");
            //getPricesBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocustomer_32x32.png");
    
            //ribbon.Items.Add(getPricesBtn);
            //operationsGroup.ItemLinks.Add(getPricesBtn);

         

            // Şirket düzenleme butonu
            //var editCompanyBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Şirketi Düzenle");
            //editCompanyBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_16x16.png");
            //editCompanyBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_32x32.png");
            //editCompanyBtn.ItemClick += (s, e) => 
            //{
            //    XtraTabPage page = tabControl.SelectedTabPage;
            //    if (page != null && page != tabMainPage)
            //    {
            //        var tabKey = page.Tag?.ToString();
            //        if (!string.IsNullOrEmpty(tabKey))
            //        {
            //            string companyName = tabKey.Substring(4); // "tab_" kısmını çıkar
            //            var company = insuranceCompanies.FirstOrDefault(c => c.InsuranceCompany.Name == companyName);
            //            if (company != null)
            //            {
                            
            //            }
            //        }
            //    }
            //};
          //  ribbon.Items.Add(editCompanyBtn);
          //;
        }


        private void CloseCurrentTab()
        {
            isClosingTab = true;
            try
            {
                XtraTabPage currentPage = tabControl.SelectedTabPage;
                if (currentPage != null && currentPage != tabMainPage)
                {
                    var tabKey = currentPage.Tag?.ToString();
                    if (!string.IsNullOrEmpty(tabKey) && browserTabs.ContainsKey(tabKey))
                    {
                        try
                        {
                            var browser = browserTabs[tabKey];
                            currentPage.Controls.Remove(browser);
                            browser.Dispose();
                            browserTabs.Remove(tabKey);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Browser kapatılırken hata: {ex.Message}");
                        }
                    }

                    tabControl.SelectedTabPage = tabMainPage;
                    tabControl.TabPages.Remove(currentPage); // Only remove, don't dispose
                }
            }
            finally
            {
                isClosingTab = false;
            }
        }
        //private void RefreshCurrentTab()
        //{
        //    XtraTabPage currentPage = tabControl.SelectedTabPage;
            
        //    // Ana sayfa değilse ve içerisinde browser varsa yenile
        //    if (currentPage != null && currentPage != tabMainPage)
        //    {
        //        var tabKey = currentPage.Tag?.ToString();
        //        if (!string.IsNullOrEmpty(tabKey) && browserTabs.ContainsKey(tabKey))
        //        {
        //            var browser = browserTabs[tabKey];
        //            if (browser != null && !browser.IsDisposed)
        //            {
        //                // Sayfayı tamamen yenile (önbellekten değil)
        //                browser.GetBrowser().Reload(true);
                        
        //                // Opsiyonel: Yenileniyor mesajı göster
        //                browser.LoadingStateChanged += (s, e) => {
        //                    if (e.IsLoading)
        //                    {
        //                        // Yükleniyor
        //                        this.Invoke(new Action(() => {
        //                            this.Text = $"{currentPage.Text} - Yenileniyor...";
        //                        }));
        //                    }
        //                    else
        //                    {
        //                        // Yükleme tamamlandı
        //                        this.Text = "Anasayfa";
        //                    }
        //                };
        //            }
        //        }
        //    }
        //}

    


       

        private async void NavItem_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            var navItem = e.Link.Item;
            var company = (InsuranceCompanyItem)navItem.Tag;
            OpenCompanyTab(company);

        }


        private async Task AddCookiesToBrowserOld(ChromiumWebBrowser browser, List<CefSharp.Cookie> cookies, string url)
        {
            var cookieManager = Cef.GetGlobalCookieManager();
            var uri = new Uri(url);

            foreach (var cookie in cookies)
            {
                // Domain'i URL'den al eğer cookie'de yoksa
                if (string.IsNullOrEmpty(cookie.Domain))
                {
                    cookie.Domain = uri.Host;
                }

                // Path'i ayarla eğer yoksa
                if (string.IsNullOrEmpty(cookie.Path))
                {
                    cookie.Path = "/";
                }

                var success = await cookieManager.SetCookieAsync(url, cookie);
                Console.WriteLine($"Cookie set: {cookie.Name} = {cookie.Value} for {cookie.Domain} - Success: {success}");
            }

            // Cookie'lerin tamamen işlenmesi için daha uzun bekleme
            await Task.Delay(1000);

            // Cookie'lerin gerçekten eklendiğini doğrula
            await VerifyCookiesAdded(cookieManager, uri.Host, cookies);
        }
        private async Task VerifyCookiesAddedOld(ICookieManager cookieManager, string domain, List<CefSharp.Cookie> expectedCookies)
        {
            try
            {
                var visitor = new CookieVisitor();
                cookieManager.VisitAllCookies(visitor);

                // Kısa bir bekleme sonrası cookie'leri kontrol et
                await Task.Delay(200);

                Console.WriteLine($"Total cookies found for verification: {visitor.Cookies.Count}");
                foreach (var cookie in expectedCookies)
                {
                    var found = visitor.Cookies.Any(c => c.Name == cookie.Name && c.Domain.Contains(domain));
                    Console.WriteLine($"Cookie {cookie.Name} verification: {found}");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Cookie ekleme hatası");
                MessageBox.Show("Cookie eklenemedi.\nHata Detayı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task AddCookiesToBrowser(ChromiumWebBrowser browser, List<CefSharp.Cookie> cookies, string url)
        {
            var rc = browser.GetBrowser().GetHost().RequestContext;
            var cookieManager = await rc.GetCookieManagerAsync();

            var uri = new Uri(url);

            foreach (var cookie in cookies)
            {
                if (string.IsNullOrEmpty(cookie.Domain))
                    cookie.Domain = "." + uri.Host;

                if (string.IsNullOrEmpty(cookie.Path))
                    cookie.Path = "/";

                if (cookie.Expires == default)
                    cookie.Expires = DateTime.Now.AddDays(7);

                bool success = await cookieManager.SetCookieAsync(url, cookie);
                Console.WriteLine($"Cookie set: {cookie.Name}={cookie.Value}; Domain={cookie.Domain}; Success={success}");
            }

            await cookieManager.FlushStoreAsync();

            await VerifyCookiesAdded(cookieManager, url, cookies);
        }

        private async Task VerifyCookiesAdded(ICookieManager cookieManager, string url, List<CefSharp.Cookie> expected)
        {
            var visitor = new CookieVisitor();
            cookieManager.VisitUrlCookies(url, includeHttpOnly: true, visitor);

            //await Task.Delay(3000);

            //Console.WriteLine($"Total cookies for {url}: {visitor.Cookies.Count}");
            foreach (var exp in expected)
            {
                bool found = visitor.Cookies.Any(c =>
                    c.Name == exp.Name &&
                    c.Domain == "."+exp.Domain &&
                    c.Value == exp.Value);
                Console.WriteLine($" • {exp.Name}: {(found ? "OK" : "MISSING")}");
            }
        }

        public class CookieVisitor : ICookieVisitor
        {
            public List<CefSharp.Cookie> Cookies { get; } = new List<CefSharp.Cookie>();

            public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
            {
                Cookies.Add(cookie);
                return true;
            }

            public void Dispose()
            {
                // Cleanup if needed
            }
        }


        private async void BuildBrowserWithProxy(string url, List<InsuranceCookie> insuranceCookies, string proxyServer, string username, string password, string tabTitle, string tabKey, string code, InsuranceCompanyItem company)
        {
            lock (lockObj)
            {
                if (!cefInitialized)
                {
                    string sessionId = Guid.NewGuid().ToString();
                    string rootCachePath = Path.Combine(Path.GetTempPath(), $"cef_root_{sessionId}");
                    string cachePath = Path.Combine(rootCachePath, "cache");

                    var settings = new CefSettings
                    {
                        RootCachePath = rootCachePath,
                        CachePath = cachePath
                    };

                    Cef.Initialize(settings);
                    cefInitialized = true;
                }
            }

            var requestContextBuilder = RequestContext.Configure().WithProxyServer(proxyServer);
            var requestContext = requestContextBuilder.Create();

            var browser = new ChromiumWebBrowser
            {
                Dock = DockStyle.Fill,
                BrowserSettings = new BrowserSettings
                {
                    DefaultEncoding = "UTF-8",
                    Javascript = CefState.Enabled,
                    LocalStorage = CefState.Enabled,
                    ImageLoading = CefState.Enabled,
                },
                RequestContext = requestContext
            };



            // Eğer kullanıcı adı ve şifre boş değilse özel RequestHandler kullan
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                browser.RequestHandler = new CustomRequestHandler(username, password);
            }
            browser.LifeSpanHandler = new CustomLifeSpanHandler();
            browser.FrameLoadEnd += Browser_FrameLoadEnd;

            browserTabs[tabKey] = browser;

            BeginInvoke(new Action(() =>
            {
                var tabPage = new XtraTabPage
                {
                    Text = tabTitle,
                    Tag = tabKey,
                    ShowCloseButton = DevExpress.Utils.DefaultBoolean.True
                };

                tabPage.Controls.Add(browser);
                tabControl.TabPages.Add(tabPage);
                tabControl.SelectedTabPage = tabPage;
            }));



            browser.IsBrowserInitializedChanged += async (sender, args) =>
            {
                if (browser.IsBrowserInitialized && insuranceCookies != null && insuranceCookies.Any())
                {
                    try
                    {
                        var uri = new Uri(url);
                        var domain = uri.Host;

                        var cookieString = string.Join("; ", insuranceCookies.Select(c => $"{c.Name}={c.Value}"));
                        var cefSharpCookies = InsuranceCookieExtensions.ParseCookieStringToCefSharp(cookieString, domain);

                        Console.WriteLine($"Setting {cefSharpCookies.Count} cookies for {domain}...");
                        var DontUseCookies = new List<string> {
                            InsuranceConstants.CorpusSigorta,
                            InsuranceConstants.BereketSigorta,
                            InsuranceConstants.AnkaraSigorta,
                            InsuranceConstants.SekerSigorta,
                            InsuranceConstants.AllianzSigorta,
                            InsuranceConstants.QuickSigorta,
                            InsuranceConstants.QuickPortal,
                        };

                        if (!DontUseCookies.Contains(code)) {
                            await AddCookiesToBrowser(browser, cefSharpCookies, url);
                        }
                        //await Task.Delay(2000); // Küçük gecikme
                        browser.LoadUrl(url);
                        Console.WriteLine("✅ Cookies set successfully.");
                        var webPage = Browser.webPageList?.FirstOrDefault(x => x.insuranceCompany == company.InsuranceCompany.Name);

                        if (webPage == null)
                        {
                            // WebPage bulunamazsa yeni oluştur
                            webPage = new WebPage
                            {
                                browser = browser,
                                insuranceCompany = company.InsuranceCompany.Name,
                                insuranceCompanyId = company.InsuranceCompany.Id, // Eğer Id property'si varsa
                                initialCookies = new List<CefSharp.Cookie>() // Boş liste ile başlat
                            };

                            // Browser.webPageList'e ekle (eğer null değilse)
                            if (Browser.webPageList == null)
                            {
                                Browser.webPageList = new List<WebPage>();
                            }
                            Browser.webPageList.Add(webPage);
                        }

                        // Cookie yenileme işlemi
                        ActivePageReloaderJob activePageReloaderJob = new ActivePageReloaderJob();
                        await activePageReloaderJob.RefreshCookies(webPage, company.Id);

                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fiyat sorgulama işlemi başarısız.\nHata Detayı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Console.WriteLine($"simpleButton3_Click Genel Hata: {ex.Message}");
                    }
                }
                else if (browser.IsBrowserInitialized)
                {
                    browser.LoadUrl(url); // Cookie yoksa yine de yükle
                }
            };


        }


        // Şirket Aç
        private void OpenCompanyTab(InsuranceCompanyItem company)
        {
            string tabKey = $"tab_{company.InsuranceCompany.Name}";

            // Check if tab already exists
            if (browserTabs.ContainsKey(tabKey))
            {
                // Find existing tab page
                foreach (XtraTabPage page in tabControl.TabPages)
                {
                    if (page.Tag?.ToString() == tabKey)
                    {
                        // Switch to existing tab and bring to front
                        tabControl.SelectedTabPage = page;
                        return;
                    }
                }

                // If not found in UI but exists in dictionary, clean up
                browserTabs.Remove(tabKey);
            }

            var proxyServer = company.ProxyUrl.ToString() ?? "";
            var username = company.ProxyUsername;
            var password = company.ProxyPassword;

            BuildBrowserWithProxy(
                company.InsuranceCompany.LoginUrl.ToString() ?? "",
                company.Cookie,
                proxyServer,
                username,
                password,
                company.InsuranceCompany.Name,
                tabKey,
                company.InsuranceCompany.Code,
                company
            );
        }
        
        
        private InsuranceCompanyItem GetCompanyFromTabKey(string tabKey)
        {
            if (string.IsNullOrEmpty(tabKey) || !tabKey.StartsWith("tab_"))
                return null;

            string companyName = tabKey.Substring(4); // "tab_" kısmını çıkar

            return insuranceCompanies?.FirstOrDefault(c =>
                string.Equals(c.InsuranceCompany.Name, companyName, StringComparison.OrdinalIgnoreCase));
        }


        private readonly HashSet<string> executedScripts = new HashSet<string>();
        private readonly Dictionary<string, DateTime> scriptExecutionTime = new Dictionary<string, DateTime>();
        public static readonly string[] smsInsuranceCompanies = InsuranceCompanies.smsInsuranceCompanies;

        private async void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (!e.Frame.IsMain || !e.Frame.IsValid || e.HttpStatusCode != 200)
                return;

            var browser = (ChromiumWebBrowser)sender;
            string tabKey = browserTabs.FirstOrDefault(x => x.Value == browser).Key;
            if (string.IsNullOrEmpty(tabKey)) return;

            // Bu URL + tabKey kombinasyonu için script zaten çalıştırıldı mı?
            string scriptKey = $"{tabKey}_{e.Frame.Url}";

            // Eğer script son 5 saniye içinde çalıştırıldıysa tekrar çalıştırma
            if (executedScripts.Contains(scriptKey) &&
                scriptExecutionTime.ContainsKey(scriptKey) &&
                (DateTime.Now - scriptExecutionTime[scriptKey]).TotalSeconds < 5)
            {
                return;
            }

            var company = GetCompanyFromTabKey(tabKey);
            if (company == null) return;

            var code = company.InsuranceCompany.Code;

            // Şirketin SMS şirketi olup olmadığını kontrol et
            bool isSmsCompany = smsInsuranceCompanies.Contains(code, StringComparer.OrdinalIgnoreCase);

            // Login script mapping kontrolü
            if (!LoginScriptMappings.ScriptMap.TryGetValue(code, out var smsScriptInfo))
                return;

            // Script'i çalıştırıldı olarak işaretle ve zamanını kaydet
            executedScripts.Add(scriptKey);
            scriptExecutionTime[scriptKey] = DateTime.Now;

            try
            {
                // DOM hazır olmasını bekle
                await WaitForDomReady(browser);

                // Login script'ini çalıştır
                var loginScript = CreateLoginScript.GenerateLoginScript(
                    company.Username,
                    company.Password,
                    smsScriptInfo,
                    smsScriptInfo.Delay
                );

                var result = await browser.GetMainFrame().EvaluateScriptAsync(loginScript);

                Console.WriteLine($"Login Script result: {result.Message}");
                Debug.WriteLine(result.Success
                    ? $"Login script executed for {company.InsuranceCompany.Name}"
                    : $"Login script failed for {company.InsuranceCompany.Name}: {result.Message}");

                // SMS şirketi ise SMS script çalıştır, değilse OTP script çalıştır
                if (isSmsCompany)
                {
                    Debug.WriteLine($"{company.InsuranceCompany.Name} is SMS company - executing SMS script");

                    // SMS script mapping kontrolü
                    if (SmsScriptMappings.ScriptMap.TryGetValue(code, out var smsScriptMappingInfo))
                    {
                        // Login'den sonra kısa bekleme
                        await Task.Delay(3000);

                        // SMS script'ini oluştur ve çalıştır
                        var smsScript = await CreateSmsScript.GenerateSmsScriptAsync(
                            company.Username,
                            company.Password,
                            smsScriptMappingInfo
                        );

                        if (!string.IsNullOrEmpty(smsScript))
                        {
                            var smsResult = await browser.GetMainFrame().EvaluateScriptAsync(smsScript);

                            Console.WriteLine($"SMS Script result: {smsResult.Message}");
                            Debug.WriteLine(smsResult.Success
                                ? $"SMS script executed successfully for {company.InsuranceCompany.Name}"
                                : $"SMS script failed for {company.InsuranceCompany.Name}: {smsResult.Message}");
                        }
                        else
                        {
                            Debug.WriteLine($"SMS script could not be generated for {company.InsuranceCompany.Name}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"SMS script mapping not found for company code: {code}");
                    }
                }
                else
                {
                    Debug.WriteLine($"{company.InsuranceCompany.Name} is OTP company - executing OTP script");

                    // OTP script mapping kontrolü
                    if (OtpScriptMappings.ScriptMap.TryGetValue(code, out var otpScriptInfo))
                    {
                        // Login'den sonra kısa bekleme
                        await Task.Delay(3000);

                        // OTP script'ini oluştur ve çalıştır
                        var otpScript = await CreateOtpScript.GenerateOtpScriptAsync(
                            company.Username,
                            company.Password,
                            otpScriptInfo
                        );

                        if (!string.IsNullOrEmpty(otpScript))
                        {
                            var otpResult = await browser.GetMainFrame().EvaluateScriptAsync(otpScript);

                            Console.WriteLine($"OTP Script result: {otpResult.Message}");
                            Debug.WriteLine(otpResult.Success
                                ? $"OTP script executed successfully for {company.InsuranceCompany.Name}"
                                : $"OTP script failed for {company.InsuranceCompany.Name}: {otpResult.Message}");
                        }
                        else
                        {
                            Debug.WriteLine($"OTP script could not be generated for {company.InsuranceCompany.Name}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"OTP script mapping not found for company code: {code}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing scripts for {company.InsuranceCompany.Name}: {ex.Message}");
                // Hata durumunda tekrar denenebilsin diye kaldır
                executedScripts.Remove(scriptKey);
                scriptExecutionTime.Remove(scriptKey);
            }
        }

        // Alternatif: Daha detaylı logging ile versiyon
        private async void Browser_FrameLoadEnd_WithDetailedLogging(object sender, FrameLoadEndEventArgs e)
        {
            if (!e.Frame.IsMain || !e.Frame.IsValid || e.HttpStatusCode != 200)
                return;

            var browser = (ChromiumWebBrowser)sender;
            string tabKey = browserTabs.FirstOrDefault(x => x.Value == browser).Key;
            if (string.IsNullOrEmpty(tabKey)) return;

            string scriptKey = $"{tabKey}_{e.Frame.Url}";

            if (executedScripts.Contains(scriptKey) &&
                scriptExecutionTime.ContainsKey(scriptKey) &&
                (DateTime.Now - scriptExecutionTime[scriptKey]).TotalSeconds < 5)
            {
                return;
            }

            var company = GetCompanyFromTabKey(tabKey);
            if (company == null) return;

            var code = company.InsuranceCompany.Code;
            var companyName = company.InsuranceCompany.Name;

            // Şirket tipi kontrolü
            bool isSmsCompany = smsInsuranceCompanies.Contains(code, StringComparer.OrdinalIgnoreCase);
            string companyType = isSmsCompany ? "SMS" : "OTP";

            Debug.WriteLine($"=== Processing {companyName} ({code}) - Type: {companyType} ===");

            if (!LoginScriptMappings.ScriptMap.TryGetValue(code, out var loginScriptInfo))
            {
                Debug.WriteLine($"Login script mapping not found for {companyName} ({code})");
                return;
            }

            executedScripts.Add(scriptKey);
            scriptExecutionTime[scriptKey] = DateTime.Now;

            try
            {
                // DOM hazır olmasını bekle
                Debug.WriteLine($"Waiting for DOM ready for {companyName}...");
                await WaitForDomReady(browser);

                // Login script çalıştır
                Debug.WriteLine($"Executing login script for {companyName}...");
                var loginScript = CreateLoginScript.GenerateLoginScript(
                    company.Username,
                    company.Password,
                    loginScriptInfo,
                    loginScriptInfo.Delay
                );

                var loginResult = await browser.GetMainFrame().EvaluateScriptAsync(loginScript);
                Debug.WriteLine($"Login script result for {companyName}: {(loginResult.Success ? "SUCCESS" : "FAILED")} - {loginResult.Message}");

                if (!loginResult.Success)
                {
                    Debug.WriteLine($"Stopping further processing for {companyName} due to login failure");
                    return;
                }

                // Bekleme süresi
                Debug.WriteLine($"Waiting 3 seconds before {companyType} script for {companyName}...");
                await Task.Delay(3000);

                if (isSmsCompany)
                {
                    // SMS Script İşlemi
                    Debug.WriteLine($"Processing SMS script for {companyName}...");

                    if (SmsScriptMappings.ScriptMap.TryGetValue(code, out var smsScriptInfo))
                    {
                        Debug.WriteLine($"SMS script mapping found for {companyName}");

                        var smsScript = await CreateSmsScript.GenerateSmsScriptAsync(
                            company.Username,
                            company.Password,
                            smsScriptInfo
                        );

                        if (!string.IsNullOrEmpty(smsScript))
                        {
                            Debug.WriteLine($"Executing SMS script for {companyName}...");
                            var smsResult = await browser.GetMainFrame().EvaluateScriptAsync(smsScript);

                            Debug.WriteLine($"SMS script result for {companyName}: {(smsResult.Success ? "SUCCESS" : "FAILED")} - {smsResult.Message}");
                        }
                        else
                        {
                            Debug.WriteLine($"SMS script generation failed for {companyName}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"SMS script mapping not found for {companyName} ({code})");
                    }
                }
                else
                {
                    // OTP Script İşlemi
                    Debug.WriteLine($"Processing OTP script for {companyName}...");

                    if (OtpScriptMappings.ScriptMap.TryGetValue(code, out var otpScriptInfo))
                    {
                        Debug.WriteLine($"OTP script mapping found for {companyName}");

                        var otpScript = await CreateOtpScript.GenerateOtpScriptAsync(
                            company.Username,
                            company.Password,
                            otpScriptInfo
                        );

                        if (!string.IsNullOrEmpty(otpScript))
                        {
                            Debug.WriteLine($"Executing OTP script for {companyName}...");
                            var otpResult = await browser.GetMainFrame().EvaluateScriptAsync(otpScript);

                            Debug.WriteLine($"OTP script result for {companyName}: {(otpResult.Success ? "SUCCESS" : "FAILED")} - {otpResult.Message}");
                        }
                        else
                        {
                            Debug.WriteLine($"OTP script generation failed for {companyName}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"OTP script mapping not found for {companyName} ({code})");
                    }
                }

                Debug.WriteLine($"=== Completed processing for {companyName} ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in Browser_FrameLoadEnd for {companyName}: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Hata durumunda tekrar denenebilsin diye kaldır
                executedScripts.Remove(scriptKey);
                scriptExecutionTime.Remove(scriptKey);
            }
        }

        // Gerekli sınıfların var olup olmadığını kontrol eden yardımcı metod
        private bool ValidateScriptMappings()
        {
            try
            {
                // Sınıfların var olup olmadığını kontrol et
                var smsScriptExists = typeof(SmsScriptMappings) != null;
                var createSmsScriptExists = typeof(CreateSmsScript) != null;

                Debug.WriteLine($"SmsScriptMappings exists: {smsScriptExists}");
                Debug.WriteLine($"CreateSmsScript exists: {createSmsScriptExists}");

                return smsScriptExists && createSmsScriptExists;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Script mapping validation error: {ex.Message}");
                return false;
            }
        }

        private async Task WaitForDomReady(ChromiumWebBrowser browser, int timeoutMs = 1000)
        {
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                if (await IsDomReady(browser))
                {
                    // DOM hazır olduğunda ek bir kısa bekleme
                    await Task.Delay(500);
                    return;
                }
                await Task.Delay(100);
            }
        }

        private async Task<bool> IsDomReady(ChromiumWebBrowser browser)
        {
            const string checkDomScript = "document.readyState === 'complete'";
            try
            {
                var result = await browser.GetMainFrame().EvaluateScriptAsync(checkDomScript);
                return result.Success && result.Result is bool ready && ready;
            }
            catch
            {
                return false;
            }
        }

        // Tab kapatıldığında veya yeni sayfaya gidildiğinde temizlik
        public void ClearExecutedScript(string tabKey)
        {
            var keysToRemove = executedScripts.Where(key => key.StartsWith($"{tabKey}_")).ToList();
            foreach (var key in keysToRemove)
            {
                executedScripts.Remove(key);
                scriptExecutionTime.Remove(key);
            }
        }

        // Eski script kayıtlarını temizlemek için (opsiyonel - bellek optimizasyonu)
        public void CleanupOldExecutions(int minutesOld = 10)
        {
            var cutoffTime = DateTime.Now.AddMinutes(-minutesOld);
            var keysToRemove = scriptExecutionTime
                .Where(kvp => kvp.Value < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                executedScripts.Remove(key);
                scriptExecutionTime.Remove(key);
            }
        }

        // Manuel olarak script'i yeniden çalıştırmak için
        public void ForceExecuteScript(string tabKey, string url)
        {
            string scriptKey = $"{tabKey}_{url}";
            executedScripts.Remove(scriptKey);
            scriptExecutionTime.Remove(scriptKey);
        }


        private void Browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            if (e.ErrorCode != CefErrorCode.None && e.Frame.IsMain)
            {
                var browser = (ChromiumWebBrowser)sender;
                var errorUrl = e.FailedUrl;
                
                // HTTPS hatası olup olmadığını kontrol et
                if (errorUrl.StartsWith("https://") && 
                    (e.ErrorCode == CefErrorCode.ConnectionFailed || 
                     e.ErrorCode == CefErrorCode.CertificateTransparencyRequired || 
                     e.ErrorCode == CefErrorCode.InsecureResponse ||
                     (int)e.ErrorCode < -200))
                {
                    string newUrl = errorUrl.Replace("https://", "http://");
                    
                    // URL'yi değiştirip tekrar yükle
                    this.Invoke(new Action(() => {
                        browser.Load(newUrl);
                    }));
                    return;
                }

                // Genel hata sayfası
                this.Invoke(new Action(() => {
                    var htmlContent = $@"
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <style>
                                body {{ font-family: Arial, sans-serif; margin: 50px; text-align: center; }}
                                h2 {{ color: #666; }}
                                p {{ color: #888; }}
                                .container {{ max-width: 600px; margin: 0 auto; }}
                                .error-code {{ background: #f8f8f8; padding: 10px; border-radius: 4px; }}
                                button {{ padding: 10px 20px; background: #0078d7; color: white; 
                                        border: none; border-radius: 4px; cursor: pointer; }}
                                button:hover {{ background: #0063b1; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Sayfa Yüklenemedi</h2>
                                <p>URL: <strong>{e.FailedUrl}</strong></p>
                                <p>Hata: <span class='error-code'>{(int)e.ErrorCode} ({e.ErrorCode})</span></p>
                                <p>{e.ErrorText}</p>
                                <p>Bu sorun genellikle geçici bağlantı hatalarından kaynaklanır.</p>
                                <button onclick='window.location.reload();'>Tekrar Dene</button>
                                <button onclick=""window.location.href = '{e.FailedUrl.Replace("https://", "http://")}'"">HTTP Bağlantı Dene</button>
                            </div>
                        </body>
                        </html>";

                    browser.LoadHtml(htmlContent, e.FailedUrl);
                }));
            }
        }

        private void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            // Geliştirme amaçlı konsol çıktısı
            System.Diagnostics.Debug.WriteLine($"CEF Console [{e.Level}]: {e.Message} ({e.Source}:{e.Line})");
        }

        private void TabControl_CloseButtonClick(object sender, EventArgs e)
        {
            isClosingTab = true;

            try
            {
                var tabPage = tabControl.SelectedTabPage;

                if (tabPage == null || tabPage == tabMainPage)
                    return;

                string tabKey = tabPage.Tag?.ToString();

                // Browser varsa önce onu temizle
                if (!string.IsNullOrEmpty(tabKey) && browserTabs.TryGetValue(tabKey, out var browser))
                {
                    try
                    {
                        // Remove browser from tab page first
                        tabPage.Controls.Remove(browser);

                        // Dispose browser
                        browser.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Tarayıcı dispose edilirken hata: " + ex.Message);
                    }

                    browserTabs.Remove(tabKey);
                }

                // Remove tab page WITHOUT disposing it
                tabControl.TabPages.Remove(tabPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sekme kapatılırken hata: " + ex.Message);
            }
            finally
            {
                isClosingTab = false;
            }
        }
        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var tabPage = tabControl.SelectedTabPage;
            if (tabPage != null && tabPage != tabMainPage)
            {
                var browser = tabPage.Controls[0] as ChromiumWebBrowser;
                if (browser != null)
                {
                    // Önce sayfayı boşalt, sonra yeniden yükle
                    browser.GetBrowser().Reload(true);
                }
            }
        }

        private void btnHome_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tabControl.SelectedTabPage = tabMainPage;
        }

        private void BtnOpenAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kullanıcıya onay sor
            var result = MessageBox.Show(
                $"Tüm sigorta şirketleri ({insuranceCompanies.Count} adet) açılacak. Devam etmek istiyor musunuz?",
                "Tüm Şirketleri Aç",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
           
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Normal kapanmayı işleyen MainForm_FormClosing metodumuz var
            // Burada sadece kaynak temizliği yapalım
            try
            {
                // Resource manager temizliği
                _resourceManager?.Dispose();

                // Tüm tarayıcıları temizle
                foreach (var browser in browserTabs.Values)
                {
                    if (browser != null && !browser.IsDisposed)
                    {
                        try
                        {
                            browser.Dispose();
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.LogError(ex, "Browser dispose edilirken hata");
                            System.Diagnostics.Debug.WriteLine($"Browser dispose edilirken hata: {ex.Message}");
                        }
                    }
                }

                // Dictionary'i temizle
                browserTabs.Clear();

                try
                {
                    if (Cef.IsInitialized == true)
                    {
                        Cef.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, "CEF kapatılırken hata");
                    System.Diagnostics.Debug.WriteLine($"CEF kapatılırken hata: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Form kapatılırken genel hata");
                System.Diagnostics.Debug.WriteLine($"Form kapatılırken genel hata: {ex.Message}");
            }

            base.OnFormClosing(e);
        }
        private void tabMainPage_Paint(object sender, PaintEventArgs e)
        {

        }
        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            // Aktif sekmeyi al
            if (tabControl.SelectedTabPage != null)
            {
                string tabKey = tabControl.SelectedTabPage.Tag?.ToString();

                if (!string.IsNullOrEmpty(tabKey) && browserTabs.TryGetValue(tabKey, out var browser))
                {
                    browser.ShowDevTools();
                }
                else
                {
                    MessageBox.Show("Aktif tarayıcı bulunamadı.");
                }
            }
            else
            {
                MessageBox.Show("Herhangi bir sekme seçili değil.");
            }
        }
      
        private async Task LoginUserAsync()
        {
            var api = new AuthApiService();
            var request = new AuthRequest
            {
                company_code = "tst",
                username = "sandernet",
                password = "ardahan91185"
            };

            try
            {
                var response = await api.LoginAsync(request);
                BaseApiService.Token = response.token;
                //MessageBox.Show("Giriş Başarılı. username: " + response.username);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giriş Hatası: " + ex.Message);
            }
        }

        private void dockPanel1_Click(object sender, EventArgs e)
        {
            dockPanel1.Options.ShowCloseButton = false;
        }

        // MainForm sınıfına eklenecek alanlar
        private Form loaderForm;
        private Label loaderLabel;
        private System.Windows.Forms.ProgressBar loaderProgressBar;

        public object GetBrowsers { get; internal set; }

        private void ShowLoader(string message)
        {
            try
            {
                // Eğer zaten açıksa kapat
                if (loaderForm != null)
                {
                    HideLoader();
                }

                // Yeni loader formu oluştur
                loaderForm = new Form()
                {
                    Text = "İşlem Yapılıyor",
                    Size = new Size(400, 150),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ShowInTaskbar = false,
                    TopMost = true
                };

                // Progress bar ekle
                loaderProgressBar = new System.Windows.Forms.ProgressBar()
                {
                    Style = ProgressBarStyle.Marquee,
                    MarqueeAnimationSpeed = 50,
                    Dock = DockStyle.Bottom,
                    Height = 25
                };

                // Label ekle
                loaderLabel = new Label()
                {
                    Text = message,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    AutoSize = false
                };

                // Kontrolleri forma ekle
                loaderForm.Controls.Add(loaderLabel);
                loaderForm.Controls.Add(loaderProgressBar);

                // Ana formun merkezi olarak konumlandır
                if (this.WindowState == FormWindowState.Normal)
                {
                    loaderForm.StartPosition = FormStartPosition.Manual;
                    loaderForm.Location = new Point(
                        this.Location.X + (this.Width - loaderForm.Width) / 2,
                        this.Location.Y + (this.Height - loaderForm.Height) / 2
                    );
                }

                // Formu göster
                loaderForm.Show(this);
                loaderForm.BringToFront();

                // UI güncellemesi için
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ShowLoader hatası: {ex.Message}");
            }
        }

        // Loader mesajını güncelleyen metod
        private void UpdateLoader(string message)
        {
            try
            {
                if (loaderForm != null && loaderLabel != null && !loaderForm.IsDisposed)
                {
                    loaderLabel.Text = message;
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateLoader hatası: {ex.Message}");
            }
        }

        // Loader'ı gizleyen metod
        private void HideLoader()
        {
            try
            {
                if (loaderForm != null && !loaderForm.IsDisposed)
                {
                    loaderForm.Close();
                    loaderForm.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"HideLoader hatası: {ex.Message}");
            }
            finally
            {
                loaderForm = null;
                loaderLabel = null;
                loaderProgressBar = null;
            }
        }

        private async void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Şirket listesi boşsa çık
            if (insuranceCompanies == null || insuranceCompanies.Count == 0)
            {
                MessageBox.Show("Açılacak şirket bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcıya onay sor
            var result = MessageBox.Show(
                $"Tüm sigorta şirketleri ({insuranceCompanies.Count} adet) açılacak.\n\nBu işlem biraz zaman alabilir.\n\nDevam etmek istiyor musunuz?",
                "Tüm Şirketleri Aç",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Loader'ı göster
            ShowLoader("Şirketler açılıyor...");

            try
            {
                int processedCount = 0;
                int totalCount = insuranceCompanies.Count;

                foreach (var company in insuranceCompanies)
                {
                    try
                    {
                        processedCount++;

                        // Progress güncelle
                        UpdateLoader($"Açılıyor: {company.InsuranceCompany.Name} ({processedCount}/{totalCount})");

                        // UI'ın donmaması için
                        Application.DoEvents();

                        // Şirket sekmesini aç
                        OpenCompanyTab(company);

                        // Her şirket arasında kısa bekleme
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Şirket açılırken hata ({company.InsuranceCompany.Name}): {ex.Message}");
                    }
                }

                // Başarı mesajı
                UpdateLoader($"Tamamlandı! {totalCount} şirket açıldı");
                await Task.Delay(1000); // 1 saniye göster
            }
            catch (Exception ex)
            {
                UpdateLoader($"Hata: {ex.Message}");
                await Task.Delay(2000); // 2 saniye hata mesajını göster
                MessageBox.Show($"Toplu açma işlemi sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogger.LogError(ex, "Toplu şirket açma hatası");
                MessageBox.Show($"Şirketler açılırken hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Loader'ı kapat
                HideLoader();
            }
        }

        private async void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Aktif sekmeyi al
                XtraTabPage currentPage = tabControl.SelectedTabPage;

                // Ana sayfa ise hiçbir şey yapma
                if (currentPage == null || currentPage == tabMainPage)
                {
                    MessageBox.Show("Ana sayfa yeniden açılamaz. Lütfen bir şirket sekmesi seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tab key'i al
                string tabKey = currentPage.Tag?.ToString();
                if (string.IsNullOrEmpty(tabKey))
                {
                    MessageBox.Show("Seçili sekme bilgisi bulunamadı.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Şirket bilgisini al
                var company = GetCompanyFromTabKey(tabKey);
                if (company == null)
                {
                    MessageBox.Show("Şirket bilgisi bulunamadı.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Loader'ı göster
                ShowLoader($"{company.InsuranceCompany.Name} yeniden açılıyor...");

                try
                {
                    // Mevcut sekmeyi kapat
                    if (browserTabs.TryGetValue(tabKey, out var browser))
                    {
                        try
                        {
                            // Browser'ı sayfadan kaldır
                            currentPage.Controls.Remove(browser);

                            // Browser'ı dispose et
                            browser.Dispose();

                            // Dictionary'den kaldır
                            browserTabs.Remove(tabKey);

                            // Script cache'ini temizle
                            ClearExecutedScript(tabKey);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Browser kapatılırken hata: {ex.Message}");
                        }
                    }

                    // Sekmeyi kapat
                    tabControl.TabPages.Remove(currentPage);

                    // Kısa bekleme - UI'ın güncellenmesi için
                    await Task.Delay(500);

                    // Loader mesajını güncelle
                    UpdateLoader($"{company.InsuranceCompany.Name} açılıyor...");

                    // Şirketi yeniden aç
                    OpenCompanyTab(company);

                    // Başarı mesajı
                    UpdateLoader($"{company.InsuranceCompany.Name} başarıyla yenilendi!");
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    UpdateLoader($"Hata: {ex.Message}");
                    await Task.Delay(2000);
                    MessageBox.Show($"Şirket yeniden açılırken hata: {ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Loader'ı kapat
                    HideLoader();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"barButtonItem6_ItemClick genel hata: {ex.Message}");
                MessageBox.Show($"İşlem sırasında hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Aktif sekmeyi al
                XtraTabPage currentPage = tabControl.SelectedTabPage;

                // Ana sayfa ise hiçbir şey yapma
                if (currentPage == null || currentPage == tabMainPage)
                {
                    MessageBox.Show("Ana sayfa yenilenemez. Lütfen bir şirket sekmesi seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tab key'i al
                string tabKey = currentPage.Tag?.ToString();
                if (string.IsNullOrEmpty(tabKey) || !browserTabs.ContainsKey(tabKey))
                {
                    MessageBox.Show("Aktif tarayıcı bulunamadı.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var browser = browserTabs[tabKey];
                if (browser == null || browser.IsDisposed)
                {
                    MessageBox.Show("Tarayıcı mevcut değil.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Şirket bilgisini al (loader için)
                var company = GetCompanyFromTabKey(tabKey);
                string companyName = company?.InsuranceCompany?.Name ?? "Sayfa";

                // Loader'ı göster
                ShowLoader($"{companyName} yenileniyor...");

                try
                {
                    // Script cache'ini temizle
                    ClearExecutedScript(tabKey);

                    // Sayfayı yenile (önbellekten değil)
                    browser.GetBrowser().Reload(true);

                    // Başarı mesajı
                    UpdateLoader($"{companyName} yenilendi!");
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    UpdateLoader($"Hata: {ex.Message}");
                    await Task.Delay(2000);
                    MessageBox.Show($"Sayfa yenilenirken hata: {ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Loader'ı kapat
                    HideLoader();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sayfa yenileme genel hata: {ex.Message}");
                MessageBox.Show($"İşlem sırasında hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Anasayfa sekmesine geç
                if (tabMainPage != null && tabControl.TabPages.Contains(tabMainPage))
                {
                    tabControl.SelectedTabPage = tabMainPage;

                    // Debug için log
                    Debug.WriteLine("Anasayfaya geçiş yapıldı.");
                }
                else
                {
                    // Eğer anasayfa sekme bulunamazsa (teorik olarak olmamalı)
                    MessageBox.Show("Anasayfa bulunamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Anasayfaya geçiş hatası: {ex.Message}");
                MessageBox.Show($"Anasayfaya geçiş sırasında hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Aktif sekmeyi kontrol et
                XtraTabPage currentPage = tabControl.SelectedTabPage;

                // Ana sayfa ise işlem yapma
                if (currentPage == null || currentPage == tabMainPage)
                {
                    MessageBox.Show("QR kod doldurma işlemi için bir şirket sekmesi seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tab key'i al
                string tabKey = currentPage.Tag?.ToString();
                if (string.IsNullOrEmpty(tabKey) || !browserTabs.ContainsKey(tabKey))
                {
                    MessageBox.Show("Aktif tarayıcı bulunamadı.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var browser = browserTabs[tabKey];
                if (browser == null || browser.IsDisposed)
                {
                    MessageBox.Show("Tarayıcı mevcut değil.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Şirket bilgisini al
                var company = GetCompanyFromTabKey(tabKey);
                if (company == null)
                {
                    MessageBox.Show("Şirket bilgisi bulunamadı.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string companyName = company.InsuranceCompany?.Name ?? "Bilinmeyen Şirket";

                // Loader'ı göster
                ShowLoader($"{companyName} için QR kod alınıyor...");

                try
                {
                    // TokenHelper ile token al
                    var tokenHelper = new TokenHelper();

                    UpdateLoader($"{companyName} için token isteniyor...");

                    string token = await tokenHelper.GetTokenAsync(company.Username, company.Password, 3);

                    if (string.IsNullOrEmpty(token))
                    {
                        UpdateLoader("Token alınamadı!");
                        await Task.Delay(2000);
                        MessageBox.Show("QR kod token'ı alınamadı. Kullanıcı adı ve şifre kontrolü yapın.", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    UpdateLoader($"{companyName} için OTP script hazırlanıyor...");

                    // Şirket kodu ile OTP script bilgisini al
                    string code = company.InsuranceCompany.Code;
                    if (!OtpScriptMappings.ScriptMap.TryGetValue(code, out var otpScriptInfo))
                    {
                        UpdateLoader("OTP script bulunamadı!");
                        await Task.Delay(2000);
                        MessageBox.Show($"'{companyName}' şirketi için OTP script tanımı bulunamadı.", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    UpdateLoader($"{companyName} için QR kod doldurma script'i çalıştırılıyor...");

                    // OTP script'ini oluştur
                    var otpScript = await CreateOtpScript.GenerateOtpScriptAsync(
                        company.Username,
                        company.Password,
                        otpScriptInfo
                    );

                    if (string.IsNullOrEmpty(otpScript))
                    {
                        UpdateLoader("OTP script oluşturulamadı!");
                        await Task.Delay(2000);
                        MessageBox.Show("OTP script oluşturulamadı.", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Script'i browser'da çalıştır
                    var result = await browser.GetMainFrame().EvaluateScriptAsync(otpScript);

                    UpdateLoader($"{companyName} QR kod doldurma tamamlandı!");
                  

                    if (result.Success)
                    {
                        Debug.WriteLine($"QR kod script başarıyla çalıştırıldı: {companyName}");
                     
                    }
                    else
                    {
                        Debug.WriteLine($"QR kod script hatası ({companyName}): {result.Message}");
                        MessageBox.Show($"QR kod doldurma script'i çalıştırılamadı:\n{result.Message}", "Script Hatası",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    UpdateLoader($"Hata: {ex.Message}");
                  

                    Debug.WriteLine($"QR kod doldurma genel hatası: {ex.Message}");
                    MessageBox.Show($"QR kod doldurma sırasında hata:\n{ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Loader'ı kapat
                    HideLoader();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"barButtonItem5_ItemClick genel hata: {ex.Message}");
                MessageBox.Show($"İşlem sırasında beklenmeyen hata:\n{ex.Message}", "Kritik Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Loader'ı güvenli şekilde kapat
                try { HideLoader(); } 
                catch 
                {
                    ErrorLogger.LogError(ex, "Beklenmeyen hata");
                    ErrorLogger.LogError(ex, "Toplu şirket açma hatası");
                    MessageBox.Show($"Şirketler açılırken hata: {ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Loader'ı kapat
                    HideLoader();
                }
            }
        }
        // Müşteri bilgilerini alma
        private KullaniciBilgileri GetMusteriBilgileri()
        {
            // Önce aktif formdan almaya çalış
            if (_hizliTeklifForm != null && !_hizliTeklifForm.IsDisposed && _hizliTeklifForm.Visible)
            {
                try
                {
                    var bilgiler = _hizliTeklifForm.GetKullaniciBilgileri();
                    Console.WriteLine("✅ Müşteri bilgileri aktif formdan alındı.");
                    return bilgiler;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Aktif formdan bilgi alınamadı: {ex.Message}");
                }
            }

            // Aksi halde saklanmış bilgileri kullan
            if (_aktifMusteriBilgileri != null)
            {
                Console.WriteLine("✅ Müşteri bilgileri cache'den alındı.");
                return _aktifMusteriBilgileri;
            }

            Console.WriteLine("❌ Müşteri bilgileri hiçbir yerden bulunamadı!");
            return null;
        }

        // Aktif şirket bilgilerini döndüren public metodlar
        public string GetAktifSirketAdi()
        {
            try
            {
                if (tabControl?.SelectedTabPage == null)
                    return null;

                if (tabControl.SelectedTabPage == tabMainPage)
                    return null;

                string tabKey = tabControl.SelectedTabPage.Tag?.ToString();
                if (string.IsNullOrEmpty(tabKey))
                    return null;

                return tabKey.StartsWith("tab_") ? tabKey.Substring(4) : tabControl.SelectedTabPage.Text;
            }
            catch
            {
                return null;
            }
        }

        public ChromiumWebBrowser GetAktifBrowser()
        {
            try
            {
                string aktifSirket = GetAktifSirketAdi();
                if (string.IsNullOrEmpty(aktifSirket))
                    return null;

                string tabKey = $"tab_{aktifSirket}";
                return browserTabs.ContainsKey(tabKey) ? browserTabs[tabKey] : null;
            }
            catch
            {
                return null;
            }
        }

        public bool IsAktifSekmeGecerli()
        {
            try
            {
                return tabControl?.SelectedTabPage != null && 
                       tabControl.SelectedTabPage != tabMainPage &&
                       !string.IsNullOrEmpty(GetAktifSirketAdi());
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetAcikSirketler()
        {
            try
            {
                List<string> acikSirketler = new List<string>();
                
                if (browserTabs == null || browserTabs.Count == 0)
                    return acikSirketler;

                foreach (var kvp in browserTabs)
                {
                    string tabKey = kvp.Key;
                    if (tabKey.StartsWith("tab_"))
                    {
                        string sirketAdi = tabKey.Substring(4);
                        acikSirketler.Add(sirketAdi);
                    }
                }

                return acikSirketler;
            }
            catch
            {
                return new List<string>();
            }
        }

        // Şirket-specific browser alma metodu - scriptlerin karışmasını önler
        public ChromiumWebBrowser GetSirketBrowser(string sirketAdi)
        {
            try
            {
                if (string.IsNullOrEmpty(sirketAdi))
                    return null;

                string tabKey = $"tab_{sirketAdi}";
                
                if (browserTabs.ContainsKey(tabKey))
                {
                    var browser = browserTabs[tabKey];
                    if (browser != null && !browser.IsDisposed)
                    {
                        Console.WriteLine($"✅ {sirketAdi} için browser bulundu - TabKey: {tabKey}");
                        return browser;
                    }
                    else
                    {
                        Console.WriteLine($"❌ {sirketAdi} browser dispose edilmiş - TabKey: {tabKey}");
                        // Dispose edilmiş browser'ı dictionary'den kaldır
                        browserTabs.Remove(tabKey);
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine($"❌ {sirketAdi} için browser bulunamadı. Mevcut browser'lar:");
                    foreach (var kvp in browserTabs)
                    {
                        Console.WriteLine($"   • {kvp.Key}");
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetSirketBrowser hatası: {ex.Message}");
                return null;
            }
        }

        // ✅ DOĞRU form açma metodu - Mevcut kodunuzu değiştirin
        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (_hizliTeklifForm == null || _hizliTeklifForm.IsDisposed)
                {
                    Console.WriteLine("✅ Yeni hızlı teklif formu oluşturuluyor...");
                    _hizliTeklifForm = new hizliTeklifFormNew();

                    // Event handler'ları ekle
                    _hizliTeklifForm.FormClosed += HizliTeklifForm_FormClosed;
                    _hizliTeklifForm.FormClosing += HizliTeklifForm_FormClosing;

                    // Form özelliklerini ayarla
                    _hizliTeklifForm.StartPosition = FormStartPosition.CenterScreen;
                    _hizliTeklifForm.ShowInTaskbar = true;
                    _hizliTeklifForm.Text = "Hızlı Teklif - SigortaVip";
                }
                else
                {
                    Console.WriteLine("✅ Mevcut hızlı teklif formu kullanılıyor...");
                }

                _hizliTeklifForm.Show(this); // MODELESS - Ana formu bloklamaz
                _hizliTeklifForm.BringToFront();
                _hizliTeklifForm.Focus();

                Console.WriteLine($"✅ Form durumu - Disposed: {_hizliTeklifForm.IsDisposed}, Visible: {_hizliTeklifForm.Visible}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Form açılırken hata: {ex.Message}");
                MessageBox.Show($"Hızlı teklif formu açılırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Form kapanmadan önce bilgileri al
        private void HizliTeklifForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_hizliTeklifForm != null && !_hizliTeklifForm.IsDisposed)
                {
                    Console.WriteLine("✅ Form kapanıyor, müşteri bilgileri alınıyor...");
                    _aktifMusteriBilgileri = _hizliTeklifForm.GetKullaniciBilgileri();

                    if (_aktifMusteriBilgileri != null)
                    {
                        Console.WriteLine($"✅ Müşteri bilgileri kaydedildi: {_aktifMusteriBilgileri.AdSoyad}");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Müşteri bilgileri null geldi");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Form kapanırken hata: {ex.Message}");
            }
        }
        public async Task<bool> AktifSirketFiyatSorgulamaYap(KullaniciBilgileri musteriBilgileri = null)
        {
            try
            {
                // Müşteri bilgilerini kontrol et
                if (musteriBilgileri == null)
                {
                    musteriBilgileri = GetMusteriBilgileri();
                }

                if (musteriBilgileri == null || string.IsNullOrEmpty(musteriBilgileri.txtKimlikNo))
                {
                    MessageBox.Show("Müşteri bilgileri bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Aktif sekmeyi kontrol et
                if (tabControl.SelectedTabPage == null)
                {
                    MessageBox.Show("Lütfen bir şirket sekmesi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Ana sayfa kontrolü
                if (tabControl.SelectedTabPage == tabMainPage)
                {
                    MessageBox.Show("Ana sayfa için fiyat sorgulama yapılamaz. Lütfen bir şirket sekmesi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                // Aktif şirket adını al
                string tabKey = tabControl.SelectedTabPage.Tag?.ToString();
                if (string.IsNullOrEmpty(tabKey))
                {
                    MessageBox.Show("Aktif sekme bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string aktifSirket = tabKey.StartsWith("tab_") ? tabKey.Substring(4) : tabControl.SelectedTabPage.Text;

                Console.WriteLine($"✅ Aktif şirket: {aktifSirket}");
                Console.WriteLine($"✅ Müşteri: {musteriBilgileri.AdSoyad}");

                // Fiyat sorgulama işlemini başlat (mevcut simpleButton3_Click metodunu çağır)
                simpleButton3_Click(null, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aktif şirket fiyat sorgulama hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public async Task<bool> FiyatSorgulamaYap(string sirketAdi, KullaniciBilgileri musteriBilgileri = null)
        {
            try
            {
                // Müşteri bilgilerini al (parametre olarak gelmediyse)
                if (musteriBilgileri == null)
                {
                    musteriBilgileri = GetMusteriBilgileri();
                }

                if (musteriBilgileri == null || string.IsNullOrEmpty(musteriBilgileri.txtKimlikNo))
                {
                    MessageBox.Show("Müşteri bilgileri bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Şirketi bul
                var company = insuranceCompanies?.FirstOrDefault(x =>
                    string.Equals(x.InsuranceCompany.Name, sirketAdi, StringComparison.OrdinalIgnoreCase));

                if (company == null)
                {
                    MessageBox.Show($"'{sirketAdi}' şirketi bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Şirket sekmesinin açık olup olmadığını kontrol et
                string tabKey = $"tab_{sirketAdi}";
                if (!browserTabs.ContainsKey(tabKey))
                {
                    // Şirketi aç
                    OpenCompanyTab(company);

                    // Açılması için bekle
                    await Task.Delay(3000);

                    if (!browserTabs.ContainsKey(tabKey))
                    {
                        MessageBox.Show($"'{sirketAdi}' şirketi açılamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // Şirket sekmesini aktif yap
                foreach (XtraTabPage page in tabControl.TabPages)
                {
                    if (page.Tag?.ToString() == tabKey)
                    {
                        tabControl.SelectedTabPage = page;
                        break;
                    }
                }

                // Fiyat sorgulama işlemini başlat
                await Task.Delay(500); // UI güncellemesi için
                simpleButton3_Click(null, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fiyat sorgulama hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ✅ Form tamamen kapandıktan sonra temizlik
        private void HizliTeklifForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Console.WriteLine("✅ Form tamamen kapandı, referans temizleniyor...");
            _hizliTeklifForm = null;
        }

        private async void simpleButton2_Click(object sender, EventArgs e)
        {
            // Null kontrol
            if (tabControl.SelectedTabPage == null)
            {
                MessageBox.Show("Lütfen bir sekme seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ana sayfa kontrolü (tabMainPage kapatılamaz)
            if (tabControl.SelectedTabPage == tabMainPage)
            {
                MessageBox.Show("Ana sayfa için cookie işlemi yapılamaz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Filtrelenmiş tab kontrolü - Explorer ve Duyuru sekmeleri
            string selectedTabText = tabControl.SelectedTabPage.Text;
            string[] excludedTabs = { "_Explorer", "Duyuru" };

            if (excludedTabs.Any(tab => selectedTabText.Contains(tab)))
            {
                MessageBox.Show("Bu sekme için cookie işlemi yapılamaz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Seçili tab'ın tag'inden şirket bilgisini al
                string tabKey = tabControl.SelectedTabPage.Tag?.ToString();
                if (string.IsNullOrEmpty(tabKey))
                {
                    MessageBox.Show("Sekme bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tab key'den şirket adını çıkar (tab_ prefix'ini kaldır)
                string companyName = tabKey.StartsWith("tab_") ? tabKey.Substring(4) : selectedTabText;

                // InsuranceCompanyItem'ı bul
                var company = insuranceCompanies?.FirstOrDefault(x =>
                    string.Equals(x.InsuranceCompany.Name, companyName, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine(company.Id);
                Console.WriteLine(company.Cookie);
                if (company == null)
                {
                    MessageBox.Show($"'{companyName}' şirketi için bilgi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Seçili tab'dan browser'ı al
                //if (!browserTabs.ContainsKey(tabKey))
                //{
                //    MessageBox.Show("Bu sekme için browser bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                var browser = browserTabs[tabKey];

                // WebPage nesnesi oluştur veya bul
                var webPage = Browser.webPageList?.FirstOrDefault(x => x.insuranceCompany == companyName);

                if (webPage == null)
                {
                    // WebPage bulunamazsa yeni oluştur
                    webPage = new WebPage
                    {
                        browser = browser,
                        insuranceCompany = companyName,
                        insuranceCompanyId = company.InsuranceCompany.Id, // Eğer Id property'si varsa
                        initialCookies = new List<Cookie>() // Boş liste ile başlat
                    };

                    // Browser.webPageList'e ekle (eğer null değilse)
                    if (Browser.webPageList == null)
                    {
                        Browser.webPageList = new List<WebPage>();
                    }
                    Browser.webPageList.Add(webPage);
                }

                // Cookie yenileme işlemi
                ActivePageReloaderJob activePageReloaderJob = new ActivePageReloaderJob();
                await activePageReloaderJob.RefreshCookies(webPage, company.Id);

                MessageBox.Show($"'{companyName}' için cookie ekleme işlemi başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show($"Gerekli parametre bulunamadı.\nDetay: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"İşlem gerçekleştirilemedi.\nDetay: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cookie eklenemedi.\nHata Detayı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void simpleButton3_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTabPage == null)
            {
                MessageBox.Show("Lütfen bir sekme seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ana sayfa kontrolü
            if (tabControl.SelectedTabPage == tabMainPage)
            {
                MessageBox.Show("Ana sayfa için fiyat sorgulama yapılamaz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Filtrelenmiş tab kontrolü
            string selectedTabText = tabControl.SelectedTabPage.Text;
            string[] excludedTabs = { "_Explorer", "Duyuru" };

            if (excludedTabs.Any(tab => selectedTabText.Contains(tab)))
            {
                MessageBox.Show("Bu sekme için fiyat sorgulama yapılamaz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Seçili tab'ın tag'inden şirket bilgisini al
                string tabKey = tabControl.SelectedTabPage.Tag?.ToString();
                if (string.IsNullOrEmpty(tabKey))
                {
                    MessageBox.Show("Sekme bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tab key'den şirket adını çıkar
                string companyName = tabKey.StartsWith("tab_") ? tabKey.Substring(4) : selectedTabText;

                // Şirket bilgisini insurance companies'den al
                var company = insuranceCompanies?.FirstOrDefault(x =>
                    string.Equals(x.InsuranceCompany.Name, companyName, StringComparison.OrdinalIgnoreCase));

                if (company == null)
                {
                    MessageBox.Show($"'{companyName}' şirketi için bilgi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Fiyat sorgulama sınıfını oluştur
                var fiyatSorgu = TrafikFiyatSorgulamaFactory.CreateFiyatSorgu(
                    company.InsuranceCompany.Code, companyName);

                if (fiyatSorgu == null)
                {
                    var supportedCompanies = TrafikFiyatSorgulamaFactory.GetSupportedCompanies();
                    var supportedList = string.Join("\n• ", supportedCompanies);

                    MessageBox.Show($"'{companyName}' şirketi için fiyat sorgulama henüz desteklenmiyor.\n\n" +
                                  $"Desteklenen şirketler:\n• {supportedList}",
                                  "Desteklenmeyen Şirket", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Seçili tab'dan browser'ı al
                if (!browserTabs.ContainsKey(tabKey))
                {
                    MessageBox.Show("Bu sekme için browser bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var browser = browserTabs[tabKey];
                var musteriBilgileri = GetMusteriBilgileri();
                string musteriOzeti = "";
                if (musteriBilgileri != null)
                {
                    musteriOzeti = $"\n\nMüşteri: {musteriBilgileri.AdSoyad}" +
                                  $"\nPlaka: {musteriBilgileri.txtPlakaNo}" +
                                  $"\nTC: {musteriBilgileri.txtKimlikNo}";
                }

                MessageBox.Show($"Bu sekme için browser bilgisi bulunamadı.{musteriOzeti}",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Test verileri
                //var kullaniciBilgileri = new KullaniciBilgileri
                //{
                //    txtKimlikNo = "47371914826",
                //    txtDogumTar = "25/02/1972",
                //    txtPlakaNo = "34CPM586",
                //    txtSeriNo = "HJ595778",
                //    txtAracKodu = "0532250",
                //    txtModel = "2020",
                //    txtTel ="5523964131",
                //    txtkullanımtarzı = "OTOMOBİL",
                //    txtMarka = "FORD",
                //    txtModelAdi = "",
                //    txtYerAdedi = "4",
                //};
                var kullaniciBilgileri = musteriBilgileri;
                // Butonu devre dışı bırak
                //simpleButton3.Enabled = false;
                //simpleButton3.Text = "Sorgulanıyor...";

                // Progress göster
                ShowLoader($"{companyName} fiyat sorgulanıyor...");

                try
                {
                    UpdateLoader($"{companyName} fiyat hesaplaması başlatılıyor...");

                    // Dinamik olarak uygun fiyat sorgulama metodunu çağır
                    var sonuc = await fiyatSorgu.TrafikSorgula(kullaniciBilgileri, browser);

                    UpdateLoader("Fiyat sorgulama tamamlandı!");
                    await Task.Delay(1000);

                    // Sonucu göster
                    if (sonuc.Durum == "Tamamlandı" || sonuc.Durum == "Başarılı")
                    {
                        var message = $"{companyName} Fiyat Sorgulama Başarılı!\n\n" +
                                      $"Brüt Prim: {sonuc.BrutPrim}\n" +
                                      $"Komisyon: {sonuc.Komisyon}\n" +
                                      $"Teklif No: {sonuc.TeklifNo}\n" +
                                      $"Firma: {sonuc.FirmaAdi}";

                        MessageBox.Show(message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Debug çıktısı
                        Console.WriteLine($"=== {companyName.ToUpper()} SONUÇ ===");
                        Console.WriteLine($"Brüt Prim: {sonuc.BrutPrim}");
                        Console.WriteLine($"Komisyon: {sonuc.Komisyon}");
                        Console.WriteLine($"Teklif No: {sonuc.TeklifNo}");
                        Console.WriteLine($"Durum: {sonuc.Durum}");
                        Console.WriteLine($"========================");
                    }
                    else
                    {
                        MessageBox.Show($"{companyName} Fiyat Sorgulama Başarısız!\n\nHata: {sonuc.Durum}",
                                      "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        Console.WriteLine($"{companyName} Hata: {sonuc.Durum}");
                    }
                }
                catch (Exception ex)
                {
                    UpdateLoader($"Hata: {ex.Message}");
                    await Task.Delay(2000);

                    MessageBox.Show($"{companyName} fiyat sorgulama hatası:\n{ex.Message}",
                                  "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Console.WriteLine($"{companyName} Exception: {ex.Message}");
                }
                finally
                {
                    HideLoader();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Fiyat sorgulama hatası");
                MessageBox.Show($"Fiyat sorgulama işlemi başarısız.\nHata Detayı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"simpleButton3_Click Genel Hata: {ex.Message}"); ;
            }
            finally
            {
                // Butonu tekrar etkinleştir
                simpleButton3.Enabled = true;
                simpleButton3.Text = "Fiyat Sorgula";

                // Güvenli şekilde loader'ı kapat
                try { HideLoader(); } 
               catch 
                {
                    ErrorLogger.LogError("Beklenmeyen hata oluştu");
                }
            }
        }

    }
  
    // Basit ilerleme formu
    public class ProgressBarForm : Form
    {
        private Label lblCaption;
        private Label lblDescription;
        private System.Windows.Forms.ProgressBar progressBar;

        public string Caption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = value; }
        }

        public string Description
        {
            get { return lblDescription.Text; }
            set { lblDescription.Text = value; }
        }

        public ProgressBarForm()
        {
            this.Size = new Size(400, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "İşlem";

            lblCaption = new Label();
            lblCaption.AutoSize = false;
            lblCaption.TextAlign = ContentAlignment.MiddleCenter;
            lblCaption.Dock = DockStyle.Top;
            lblCaption.Height = 30;
            lblCaption.Font = new Font(lblCaption.Font.FontFamily, 12, FontStyle.Bold);
            this.Controls.Add(lblCaption);

            lblDescription = new Label();
            lblDescription.AutoSize = false;
            lblDescription.TextAlign = ContentAlignment.MiddleCenter;
            lblDescription.Dock = DockStyle.Top;
            lblDescription.Height = 30;
            lblDescription.Top = lblCaption.Bottom;
            this.Controls.Add(lblDescription);

            progressBar = new System.Windows.Forms.ProgressBar();
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Dock = DockStyle.Top;
            progressBar.Height = 30;
            progressBar.Top = lblDescription.Bottom + 10;
            progressBar.MarqueeAnimationSpeed = 30;
            this.Controls.Add(progressBar);
        }
    }

    // Şirket arama formu
    public class CompanySearchForm : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraEditors.SearchControl txtSearch;
        private DevExpress.XtraGrid.GridControl gridCompanies;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private List<InsuranceCompanyInfo> allCompanies;
        private List<InsuranceCompanyInfo> filteredCompanies;

        public InsuranceCompanyInfo SelectedCompany { get; private set; }

        public CompanySearchForm(List<InsuranceCompanyInfo> companies)
        {
            this.allCompanies = companies;
            this.filteredCompanies = new List<InsuranceCompanyInfo>(companies);
            InitializeComponents();
            PopulateCompanyList();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(600, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Şirket Ara (CTRL+K)";
            this.KeyPreview = true;
            this.KeyDown += (s, e) => 
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter && gridView.SelectedRowsCount > 0)
                {
                    SelectCompanyAndClose();
                }
            };

            // Panel oluştur
            var panelSearch = new DevExpress.XtraEditors.PanelControl();
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Height = 50;
            panelSearch.Padding = new Padding(10, 10, 10, 10);
            this.Controls.Add(panelSearch);

            // Arama kontrolü
            txtSearch = new DevExpress.XtraEditors.SearchControl();
            txtSearch.Dock = DockStyle.Fill;
            txtSearch.Properties.NullValuePrompt = "Şirket adı yazın...";
            txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            txtSearch.Properties.ShowSearchButton = true;
            txtSearch.Properties.ShowClearButton = true;
            txtSearch.Properties.Buttons[0].Caption = "Ara";
            txtSearch.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
            txtSearch.Properties.Client = gridCompanies;
            txtSearch.Properties.EditValueChanging += (s, e) => FilterCompanies(e.NewValue?.ToString());
            panelSearch.Controls.Add(txtSearch);

            // Grid kontrolü
            gridCompanies = new DevExpress.XtraGrid.GridControl();
            gridCompanies.Dock = DockStyle.Fill;
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridCompanies.MainView = gridView;
            
            // Grid görünümünü yapılandır
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsCustomization.AllowColumnMoving = false;
            gridView.OptionsCustomization.AllowFilter = false;
            gridView.OptionsCustomization.AllowGroup = false;
            gridView.OptionsCustomization.AllowSort = true;
            gridView.OptionsDetail.EnableMasterViewMode = false;
            gridView.OptionsFind.AlwaysVisible = false;
            gridView.OptionsMenu.EnableColumnMenu = false;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsView.ShowIndicator = false;
            gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView.DoubleClick += (s, e) => SelectCompanyAndClose();
            
            // Sütunları ekle
            var colName = gridView.Columns.Add();
            colName.Caption = "Şirket Adı";
            colName.FieldName = "CompanyName";
            colName.Visible = true;
            colName.Width = 300;
            
            var colLastAccess = gridView.Columns.Add();
            colLastAccess.Caption = "Son Erişim";
            colLastAccess.FieldName = "LastAccessTime";
            colLastAccess.Visible = true;
            colLastAccess.Width = 180;
            colLastAccess.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            colLastAccess.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            
            var colDescription = gridView.Columns.Add();
            colDescription.Caption = "Açıklama";
            colDescription.FieldName = "Description";
            colDescription.Visible = true;
            colDescription.Width = 300;
            
            this.Controls.Add(gridCompanies);

            // Butonlar için panel
            var buttonPanel = new DevExpress.XtraEditors.PanelControl();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);
            this.Controls.Add(buttonPanel);

            // İptal butonu
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnCancel.Text = "İptal";
            btnCancel.Width = 100;
            btnCancel.Height = 35;
            btnCancel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            btnCancel.Location = new Point(buttonPanel.Width - 230, 12);
            btnCancel.Click += (s, e) => 
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            buttonPanel.Controls.Add(btnCancel);

            // Tamam butonu
            btnOK = new DevExpress.XtraEditors.SimpleButton();
            btnOK.Text = "Seç";
            btnOK.Width = 100;
            btnOK.Height = 35;
            btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            btnOK.Appearance.ForeColor = System.Drawing.Color.White;
            btnOK.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            btnOK.Location = new Point(buttonPanel.Width - 120, 12);
            btnOK.Click += (s, e) => SelectCompanyAndClose();
            buttonPanel.Controls.Add(btnOK);

            // Kontrollerin sıralaması
            this.Controls.SetChildIndex(buttonPanel, 0);
            this.Controls.SetChildIndex(gridCompanies, 1);
            this.Controls.SetChildIndex(panelSearch, 2);

            // Başlangıçta arama kutusuna odaklan
            this.Load += (s, e) => txtSearch.Focus();
        }

        private void FilterCompanies(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredCompanies = new List<InsuranceCompanyInfo>(allCompanies);
            }
            else
            {
                searchText = searchText.ToLower();
                filteredCompanies = allCompanies
                    .Where(c => c.CompanyName.ToLower().Contains(searchText) || 
                               (c.Description != null && c.Description.ToLower().Contains(searchText)))
                    .ToList();
            }

            PopulateCompanyList();
        }

        private void PopulateCompanyList()
        {
            // Veri kaynağı olarak filtrelenmiş şirketleri kullan
            gridCompanies.DataSource = null;
            gridCompanies.DataSource = filteredCompanies;

            if (gridView.RowCount > 0)
            {
                gridView.FocusedRowHandle = 0;
            }
        }

        private void SelectCompanyAndClose()
        {
            if (gridView.SelectedRowsCount > 0)
            {
                int rowHandle = gridView.GetSelectedRows()[0];
                if (rowHandle >= 0)
                {
                    SelectedCompany = gridView.GetRow(rowHandle) as InsuranceCompanyInfo;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }

    // Fiyat sonuçlarını göstermek için yeni bir form
   
}

public struct INTERNET_PROXY_INFO
{
    public int dwAccessType;
    public IntPtr proxy;
    public IntPtr proxyBypass;
}

public interface IAuthenticate
{
    [return: MarshalAs(UnmanagedType.I4)]
    [PreserveSig]
    int Authenticate(ref IntPtr phwnd, ref IntPtr pszUsername, ref IntPtr pszPassword);
}

public interface IServiceProvider
{
    [return: MarshalAs(UnmanagedType.I4)]
    [PreserveSig]
    int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
}