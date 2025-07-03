using System;

namespace SigortaVip.Models
{
    class Company
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public int UserLimit { get; set; }
        public DateTime LicenceExpiryDate { get; set; }
    }
}
