using System;

namespace SigortaVip.Dto
{
    class CompanyDto
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserLimit { get; set; }
        public DateTime LicenceExpiryDate { get; set; }
    }
}
