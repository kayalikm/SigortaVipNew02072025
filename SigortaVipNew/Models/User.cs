using System;

namespace SigortaVip.Models
{
    class User
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public Boolean IsAdmin { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
