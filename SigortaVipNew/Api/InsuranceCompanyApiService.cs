using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SigortaVip.Models;
using System.Linq;

namespace SigortaYazilim.Api
{
    public class InsuranceCompanyApiService : BaseApiService
    {
        public InsuranceCompanyApiService() : base()
        {
            // Constructor
        }

        public async Task<List<InsuranceCompanyItem>> GetAllAsync()
        {
            try
            {
                var content = await GetAsync("insurance-company-items/");
                return JsonConvert.DeserializeObject<List<InsuranceCompanyItem>>(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<InsuranceCompanyItemResponse> GetCompanyItemAsync(int id)
        {
            try
            {
                var content = await GetAsync($"insurance-company-items/{id}/");
                return JsonConvert.DeserializeObject<InsuranceCompanyItemResponse>(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetCompanyItemAsync error: {ex.Message}");
                return null;
            }
        }
    }
} 