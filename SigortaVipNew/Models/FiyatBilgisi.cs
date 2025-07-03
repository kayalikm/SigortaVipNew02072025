
using System.Drawing;

namespace SigortaVip.Models
{
    public class FiyatBilgisi
    {
       

        public string FirmaAdi { get; set; }
        public string Durum { get; set; }
        public string BrutPrim { get; set; }
        public string Pesin { get; set; }
        public string Komisyon { get; set; }
        public string TeklifNo { get; set; }
        public string Sure { get; set; }
        public string TcKimlik { get; set; }
        public string TeklifTipi { get; set; }
        //public DataGridViewProgressColumn progressColumn { get; set; }
        public int fiyatRowIndex { get; set; }
        public Color DurumRengi { get; set; }
    }
}
