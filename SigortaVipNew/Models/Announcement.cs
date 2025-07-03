using System;

namespace SigortaVip.Models
{
    class Announcement
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }
        public long CompanyId { get; set; }
    }
}
