using SigortaVip.FiyatSorgulamaFactory.Concrete;
using SigortaVip.FiyatSorgulamaFactory.Interface;

using System.Collections.Generic;

namespace SigortaVip.Helpers.TrafikFiyatSorgulamaFactory
{
    public static class TrafikFiyatSorgulamaFactory
    {
        public static IFiyatSorgu CreateFiyatSorgu(string companyCode, string companyName)
        {
            // Şirket koduna göre uygun fiyat sorgulama sınıfını döndür
            if (!string.IsNullOrEmpty(companyCode))
            {
                switch (companyCode.ToUpper())
                {
                    case "KORU":
                    case "KORU_SIGORTA":
                        return new KoruSigortaFiyat();

                    case "DOGA":
                    case "DOGA_SIGORTA":
                        return new DogaSigorta2Fiyat();
                    case "QUICK":
                    case "QUICK_SIGORTA":
                        return new QuickSigortaTrafikFiyat();
                    case "RAY":
                    case "RAY_SIGORTA":
                        return new RaySigortaTrafikFiyat();
                    case "ANKARA":
                    case "ANKARA_SIGORTA":
                        return new AnkaraSigortaTrafikFiyat(); 
                    case "SEKER":
                    case "SEKER_SIGORTA":
                        return new AnkaraSigortaTrafikFiyat(); 
                    case "CORPUS":
                    case "CORPUS_SIGORTA":
                        return new CorpusSigortaTrafikFiyat(); 
                    case "BEREKET":
                    case "BEREKET_SIGORTA":
                        return new BereketSigortaTrafikFiyat();
                    case "HEPİYİ":
                    case "HEPİYİ_SIGORTA":
                        return new HepiyiSigortaTrafikFiyat(); 
                    case "ALLİANZ_SIGORTA":
                        return new AllianzSigortaTrafikFiyat();
                    case "NEOVA_SIGORTA":
                        return new NeovaSigortaTrafikFiyat(); 

                        // Yeni şirketler buraya eklenecek
                        // case "AK":
                        //     return new AkSigortaFiyat();
                }
            }

            // Şirket adına göre fallback
            return GetFiyatSorguByName(companyName);
        }

        private static IFiyatSorgu GetFiyatSorguByName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
                return null;

            string name = companyName.ToLower().Replace(" ", "");

            if (name.Contains("koru"))
                return new KoruSigortaFiyat();
            else if (name.Contains("doğa") || name.Contains("doga"))
                return new DogaSigorta2Fiyat();
            else if (name.Contains("quick") || name.Contains("quick"))
                return new QuickSigortaTrafikFiyat();
            else if (name.Contains("ray") || name.Contains("ray"))
                return new RaySigortaTrafikFiyat();
            else if (name.Contains("ankara") || name.Contains("ankara"))
                return new AnkaraSigortaTrafikFiyat();
            else if (name.Contains("şeker") || name.Contains("şeker"))
                return new SekerSigortaTrafikFiyat();
            else if (name.Contains("corpus") || name.Contains("corpus"))
                return new CorpusSigortaTrafikFiyat(); //BereketSigortaTrafikFiyat
            else if (name.Contains("bereket") || name.Contains("bereket"))
                return new BereketSigortaTrafikFiyat(); //
            else if (name.Contains("hepiyi") || name.Contains("hepiyi"))
                return new HepiyiSigortaTrafikFiyat(); //else if (name.Contains("hepiyi") || name.Contains("hepiyi"))
            else if (name.Contains("allianz") || name.Contains("allianz"))
                return new AllianzSigortaTrafikFiyat(); //
            else if (name.Contains("hdi") || name.Contains("hdi")) //SompoJapanSigortaTrafik
                return new HdiSigortaTrafikFiyat(); //
            else if (name.Contains("sompo") || name.Contains("sompo")) //SompoJapanSigortaTrafik
                return new SompoJapanSigortaTrafik(); //
            else if (name.Contains("neova") || name.Contains("neova")) //SompoJapanSigortaTrafik
                return new NeovaSigortaTrafikFiyat(); //


            // Yeni şirketler buraya eklenecek

            return null; // Desteklenmeyen şirket
        }

        // Desteklenen şirketleri kontrol etmek için
        public static bool IsSupported(string companyCode, string companyName)
        {
            return CreateFiyatSorgu(companyCode, companyName) != null;
        }

        // Desteklenen şirketlerin listesi
        public static List<string> GetSupportedCompanies()
        {
            return new List<string>
            {
                "Koru Sigorta",
                "Doğa Sigorta",
                "Quick Sigorta",
                "Ray Sigorta",
                "Ankara Sigorta",
                "Şeker Sigorta",
                "Corpus Sigorta",
                "Hepiyi Sigorta",
                "Allianz Sigorta",
                "Hdi Sigorta",
                "Sompo Japan Sigorta",
                // Yeni şirketler buraya eklenecek
            };
        }
    }
}