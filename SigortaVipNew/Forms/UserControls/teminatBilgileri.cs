using SigortaVip.Constant;
using SigortaVip.Models;
using SigortaVip.Models.Teminatlar;
using SigortaVip.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sigorta_Re
{
    public partial class teminatBilgileri : UserControl
    {
        List<ComboBox> comboBoxes;
        string pattern = "^[0-9]+$"; //Sadece sayıları içeren Regex pattern'i
        Array veriler; //Ekrandaki tableRows UserControl'lerini içeren dizi
        long sayisalDeger, sayisalComboboxDeger; //Comboboxlar'da seçilen string değerlerini sayısal değerlere çevirip tutan değişkenler

        //private ITeminatSablonService _teminatSablonService;
        //private List<TeminatSablon> _kullaniciSablonlari;

        public teminatBilgileri()
        {
            InitializeComponent();

            //_teminatSablonService = InstanceFactory.GetInstance<ITeminatSablonService>();

            sorgu();

            
        }

        #region ayar metodları start
        private void odemeAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxOdeme.Items.Count; i++)
                    {
                        if (deger != "Peşin" && deger != "Maksimum Taksit")
                        {
                            bool a = Regex.IsMatch(deger.Split(' ')[0], pattern);
                            bool b = Regex.IsMatch(item.cbxOdeme.GetItemText(item.cbxOdeme.Items[i]).Split(' ')[0], pattern);

                            if (a && b)
                            {
                                sayisalDeger = Convert.ToInt64(deger.Split(' ')[0]);
                                sayisalComboboxDeger = Convert.ToInt64(item.cbxOdeme.GetItemText(item.cbxOdeme.Items[i]).Split(' ')[0]);
                                if (sayisalDeger == sayisalComboboxDeger)
                                {
                                    item.cbxOdeme.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (deger)
                            {
                                case "Peşin":
                                    item.cbxOdeme.SelectedIndex = 0;
                                    break;
                                case "Maksimum Taksit":
                                    item.cbxOdeme.SelectedIndex = item.cbxOdeme.Items.Count - 1;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private void hukuksalKorumaAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxHukuk.Items.Count; i++)
                    {
                        if (deger != "Minimum" && deger != "Maksimum")
                        {
                            bool a = Regex.IsMatch(deger.Split('-')[0].Replace(".", string.Empty), pattern);
                            bool b = Regex.IsMatch(item.cbxHukuk.GetItemText(item.cbxHukuk.Items[i]).Split('-')[0].Replace(".", string.Empty), pattern);

                            if (a && b)
                            {
                                sayisalDeger = Convert.ToInt64(deger.Split('-')[0].Replace(".", string.Empty));
                                sayisalComboboxDeger = Convert.ToInt64(item.cbxHukuk.GetItemText(item.cbxHukuk.Items[i]).Split('-')[0].Replace(".", string.Empty));
                                if (sayisalDeger == sayisalComboboxDeger)
                                {
                                    item.cbxHukuk.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (deger)
                            {
                                case "Minimum":
                                    item.cbxHukuk.SelectedIndex = 0;
                                    break;
                                case "Maksimum":
                                    item.cbxHukuk.SelectedIndex = item.cbxHukuk.Items.Count - 1;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private void tedaviMasraflariAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxTedavi.Items.Count; i++)
                    {
                        if (deger == "Yok")
                        {
                            item.cbxTedavi.SelectedIndex = 0;
                            break;
                        }
                        else if (deger.ToLower() == item.cbxTedavi.GetItemText(item.cbxTedavi.Items[i]).ToLower())
                        {
                            item.cbxTedavi.SelectedIndex = i;
                            break;
                        }
                        else
                        {
                            bool a = Regex.IsMatch(deger.Split('-')[0].Replace(".", string.Empty), pattern);
                            bool b = Regex.IsMatch(item.cbxTedavi.GetItemText(item.cbxTedavi.Items[i]).Split('-')[0].Replace(".", string.Empty), pattern);
                            if (a && b)
                            {
                                item.cbxTedavi.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void indirimAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxIndirim.Items.Count; i++)
                    {
                        if (deger.ToLower() == item.cbxIndirim.GetItemText(item.cbxIndirim.Items[i]).ToLower())
                        {
                            item.cbxIndirim.SelectedIndex = i;
                            break;
                        }
                        else if (deger == "Yok")
                        {
                            item.cbxIndirim.SelectedIndex = 0;
                            break;
                        }
                    }
                }
            }
        }
        private void kullanimTarziAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxKullanim.Items.Count; i++)
                    {
                        if (deger.ToLower() == item.cbxKullanim.GetItemText(item.cbxKullanim.Items[i]).ToLower())
                        {
                            item.cbxKullanim.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }
        private void onarimSekliAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxOnarim.Items.Count; i++)
                    {
                        if (deger.ToLower() == item.cbxOnarim.GetItemText(item.cbxOnarim.Items[i]).ToLower())
                        {
                            item.cbxOnarim.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }
        private void maneviTazminatAyarla()
        {
            if (veriler.Length > 0)
            {
                if (chbManevi.Checked)
                {
                    foreach (tableRows item in veriler)
                    {
                        for (int i = 0; i < item.cbxTazminat.Items.Count; i++)
                        {
                            if (item.cbxTazminat.GetItemText(item.cbxTazminat.Items[i]) == "Var")
                            {
                                item.cbxTazminat.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (tableRows item in veriler)
                    {
                        for (int i = 0; i < item.cbxTazminat.Items.Count; i++)
                        {
                            item.cbxTazminat.SelectedIndex = 0;
                            break;
                        }
                    }
                }
            }
        }
        private void anahtarTeminatAyarla()
        {
            if (veriler.Length > 0)
            {
                if (chbAnahtar.Checked)
                {
                    foreach (tableRows item in veriler)
                    {
                        for (int i = 0; i < item.cbxAnahtar.Items.Count; i++)
                        {
                            if (item.cbxAnahtar.GetItemText(item.cbxAnahtar.Items[i]) == "Var")
                            {
                                item.cbxAnahtar.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (tableRows item in veriler)
                    {
                        for (int i = 0; i < item.cbxAnahtar.Items.Count; i++)
                        {
                            item.cbxAnahtar.SelectedIndex = 0;
                            break;
                        }
                    }
                }
            }
        }
        private void ikameAracAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxIkame.Items.Count; i++)
                    {
                        if (deger == item.cbxIkame.GetItemText(item.cbxIkame.Items[i]))
                        {
                            item.cbxIkame.SelectedIndex = i;
                            break;
                        }
                        else if (deger == "Yok")
                        {
                            item.cbxIkame.SelectedIndex = 0;
                            break;
                        }
                    }
                }
            }
        }
        private void meslekAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxMeslek.Items.Count; i++)
                    {
                        if (deger.ToLower() == item.cbxMeslek.GetItemText(item.cbxMeslek.Items[i]).ToLower())
                        {
                            item.cbxMeslek.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }
        private void ferdiAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxFerdi.Items.Count; i++)
                    {
                        if (deger != "Minimum" && deger != "Maksimum")
                        {
                            bool a = Regex.IsMatch(deger.Split('-')[0].Replace(".", string.Empty), pattern);
                            bool b = Regex.IsMatch(item.cbxFerdi.GetItemText(item.cbxFerdi.Items[i]).Split('-')[0].Replace(".", string.Empty), pattern);

                            if (a && b)
                            {
                                sayisalDeger = Convert.ToInt64(deger.Split('-')[0].Replace(".", string.Empty));
                                sayisalComboboxDeger = Convert.ToInt64(item.cbxFerdi.GetItemText(item.cbxFerdi.Items[i]).Split('-')[0].Replace(".", string.Empty));
                                if (sayisalDeger == sayisalComboboxDeger)
                                {
                                    item.cbxFerdi.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (deger)
                            {
                                case "Minimum":
                                    item.cbxFerdi.SelectedIndex = 0;
                                    break;
                                case "Maksimum":
                                    item.cbxFerdi.SelectedIndex = item.cbxFerdi.Items.Count - 1;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private void immAyarla(string deger)
        {
            if (veriler.Length > 0)
            {
                foreach (tableRows item in veriler)
                {
                    for (int i = 0; i < item.cbxImm.Items.Count; i++)
                    {
                        if (deger != "Minimum" && deger != "Maksimum")
                        {
                            bool a = Regex.IsMatch(deger.Split('-')[0].Replace(".", string.Empty), pattern);
                            bool b = Regex.IsMatch(item.cbxImm.GetItemText(item.cbxImm.Items[i]).Split('-')[0].Replace(".", string.Empty), pattern);

                            if (a && b)
                            {
                                sayisalDeger = Convert.ToInt64(deger.Split('-')[0].Replace(".", string.Empty));
                                sayisalComboboxDeger = Convert.ToInt64(item.cbxImm.GetItemText(item.cbxImm.Items[i]).Split('-')[0].Replace(".", string.Empty));
                                if (sayisalDeger == sayisalComboboxDeger)
                                {
                                    item.cbxImm.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            switch (deger)
                            {
                                case "Minimum":
                                    item.cbxImm.SelectedIndex = 0;
                                    break;
                                case "Maksimum":
                                    item.cbxImm.SelectedIndex = item.cbxImm.Items.Count - 1;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region sablon metodları start
        private void sablonSil()
        {
            //if (_kullaniciSablonlari != null)
            //{
            ////    if (_kullaniciSablonlari.Count > 0)
            ////    {
            ////        if (cbxSablonAdi.SelectedItem != null)
            ////        { //TeminatSablon seciliSablon = _teminatSablonService.GetSablonByName(cbxSablonAdi.GetItemText(cbxSablonAdi.SelectedItem), Convert.ToInt32(AktifKullaniciBilgileri.AktifKullanici.id));
            ////            //_teminatSablonService.SablonSil(seciliSablon);
            ////            //if (sablonVarmi())
            ////            //    //sablonMenusuGetir();
            ////            else
            ////                cbxSablonAdi.Items.Clear();
            ////            MessageBox.Show("Şablon silindi", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ////            ParentForm.Refresh();
            ////        }
            ////        else
            ////            MessageBox.Show("Lütfen silinecek şablonu seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////    }
            ////    else
            ////        MessageBox.Show("Silinecek Şablon Yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////}
        }
        private bool sablonAdiKontrol() //Aynı isimde şablon kaydetmemek için
        {
            foreach (var item in cbxSablonAdi.Items)
            {
                if (cbxSablonAdi.GetItemText(item) == cbxSablonAdi.Text)
                    return false;
            }
            return true;
        }
        //private void sablonuKaydet()
        //{
        //    if (sablonAdiKontrol())
        //    {
        //        if (cbxSablonAdi.Text != string.Empty)
        //        {
        //            TeminatSablon yeniSablon = new TeminatSablon
        //            {
        //                kullaniciId = Convert.ToInt32(AktifKullaniciBilgileri.AktifKullanici.id),
        //                sablonAdi = cbxSablonAdi.Text,
        //                immIndex = cfgcbxImm.SelectedIndex,
        //                ferdiIndex = cfgcbxFerdi.SelectedIndex,
        //                meslekIndex = cfgcbxMeslek.SelectedIndex,
        //                ikameIndex = cfgcbxİkame.SelectedIndex,
        //                onarimIndex = cfgcbxOnarim.SelectedIndex,
        //                kullanimIndex = cfgcbxKullanim.SelectedIndex,
        //                indirimIndex = cfgcbxIndirim.SelectedIndex,
        //                lpgIndex = cfgcbxLpg.SelectedIndex,
        //                tedaviIndex = cfgcbxTedavi.SelectedIndex,
        //                hukuksalIndex = cfgcbxHukuk.SelectedIndex,
        //                kasaIndex = cfgcbxKasa.SelectedIndex,
        //                aksesuarIndex = cfgcbxAksesuar.SelectedIndex,
        //                yakitIndex = cfgcbxYakit.SelectedIndex,
        //                odemeIndex = cfgcbxOdeme.SelectedIndex,
        //                depremIndex = chbDeprem.Checked,
        //                selIndex = chbSel.Checked,
        //                camIndex = chbCam.Checked,
        //                maneviIndex = chbManevi.Checked,
        //                anahtarIndex = chbAnahtar.Checked,
        //                engelliIndex = chbEngelli.Checked,
        //                filoIndex = chbFilo.Checked,
        //                markaIndex = chbMarka.Checked
        //            };
        //            _teminatSablonService.SablonEkle(yeniSablon);
        //            if (sablonVarmi())
        //                sablonMenusuGetir();
        //            MessageBox.Show("Şablon kaydedildi", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            ParentForm.Refresh();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Lütfen Şablon adı girin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Aynı isimde şablon zaten mevcut!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}

        private void sablonMenusuGetir()
        {
            //cbxSablonAdi.Items.Clear();
            //foreach (TeminatSablon sablon in _kullaniciSablonlari)
            //{
            //    cbxSablonAdi.Items.Add(sablon.sablonAdi);
            //}
        }

        private void sablonGetir(string sablonAdi = null)
        {
            //if (sablonAdi != string.Empty && sablonAdi != null)
            //{
            //    if (_kullaniciSablonlari.Count > 0 && cbxSablonAdi.SelectedItem != null)
            //    {
            //        foreach (TeminatSablon sablon in _kullaniciSablonlari)
            //        {
            //            if (sablon.sablonAdi == sablonAdi)
            //            {
            //                cfgcbxImm.SelectedIndex = sablon.immIndex;
            //                cfgcbxFerdi.SelectedIndex = sablon.ferdiIndex;
            //                cfgcbxMeslek.SelectedIndex = sablon.meslekIndex;
            //                cfgcbxİkame.SelectedIndex = sablon.ikameIndex;
            //                cfgcbxOnarim.SelectedIndex = sablon.onarimIndex;
            //                cfgcbxKullanim.SelectedIndex = sablon.kullanimIndex;
            //                cfgcbxIndirim.SelectedIndex = sablon.indirimIndex;
            //                cfgcbxLpg.SelectedIndex = sablon.lpgIndex;
            //                cfgcbxTedavi.SelectedIndex = sablon.tedaviIndex;
            //                cfgcbxHukuk.SelectedIndex = sablon.hukuksalIndex;
            //                cfgcbxKasa.SelectedIndex = sablon.kasaIndex;
            //                cfgcbxAksesuar.SelectedIndex = sablon.aksesuarIndex;
            //                cfgcbxYakit.SelectedIndex = sablon.yakitIndex;
            //                cfgcbxOdeme.SelectedIndex = sablon.odemeIndex;
            //                chbDeprem.Checked = sablon.depremIndex;
            //                chbSel.Checked = sablon.selIndex;
            //                chbCam.Checked = sablon.camIndex;
            //                chbManevi.Checked = sablon.maneviIndex;
            //                chbAnahtar.Checked = sablon.anahtarIndex;
            //                chbEngelli.Checked = sablon.engelliIndex;
            //                chbFilo.Checked = sablon.filoIndex;
            //                chbMarka.Checked = sablon.markaIndex;
            //                break;
            //            }
            //        }
            //    }
            //}
            //else
            //    MessageBox.Show("Lütfen geçerli bir Şablon seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //private bool sablonVarmi()
        //{
        //    //_kullaniciSablonlari = _teminatSablonService.GetSablonByUserId(Convert.ToInt32(AktifKullaniciBilgileri.AktifKullanici.id));
        //    //if (_kullaniciSablonlari.Count > 0)
        //    //    return true;
        //    //else
        //    //    return false;
        //}

        private void defaultSablonGetir()
        {
            foreach (ComboBox comboBox in comboBoxes)
            {
                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;
            }
            foreach (CheckBox item in pnlConfig3.Controls.OfType<CheckBox>())
            {
                if (item.Checked)
                    item.Checked = false;
            }
        }

        #endregion
        private void sorguIcinKaydet()
        {
            Panel tablePanel = pnlTable.Controls.Find("pnlRows", true).OfType<Panel>().FirstOrDefault();
            foreach (tableRows row in tablePanel.Controls)
            {
                switch (row.Controls[0].Controls["lblSirket"].Text)
                {
                    case InsuranceConstants.AkSigorta:
                        AkSigortaTeminat.TeminatVarMi = true;
                        AkSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.ManeviTazminat = ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        AkSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        AkSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        AkSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        AkSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        continue;
                    case InsuranceConstants.AllianzSigorta:
                        AllianzTeminat.TeminatVarMi = true;
                        AllianzTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        AllianzTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        AllianzTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        AllianzTeminat.ManeviTazminat = ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem.ToString() : "";
                        AllianzTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        AllianzTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        AllianzTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        AllianzTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        AllianzTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        AllianzTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        AllianzTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        AllianzTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        AllianzTeminat.SelMuafiyeti = chbSel.Checked;
                        AllianzTeminat.CamMuafiyeti = chbCam.Checked;
                        AllianzTeminat.EngelliAraci = chbEngelli.Checked;
                        continue;
                    case InsuranceConstants.AnkaraSigorta:
                        AnkaraSigortaTeminat.TeminatVarMi = true;
                        AnkaraSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.ManeviTazminat = chbManevi.Checked;
                        AnkaraSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        AnkaraSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        AnkaraSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        AnkaraSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        AnkaraSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        continue;
                    case InsuranceConstants.AcnTurkSigorta:
                        AcnTurkSigortaTeminat.TeminatVarMi = true;
                        AcnTurkSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        AcnTurkSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        AcnTurkSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        AcnTurkSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        AcnTurkSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        AcnTurkSigortaTeminat.KasaBedeli = ((ComboBox)pnlConfig2.Controls["cfgcbxKasa"]).SelectedItem.ToString();
                        continue;
                    case InsuranceConstants.AveonSigorta:
                        AveonSigortaTeminat.TeminatVarMi = true;
                        AveonSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        AveonSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        AveonSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        AveonSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        AveonSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        AveonSigortaTeminat.KasaBedeli = ((ComboBox)pnlConfig2.Controls["cfgcbxKasa"]).SelectedItem.ToString();
                        continue;
                    case InsuranceConstants.CorpusSigorta:
                        CorpusTeminat.TeminatVarMi = true;
                        CorpusTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        CorpusTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        CorpusTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        CorpusTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        CorpusTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        CorpusTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        CorpusTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        CorpusTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        CorpusTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        CorpusTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        CorpusTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        CorpusTeminat.SelMuafiyeti = chbSel.Checked;
                        CorpusTeminat.CamMuafiyeti = chbCam.Checked;
                        CorpusTeminat.EngelliAraci = chbEngelli.Checked;
                        CorpusTeminat.LpgDegeri = ((ComboBox)pnlConfig2.Controls["cfgcbxLpg"]).SelectedItem.ToString();

                        continue;
                    case InsuranceConstants.KoruSigorta:
                        KoruSigortaTeminat.TeminatVarMi = true;
                        KoruSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        KoruSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        KoruSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        KoruSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        KoruSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        KoruSigortaTeminat.KasaBedeli = ((ComboBox)pnlConfig2.Controls["cfgcbxKasa"]).SelectedItem.ToString();

                        continue;
                    case InsuranceConstants.DogaSigorta:
                        DogaSigortaTeminat.TeminatVarMi = true;
                        DogaSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        DogaSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        DogaSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        DogaSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        DogaSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        DogaSigortaTeminat.KasaBedeli = ((ComboBox)pnlConfig2.Controls["cfgcbxKasa"]).SelectedItem.ToString();

                        continue;

                    case InsuranceConstants.GriSigorta:
                        GriSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.TeminatVarMi = true;
                        GriSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        GriSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        GriSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        GriSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        GriSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        GriSigortaTeminat.KasaBedeli = ((ComboBox)pnlConfig2.Controls["cfgcbxKasa"]).SelectedItem.ToString();
                        GriSigortaTeminat.AnahtarTeminati = chbAnahtar.Checked;

                        continue;
                    case InsuranceConstants.BereketSigorta:
                        BereketSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.TeminatVarMi = true;
                        BereketSigortaTeminat.ManeviTazminat = ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        BereketSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        BereketSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        BereketSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        BereketSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        BereketSigortaTeminat.ManeviTazminatVarMi = chbManevi.Checked;
                        BereketSigortaTeminat.AnahtarKaybiVarMi = chbAnahtar.Checked;
                        BereketSigortaTeminat.LpgDegeri = ((ComboBox)pnlConfig2.Controls["cfgcbxLpg"]).SelectedItem.ToString();



                        continue;
                    case InsuranceConstants.AnaSigorta:
                        AnaSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.TeminatVarMi = true;
                        AnaSigortaTeminat.ManeviTazminat = ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        AnaSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        AnaSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        AnaSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        AnaSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        AnaSigortaTeminat.LpgDegeri = ((ComboBox)pnlConfig2.Controls["cfgcbxLpg"]).SelectedItem.ToString();
                        AnaSigortaTeminat.AnahtarTeminatiVarMi = chbAnahtar.Checked;

                        continue;
                    case InsuranceConstants.HdiSigorta:
                        HdiSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.TeminatVarMi = true;
                        HdiSigortaTeminat.ManeviTazminat = ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        HdiSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        HdiSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        HdiSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        HdiSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        HdiSigortaTeminat.LpgDegeri = ((ComboBox)pnlConfig2.Controls["cfgcbxLpg"]).SelectedItem.ToString();

                        continue;
                    case InsuranceConstants.AnadoluSigorta:
                        AnadoluSigortaTeminat.Imm = ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxImm"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.TeminatVarMi = true;
                        AnadoluSigortaTeminat.ManeviTazminat = ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTazminat"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.FerdiKaza = ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxFerdi"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.MeslekIndirimi = ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxMeslek"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.HukuksalKoruma = ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxHukuk"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.TedaviMasraflari = ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxTedavi"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.IkameArac = ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIkame"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.Onarim = ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOnarim"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.Indirim = ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxIndirim"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.OdemeSekli = ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxOdeme"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.AnahtarKaybi = ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem != null ? ((ComboBox)row.Controls[0].Controls["cbxAnahtar"]).SelectedItem.ToString() : "";
                        AnadoluSigortaTeminat.DepremMuafiyeti = chbDeprem.Checked;
                        AnadoluSigortaTeminat.SelMuafiyeti = chbSel.Checked;
                        AnadoluSigortaTeminat.CamMuafiyeti = chbCam.Checked;
                        AnadoluSigortaTeminat.EngelliAraci = chbEngelli.Checked;
                        AnadoluSigortaTeminat.LpgDegeri = ((ComboBox)pnlConfig2.Controls["cfgcbxLpg"]).SelectedItem.ToString();

                        continue;

                }
            }
            MessageBox.Show("Teminat Bilgileri Kaydedildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void sorgu()
        {
            tableUserControl tablo = new tableUserControl();
            tablo.Dock = DockStyle.Fill;
            pnlTable.Controls.Clear();
            pnlTable.Controls.Add(tablo);

            veriler = this.Controls.Find("tableRows", true);

            comboBoxes = pnlConfig.Controls.OfType<ComboBox>().ToList();
            comboBoxes.AddRange(pnlConfig2.Controls.OfType<ComboBox>());
            comboBoxes.AddRange(pnlConfig4.Controls.OfType<ComboBox>());
            StaticMethods.setDefault(comboBoxes.ToArray());

            //if (sablonVarmi())
            //    sablonMenusuGetir();

            //defaultSablonGetir();
        }

        #region events start

        private void cfgcbxFerdi_SelectedIndexChanged(object sender, EventArgs e)
        {
            ferdiAyarla(cfgcbxFerdi.GetItemText(cfgcbxFerdi.SelectedItem));
        }

        private void cfgcbxMeslek_SelectedIndexChanged(object sender, EventArgs e)
        {
            meslekAyarla(cfgcbxMeslek.GetItemText(cfgcbxMeslek.SelectedItem));
        }

        private void cfgcbxİkame_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ikameAracAyarla(cfgcbxİkame.GetItemText(cfgcbxİkame.SelectedItem));
        }

        private void btnSablonSec_Click(object sender, EventArgs e)
        {
            //sablonGetir(cbxSablonAdi.GetItemText(cbxSablonAdi.SelectedItem));
        }

        private void btnSablonuKaydet_Click(object sender, EventArgs e)
        {
            //sablonuKaydet();
        }

        private void btnSablonSil_Click(object sender, EventArgs e)
        {
            //sablonSil();
        }

        private void chbManevi_CheckedChanged(object sender, EventArgs e)
        {
            //maneviTazminatAyarla();
        }

        private void cfgcbxOnarim_SelectedIndexChanged(object sender, EventArgs e)
        {
            onarimSekliAyarla(cfgcbxOnarim.GetItemText(cfgcbxOnarim.SelectedItem));
        }

        private void cfgcbxKullanim_SelectedIndexChanged(object sender, EventArgs e)
        {
            kullanimTarziAyarla(cfgcbxKullanim.GetItemText(cfgcbxKullanim.SelectedItem));
        }

        private void cfgcbxIndirim_SelectedIndexChanged(object sender, EventArgs e)
        {
            indirimAyarla(cfgcbxIndirim.GetItemText(cfgcbxIndirim.SelectedItem));
        }

        private void cfgcbxTedavi_SelectedIndexChanged(object sender, EventArgs e)
        {
            tedaviMasraflariAyarla(cfgcbxTedavi.GetItemText(cfgcbxTedavi.SelectedItem));
        }

        private void cfgcbxHukuk_SelectedIndexChanged(object sender, EventArgs e)
        {
            hukuksalKorumaAyarla(cfgcbxHukuk.GetItemText(cfgcbxHukuk.SelectedItem));
        }

        private void cfgcbxOdeme_SelectedIndexChanged(object sender, EventArgs e)
        {
            odemeAyarla(cfgcbxOdeme.GetItemText(cfgcbxOdeme.SelectedItem));
        }

        private void chbAnahtar_CheckedChanged(object sender, EventArgs e)
        {
            anahtarTeminatAyarla();
        }

        private void btnSorgu_Click(object sender, EventArgs e)
        {
            sorguIcinKaydet();
            ParentForm.Close();
        }

        private void btnSifirla_Click(object sender, EventArgs e)
        {
            defaultSablonGetir();
            AkSigortaTeminat.TeminatVarMi = false;
            AllianzTeminat.TeminatVarMi = false;
            AnkaraSigortaTeminat.TeminatVarMi = false;
            AcnTurkSigortaTeminat.TeminatVarMi = false;
            CorpusTeminat.TeminatVarMi = false;
            KoruSigortaTeminat.TeminatVarMi = false;
            DogaSigortaTeminat.TeminatVarMi = false;
            GriSigortaTeminat.TeminatVarMi = false;
            BereketSigortaTeminat.TeminatVarMi = false;
            AnaSigortaTeminat.TeminatVarMi = false;
            HdiSigortaTeminat.TeminatVarMi = false;
            AnaSigortaTeminat.TeminatVarMi = false;
            MessageBox.Show("Teminatlar Sıfırlandı", "Teminatlar Ayarları", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ParentForm.Close();
        }

        private void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            sorgu();
        }

        private void cfgcbxImm_SelectedIndexChanged(object sender, EventArgs e)
        {
            immAyarla(cfgcbxImm.GetItemText(cfgcbxImm.SelectedItem));
        }
        #endregion
    }
}
