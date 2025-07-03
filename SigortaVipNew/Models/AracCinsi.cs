using System.Collections.Generic;

namespace SigortaVip.Models
{
    internal class AracCinsi
    {
        public class AracCins
        {
            public string Number { get; set; }
            public string Cins { get; set; }
        }
        public List<AracCins> AracCinsGetir()
        {
            List<AracCins> Araclist = new List<AracCins>();
            Araclist.Add(new AracCins { 
              Number = "01",
              Cins = "OTOMOBİL"
            });
            Araclist.Add(new AracCins
            {
                Number = "02",
                Cins = "TAKSİ"
            });
            Araclist.Add(new AracCins
            {
                Number = "03",
                Cins = "MİNİBÜS(SÜRÜCÜ DAHİL 10-17 KOLTUK)"
            });
            Araclist.Add(new AracCins
            {
                Number = "04",
                Cins = "OTOBÜS(SÜRÜCÜ DAHİL 18-30 KOLTUK)"
            });
            Araclist.Add(new AracCins
            {
                Number = "05",
                Cins = "OTOBÜS(SÜRÜCÜ DAHİL 31 VE ÜSTÜ KOLTUK)"
            });
            Araclist.Add(new AracCins
            {
                Number = "06",
                Cins = "KAMYONET"
            });
            Araclist.Add(new AracCins
            {
                Number = "07",
                Cins = "KAMYON"
            });
            Araclist.Add(new AracCins
            {
                Number = "08",
                Cins = "İŞ MAKİNESİ"
            });
            Araclist.Add(new AracCins
            {
                Number = "09",
                Cins = "TRAKTÖR"
            });
            Araclist.Add(new AracCins
            {
                Number = "10",
                Cins = "RÖMORK"
            });
            Araclist.Add(new AracCins
            {
                Number = "11",
                Cins = "MOTOSİKLET"
            });
            Araclist.Add(new AracCins
            {
                Number = "12",
                Cins = "TANKER"
            });
            Araclist.Add(new AracCins
            {
                Number = "13",
                Cins = "ÇEKİCİ"
            });
            Araclist.Add(new AracCins
            {
                Number = "14",
                Cins = "ÖZEL AMAÇLI TAŞIT"
            });
            Araclist.Add(new AracCins
            {
                Number = "15",
                Cins = "TARIM MAKİNESİ"
            });
            Araclist.Add(new AracCins
            {
                Number = "16",
                Cins = "DOLMUŞ (SÜRÜCÜ DAHİL 5-8 KOLTUK)"
            });
            Araclist.Add(new AracCins
            {
                Number = "17",
                Cins = "DOLMUŞ/MİNİBÜS (SÜRÜCÜ DAHİL 9-15 KOLTUK)"
            });
            Araclist.Add(new AracCins
            {
                Number = "18",
                Cins = "DOLMUŞ/OTOBÜS (SÜRÜCÜ DAHİL 16-30 KOLTUK)"
            });
            Araclist.Add(new AracCins
            {
                Number = "19",
                Cins = "DOLMUŞ/OTOBÜS (SÜRÜCÜ DAHİL 31 VE ÜSTÜ KOLTUK)"
            });

            return Araclist;
        }
        public AracCins AracCinsGetirSayiyaGore(string number)
        {
            List<AracCins> Araclist = new List<AracCins>();
            Araclist = AracCinsGetir();

            foreach (var arac in Araclist)
            {
                if (arac.Number == number)
                {
                    return arac;
                }
            }
            return new AracCins();
        }
        public AracCins AracCinsGetirIsmeGore(string isim)
        {
            List<AracCins> Araclist = new List<AracCins>();
            Araclist = AracCinsGetir();

            foreach (var arac in Araclist)
            {
                if (arac.Cins == isim)
                {
                    return arac;
                }
            }
            return new AracCins();
        }
    }
}
