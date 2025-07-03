using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SigortaVip.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SigortaVip.Models
{
    public class ApiTools
    {
        public static async Task<string> GenerateToken()
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://vip.sigortayazilim.com.tr/Home/GenerateToken");

            //var json = $@"{{  
            //               ""Username"": ""{NewLoginForm.kullaniciAdi}"",  
            //               ""Password"": ""{NewLoginForm.kullaniciSifre}"",  
            //               ""acentaKodu"": ""{NewLoginForm.acentaKodu}""  
            //            }}";
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            //request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            return tokenResponse?.Token;
        }

        public static async Task<string> PostSeciliSirketlerUpdate(string id, List<Sirket> seciliSirketler, string token)
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://vip.sigortayazilim.com.tr/Home/PostSeciliSirketlerUpdate");

            string jsonSirketler = JsonConvert.SerializeObject(seciliSirketler);

            var payload = new
            {
                Id = id,
                seciliSirketler = jsonSirketler,
                Token = token
            };

            var json = JsonConvert.SerializeObject(payload);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public class Sirket
        {
            public string SirketId { get; set; }
            public string SirketIsmi { get; set; }
            public string CheckboxDegeri { get; set; }
        }

        public class TokenResponse
        {
            public bool Success { get; set; }
            public string Token { get; set; }
        }
    }
}
