using System.Collections.Generic;

namespace SigortaVip.Models
{
    internal class fiyatBilgileri
    {
        public string TcVeyaVergiNo { get; set; }
        public string Plaka { get; set; }
        public List<Fiyat> FiyatList { get; set; }
    }
    internal class Fiyat
    {
        public string FirmaAdi { get; set; }
        public string BrutPrim { get; set; }
        public string Komisyon { get; set; }
        public string TeklifNo { get; set; }
        public string TeklifTipi { get; set; }
        public string Durum { get; set; }
        public string KBrutPrim { get; set; }
        public string KKomisyon { get; set; }
        public string KTeklifNo { get; set; }
        public string Pesin { get; set; }
    }
}
