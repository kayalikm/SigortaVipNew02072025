namespace SigortaVip.Forms
{
    internal class QueryRequestDto
    {
        public string query_type { get; set; }
        public int insurance_company_item_id { get; set; }
        public string identity_number { get; set; }
        public string birth_date { get; set; }
        public string plate_number { get; set; }
        public string document_serial { get; set; }
    }
}