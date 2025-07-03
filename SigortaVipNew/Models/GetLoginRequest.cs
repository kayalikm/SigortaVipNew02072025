using Newtonsoft.Json;
//using sigortaVip.Entities.Concrete;
using SigortaVip.Utility;
using System.Collections.Generic;

namespace SigortaVip.Models
{
    internal class GetLoginRequest
    {
        public HashSet<Company> companies = new HashSet<Company>();
        //public static Sirket sirket = new Sirket();

        public class _sirket
        {
            public string sirketKodu { get; set; }
            public string sirketAdi { get; set; }
        }
        public class UserSetting
        {
            public string Name { get; set; }
            public string Password { get; set; }
        }
        public static List<_sirket> _sirketGetir()
        {
            List<_sirket> _sirketler = new List<_sirket>();

            _sirketler.Add(new _sirket
            {
                sirketAdi = "tst",
                sirketKodu = "KAYALIK SİGORTA"
            });
            _sirketler.Add(new _sirket
            {
                sirketAdi = "002",
                sirketKodu = "EKTAŞ EFE SİGORTA"
            });
            _sirketler.Add(new _sirket
            {
                sirketAdi = "003",
                sirketKodu = "KUZEY SİGORTA"
            });
            _sirketler.Add(new _sirket
            {
                sirketAdi = "enkay",
                sirketKodu = "ENKAY"
            });
            _sirketler.Add(new _sirket
            {
                sirketAdi = "004",
                sirketKodu = "ADMİN DERAY"
            });

            return _sirketler;
        }

        public static void GetUserSetting()
        {
            //var resp = ApiClient.getInstance().listAdmin(sirket.id);
            //if (!resp.IsSuccessful)
            //{
            //    return;
            //}
            //var data = JsonConvert.DeserializeObject<List<User>>(resp.Content);

        }
    }
}
