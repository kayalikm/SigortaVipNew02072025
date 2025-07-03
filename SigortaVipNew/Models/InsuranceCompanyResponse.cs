using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace SigortaVipNew.Models
{
    public class InsuranceCompanyResponse
    {
        [JsonPropertyName("results")]
        public List<InsuranceCompanyItem> Results { get; set; }
    }

    public class InsuranceCompanyItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("query_types")]
        public List<object> QueryTypes { get; set; }

        [JsonPropertyName("same_insurance_company_items")]
        public List<object> SameInsuranceCompanyItems { get; set; }

        [JsonPropertyName("insurance_company")]
        public InsuranceCompanyDto InsuranceCompany { get; set; }

        // API response'dan gelen cookies array'i
        [JsonPropertyName("cookies")]
        public List<ApiCookie> Cookies { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("sms_code")]
        public string SmsCode { get; set; }

        [JsonPropertyName("totp_code")]
        public string TotpCode { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }

        [JsonPropertyName("proxy_username")]
        public string ProxyUsername { get; set; }

        [JsonPropertyName("proxy_password")]
        public string ProxyPassword { get; set; }

        [JsonPropertyName("is_proxy_active")]
        public bool IsProxyActive { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("is_car_query")]
        public bool IsCarQuery { get; set; }

        [JsonPropertyName("cookie_use")]
        public bool CookieUse { get; set; }

        [JsonPropertyName("cookie")]
        public string CookieString { get; set; }

        // Cookie string'ini InsuranceCookie listesine dönüştüren property (fallback olarak)
        [JsonIgnore]
        public List<InsuranceCookie> Cookie =>
            !string.IsNullOrEmpty(CookieString)
                ? InsuranceCookieExtensions.ParseCookieString(CookieString)
                : new List<InsuranceCookie>();

        // API'den gelen cookies'leri InsuranceCookie formatına dönüştüren property
        [JsonIgnore]
        public List<InsuranceCookie> InsuranceCookies =>
            Cookies?.Select(c => c.ToInsuranceCookie()).ToList() ?? new List<InsuranceCookie>();

        // Tüm cookie'leri birleştiren property (hem string'den parse edilenler hem de API'den gelenler)
        [JsonIgnore]
        public List<InsuranceCookie> AllCookies
        {
            get
            {
                var allCookies = new List<InsuranceCookie>();

                // API'den gelen cookies varsa onları ekle
                if (Cookies?.Any() == true)
                {
                    allCookies.AddRange(InsuranceCookies);
                }
                // Cookie string varsa onu parse et ve ekle
                else if (!string.IsNullOrEmpty(CookieString))
                {
                    allCookies.AddRange(Cookie);
                }

                return allCookies;
            }
        }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("company")]
        public int Company { get; set; }

        [JsonPropertyName("partage")]
        public object Partage { get; set; }
    }

    // API response'dan gelen cookie modeli
    public class ApiCookie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("expires")]
        public DateTime? Expires { get; set; }

        [JsonPropertyName("creation")]
        public DateTime? Creation { get; set; }

        [JsonPropertyName("last_access")]
        public DateTime? LastAccess { get; set; }

        [JsonPropertyName("http_only")]
        public bool HttpOnly { get; set; }

        [JsonPropertyName("secure")]
        public bool Secure { get; set; }

        [JsonPropertyName("same_site")]
        public int SameSite { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("insurance_company_item")]
        public int InsuranceCompanyItem { get; set; }

        // ApiCookie'yi InsuranceCookie'ye dönüştürme metodu
        public InsuranceCookie ToInsuranceCookie()
        {
            return new InsuranceCookie
            {
                Name = this.Name,
                Value = this.Value,
                Domain = this.Domain,
                Path = this.Path,
                Expires = this.Expires,
                Secure = this.Secure,
                HttpOnly = this.HttpOnly
            };
        }
    }

    public class InsuranceCompanyDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("login_url")]
        public string LoginUrl { get; set; }

        [JsonPropertyName("explorer_url")]
        public string ExplorerUrl { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }
    }

    public class InsuranceCookie
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("expires")]
        public DateTime? Expires { get; set; }

        [JsonPropertyName("secure")]
        public bool Secure { get; set; }

        [JsonPropertyName("httpOnly")]
        public bool HttpOnly { get; set; }

        // InsuranceCookie'yi CefSharp.Cookie'ye dönüştürme metodu
        public CefSharp.Cookie ToCefSharpCookie()
        {
            return new CefSharp.Cookie
            {
                Name = this.Name,
                Value = this.Value,
                Domain = this.Domain ?? "portal.dogasigorta.com",
                Path = this.Path ?? "/",
                Expires = this.Expires,
                Secure = this.Secure,
                HttpOnly = this.HttpOnly,
            };
        }
    }

    // Extension method for List conversion
    public static class InsuranceCookieExtensions
    {
        public static List<CefSharp.Cookie> ToCefSharpCookies(this List<InsuranceCookie> insuranceCookies)
        {
            return insuranceCookies?.Select(c => c.ToCefSharpCookie()).ToList() ?? new List<CefSharp.Cookie>();
        }

        // Cookie string'ini parse edip InsuranceCookie listesine dönüştürme
        public static List<InsuranceCookie> ParseCookieString(string cookieString, string domain = "")
        {
            if (string.IsNullOrWhiteSpace(cookieString))
                return new List<InsuranceCookie>();

            var cookies = new List<InsuranceCookie>();

            // Cookie string'ini '; ' ile böl
            var cookiePairs = cookieString.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var cookiePair in cookiePairs)
            {
                // Her cookie'yi '=' ile böl
                var parts = cookiePair.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    cookies.Add(new InsuranceCookie
                    {
                        Name = parts[0].Trim(),
                        Value = parts[1].Trim(),
                        Domain =  domain,
                        Path = "/",
                        Secure = false,
                        HttpOnly = false,
                        Expires = DateTime.Now.AddDays(1),
                    });
                }
            }

            return cookies;
        }

        // Cookie string'ini direkt CefSharp.Cookie listesine dönüştürme
        public static List<CefSharp.Cookie> ParseCookieStringToCefSharp(string cookieString, string domain = "")
        {
            var insuranceCookies = ParseCookieString(cookieString, domain);
            return insuranceCookies.ToCefSharpCookies();
        }

        // ApiCookie listesini CefSharp.Cookie listesine dönüştürme
        public static List<CefSharp.Cookie> ToCefSharpCookies(this List<ApiCookie> apiCookies)
        {
            return apiCookies?.Select(c => c.ToInsuranceCookie().ToCefSharpCookie()).ToList() ?? new List<CefSharp.Cookie>();
        }
    }
}