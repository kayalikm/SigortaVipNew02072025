namespace SigortaVip.Models
{
    public class InsuranceCompany
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ExplorerUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ProxyUrl { get; set; }
        public string ProxyUserName { get; set; }
        public string ProxyPassword { get; set; }
        public bool IsProxyUse { get; set; }
        public long CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string Phone { get; set; }
    }
}
