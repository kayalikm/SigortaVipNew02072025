using System;

namespace SigortaVip.Models
{
    public class AracSorguInfo
    {
        public string CompanyName { get; set; }
        public string CompanyUrl { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public DateTime LastAccessTime { get; set; }
        public bool IsFavorite { get; set; }
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}