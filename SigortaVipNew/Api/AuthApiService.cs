using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SigortaVipNew.Models;

namespace SigortaYazilim.Api
{
    public class AuthApiService : BaseApiService
    {
        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            // Gönderilecek veriyi JSON'a çevir
            var json = JsonConvert.SerializeObject(request);

            // API'ye POST isteği gönder
            var response = await PostAsync("login/", json);

            // Debug için loglama
            Console.WriteLine($"Login response: {response}");

            // Dönen JSON'u modele çevir
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(response);

            // Token'ı globalde saklamak istersen buraya ekleyebilirsin:
            // BaseApiService.Token = authResponse.token;

            return authResponse;
        }
    }
}
