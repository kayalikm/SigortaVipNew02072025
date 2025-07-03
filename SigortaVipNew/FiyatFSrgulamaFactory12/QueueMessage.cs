
namespace SigortaVip.FiyatSorgulamaFactory
{
    public class QueueMessage
    {
        public string InsuranceCompany { get; set; }
        public string SorguTipi { get; set; }
        public int RowIndex { get; set; }

        // Yeni özellik
        public string UniqueKey => $"{InsuranceCompany?.Trim()}_{SorguTipi}";
    }
}
