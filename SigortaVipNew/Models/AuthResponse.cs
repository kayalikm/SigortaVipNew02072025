using System.Collections.Generic;

namespace SigortaVipNew.Models
{
    public class AuthResponse
    {
        public string token { get; set; }
        public int user_id { get; set; }  // int çünkü JSON'da sayı
        public string username { get; set; }
        public string email { get; set; }
        public CompanyDto company { get; set; }
        public bool is_admin { get; set; }
        public List<string> roles { get; set; }
    }

    public class CompanyDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
    }
}
