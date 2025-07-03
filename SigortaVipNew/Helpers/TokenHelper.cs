using System;
using System.Threading.Tasks;
using System.Web;

namespace SigortaVip.Helpers
{
    public class TokenHelper : BaseApiService
    {
        public async Task<string> GetTokenAsync(string userName, string password, int maxAttempt = 3)
        {
            string token = "";

            for (int i = 0; i < maxAttempt; i++)
            {
                try
                {
                    string encodedPassword = HttpUtility.UrlEncode(password);
                    string encodedUserName = HttpUtility.UrlEncode(userName);
                    var endpoint = $"totp?username={encodedUserName}&password={encodedPassword}";

                    Console.WriteLine($"Token request endpoint: {endpoint}"); // Debug için loglama

                    token = await GetAsync(endpoint);

                    if (!string.IsNullOrEmpty(token))
                    {
                        // Token'ı global olarak set et
                        Token = token;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token request error on attempt {i + 1}: {ex.Message}");
                    // Loglama yapılabilir
                }

                if (i < maxAttempt - 1) // Son denemede bekleme yapma
                {
                    await Task.Delay(3000);
                }
            }

            return token;
        }
    }
}