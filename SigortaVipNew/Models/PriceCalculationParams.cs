using System;

namespace SigortaVipNew.Models
{
    public class PriceCalculationParams
    {
        public string VehicleType { get; set; } = "Otomobil";  // Araç tipi
        public int ModelYear { get; set; } = DateTime.Now.Year;  // Model yılı
        public string MakeModel { get; set; } = "Toyota Corolla";  // Marka/Model
        public bool HasDamageRecord { get; set; } = false;  // Hasar kaydı var mı
        public string InsuranceType { get; set; } = "Trafik Sigortası";  // Sigorta tipi
        public decimal EstimatedValue { get; set; } = 300000;  // Tahmini değer
        public string LicensePlate { get; set; } = "";  // Plaka
        public string IdentityNumber { get; set; } = "";  // TC Kimlik No
    }
} 