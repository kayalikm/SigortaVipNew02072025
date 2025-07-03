using System;

namespace SigortaVip.Dto
{
    class CookieDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime Creation { get; set; }
        public DateTime LastAccess { get; set; }
        public int SameSite { get; set; }
        public int Priority { get; set; }
        public int InsuranceCompanyId { get; set; }
    }
}
