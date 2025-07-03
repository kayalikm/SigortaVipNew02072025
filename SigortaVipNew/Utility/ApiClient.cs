using RestSharp;
using SigortaVip.Dto;
using SigortaVip.Models;
using System.Collections.Generic;
using System.Configuration;

namespace SigortaVip.Utility
{
    class ApiClient
    {
    //    private static ApiClient apiClient = null;
    //    private static RestClient client = null;

    //    private ApiClient()
    //    {
    //        client = new RestClient(ConfigurationManager.AppSettings["apiBaseUrl"]);
    //    }

    //    public static ApiClient getInstance()
    //    {
    //        if (apiClient == null)
    //            return new ApiClient();

    //        return apiClient;
    //    }

    //    public IRestResponse listCookies(string insuranceCompany)
    //    {
    //        RestRequest request = new RestRequest("cookie", Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        request.AddParameter("insuranceCompany", insuranceCompany);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse getInsuranceCompany(string insuranceCompany)
    //    {
    //        RestRequest request = new RestRequest("insuranceCompany", Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        request.AddParameter("insuranceCompany", insuranceCompany);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse listInsuranceCompanies()
    //    {
    //        RestRequest request = new RestRequest("insuranceCompany", Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        return client.Execute(request);
    //    }

    //    public IRestResponse updateInsuranceCompany(InsuranceCompany insuranceCompany)
    //    {
    //        RestRequest request = new RestRequest("insuranceCompany/update", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(insuranceCompany);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse updateInsuranceCompanyByUsername(InsuranceCompany insuranceCompany)
    //    {
    //        RestRequest request = new RestRequest("insuranceCompany/update2", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(insuranceCompany);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse login(string userName, string password)
    //    {
    //        RestRequest request = new RestRequest("login", Method.POST);
    //        request.AddJsonBody(new LoginDto { UserName = userName, Password = password });
    //        return client.Execute(request);
    //    }

    //    public IRestResponse deleteUser(long id)
    //    {
    //        RestRequest request = new RestRequest("user/delete/" + id, Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse addUser(User user)
    //    {
    //        RestRequest request = new RestRequest("user", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(user);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse updateUser(User user)
    //    {
    //        RestRequest request = new RestRequest("user/update", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(user);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse listUsers()
    //    {
    //        RestRequest request = new RestRequest("user", Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        return client.Execute(request);
    //    }

    //    public IRestResponse addCookies(List<CookieDto> cookies)
    //    {
    //        RestRequest request = new RestRequest("cookie", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(cookies);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse getDate()
    //    {
    //        RestRequest request = new RestRequest("base/date", Method.GET);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse deleteCookie(long insuranceCompanyId)
    //    {
    //        RestRequest request = new RestRequest("cookie/delete/insuranceCompany/" + insuranceCompanyId, Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse deleteCookie(string insuranceCompany)
    //    {
    //        RestRequest request = new RestRequest("cookie/delete/insuranceCompanyByName/" + insuranceCompany, Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse listAnnouncement()
    //    {
    //        RestRequest request = new RestRequest("announcement", Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        return client.Execute(request);
    //    }

    //    public IRestResponse addAnnouncement(Announcement announcement)
    //    {
    //        RestRequest request = new RestRequest("announcement", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(announcement);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse deleteAnnouncement(long announcementId)
    //    {
    //        RestRequest request = new RestRequest("announcement/delete/" + announcementId, Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse updateAnnouncement(Announcement announcement)
    //    {
    //        RestRequest request = new RestRequest("announcement/update", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(announcement);
    //        return client.Execute(request);
    //    }


    //    public IRestResponse addCompany(CompanyDto companyDto)
    //    {
    //        RestRequest request = new RestRequest("company", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(companyDto);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse getCompany()
    //    {
    //        RestRequest request = new RestRequest("company/list", Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        return client.Execute(request);
    //    }

    //    public IRestResponse deleteCompany(long companyId)
    //    {

    //        RestRequest request = new RestRequest("company/delete/" + companyId, Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse listAdmin(long id)
    //    {

    //        RestRequest request = new RestRequest("user/list/companyId/" + id, Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        return client.Execute(request);
    //    }

    //    public IRestResponse addAdminUser(User user)
    //    {
    //        RestRequest request = new RestRequest("user/AddAdminUser", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(user);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse updateAdminUser(User user)
    //    {
    //        RestRequest request = new RestRequest("user/updateAdminUser", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(user);
    //        return client.Execute(request);
    //    }

    //    public IRestResponse updateCompany(Company company)
    //    {
    //        RestRequest request = new RestRequest("company/update", Method.POST);
    //        request.AddHeader("Authorization", MainForm._token);
    //        request.AddJsonBody(company);
    //        return client.Execute(request);
    //    }


    //    public IRestResponse ActiveInsuranceCompany()
    //    {

    //        RestRequest request = new RestRequest("insuranceCompany/list", Method.GET);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        return client.Execute(request);
    //    }


    //    public IRestResponse AddLicenses(LicenceDto licenceDto,string token)
    //    {

    //        RestRequest request = new RestRequest("licence/activate", Method.POST);
    //        request.AddHeader("Authorization", token);
    //        request.AddJsonBody(licenceDto);
    //        return client.Execute(request);



    //    }

    //    public IRestResponse deleteLicense(long id)
    //    {
    //        RestRequest request = new RestRequest("licence/deactivate/user/" + id, Method.POST);
    //        request.AddHeader("Authorization", MainForm._token.ToString());
    //        return client.Execute(request);

    //    }

     
    }
}
