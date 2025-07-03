using SigortaVip.Dto;
using SigortaVip.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SigortaVipNew.Api
{
    internal class CookieApiService : BaseApiService
    {
        private static CookieApiService _instance = null;
        private static readonly object _lock = new object();

        private CookieApiService() : base()
        {
        }

        public static CookieApiService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new CookieApiService();
                }
            }
            return _instance;
        }

        /// <summary>
        /// Cookie listesini API'ye gönderir
        /// </summary>
        /// <param name="cookies">Gönderilecek cookie listesi</param>
        /// <returns>API yanıtı</returns>
        public async Task<string> AddCookiesAsync(List<CookieDto> cookies,int insuranceCompanyId)
        {
            try
            {
                string json = JsonSerializer.Serialize(cookies).ToString();
                Console.WriteLine(insuranceCompanyId);
                string url = $"insurance-company-items/{insuranceCompanyId}/update_cookies_bulk/";
                return await PostAsync(url, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddCookiesAsync hatası: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Senkron cookie ekleme metodu (geriye uyumluluk için)
        /// </summary>
        /// <param name="cookies">Gönderilecek cookie listesi</param>
        /// <returns>API yanıtı</returns>
        public string AddCookies(List<CookieDto> cookies, int insuranceCompanyId)
        {
            return AddCookiesAsync(cookies, insuranceCompanyId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Belirli bir sigorta şirketi için cookie listesini getirir
        /// </summary>
        /// <param name="insuranceCompany">Sigorta şirketi adı</param>
        /// <returns>Cookie listesi JSON string</returns>
        public async Task<string> ListCookiesAsync(string insuranceCompany)
        {
            try
            {
                return await GetAsync($"cookie?insuranceCompany={Uri.EscapeDataString(insuranceCompany)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ListCookiesAsync hatası: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Senkron cookie listeleme metodu (geriye uyumluluk için)
        /// </summary>
        /// <param name="insuranceCompany">Sigorta şirketi adı</param>
        /// <returns>Cookie listesi JSON string</returns>
        public string ListCookies(string insuranceCompany)
        {
            return ListCookiesAsync(insuranceCompany).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sigorta şirketi ID'sine göre cookie'leri siler
        /// </summary>
        /// <param name="insuranceCompanyId">Sigorta şirketi ID</param>
        /// <returns>API yanıtı</returns>
        public async Task<string> DeleteCookieByCompanyIdAsync(long insuranceCompanyId)
        {
            try
            {
                return await PostAsync($"cookie/delete/insuranceCompany/{insuranceCompanyId}", "{}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteCookieByCompanyIdAsync hatası: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Senkron ID ile cookie silme metodu (geriye uyumluluk için)
        /// </summary>
        /// <param name="insuranceCompanyId">Sigorta şirketi ID</param>
        /// <returns>API yanıtı</returns>
        public string DeleteCookieByCompanyId(long insuranceCompanyId)
        {
            return DeleteCookieByCompanyIdAsync(insuranceCompanyId).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sigorta şirketi adına göre cookie'leri siler
        /// </summary>
        /// <param name="insuranceCompany">Sigorta şirketi adı</param>
        /// <returns>API yanıtı</returns>
        public async Task<string> DeleteCookieByCompanyNameAsync(string insuranceCompany)
        {
            try
            {
                return await PostAsync($"cookie/delete/insuranceCompanyByName/{Uri.EscapeDataString(insuranceCompany)}", "{}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteCookieByCompanyNameAsync hatası: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Senkron isim ile cookie silme metodu (geriye uyumluluk için)
        /// </summary>
        /// <param name="insuranceCompany">Sigorta şirketi adı</param>
        /// <returns>API yanıtı</returns>
        public string DeleteCookieByCompanyName(string insuranceCompany)
        {
            return DeleteCookieByCompanyNameAsync(insuranceCompany).GetAwaiter().GetResult();
        }

        /// <summary>
        /// API bağlantısının sağlıklı olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Bağlantı durumu</returns>
        public async Task<bool> IsApiHealthyAsync()
        {
            try
            {
                await GetAsync("base/date");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Senkron API sağlık kontrolü
        /// </summary>
        /// <returns>Bağlantı durumu</returns>
        public bool IsApiHealthy()
        {
            return IsApiHealthyAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Cookie response'unu deserialize eder
        /// </summary>
        /// <param name="jsonResponse">JSON yanıt</param>
        /// <returns>Cookie DTO listesi</returns>
        public List<CookieDto> DeserializeCookies(string jsonResponse)
        {
            try
            {
                return JsonSerializer.Deserialize<List<CookieDto>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeserializeCookies hatası: {ex.Message}");
                return new List<CookieDto>();
            }
        }

        /// <summary>
        /// Token'ı günceller (BaseApiService'teki static Token'ı kullanır)
        /// </summary>
        /// <param name="token">Yeni token</param>
        public void UpdateToken(string token)
        {
            BaseApiService.Token = token;
        }
    }
}