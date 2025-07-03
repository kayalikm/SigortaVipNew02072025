using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory.Concrete;
using SigortaVip.FiyatSorgulamaFactory.Interface;

using System;

namespace SigortaVip.FiyatSorgulamaFactory
{
    public class ConcreteFiyatSorgulaFactory : FiyatSorgulaFactory
    {
        public override IFiyatSorgu GetFiyatSorgu(string insuranceCompany)
        {
            switch (insuranceCompany)
            {
                // Şeker Sigorta
                
                case "SekerSigortaTrafikFiyat":
                case "Şeker Sigorta":
                    return new SekerSigortaTrafikFiyat();

                // Allianz Sigorta
                case "AllianzSigortaTrafikFiyat":
                case "Allianz Sigorta":
                    return new AllianzSigortaTrafikFiyat();
                
                // Bereket Sigorta
                case "BereketSigortaTrafikFiyat":
                case "Bereket Sigorta":
                    return new BereketSigortaTrafikFiyat();
                
                // Corpus Sigorta
                case "CorpusSigortaTrafikFiyat":
                case "Corpus Sigorta":
                    return new CorpusSigortaTrafikFiyat();
                
                // Doğa Sigorta
                case "DogaSigorta2Fiyat":
                case "Doğa Sigorta":
                    return new DogaSigorta2Fiyat();
                
                // HDI Sigorta
                case "HdiSigortaTrafikFiyat":
                case "HDI Sigorta":
                case "Hdi Sigorta":
                    return new HdiSigortaTrafikFiyat();
                
                // Hepıyı Sigorta
                case "HepiyiSigortaTrafikFiyat":
                case "Hepıyı Sigorta":
                case "HepIyiSigorta":
                    return new HepiyiSigortaTrafikFiyat();
                
                // Ankara Sigorta
                case "AnkaraSigortaTrafikFiyat":
                case "Ankara Sigorta":
                    return new AnkaraSigortaTrafikFiyat();
                
                // Sompo Japan Sigorta
                case "SompoJapanSigortaTrafik":
                case "SompoSigortaTrafikFiyat":
                case "SompoJapanSigortaTrafikFiyat":
                case "Sompo Sigorta":
                    return new SompoJapanSigortaTrafik();
                
                // Ray Sigorta
                case "RaySigortaTrafikFiyat":
                case "Ray Sigorta":
                    return new RaySigortaTrafikFiyat();
                
                // Quick Sigorta
                case "QuickSigortaTrafikFiyat":
                case "Quick Sİgorta":
                    return new QuickSigortaTrafikFiyat();
                
                // Koru Sigorta
                case "KoruSigortaFiyat":
                case "Koru Sigorta":
                    return new KoruSigortaFiyat();
                case "NeovaSigortaFiyat":
                case "Neova Sigorta":
                    return new NeovaSigortaTrafikFiyat();


                default:
                    throw new ApplicationException(string.Format("FiyatSorgu '{0}' şirketi için oluşturulamadı.", insuranceCompany));
            }
        }
    }
}
