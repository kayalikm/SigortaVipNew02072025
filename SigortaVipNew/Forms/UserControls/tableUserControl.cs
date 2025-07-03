using SigortaVip.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sigorta_Re
{
    public partial class tableUserControl : UserControl
    {
        //private ISigortaSirketService _sigortaSirketService;
        //private ITeminatBilgiService _teminatBilgiService;

        //private List<SigortaSirket> _sirketler;

        //public tableUserControl()
        //{
        //    InitializeComponent();
        //    _sigortaSirketService = InstanceFactory.GetInstance<ISigortaSirketService>();
        //    _teminatBilgiService = InstanceFactory.GetInstance<ITeminatBilgiService>();

        //    tblHeader.BackColor = SystemColors.ControlLightLight;
        //    getData();
        //    checkScroolBar();
        //}

        private void checkScroolBar()
        {
            int count = 0;
            foreach (var item in pnlRows.Controls)
            {
                if (item.GetType().Equals(typeof(tableRows)))
                    count++;
            }
            if (count > 15)
                tblHeader.Padding = new Padding(0, 0, 17, 0);
            else
                tblHeader.Padding = new Padding(0, 0, 0, 0);
        }

        private void getData()
        {
            //if (_sirketler == null)
            //    _sirketler = _sigortaSirketService.TumSirketleriGetir();

            //List<SirketDetayi> sirketler = new List<SirketDetayi>();

            //SirketDetayi sirketDetayi;
            //foreach (SigortaSirket sirket in _sirketler)
            //{
            //    sirketDetayi = new SirketDetayi
            //    {
            //        sirketId = sirket.id,
            //        sirketAdi = sirket.sirketAdi,
            //        sirketLogoUrl = sirket.sirketLogoUrl,
            //        teminatBilgileri = _teminatBilgiService.GetTeminatBilgiBySirketId(sirket.id)
            //    };
            //    sirketler.Add(sirketDetayi);
            //}

            //foreach (SirketDetayi sirketDetay in sirketler)
            //{
            //    if(sirketDetay.teminatBilgileri.Count > 0)
            //    {
            //        tableRows teminatSatir = new tableRows();
            //        teminatSatir.Dock = DockStyle.Top;
            //        teminatSatir.lblSirket.Text = sirketDetay.sirketAdi;
            //        //teminatSatir.pcbSirketLogo eklenecek
            //        foreach (TeminatBilgi teminat in sirketDetay.teminatBilgileri)
            //        {
            //            switch (teminat.teminatId)
            //            {
            //                case 1:
            //                    teminatSatir.cbxImm.Items.Add(teminat.deger);
            //                    break;
            //                case 2:
            //                    teminatSatir.cbxFerdi.Items.Add(teminat.deger);
            //                    break;
            //                case 3:
            //                    teminatSatir.cbxMeslek.Items.Add(teminat.deger);
            //                    break;
            //                case 4:
            //                    teminatSatir.cbxTazminat.Items.Add(teminat.deger);
            //                    break;
            //                case 5:
            //                    teminatSatir.cbxHukuk.Items.Add(teminat.deger);
            //                    break;
            //                case 6:
            //                    teminatSatir.cbxTedavi.Items.Add(teminat.deger);
            //                    break;
            //                case 7:
            //                    teminatSatir.cbxKullanim.Items.Add(teminat.deger);
            //                    break;
            //                case 8:
            //                    teminatSatir.cbxIkame.Items.Add(teminat.deger);
            //                    break;
            //                case 9:
            //                    teminatSatir.cbxOnarim.Items.Add(teminat.deger);
            //                    break;
            //                case 10:
            //                    teminatSatir.cbxIndirim.Items.Add(teminat.deger);
            //                    break;
            //                case 11:
            //                    teminatSatir.cbxOdeme.Items.Add(teminat.deger);
            //                    break;
            //                case 12:
            //                    teminatSatir.cbxAnahtar.Items.Add(teminat.deger);
            //                    break;
            //            }
                    //}
                    //StaticMethods.setDefault(new ComboBox[] {
                    //        teminatSatir.cbxImm, teminatSatir.cbxFerdi,
                    //        teminatSatir.cbxMeslek , teminatSatir.cbxTazminat,
                    //        teminatSatir.cbxHukuk, teminatSatir.cbxTedavi ,
                    //        teminatSatir.cbxKullanim, teminatSatir.cbxIkame,
                    //        teminatSatir.cbxOnarim, teminatSatir.cbxIndirim,
                    //        teminatSatir.cbxOdeme, teminatSatir.cbxAnahtar
                    //    });
                    //pnlRows.Controls.Add(teminatSatir);
                //}
            //}
        }

    }
}
