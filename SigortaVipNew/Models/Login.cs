using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace SigortaVip.Models
{
    internal class Login
    {
        // Username password ve kurum adı gönderek login işlemi yapıp gerekli iki session id almaya yarar 
//        public FirmaSessionBilgileri login(string username, string password, string company)
//        {
//            var client = new RestClient("http://api.sigortayazilim.com//api/login");
//            client.Timeout = -1;
//            var request = new RestRequest(Method.POST);
//            request.AddHeader("Content-Type", "application/json");
//            var body = @"{
//" + "\n" +
//            @"    ""UserName"": """ + username + @""",
//" + "\n" +
//            @"    ""Password"": """ + password + @"""
//" + "\n" +
//            @"}";
//            request.AddParameter("application/json", body, ParameterType.RequestBody);
//            IRestResponse response = client.Execute(request);
//            dynamic stuff = JsonConvert.DeserializeObject(response.Content);

//            string token = stuff.token;

//            client = new RestClient("http://api.sigortayazilim.com/api/cookie?insuranceCompany=" + company + "");
//            client.Timeout = -1;
//            request = new RestRequest(Method.GET);
//            request.AddHeader("Authorization", token);
//            request.AlwaysMultipartFormData = true;
//            response = client.Execute(request);
//            response = client.Execute(request);
//            stuff = JsonConvert.DeserializeObject(response.Content);
            
//            FirmaSessionBilgileri firmaSessionBilgileri = new FirmaSessionBilgileri();
//            firmaSessionBilgileri.SessionList = new List<FirmaSessionBilgileri.Session>();

//            foreach (var item  in stuff)
//            {
//                firmaSessionBilgileri.SessionList.Add(new FirmaSessionBilgileri.Session
//                {
//                    SessionName = item.name,
//                    SessionValue = item.value
//                });
//            }
          


//            return firmaSessionBilgileri;
//        }
    }
}
