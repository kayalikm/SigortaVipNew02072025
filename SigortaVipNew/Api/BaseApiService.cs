using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

public class BaseApiService
{
    protected readonly HttpClient _client;

    public static string Token { get; set; }  // Global token tutucu

    public BaseApiService()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://api.kayaliksigorta.com/api/v1/")
        };

        // Header'ı her istekten önce eklemiyoruz çünkü token dinamik olabilir.
    }

    private void SetAuthorizationHeader()
    {
        _client.DefaultRequestHeaders.Clear();
        if (!string.IsNullOrEmpty(Token))
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Token", Token);
        }
    }

    protected async Task<string> GetAsync(string endpoint)
    {
        SetAuthorizationHeader();  // Header’ı her istek öncesi ekle
        var response = await _client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task<string> PostAsync(string endpoint, string json)
    {
        //SetAuthorizationHeader();  // Header’ı her istek öncesi ekle
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
