using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigortaVip.Dto
{
    public class QueryRequestDto
    {
        public string query_type { get; set; }
        public int insurance_company_item_id { get; set; }
        public string identity_number { get; set; }
        public string birth_date { get; set; }
        public string plate_number { get; set; }
        public string document_serial { get; set; }
    }
} 