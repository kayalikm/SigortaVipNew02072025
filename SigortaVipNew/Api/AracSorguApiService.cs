using SigortaVip.Dto;
using SigortaVip.Forms;
using SigortaVip.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SigortaVipNew.Api
{
    internal class AracSorguApiService : BaseApiService
    {
        public async Task<string> AddCookiesAsync(List<CookieDto> cookies, int insuranceCompanyId)
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

        public async Task<string> CreateQueryAsync(QueryRequestDto queryRequest)
        {
            try
            {
                string json = JsonSerializer.Serialize(queryRequest);
                Console.WriteLine($"Query Request: {json}");
                string url = "query/queries/create/";
                return await PostAsync(url, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateQueryAsync hatası: {ex.Message}");
                throw;
            }
        }
    }
}