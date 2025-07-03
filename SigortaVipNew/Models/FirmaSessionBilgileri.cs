using System.Collections.Generic;

namespace SigortaVip.Models
{
    internal class FirmaSessionBilgileri
    {
        public List<Session> SessionList { get; set; }
        public class Session
        {
            public string SessionName { get; set; }
            public string SessionValue { get; set; }
        }
    }
    
}
