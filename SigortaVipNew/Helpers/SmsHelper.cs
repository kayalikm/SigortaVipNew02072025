using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SigortaVip.Helpers
{
    public static class SmsHelper
    {
        public static async Task<string> GetSmsAsync(string userName, string password, int maxAttempt = 3)
        {
            string token = "";

            for (int i = 0; i < maxAttempt; i++)
            {
                try
                {
                    string encodedPassword = HttpUtility.UrlEncode(password);
                    string encodedUserName = HttpUtility.UrlEncode(userName);

                    var address = $"http://209.182.238.254:3001/api/get-sms-code?username={HttpUtility.UrlEncode(userName)}";
                    var request = (HttpWebRequest)WebRequest.Create(address);

                    request.Timeout = 3000;
                    request.ReadWriteTimeout = 3000;

                    using (var response = (HttpWebResponse)await request.GetResponseAsync())
                    using (var dataStream = response.GetResponseStream())
                    using (var reader = new StreamReader(dataStream))
                    {
                        token = await reader.ReadToEndAsync();
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        break;
                    }
                }
                catch (WebException)
                {
                    // Loglama yapılabilir
                }

                await Task.Delay(3000);
            }

            return token;
        }
    }
}
