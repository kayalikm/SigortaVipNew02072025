using System.Collections.Generic;

namespace SigortaVip.Models
{
    internal class QuerryItems
    {
        public string adSoyad { get; set; }
        public List<string> sessionId { get; set; }
        public string tcKimlik { get; set; }
        public string vergiNo { get; set; }
        public string Plaka { get; set; }
        public string plakaKodu { get; set; }
        public string belgeNo { get; set; }
        public string belgeKodu { get; set; }
        public string dogumTarihi { get; set; }
        public string sasiNo { get; set; }
        public string aracKodu { get; set; }
        public string FirmaAdi { get; set; }
        public string AracTarz { get; set; }
        public string TescilTarihi { get; set; }
        public string MotorNo { get; set; }
        public string KullanimTarzi { get; set; }
        public string Model { get; set; }
        public string Marka { get; set; }

        public FirmaSessionBilgileri firmaSessionBilgileri { get; set; }
        public OncekiPolice OncekiPolice { get; set; }

    }
}
