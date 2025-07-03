using CefSharp;
using DevExpress.XtraNavBar;
using Quartz;
using SigortaVip.Business;
using SigortaVip.Constant;
using SigortaVip.Dto;
using SigortaVipNew;
using SigortaVipNew.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigortaVip.Utility.ScheduledJobs
{
    class ActivePageReloaderJob : IJob
    {
        // Execute metodunu async Task olarak değiştir
        public async Task Execute(IJobExecutionContext context)
        {
            if (Browser.webPageList.Count != 0)
            {
                // Tüm sayfaları paralel olarak işle
                //var tasks = Browser.webPageList.Select(page => RefreshCookies(page));
                //await Task.WhenAll(tasks);
            }
        }

        // RefreshCookies metodunu async Task olarak değiştir
        public async Task RefreshCookies(WebPage page,int insuranceCompanyId)
        {
            try
            {
                if (page.browser.Address == "https://sigorta.neova.com.tr:5443/IntegrationLogin.aspx?ReturnUrl=%2f%3fhttps%3a%2f%2fsigorta.neova.com.tr%3a5443%2fUIFrameSet.aspx&https://sigorta.neova.com.tr:5443/UIFrameSet.aspx")
                {
                    page.browser.Load("https://sigorta.neova.com.tr:5443/UIFrameSet.aspx");
                }

                if (checkUrlIsLoginPage(page.browser.Address, page))
                {
                    return;
                }

                CookieVisitor _cookieVisitor = new CookieVisitor();

                List<Cookie> newCookies = await page.browser.GetCookieManager().VisitAllCookiesAsync();
                List<Cookie> legacyCookies = page.initialCookies;
                List<CookieDto> newCookiesDtoList = new List<CookieDto>();

                foreach (var newCookie in newCookies)
                {
                    try
                    {
                        if (newCookie != null &&
                            !string.IsNullOrEmpty(newCookie.Domain) &&
                            !newCookie.Domain.Contains("google.com") &&
                            !newCookie.Domain.Contains("facebook.com") &&
                            !newCookie.Domain.Contains(".doubleclick.net") &&
                            !newCookie.Domain.Contains("neoport") &&
                            !newCookie.Domain.Contains("youtube") &&
                            !newCookie.Domain.Contains("af.sts.mapfre."))
                        {
                            var checkCookie = legacyCookies.FirstOrDefault(x => x.Name == newCookie.Name && x.Value == newCookie.Value);
                            if (true /*checkCookie == null || page.insuranceCompany == InsuranceConstants.RaySigorta*/)
                            {
                                newCookiesDtoList.Add(new CookieDto
                                {
                                    Creation = newCookie.Creation,
                                    Value = newCookie.Value,
                                    Domain = newCookie.Domain,
                                    Expires = newCookie.Expires,
                                    HttpOnly = newCookie.HttpOnly,
                                    Id = 0,
                                    InsuranceCompanyId = (int)page.insuranceCompanyId,
                                    LastAccess = newCookie.LastAccess,
                                    Name = newCookie.Name,
                                    Path = newCookie.Path,
                                    Priority = (int)newCookie.Priority,
                                    SameSite = (int)newCookie.SameSite,
                                    Secure = newCookie.Secure
                                });
                                Console.WriteLine(newCookiesDtoList);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Cookie işleme hatası: {ex.Message}");
                    }
                }

                if (page.insuranceCompany == InsuranceConstants.NeovaSigorta && newCookiesDtoList.Count < 3)
                {
                    return;
                }

                if (newCookiesDtoList.Count > 0)
                {
                    try
                    {
                        // Modern HttpClient tabanlı servis kullanımı
                        var cookieService = CookieApiService.GetInstance();

                        // Token'ı güncelle (MainForm._token'dan alıyoruz)
                        // TODO: MainForm._token null check'i ekle
                        if (!string.IsNullOrEmpty(MainForm._token))
                        {
                            cookieService.UpdateToken(MainForm._token);
                        }

                        // Asenkron cookie ekleme
                        var response = await cookieService.AddCookiesAsync(newCookiesDtoList, insuranceCompanyId);

                        // Başarılı yanıt kontrolü (artık string response geliyor)
                        if (!string.IsNullOrEmpty(response))
                        {
                            //bug !! to set only specific domain
                            page.setinitialCookies(newCookies);
                            //page.setinitialCookies(newCookies.Where(x => x.Domain == newCookiesDtoList.FirstOrDefault().Domain).ToList());

                            Console.WriteLine($"Cookies başarıyla eklendi: {newCookiesDtoList.Count} adet - {page.insuranceCompany}");
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Cookie ekleme hatası - {page.insuranceCompany}: {ex.Message}");
                        // Hata durumunda eski cookie'leri koruyabilirsiniz
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RefreshCookies genel hatası - {page?.insuranceCompany}: {ex.Message}");
            }
        }

        private string getDomain(string fullUrl)
        {
            try
            {
                int index = fullUrl.IndexOf(".com");
                if (index == -1) return string.Empty;

                fullUrl = fullUrl.Substring(0, index);

                int index2 = fullUrl.LastIndexOf(".");
                if (index2 != -1)
                {
                    return fullUrl.Substring(index2 + 1);
                }
                else
                {
                    index2 = fullUrl.LastIndexOf("/");
                    return fullUrl.Substring(index2 + 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"getDomain hatası: {ex.Message}");
                return string.Empty;
            }
        }

        private bool checkUrlIsLoginPage(string url, WebPage page)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return false;

                if (
                    (
                           url.Contains("https://sat2.aksigorta.com.tr/auth/login")
                        || url.Contains("https://cas.allianz.com.tr/cas/login")
                        || url.Contains("https://giris.anadolusigorta.com.tr/singleSignOnApi/login")
                        || url.Contains("https://nareks.bereket.com.tr/Login")
                        || url.Contains("https://sigorta.corpussigorta.com.tr/Login")
                        || url.Contains("https://adaauth.dogasigorta.com/Account/Login")
                        || url.Contains("https://web.hdisigorta.com.tr/usergiris.php")
                        || url.Contains("https://auth.korusigortaportal.com/Account/Login")
                        || url.Contains("https://auth.grisigorta.com.tr/Account/Login")
                        || url.Contains("https://portal.magdeburger.com.tr/login")
                        || url.Contains("https://www.babonline.com/Mapfre/Login")
                        || url.Contains("https://sigorta.neova.com.tr:5443/IntegrationLogin")
                        || url.Contains("https://tmtsigorta.vizyoneks.com.tr/Login")
                        || url.Contains("https://galaksi.turknippon.com/acente-giris")
                        || url.Contains("https://pusula.turkiyesigorta.com.tr/login.seam")
                        || url.Contains("https://www.unicosigorta.com.tr/online-islemler/acente/unikolay-giris")
                        || url.Contains("https://neoport.neova.com.tr/_layouts/15/Neova.Authentication/CustomLogin")
                        || url.Contains("https://rayexpress.raysigorta.com.tr/Login")
                        || url.Contains("https://af.sts.mapfre.net/adfs/ls")
                        || url == ("https://www.unicosigorta.com.tr")
                        || url == ("https://www.unicosigorta.com.tr/online-islemler")
                        || url == ("https://www.unicosigorta.com.tr/kurumsal")
                        || url == ("https://www.unicosigorta.com.tr/bireysel")
                        || url == ("https://www.unicosigorta.com.tr/authentication")
                        || url == ("https://online.ankarasigorta.com.tr/")
                        || url == ("https://online.ankarasigorta.com.tr")
                        || url == ("https://www.quicksigorta.com/")
                        || url == ("https://www.quicksigorta.com")
                    )
                        && !url.Contains("https://www.unicosigorta.com.tr/online-islemler/acente")
                )
                {
                    return true;
                }

                if (MainForm.activeTabPage != page?.insuranceCompany && !string.IsNullOrEmpty(MainForm.activeTabPage))
                {
                    try
                    {
                        page?.browser?.Reload();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Browser reload hatası: {ex.Message}");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"checkUrlIsLoginPage hatası: {ex.Message}");
                return false;
            }
        }
    }
}