using CefSharp;
using CefSharp.WinForms;
using SigortaVip.Constant;
using SigortaVip.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using InsuranceCompany = SigortaVip.Models.InsuranceCompany;

namespace SigortaVip.Business
{
    class BrowserManager
    {
        private static CefSettings settings = null;

        private static readonly Lazy<BrowserManager> _lazyInstance = new Lazy<BrowserManager>(() =>
        {
            var settings = new CefSettings
            {
                PersistSessionCookies = true,
                CefCommandLineArgs = { ["disable-features"] = "SameSiteByDefaultCookies" }
            };

            //if(!Cef.IsInitialized)
            //    Cef.Initialize(settings);

            return new BrowserManager();
        });
        public static BrowserManager Instance => _lazyInstance.Value;

        private static BrowserManager browserManager = null;

        public static BrowserManager getInstance()
        {
            if (browserManager == null)
                browserManager = new BrowserManager();

            return browserManager;
        }
        private BrowserManager()
        {
            if (settings == null)
            {
                settings = new CefSettings();
                settings.PersistSessionCookies = true;
                settings.CefCommandLineArgs["disable-features"] += ",SameSiteByDefaultCookies";
                Cef.Initialize(settings);
            }
        }
        public ChromiumWebBrowser createWebBrowser(string url)
        {
            var browser = new ChromiumWebBrowser(url, new RequestContext());
            browser.DownloadHandler = new DownloadHandler();
            browser.Height = 500;
            browser.Dock = DockStyle.Fill;
            return browser;
        }
        public WebBrowser createExplorerBrowser()
        {
            var explorer = new WebBrowser();
            explorer.Height = 500;
            explorer.Dock = DockStyle.Fill;
            explorer.ScriptErrorsSuppressed = true;
            return explorer;
        }
        public void reloadPage(ChromiumWebBrowser browser)
        {
            if (browser.IsBrowserInitialized)
                browser.GetMainFrame().ExecuteJavaScriptAsync("location.reload();");
        }
        public static string getUsernameHtmlElementId(string insuranceCompany, bool sfs = false)
        {
            //AK için set
            switch (insuranceCompany)
            {
                case InsuranceConstants.AkSigorta:
                    return "fpf-username";

                case InsuranceConstants.AxaSigorta:
                    return "edtusername";

                case InsuranceConstants.AllianzSigorta:
                    return "username";

                case InsuranceConstants.AnadoluSigorta:
                    return "username";

                case InsuranceConstants.BereketSigorta:
                    return "txtUsername";

                case InsuranceConstants.CorpusSigorta:
                    return "txtUsername";

                case InsuranceConstants.OrientSigorta:
                    return "txtUsername";

                case InsuranceConstants.DogaSigorta:
                    return "Username";

                case InsuranceConstants.GriSigorta:
                    return "Username";

                case InsuranceConstants.HdiSigorta:
                    return "username";

                case InsuranceConstants.KoruSigorta:
                    return "Username";

                case InsuranceConstants.MagdeburgerSigorta:                    
                    return sfs == false ? "kullanici_adi" : "kullanici_adi";

                case InsuranceConstants.MapfreSigorta:
                    return "i0116";

                case InsuranceConstants.NeovaSigorta:
                    return "ctl00_PlaceHolderMain_txtUsername";

                case InsuranceConstants.QuickSigorta:
                    return "agencyLoginUsername";
                case InsuranceConstants.QuickSigorta2:
                    return "agencyLoginUsername";
                case InsuranceConstants.RaySigorta:
                    return "User.UserName";

                case InsuranceConstants.TmtSigorta:
                    return "txtUsername";

                case InsuranceConstants.TurkiyeSigorta:
                    return "j_id12:j_id68:username";

                case InsuranceConstants.TurkNipponSigorta:
                    return "Username";

                case InsuranceConstants.UnicoSigorta:
                    return "username";

                case InsuranceConstants.Tramer:
                    return "hd_userName";

                case InsuranceConstants.SompoJapanSigorta:
                    return "durryX";

                case InsuranceConstants.SekerSigorta:
                    return "loginForm:j_id31";

                case InsuranceConstants.AtlasSigorta:
                    return "txtUsername";

                case InsuranceConstants.AnkaraSigorta:
                    return "document.querySelectorAll('.input-group')[0].children[6].value";
                case InsuranceConstants.GrupamaSigorta:
                    return "txtUsername";
                case InsuranceConstants.HepIyiSigorta:
                    return "UserName";
                case InsuranceConstants.AcnTurkSigorta:
                    return "userName";
                case InsuranceConstants.AveonSigorta:
                    return "Username";
                case InsuranceConstants.ZurichSigorta:
                    return "username";
                default:
                    return "";
            }
        }

