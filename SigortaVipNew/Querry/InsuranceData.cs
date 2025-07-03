using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Windows;


namespace SigortaVip.Querry
{
    public static class InsuranceData
    {

        //public static async Task<dynamic> MakeApiRequest(dynamic queryItems)
        //{
        //    string apiUrl = "http://209.182.238.254:3001/api/query-information";
        //    string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."; // kısaltıldı

        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        string jsonContent = JsonConvert.SerializeObject(queryItems);
        //        StringContent stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //        //try
        //        //{
        //        //    HttpResponseMessage response = await client.PostAsync(apiUrl, stringContent);
        //        //    string apiResponse = await response.Content.ReadAsStringAsync();
        //        //    //olabilir abi ama bence json parse etmiyor gibi bak
        //        //    if (response.IsSuccessStatusCode)
        //        //    {
        //        //        // burada veritipine göre hata var bakılacak TODO
        //        //        dynamic jsonResponse = JsonConvert.DeserializeObject(apiResponse);
        //        //        MessageBox.Show(apiResponse); // JSON string göster
        //        //        //MessageBox.Show(jsonResponse); // JSON string göster
        //        //        return apiResponse;
        //        //    }
        //        //    else
        //        //    {
        //        //        MessageBox.Show($"Hata Kodu: {response.StatusCode}\nMesaj: {apiResponse}");
        //        //        return null;
        //        //    }
        //        //}
        //        //catch (Exception e)
        //        //{
        //        //    MessageBox.Show($"İstisna: {e.Message}");
        //        //    return null;
        //        //}
        //    }
        //}
        //public static async Task<dynamic>GetBirthDateFromSompoJapan(dynamic queryItems)
        //{
        //    string apiUrl = "http://209.182.238.254:3001/api/query-information-birthdate";
        //    //string apiUrl = "http://188.119.41.56:3001/api/query-information-birthdate"; //devolepment
        //    //TODO this data will be get api request /api/login method post username and password 
        //    string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImVtcmFoc2FuZGVyIiwidXNlcklkIjoxLCJpc0FkbWluIjp0cnVlLCJpYXQiOjE3MDA2ODk3ODQsImV4cCI6MTczMjIyNTc4NH0.2l4Acu7G_Zmw-svzLRbSfBWvluzuKSo1GvXNMbUNcOw";
        //    var requestData = queryItems;
       
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(apiUrl);
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
        //        StringContent stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //        try
        //        {
        //            HttpResponseMessage response = await client.PostAsync(apiUrl, stringContent);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                string apiResponse = await response.Content.ReadAsStringAsync();
        //                dynamic jsonRepsonse = JsonConvert.DeserializeObject(apiResponse);
        //                Console.WriteLine(jsonRepsonse);
        //                return jsonRepsonse;
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Hata Kodu:{response.StatusCode}");
        //                return null;

        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine($"Hata: {e.Message}");
        //            return null;
        //        }


        //    }
        //}

    }
}
