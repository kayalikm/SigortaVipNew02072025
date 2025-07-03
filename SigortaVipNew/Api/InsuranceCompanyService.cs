using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SigortaVipNew.Models;

namespace SigortaVipNew.Api
{
    public class InsuranceCompanyApiService : BaseApiService
    {
        public async Task<List<InsuranceCompanyItem>> GetAllAsync()
        {
            var json = await GetAsync("insurance-company-items/all_items_no_pagination/");

            return JsonSerializer.Deserialize<List<InsuranceCompanyItem>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