        public static string getPasswordHtmlElementId(string insuranceCompany, bool sfs = false)
        {
            //Ak için set
            switch (insuranceCompany)
            {
                case InsuranceConstants.AkSigorta:
                    return "fpf-password";

                case InsuranceConstants.AxaSigorta:
                    return "edtpassword";

                case InsuranceConstants.AllianzSigorta:
                    return "password";

                case InsuranceConstants.AnadoluSigorta:
                    return "password";

                case InsuranceConstants.BereketSigorta:
                    return "txtPassword";

                case InsuranceConstants.CorpusSigorta:
                    return "txtPassword";

                case InsuranceConstants.OrientSigorta:
                    return "txtPassword";

                case InsuranceConstants.DogaSigorta:
                    return "Password";

                case InsuranceConstants.GriSigorta:
                    return "Password";

                case InsuranceConstants.HdiSigorta:
                    return "password";

                case InsuranceConstants.KoruSigorta:
                    return "Password";

                case InsuranceConstants.MagdeburgerSigorta:
                    return sfs == false ? "kullanici_sifre" : "kullanici_sifre";

                case InsuranceConstants.MapfreSigorta:
                    return "i0118";

                case InsuranceConstants.NeovaSigorta:
                    return "ctl00_PlaceHolderMain_txtPassword";

                case InsuranceConstants.QuickSigorta:
                    return "agencyLoginPassword";
                case InsuranceConstants.QuickSigorta2:
                    return "agencyLoginPassword";
                case InsuranceConstants.RaySigorta:
                    return "password";

                case InsuranceConstants.TmtSigorta:
                    return "txtPassword";

                case InsuranceConstants.TurkiyeSigorta:
                    return "j_id12:j_id77:password";

                case InsuranceConstants.TurkNipponSigorta:
                    return "Password";

                case InsuranceConstants.UnicoSigorta:
                    return "password";

                case InsuranceConstants.Tramer:
                    return "hd_userPW";

                case InsuranceConstants.SompoJapanSigorta:
                    return "durryY";

                case InsuranceConstants.SekerSigorta:
                    return "loginForm:j_id35";

                case InsuranceConstants.AtlasSigorta:
                    return "txtPassword";

                case InsuranceConstants.AnkaraSigorta:
                    return "input-group";
                case InsuranceConstants.GrupamaSigorta:
                    return "txtPassword";
                case InsuranceConstants.HepIyiSigorta:
                    return "Password";
                case InsuranceConstants.AcnTurkSigorta:
                    return "password";
                case InsuranceConstants.AveonSigorta:
                    return "Password";
                case InsuranceConstants.ZurichSigorta:
                    return "password";
                default:
                    return "";
            }
        }
        #region SMS
        private static Dictionary<string, string> smsButtonElements = new Dictionary<string, string>()
        {
            { InsuranceConstants.AkSigorta, "document.getElementsByClassName('btn btn-primary')[0].click()" },
            { InsuranceConstants.AllianzSigorta, "document.querySelector('#redirect').click()" },
            { InsuranceConstants.MagdeburgerSigorta, "" },
            { InsuranceConstants.QuickSigorta, "" },
            { InsuranceConstants.QuickSigorta2, "" },
            { InsuranceConstants.RaySigorta, "" },
            { InsuranceConstants.UnicoSigorta, "document.getElementById('validateOtp').click()" },
            { InsuranceConstants.SompoJapanSigorta, "" },
            { InsuranceConstants.AnadoluSigorta, "" },
            { InsuranceConstants.AnkaraSigorta, "" },
            { InsuranceConstants.AxaSigorta, "" },
            { InsuranceConstants.MapfreSigorta, "" },
            { InsuranceConstants.HepIyiSigorta, "document.getElementsByClassName('btn btn-lg btn-primary fw-bolder')[0].click()" }
        };
        public static string getSmsButtonElement(string insuranceCompany)
        {
            if (smsButtonElements.TryGetValue(insuranceCompany, out string element))
            {
                return element;
            }
            else
            {
                return "";
            }
        }
        public static string getSMSHtmlElementId(string insuranceCompany)
        {
            switch (insuranceCompany)
            {
                case InsuranceConstants.AkSigorta:
                    return "#smsPassword";
                case InsuranceConstants.AllianzSigorta:
                    return "#smsToken";
                case InsuranceConstants.MagdeburgerSigorta:
                    return "input[name=\"sms_code\"]";
                case InsuranceConstants.QuickSigorta:
                    return "input[data-elm-id=\"verification-code\"]";
                case InsuranceConstants.QuickSigorta2:
                    return "input[data-elm-id=\"verification-code\"]";
                case InsuranceConstants.RaySigorta:
                    return "input[ng-model=\"OtpSmsSifre\"]";
                case InsuranceConstants.UnicoSigorta:
                    return "#userOtpPass";
                case InsuranceConstants.SompoJapanSigorta:
                    return "#txtSmsCode";
                case InsuranceConstants.AnadoluSigorta:
                    return "input[name=\"smsValidationCode\"]";
                case InsuranceConstants.AnkaraSigorta:
                    return "#Code";
                case InsuranceConstants.AxaSigorta:
                    return "edtSmsToken";
                case InsuranceConstants.MapfreSigorta:
                    return "#idTxtBx_SAOTCC_OTC";
                case InsuranceConstants.HepIyiSigorta:
                    return "#authenticationCode";
                default:
                    return "";
            }
        }
        public async Task SetSMS(InsuranceCompany insuranceCompany, string smsElementId, ChromiumWebBrowser browser)
        {
            try
            {
                const int maxAttempts = 5;
                const int delayMilliseconds = 5000;
                const int setSmsDelayMilliseconds = 1500;
                const int akSigortaDelayMilliseconds = 3000;

                string token = await GetSMSCodeAsync(insuranceCompany, maxAttempts, delayMilliseconds);

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"{insuranceCompany.Title}: Sms alınamadı");
                }
                else
                {
                    if (insuranceCompany.Title == InsuranceConstants.AkSigorta)
                    {
                        await Task.Delay(akSigortaDelayMilliseconds);
                        string script = $@"var focusEvent = new Event(""focus"");
                                    var changeEvent = new Event(""change"");
                                    var inputEvent = new Event(""input"");
                                    var blurEvent = new Event(""blur"");
                                    var sms = document.getElementById(""smsPassword"");
                                    sms.dispatchEvent(focusEvent);
                                    sms.value = ""{token}"";
                                    sms.dispatchEvent(changeEvent);
                                    sms.dispatchEvent(inputEvent);
                                    sms.dispatchEvent(blurEvent);
                                    document.querySelector("".modal-content button"").click();";
                        browser.ExecuteScriptAsync(script);
                    }
                    else
                    {
                        string setSMSScript = $"document.querySelector('{smsElementId}').value='{token}';";
                        browser.ExecuteScriptAsyncWhenPageLoaded(setSMSScript);
                        await Task.Delay(setSmsDelayMilliseconds);
                        string elementName = getSmsButtonElement(insuranceCompany.Title);
                        browser.ExecuteScriptAsyncWhenPageLoaded(elementName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static async Task<string> GetSMSCodeAsync(InsuranceCompany insuranceCompany, int maxAttempts, int delayMilliseconds)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                string token = await Task.Run(() => getSMSCode(insuranceCompany));

                if (!string.IsNullOrEmpty(token))
                {
                    return token;
                }

                await Task.Delay(delayMilliseconds);
            }

            return null;
        }
        public static string getSMSCode(InsuranceCompany insuranceCompany)
        {
            string token = "";
            for (int i = 0; i < 10; i++)
            {
                var address = $"http://209.182.238.254:3001/api/get-sms-code?username={HttpUtility.UrlEncode(insuranceCompany.UserName)}";

                var request = (HttpWebRequest)WebRequest.Create(address);
                request.Timeout = 3000;
                request.ReadWriteTimeout = 3000;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    token = reader.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(token))
                {
                    break;
                }

                Thread.Sleep(1000);
            }
            return token;
        }
        #endregion
        #region OTP
        public static string getOTPHtmlElementId(string insuranceCompany)
        {
            switch (insuranceCompany)
            {
                case InsuranceConstants.BereketSigorta:
                    return "txtGAKod";

                case InsuranceConstants.CorpusSigorta:
                    return "txtGAKod";

                case InsuranceConstants.AtlasSigorta:
                    return "txtGAKod";

                case InsuranceConstants.OrientSigorta:
                    return "txtGAKod";

                case InsuranceConstants.TmtSigorta:
                    return "txtGAKod";

                case InsuranceConstants.DogaSigorta:
                    return "Code";

                case InsuranceConstants.GriSigorta:
                    return "Code";

                case InsuranceConstants.HdiSigorta:
                    return "token";

                case InsuranceConstants.KoruSigorta:
                    return "Code";

                case InsuranceConstants.NeovaSigorta:
                    return "txtGACode";

                case InsuranceConstants.TurkNipponSigorta:
                    return "Gauthcode";
                case InsuranceConstants.GrupamaSigorta:
                    return "txtGAKod";

                case InsuranceConstants.AnkaraSigorta:
                    return "Code";

                case InsuranceConstants.AveonSigorta:
                    return "Code";
                case InsuranceConstants.UnicoSigorta:
                    return "userOtpPass";

                default:
                    return "";
            }

        }
        public async Task AutoOTP(InsuranceCompany insuranceCompany, string otpElementId, ChromiumWebBrowser browser)
        {
            string setOTPScript = "";
            SetOTPAsync(insuranceCompany, otpElementId, browser);

            switch (insuranceCompany.Title)
            {
                case InsuranceConstants.BereketSigorta:
                    setOTPScript = "document.getElementById('btnLoginUser').click();";
                    await Task.Delay(5000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    setOTPScript = "document.getElementById('ext-gen69').click();";
                    break;
                case InsuranceConstants.CorpusSigorta:
                    setOTPScript = "document.getElementById('btnLoginUser').click();";
                    await Task.Delay(5000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    setOTPScript = "document.getElementById('ext-gen63').click();";
                    break;
                case InsuranceConstants.GrupamaSigorta:
                    setOTPScript = "document.getElementById('btnLoginUser').click();";
                    await Task.Delay(5000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    setOTPScript = "document.querySelector(\"#winGAC button\").click();";
                    break;
                case InsuranceConstants.TmtSigorta:
                    setOTPScript = "document.getElementById('btnLoginUser').click();";
                    await Task.Delay(5000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    setOTPScript = "document.getElementById('ext-gen61').click();";
                    break;
                case InsuranceConstants.UnicoSigorta:
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    setOTPScript = "document.getElementById('validateGauthOtp').click();";
                    break;
                case InsuranceConstants.OrientSigorta:
                    setOTPScript = "document.getElementById('btnLoginUser').click();";
                    await Task.Delay(5000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    setOTPScript = "document.getElementById('ext-gen61').click();";
                    break;
                case InsuranceConstants.AtlasSigorta:
                    setOTPScript = "document.getElementById('btnLoginUser').click();";
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    await Task.Delay(5000);
                    setOTPScript = "document.getElementById('ext-gen61').click();";
                    break;
                case InsuranceConstants.TurkNipponSigorta:
                    setOTPScript = "DoLogin();DoLogin();";
                    break;
                case InsuranceConstants.GriSigorta:
                case InsuranceConstants.KoruSigorta:
                case InsuranceConstants.DogaSigorta:
                case InsuranceConstants.AveonSigorta:
                    setOTPScript = "document.getElementsByName('button')[0].click();";
                    await Task.Delay(5000);
                    SetOTPAsync(insuranceCompany, otpElementId, browser);
                    break;
                case InsuranceConstants.HdiSigorta:
                    setOTPScript = "document.getElementById('loginForm').submit();";
                    await Task.Delay(6000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    SetOTPAsync(insuranceCompany, otpElementId, browser);
                    setOTPScript = "document.getElementById('totpForm').submit();";
                    break;
                case InsuranceConstants.NeovaSigorta:
                    setOTPScript = "document.getElementsByClassName('nv-popup-close')[0].click();";
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    Task.Factory.StartNew(() =>
                    {
                        
                        setOTPScript = "document.getElementById('nv-login').click();";
                        browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(6000);
                            SetOTPAsync(insuranceCompany, otpElementId, browser);
                            Thread.Sleep(1000);
                            setOTPScript = "document.getElementById('btnValidateTwoFactor').click();";
                            browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                        });
                    });

                    //setOTPScript = "document.getElementsByClassName('nv-popup-close')[0].click();";
                    //await Task.Delay(3000);
                    //browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    //setOTPScript = "document.getElementById('nv-login').click();";
                    //await Task.Delay(3000);
                    //browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    //SetOTPAsync(insuranceCompany, otpElementId, browser);
                    //setOTPScript = "document.getElementById('btnValidateTwoFactor').click();";
                    break;
                case InsuranceConstants.AnkaraSigorta:
                    setOTPScript = "document.querySelectorAll('.input-group')[0].children[6].value";
                    await Task.Delay(5000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    setOTPScript = "document.querySelectorAll('.input-group')[1].children[10].value";
                    await Task.Delay(5000);
                    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
                    SetOTPAsync(insuranceCompany, otpElementId, browser);
                    setOTPScript = "document.getElementsByClassName('btn-primary btn')[0].click()";
                    break;
                default:
                    throw new NotSupportedException("Insurance company not supported");
            }

            browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
        }

        public async Task SetOTPAsync(InsuranceCompany insuranceCompany, string otpElementId, ChromiumWebBrowser browser, string by = "id")
        {
            //try
            //{
            //    string p = HttpUtility.UrlEncode(insuranceCompany.Password);
            //    string address = $"http://209.182.238.254:3001/api/token?username={HttpUtility.UrlEncode(insuranceCompany.UserName)}&password={p}";

            //    ////using var httpClient = new HttpClient();
            //    //httpClient.Timeout = TimeSpan.FromSeconds(3);

            //    //var response = await httpClient.GetAsync(address);
            //    //response.EnsureSuccessStatusCode();

            //    //string token = await response.Content.ReadAsStringAsync();

            //    string setOTPScript = "";
            //    if (by == "id")
            //    {
            //        setOTPScript = $"document.getElementById('{otpElementId}').value='{token}';";
            //    }
            //    else if (by == "name")
            //    {
            //        setOTPScript = $"document.getElementsByName('{otpElementId}')[0].value='{token}';";
            //    }

            //    browser.ExecuteScriptAsyncWhenPageLoaded(setOTPScript);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }


        #endregion
        public InsuranceCompany ic { get; set; }
        public ChromiumWebBrowser br { get; set; }
        public async void setUserNameAndPassword(InsuranceCompany insuranceCompany, ChromiumWebBrowser browser)
        {
            if (browser.IsBrowserInitialized)
            {
                string userNameHtmlElementId = getUsernameHtmlElementId(insuranceCompany.Title);
                string passwordHtmlElementId = getPasswordHtmlElementId(insuranceCompany.Title);

                var setUserNameAndPasswordScript =
                    $"document.getElementById('{userNameHtmlElementId}').value='{insuranceCompany.UserName}';" +
                    $"document.getElementById('{passwordHtmlElementId}').value='{AesOperation.DecryptString(insuranceCompany.Password)}';";

                if (insuranceCompany.Title == InsuranceConstants.QuickSigorta)
                {
                    browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementsByClassName('qs-btn qs-btn-blue')[0].click()");
                    await Task.Delay(100);
                }

                if (insuranceCompany.Title == InsuranceConstants.AkSigorta)
                {
                    //browser.ExecuteScriptAsyncWhenPageLoaded("setTimeout(function(){var e=new Event(\"focus\"),t=new Event(\"change\"),n=new Event(\"input\"),a=new Event(\"blur\"),c=document.getElementById(\"fpf-username\");c.dispatchEvent(e),c.value=\"" + insuranceCompany.UserName + "\",c.dispatchEvent(t),c.dispatchEvent(n),c.dispatchEvent(a);var i=document.getElementById(\"fpf-password\");i.dispatchEvent(e),i.value=\"" + AesOperation.DecryptString(insuranceCompany.Password) + "\",i.dispatchEvent(t),i.dispatchEvent(n),i.dispatchEvent(a),document.getElementById(\"lf-giris-yap\").click()},4e3);");
                    //await Task.Delay(3000);
                }
                else
                {
                    await Task.Run(() =>
                    {
                        browser.ExecuteScriptAsyncWhenPageLoaded(setUserNameAndPasswordScript);
                    });
                }

                if (insuranceCompany.Title == InsuranceConstants.AnadoluSigorta)
                {
                    browser.ExecuteScriptAsyncWhenPageLoaded($"var createEvent = function(name) {{ var event = document.createEvent('Event'); event.initEvent(name, true, true); return event; }};document.getElementById('{userNameHtmlElementId}').dispatchEvent(createEvent('change'));document.getElementById('{passwordHtmlElementId}').dispatchEvent(createEvent('change'));");
                }
                if (insuranceCompany.Title == InsuranceConstants.AllianzSigorta)
                {
                    browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementsByClassName('btn-submit')[0].click()");
                    await Task.Delay(100);
                }
                if (insuranceCompany.Title == InsuranceConstants.AcnTurkSigorta)
                {
                    browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementById('btnlogin').click()");
                }
                if (insuranceCompany.Title == InsuranceConstants.QuickSigorta)
                {
                    browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementsByClassName('button-stroke btn-animate AgencyLogin')[0].click()");
                    browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementById('rc-anchor-alert').click()");
                }
                if (insuranceCompany.Title == InsuranceConstants.HepIyiSigorta)
                {
                    browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementById('kt_sign_in_submit').click()");
                }
                if (insuranceCompany.Title == InsuranceConstants.MagdeburgerSigorta)
                {
                    browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementsByClassName('btn btn-primary btn-block loginButton waves-effect waves-light')[0].click()");
                }

                string otpElementId = getOTPHtmlElementId(insuranceCompany.Title);
                if (!string.IsNullOrEmpty(otpElementId))
                {
                    await Task.Delay(100);
                    AutoOTP(insuranceCompany, otpElementId, browser);
                }

                ic = insuranceCompany;
                br = browser;
                //System.Threading.Timer timer = new System.Threading.Timer(new TimerCallback(smsDoldur), null, 5000, Timeout.Infinite);
            }
        }
        bool unico = true;
        void smsDoldur(object state)
        {
            string title = ic.Title;

            if (title == InsuranceConstants.UnicoSigorta)
            {
                if (unico)
                {
                    SetUserNameAndPasswordByNameAsync(ic, br);
                    unico = false;
                    return;
                }
                else
                {
                    br.ExecuteScriptAsyncWhenPageLoaded("getOtpStatus()");
                }
            }

            SetSMS(ic, getSMSHtmlElementId(title), br);
        }
        public void setOtpForAkSigorta(string pass, ChromiumWebBrowser browser)
        {
            if (browser.IsBrowserInitialized)
            {
                browser.GetBrowser().GetHost().SendMouseClickEvent(100, 100, MouseButtonType.Left, false, 1, CefEventFlags.None);
                Thread.Sleep(1000);
                browser.GetBrowser().GetHost().SendMouseClickEvent(100, 100, MouseButtonType.Left, true, 1, CefEventFlags.None);
                Thread.Sleep(1001);

                sendKeysFromString("Tab", browser);
                sendKeysFromString(pass, browser);

                //var sc = $"document.getElementsByTagName('form')[0].submit();";
                //browser.ExecuteScriptAsync(sc);
            }
        }
        public void setUserNameAndPasswordForAnkaraSigorta(string userName, string password, ChromiumWebBrowser webBrowser)
        {
            var sb = new StringBuilder();
            sb.AppendLine("setTimeout(function(){var t=document.getElementsByClassName(\"input-group\")[0],e=document.getElementsByClassName(\"input-group\")[1],l=t.querySelectorAll(\".form-control\"),n=e.querySelectorAll(\".form-control\"),s=[],i=[];for(let o=0;o<l.length;o++)l[o].classList.contains(\"hidden\")||s.push(l[o]);var r=s[0];for(let a=0;a<s.length;a++)s[a].clientWidth>r.clientWidth&&(r=s[a]);for(let c=0;c<n.length;c++)n[c].classList.contains(\"hidden\")||i.push(n[c]);var h=i[0];for(let u=0;u<i.length;u++)i[u].clientWidth>h.clientWidth&&(h=i[u]);r.value='" + userName + "',h.value='" + password + "'},3e3);");
            webBrowser.ExecuteScriptAsyncWhenPageLoaded(sb.ToString());
        }
        public async Task SetUserNameAndPasswordByNameAsync(InsuranceCompany insuranceCompany, ChromiumWebBrowser browser)
        {
            if (!browser.IsBrowserInitialized)
            {
                return;
            }

            string userNameHtmlElementId = getUsernameHtmlElementId(insuranceCompany.Title);
            string passwordHtmlElementId = getPasswordHtmlElementId(insuranceCompany.Title);
            var setUserNameAndPasswordScript =
                $"document.getElementsByName('{userNameHtmlElementId}')[0].value='{insuranceCompany.UserName}';" +
                $"document.getElementsByName('{passwordHtmlElementId}')[0].value='{AesOperation.DecryptString(insuranceCompany.Password)}';" +
                $"document.getElementById('LogonForm').submit();";

            if (insuranceCompany.Title == InsuranceConstants.UnicoSigorta)
            {
                browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementsByClassName('box v6')[0].click()");
                await Task.Delay(100);
                browser.ExecuteScriptAsyncWhenPageLoaded(setUserNameAndPasswordScript);
                browser.ExecuteScriptAsyncWhenPageLoaded("document.getElementById('btn-login').click()");
            }

            var timer = new System.Threading.Timer(smsDoldur, null, 500, Timeout.Infinite);
        }
        internal void setUserNameAndPasswordByForceDetect(string v1, string v2, string title, ChromiumWebBrowser browser)
        {
            throw new NotImplementedException();
        }
        public void sendKeysFromString(string keys, ChromiumWebBrowser browser)
        {
            if (keys == "Enter")
            {
                var ev = new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.Enter, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.RawKeyDown, IsSystemKey = false };

                browser.GetBrowser().GetHost().SendKeyEvent(ev);
                return;
            }

            if (keys == "space")
            {
                var ev = new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.Space, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.KeyUp, IsSystemKey = false };
                browser.GetBrowser().GetHost().SendKeyEvent(ev);
                return;
            }

            if (keys == "Tab")
            {
                var ev = new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.Tab, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.KeyDown, IsSystemKey = false };
                browser.GetBrowser().GetHost().SendKeyEvent(ev);
                return;
            }

            List<KeyEvent> events = new List<KeyEvent>();
            foreach (var item in keys.ToUpper())
            {
                switch (item)
                {
                    case '0':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D0, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '1':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D1, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '2':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D2, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '3':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D3, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '4':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D4, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '5':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D5, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '6':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D6, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '7':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D7, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '8':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D8, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '9':
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys(Keys.D9, false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                    case '.':

                        break;

                    default:
                        events.Add(new KeyEvent() { FocusOnEditableField = true, WindowsKeyCode = GetCharsFromKeys((Keys)Enum.Parse(typeof(Keys), item.ToString()), false, false)[0], Modifiers = CefEventFlags.None, Type = KeyEventType.Char, IsSystemKey = false });
                        break;
                }
            }

            foreach (KeyEvent ev in events)
            {
                browser.GetBrowser().GetHost().SendKeyEvent(ev);
            }
        }
        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
        byte[] keyboardState,
        [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
        StringBuilder receivingBuffer,
        int bufferSize, uint flags);
        static string GetCharsFromKeys(Keys keys, bool shift, bool altGr)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            if (shift)
                keyboardState[(int)Keys.ShiftKey] = 0xff;
            if (altGr)
            {
                keyboardState[(int)Keys.ControlKey] = 0xff;
                keyboardState[(int)Keys.Menu] = 0xff;
            }
            ToUnicode((uint)keys, 0, keyboardState, buf, 256, 0);
            return buf.ToString();
        }

        //internal static object getInstance()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
