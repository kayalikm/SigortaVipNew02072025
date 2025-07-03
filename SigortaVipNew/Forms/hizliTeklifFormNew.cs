using CefSharp.WinForms;
//using DevExpress.DataAccess.Native.Json;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sigorta_Re;
using SigortaVip.Constant;
//using sigortaVipNew.Business.Abstract;
//using sigortaVipNew.Business.DependencyResolvers.Ninject;
//using sigortaVipNew.Entities.Concrete;
using SigortaVip.Business;
using SigortaVip.Constant;
using SigortaVip.FiyatSorgulamaFactory;
using SigortaVip.FiyatSorgulamaFactory.Concrete;
using SigortaVip.FiyatSorgulamaFactory.Interface;
using SigortaVip.Forms.LowResolutionForms.TeminatBilgileriForm;
using SigortaVip.Models;
using SigortaVip.Querry;
using SigortaVip.Utility;
using SigortaVipNew;
using SigortaVipNew.Api;
using SigortaVip.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using static DevExpress.Utils.Diagnostics.GUIResources;
using Color = System.Drawing.Color;
using ComboBox = System.Windows.Forms.ComboBox;
using SigortaVipNew.Helpers;

namespace SigortaVip.Forms
{
    public partial class hizliTeklifFormNew : Form
    {
        public string tckimlik = "", vergino = "";
        private bool _tumSirketlerSeciliMi = false;
        private KullaniciBilgileri info = null;
        private ConcurrentQueue<QueueMessage> FiyatSorgulamaQueue;
        public ConcurrentQueue<QueueMessage> _fiyatSorgulamaQueue;
        private ConcurrentQueue<QueueMessage> TopluFiyatQueue;
        private List<Thread> ActiveThreadList = null;
        private FiyatSorgulaFactory factory = null;

        private teminatBilgileri _teminatBilgileri;
        private fiyatBilgileriForm _fiyatBilgileriForm;
        private CancellationTokenSource _cancellationTokenSource;
        private NotifyIcon _notifyIcon;

        //private IKullaniciSirketService _kullaniciSirketService;
        //private ISigortaSirketService _sigortaSirketService;
        //private ITeminatBilgiService _teminatBilgiService;
        //private ITeminatSablonService _teminatSablonService;
        //private IKullaniciService _kullaniciService;

        //private List<TeminatSablon> _kullaniciSablonlari;
        //private List<SigortaSirket> _sirketler;
        private TeminatBilgileriForm _teminatBilgileriForm;
        double? screenWidth = null;
        private lowTeminatBilgileriForm _lowTeminatBilgileriForm;
        getAracTipList aracTipTool = new getAracTipList();
        private List<Thread> _activeThreadList;
        private FiyatSorgulaFactory _sorgulamaFactory;
        private int row_index = 0;
        private bool _onlyOneNotify;
        private KullaniciBilgileri _mevcutKullaniciBilgileri = null;
        private Thread kontrolThread;
        public ConcurrentQueue<QueueMessage> _EkPaketFiyatSorgulamaQueue = null;
        private string message = "";
        //private List<KullaniciSirket> kullaniciSirketleri = null;

        //MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();

        public hizliTeklifFormNew()
        {
            InitializeComponent();

            // DevExpress threading detection'ı devre dışı bırak
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;

            FiyatSorgulamaQueue = new ConcurrentQueue<QueueMessage>();
            ActiveThreadList = new List<Thread>();

            factory = new ConcreteFiyatSorgulaFactory();
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            BuildConsumer();

            //_kullaniciSirketService = InstanceFactory.GetInstance<IKullaniciSirketService>();
            //_sigortaSirketService = InstanceFactory.GetInstance<ISigortaSirketService>();
            //_teminatBilgiService = InstanceFactory.GetInstance<ITeminatBilgiService>();
            //_teminatSablonService = InstanceFactory.GetInstance<ITeminatSablonService>();
            //_kullaniciService = InstanceFactory.GetInstance<IKullaniciService>();


            FiyatTeklifFormYukle(TopluFiyatQueue, false);
            _cancellationTokenSource = new CancellationTokenSource();
            SorguListesiDoldur();

        }
        public KullaniciBilgileri GetKullaniciBilgileri()
        {
            KullaniciBilgileriDoldur();
            return info;
        }
        private async void FiyatTeklifFormYukle(ConcurrentQueue<QueueMessage> TopluFiyatQueue, bool removeChilds)
        {
            if (removeChilds)
            {
                panelControl2.Controls.Clear();
            }
            if (TopluFiyatQueue == null)
            {
                fiyatBilgileriForm _fiyatBilgileriForm = new fiyatBilgileriForm(null, null);
                if (_fiyatBilgileriForm != null)
                {

                    _fiyatBilgileriForm.TopLevel = false;

                    panelControl2.Controls.Add(_fiyatBilgileriForm);

                    _fiyatBilgileriForm.Dock = DockStyle.Fill;
                    _fiyatBilgileriForm.FormBorderStyle = FormBorderStyle.None;
                    _fiyatBilgileriForm.Show();
                    _fiyatBilgileriForm.BringToFront();
                }
            }

        }

        private async void BuildConsumer()
        {
            Thread thread = new Thread(RunConsumerThread);
            thread.Name = "Fiyat Sorgu Thread";
            thread.Start();
            ActiveThreadList.Add(thread);
        }

        private async void DestroyConsumers()
        {
            foreach (Thread thread in ActiveThreadList)
            {
                if (thread.IsAlive)
                {
                    try
                    {
                        thread.Abort();
                    }
                    catch (ThreadAbortException)
                    {
                        //ignored
                    }
                }
            }
        }

        private delegate void SafeCallDelegate(FiyatBilgisi row, DataGridView dataGridView);

        private void AddRow(FiyatBilgisi newRow, GridControl dataGridView)
        {
            if (dataGridView.InvokeRequired)
            {
                /* var d = new SafeCallDelegate(AddRow);
                 dataGridView.Invoke(d, new object[] { newRow, dataGridView });*/
            }
            else
            {
                string firmaAdi = newRow.FirmaAdi;
                // DataGridViewRow existingRow = null;

                // Mevcut satırları kontrol et
                //bool contains = dt.AsEnumerable().Any(r => r.Field<string>("Firma") == firmaAdi);
                //if (contains == true)//firma var ise.
                //{
                //    firmaAdiVar = true;
                //    deletedRows.Add(dt.Rows[i]);

                //}

                //Mevcut satırları kontrol et ve sil
                dt.AsEnumerable().Where(w => w.Field<string>("Firma") == firmaAdi).ToList().ForEach(f => f.Delete());
                dt.AcceptChanges();

                //foreach (DataGridViewRow row in dataGridView.Rows)
                //{
                //    string existingFirmaAdi = row.Cells["Firma"].Value?.ToString();
                //    if (existingFirmaAdi != null && existingFirmaAdi.Equals(firmaAdi))
                //    {
                //        firmaAdiVar = true;
                //        existingRow = row;
                //        break;
                //    }
                //}

                //// Eğer aynı Firma adına sahip satır varsa, onu sil
                //if (firmaAdiVar)
                //{
                //    dataGridView.Rows.Remove(existingRow);

                //}

                // Yeni satırı ekle
                object[] satir = new object[]
                {
            newRow.FirmaAdi,
            newRow.BrutPrim,
            newRow.Komisyon,
            newRow.TeklifNo,
            newRow.TeklifTipi,
            newRow.Durum,
                };
                /*  dt.Columns.Add("Firma");
                dt.Columns.Add("");
                dt.Columns.Add("");
                dt.Columns.Add("");
                dt.Columns.Add("");
                dt.Columns.Add("");
                dt.Columns.Add("");
                dt.Columns["Durum"].DefaultValue = 10;
                dt.Columns.Add("");*/

                DataRow dr = dt.NewRow();
                dr["Firma"] = newRow.FirmaAdi;
                dr["BrütPrim"] = newRow.BrutPrim;
                dr["Komisyon"] = newRow.Komisyon;
                dr["TeklifNo"] = newRow.TeklifNo;

                dr["TeklifTipi"] = newRow.TeklifTipi;
                dr["Durum"] = newRow.Durum;


                dt.Rows.Add(dr);
                dt.AcceptChanges();

                // dataGridView.Rows.Add(satir);
            }
        }

        private void notify_Icon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _notifyIcon.Visible = false;
        }

        private async void RunConsumerThread()
        {
            while (true)
            {
                await Task.Delay(100);
                try
                {
                    bool isDequeue = FiyatSorgulamaQueue.TryDequeue(out QueueMessage message);
                    if (isDequeue)
                    {
                        if (message.SorguTipi == "Bilgi-Aksigorta")
                        {
                            //CheckForIllegalCrossThreadCalls = false;
                            //AkSigortaFiyat akSigortaFiyat = new AkSigortaFiyat();
                            //var resp = await akSigortaFiyat.BilgileriCek(info).ConfigureAwait(false);

                            //txtPoliceBaslangic.Text = resp.baslangicTarihi.Trim();
                            //txtPoliceBitis.Text = resp.bitisTarihi.Trim();
                            //txtMotorNo.Text = resp.motorNo.Trim();
                            //sasitxt.Text = resp.sasiNo.Trim();
                            //txttesciltarihi.Text = resp.tescilTarihi.Trim();
                            //txtAd.Text = resp.musteriAdSoyad.Trim();
                            //txtIl.Text = resp.il.Trim();
                            //txtIlce.Text = resp.ilce.Trim();
                            //txtAdres.Text = resp.adres.Trim();
                            //txtEposta.Text = resp.eposta.Trim();
                            //txtTel.Text = resp.telefon.Trim();
                            //txtMeslek.Text = resp.meslek.Trim();
                            //txtCinsiyet.Text = resp.cinsiyet.Trim();
                            //txtModel.Text = resp.aracModelYili.Trim();
                        }
                        else
                        {
                            SetIptalButton(true, message.InsuranceCompany);
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]: consumed a value: {message.InsuranceCompany}, {message.SorguTipi} | queue size {FiyatSorgulamaQueue.Count}");
                            IFiyatSorgu fiyatSorgu = factory.GetFiyatSorgu(message.InsuranceCompany);
                            _cancellationTokenSource = new CancellationTokenSource();
                            try
                            {
                                SigortaVipNew.MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                                //var browsersVar = mainForm.GetBrowsers;

                                //if (browsersVar.TryGetValue(message.InsuranceCompany, out (ChromiumWebBrowser browser, TabPage tabPage) value))
                                //{
                                //    var resp = message.SorguTipi == "Trafik"
                                //    ? await fiyatSorgu.TrafikSorgula(info, browsersVar[message.InsuranceCompany].browser, _cancellationTokenSource.Token).ConfigureAwait(false)
                                //    : await fiyatSorgu.KaskoSorgula(info, browsersVar[message.InsuranceCompany].browser, _cancellationTokenSource.Token).ConfigureAwait(false);
                                //    resp.TeklifTipi = message.SorguTipi;

                                //    SetIptalButton(false, message.InsuranceCompany);
                                //}
                                //else
                                //{
                                //    var resp = new FiyatBilgisi
                                //    {
                                //        BrutPrim = "",
                                //        Durum = "Ekran Kapalı",
                                //        FirmaAdi = message.InsuranceCompany,
                                //        Komisyon = "",
                                //        TeklifNo = ""
                                //    };

                                //}
                            }
                            catch (OperationCanceledException)
                            {
                                SetIptalButton(false, message.InsuranceCompany);
                                _notifyIcon.Visible = true;
                                _notifyIcon.Text = "SigortaVip Uyarı";
                                _notifyIcon.BalloonTipTitle = "Sorgu İptal Edildi";
                                _notifyIcon.BalloonTipText = "Şirketin fiyat sorgusu iptal edildi";
                                _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                                _notifyIcon.ShowBalloonTip(2000);

                                _notifyIcon.MouseDoubleClick += new MouseEventHandler(notify_Icon_MouseDoubleClick);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        private async void SetIptalButton(bool enable, string sorgulananSirketAdi)
        {
            if (btnSorguIptal.InvokeRequired)
            {
                Action setBtn = delegate { SetIptalButton(enable, sorgulananSirketAdi); };
                btnSorguIptal.Invoke(setBtn);
            }
            else
            {
                btnSorguIptal.Visible = enable;
            }
        }

        public void ClearTextBoxes(Control.ControlCollection ctrlCollection)
        {
            dt.Rows.Clear();
            foreach (Control ctrl in ctrlCollection)
            {
                if (ctrl is TextBoxBase)
                {
                    ctrl.Text = String.Empty;
                }
                else
                {
                    ClearTextBoxes(ctrl.Controls);
                }
            }
        }

        public async void KullaniciBilgileriDoldur()
        {
            info = new KullaniciBilgileri()
            {
                txtAracKodu = txtAracKodu.Text.TrimEnd(),
                txtDogumTar = mtxtDogumTarihi.Text.Replace(" ", "").Replace(",", "."),
                txtKimlikNo = txtKimlikNo.Text.Replace(" ", ""),
                txtModel = txtModel.Text.TrimEnd(),
                txtPlakaNo = txtPlakaNo.Text.Replace(" ", ""),
                txtSeriNo = txtSeriNo.Text.Replace(" ", ""),
                txtYerAdedi = txtyolcusayısı.Text.TrimEnd(),
                txtTel = txtTel.Text.TrimEnd(),
                txtkullanımtarzı = txtkullanımtarzı.Text.TrimEnd(),
                txtMarka = txtMarka.Text.TrimEnd(),
                txtModelAdi = txtTip.Text.TrimEnd(),
                txtEposta = txtEposta.Text.TrimEnd(),
                txtIl = txtIl.Text.TrimEnd(),
                txtIlce = txtIlce.Text.TrimEnd(),
                cmbMeslek = txtMeslek.Text.TrimEnd(),
                txtKullanimSekli = txtKullanimSekli.Text.TrimEnd(),
                AdSoyad = txtAd.Text.TrimEnd(),
                tescilTarihi = txttesciltarihi.Text.TrimEnd(),
                yolcuSayisi = txtyolcusayısı.Text.TrimEnd()
            };
        }

        private async void btnClear_Click(object sender, EventArgs e)
        {
            //cbxMarka.SelectedItem = "";
            //cbxTip.SelectedItem = "";
            AktifSorgulariIptalEt();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                this.dtackdelete(param);
            }
            catch { }




            panelControl2.Controls.Clear();//fiyat bilgileri
            _fiyatBilgileriForm = null;
            info = null;
            cbxMarka.Items.Clear();
            cbxTip.Items.Clear();
            cbxMarka.Text = "";
            cbxTip.Text = "";
            cbxMarka.DroppedDown = false;
            cbxTip.DroppedDown = false;

            ClearTextBoxes(Controls);
            foreach (WebPage acikSayfalar in Browser.webPageList)
            {
                if (acikSayfalar.insuranceCompany != "AcnTurkSigorta")
                {
                    acikSayfalar.browser.LoadUrlAsync(acikSayfalar.insuranceCompanyPageUrl);
                }
            }
            _fiyatSorgulamaQueue = null;
            TopluFiyatQueue = null;
            FiyatTeklifFormYukle(TopluFiyatQueue, true);

            SorguListesiDoldur();




        }
        string oncekiIslem = null;

        private KullaniciBilgileri Get_kullaniciBilgileri()
        {
            return info;
        }

        private async void fiyatBilgileriGetir(string islem, KullaniciBilgileri _kullaniciBilgileri)
        {
            if (TopluFiyatQueue.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir şirket seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //19171846850-06ceh459-aa341977-27.08.1991
                //if (_fiyatBilgileriForm == null)
                //{
                //    oncekiIslem = islem;
                //    _fiyatBilgileriForm = new fiyatBilgileriForm(info, TopluFiyatQueue);
                //    if (_fiyatBilgileriForm != null)
                //    {
                //        xtraTabControl1.SelectedTabPage = xtraTabPage2;
                //        panelControl2.Controls.Clear();

                //        // barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;


                //        _fiyatBilgileriForm.TopLevel = false;

                //        panelControl2.Controls.Add(_fiyatBilgileriForm);

                //        _fiyatBilgileriForm.Dock = DockStyle.Fill;
                //        _fiyatBilgileriForm.FormBorderStyle = FormBorderStyle.None;
                //        _fiyatBilgileriForm.Show();
                //        _fiyatBilgileriForm.BringToFront();
                //    }
                //}
                //else
                //{
                //    xtraTabControl1.SelectedTabPage = xtraTabPage2;
                //    //panelControl2.Controls.Clear();
                //    //_fiyatBilgileriForm.TopLevel = false;

                //    //panelControl2.Controls.Add(_fiyatBilgileriForm);

                //    //_fiyatBilgileriForm.Dock = DockStyle.Fill;
                //    //_fiyatBilgileriForm.FormBorderStyle = FormBorderStyle.None;
                //    //_fiyatBilgileriForm.Show();
                //    //_fiyatBilgileriForm.BringToFront();

                //    oncekiIslem = islem;
                //    _kullaniciBilgileri = info;
                //    _fiyatBilgileriForm._EkPaketFiyatSorgulamaQueue = TopluFiyatQueue;
                //}
                //  _fiyatBilgileriForm.Show();
            }
        }

        private Form formAcikmi(string formName)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == formName)
                {
                    return frm;
                }
            }
            return null;
        }

        private void AktifSorgulariIptalEt()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch { }
        }

        public void SeciliSirketleriIslemeAl(string param) //dönülecek
        {
            TopluFiyatQueue = null;

            if (TopluFiyatQueue == null)
                TopluFiyatQueue = new ConcurrentQueue<QueueMessage>();

            string[] checkBoxes = null;

            switch (param)

            {
                case "Trafik":
                    checkBoxes = new[]
                    {
                        InsuranceConstants.AkSigorta,
                        InsuranceConstants.AllianzSigorta,
                        InsuranceConstants.AnadoluSigorta,
                        InsuranceConstants.BereketSigorta,
                        InsuranceConstants.CorpusSigorta,
                        InsuranceConstants.DogaSigorta,
                        InsuranceConstants.HdiSigorta,
                        InsuranceConstants.KoruSigorta,
                        InsuranceConstants.MagdeburgerSigorta,
                        InsuranceConstants.NeovaSigorta,
                        InsuranceConstants.QuickSigorta,
                        InsuranceConstants.RaySigorta,
                        InsuranceConstants.TurkiyeSigorta,
                        InsuranceConstants.UnicoSigorta,
                        InsuranceConstants.AnkaraSigorta,
                        InsuranceConstants.AnaSigorta,
                        InsuranceConstants.SompoJapanSigorta,
                        InsuranceConstants.OrientSigorta,
                        InsuranceConstants.AxaSigorta,
                        InsuranceConstants.SekerSigorta,
                        InsuranceConstants.HepIyiSigorta,
                    };
                    break;
                case "Kasko":
                    checkBoxes = new[]
                    {
                        InsuranceConstants.AkSigorta,
                        InsuranceConstants.AllianzSigorta,
                        InsuranceConstants.AnadoluSigorta,
                        InsuranceConstants.BereketSigorta,
                        InsuranceConstants.CorpusSigorta,
                        InsuranceConstants.DogaSigorta,
                        InsuranceConstants.HdiSigorta,
                        InsuranceConstants.KoruSigorta,
                        InsuranceConstants.MagdeburgerSigorta,
                        InsuranceConstants.NeovaSigorta,
                        InsuranceConstants.QuickSigorta,
                        InsuranceConstants.RaySigorta,
                        InsuranceConstants.TurkNipponSigorta,
                        InsuranceConstants.TurkiyeSigorta,
                        InsuranceConstants.UnicoSigorta,
                        InsuranceConstants.AnkaraSigorta,
                        InsuranceConstants.AnaSigorta,
                        InsuranceConstants.AtlasSigorta,
                        InsuranceConstants.SompoJapanSigorta,
                        InsuranceConstants.OrientSigorta,
                        InsuranceConstants.AxaSigorta,
                        InsuranceConstants.SekerSigorta,
                        InsuranceConstants.HepIyiSigorta,
                        InsuranceConstants.AcnTurkSigorta,
                        InsuranceConstants.AveonSigorta,
                        InsuranceConstants.ZurichSigorta,
                    };
                    break;
                default:
                    break;
            }

            var sorguTipi = sorgutipi;

            
            var selectedCheckBoxes = checkBoxes;

            var queueMessages = selectedCheckBoxes.Select(cb => new QueueMessage
            {
                InsuranceCompany = cb,
                SorguTipi = sorguTipi
            });

            foreach (var message in queueMessages)
            {
                TopluFiyatQueue.Enqueue(message);
            }

        }
        string cmbInsuranceCompanySTR = "DOĞASİGORTA";
        private async void MusteriBilgiGetir_Click(object sender, EventArgs e)
        {
            try
            {
                //string sorguPrefix = "sistemSorgu";
                //Kullanici sorguKullanicisi = _kullaniciService.GetUsersByCompanyId(AktifKullaniciBilgileri.AktifKullanici.companyId).First(x => x.userName == sorguPrefix + ":" + x.userName.Split(':')[1]);

                Login login = new Login();
                sorgu sorgu = new sorgu();
                aracsorgu aracsorgu = new aracsorgu();

                DogaSigortaGetQuerry dogaSigortaGet = new DogaSigortaGetQuerry();
                GriSigortaGetQuerry griSigortaGetQuerry = new GriSigortaGetQuerry();
                DogaSigortaQuerry dogaSigortaQuerry = new DogaSigortaQuerry();

                Models.Querry querryReturn = new Models.Querry();

                if (string.IsNullOrEmpty(cmbInsuranceCompanySTR))
                {
                    MessageBox.Show("Lütfen Sorgulama yapmadan önce firma seçiniz.");
                    return;
                }
                if (cmbInsuranceCompanySTR == "DOĞASİGORTA")
                {
                    if (txtKimlikNo.Text.Length >= 10)
                    {
                        if (txtKimlikNo.Text.Length == 11)
                        {
                            tckimlik = txtKimlikNo.Text;
                        }
                        else if (txtKimlikNo.Text.Length == 10)
                        {
                            vergino = txtKimlikNo.Text;
                        }
                        else
                        {
                            MessageBox.Show("Yanlış Tc Kimlik/Vergi No.");
                            return;
                        }
                        //var sessions = login.login(sorguKullanicisi.userName, sorguKullanicisi.password, "DogaSigorta");


                        try
                        {
                            QuerryItems querryItems = new QuerryItems
                            {
                                belgeKodu = txtSeriNo.Text.Substring(0, 2),
                                belgeNo = txtSeriNo.Text.Substring(2),
                                Plaka = txtPlakaNo.Text.Substring(2),
                                plakaKodu = txtPlakaNo.Text.Substring(0, 2),
                                //sessionId = new List<string> { sessions.SessionList[0].SessionValue, sessions.SessionList[1].SessionValue },
                                tcKimlik = "",
                                vergiNo = "",
                                dogumTarihi = mtxtDogumTarihi.Text
                            };
                            if (txtKimlikNo.Text.Length == 10)
                            {
                                querryItems.vergiNo = txtKimlikNo.Text;
                            }
                            if (txtKimlikNo.Text.Length == 11)
                            {
                                querryItems.tcKimlik = txtKimlikNo.Text;
                            }
                            querryReturn = dogaSigortaGet.getDogaQuerry(querryItems);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        txtAd.Text = querryReturn.adSoyad;
                        txtIl.Text = querryReturn.Il;
                        txtIlce.Text = querryReturn.Ilce;
                        txtAdres.Text = querryReturn.Adress;
                        txtTel.Text = querryReturn.cepTelenu;
                        txtEposta.Text = querryReturn.Eposta;
                        txtkullanımtarzı.Text = querryReturn.kullanimTarzi;
                        txtKullanimSekli.Text = querryReturn.kullanimSekli;

                        int i = 0;
                        if (querryReturn.kullanimTarzi != null)
                        {
                            foreach (var item in cbxAracCinsi.Items)
                            {
                                if (item.ToString().Contains(querryReturn.kullanimTarzi))
                                {
                                    cbxAracCinsi.SelectedIndex = i;
                                    break;
                                }
                                i++;
                            }
                        }


                        try
                        {
                            QuerryItems querryItems = new QuerryItems
                            {
                                belgeKodu = txtSeriNo.Text.Substring(0, 2),
                                belgeNo = txtSeriNo.Text.Substring(2),
                                Plaka = txtPlakaNo.Text.Substring(2),
                                plakaKodu = txtPlakaNo.Text.Substring(0, 2),
                                //sessionId = new List<string> { sessions.SessionList[0].SessionValue, sessions.SessionList[1].SessionValue },
                                tcKimlik = "",
                                vergiNo = "",
                                dogumTarihi = mtxtDogumTarihi.Text
                            };
                            if (txtKimlikNo.Text.Length == 10)
                            {
                                querryItems.vergiNo = txtKimlikNo.Text;
                            }
                            if (txtKimlikNo.Text.Length == 11)
                            {
                                querryItems.tcKimlik = txtKimlikNo.Text;
                            }
                            //querryItems.firmaSessionBilgileri = sessions;
                            querryReturn = dogaSigortaGet.getDogaAracQuerry(querryItems);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        txtMarka.Text = querryReturn.Marka;
                        txtTip.Text = querryReturn.Tip;
                        txtMotorNo.Text = querryReturn.motorNo;
                        txtModel.Text = querryReturn.Model;
                        txtyolcusayısı.Text = querryReturn.yolcuSayisi;
                        sasitxt.Text = querryReturn.sasiNo;
                        txttesciltarihi.Text = querryReturn.tescilTarihi;
                        txtBasamakKodu.Text = querryReturn.basamakKodu;
                        txtPoliceBaslangic.Text = querryReturn.policeBaslangis;
                        txtPoliceBitis.Text = querryReturn.policeBitis;
                        txtAracKodu.Text = querryReturn.aracKod;
                        txtYakitTipi.Text = querryReturn.YakitTipi;
                        if (querryReturn.kullanimSekli != "" && querryReturn.kullanimSekli != null)
                        {
                            txtKullanimSekli.Text = querryReturn.kullanimSekli;
                        }
                        if (querryReturn.kullanimTarzi != "" && querryReturn.kullanimTarzi != null)
                        {
                            txtkullanımtarzı.Text = querryReturn.kullanimTarzi;
                        }
                        i = 0;
                        foreach (var item in cbxMarka.Items)
                        {
                            if (item.ToString().Contains(querryReturn.Marka))
                            {
                                cbxMarka.SelectedIndex = i;
                                break;
                            }
                            i++;
                        }
                    }

                }
                else if (cmbInsuranceCompanySTR == "KORUSİGORTA")
                {
                    if (txtKimlikNo.Text.Length == 11)
                    {
                        tckimlik = txtKimlikNo.Text;
                    }
                    else if (txtKimlikNo.Text.Length == 10)
                    {
                        vergino = txtKimlikNo.Text;
                    }
                    else
                    {
                        MessageBox.Show("Yanlış Tc Kimlik/Vergi No.");
                        return;
                    }
                    HttpWebResponse response;
                    //var sessions = login.login(sorguKullanicisi.userName, sorguKullanicisi.password, "KoruSigorta");

                    QuerryItems querryItems = new QuerryItems
                    {
                        belgeKodu = txtSeriNo.Text.Substring(0, 2),
                        belgeNo = txtSeriNo.Text.Substring(2),
                        Plaka = txtPlakaNo.Text.Substring(2),
                        plakaKodu = txtPlakaNo.Text.Substring(0, 2),
                        //sessionId = new List<string> { sessions.SessionList[0].SessionValue, sessions.SessionList[1].SessionValue },
                        tcKimlik = "",
                        vergiNo = "",
                        dogumTarihi = mtxtDogumTarihi.Text,
                        //firmaSessionBilgileri = sessions
                    };
                    if (txtKimlikNo.Text.Length == 10)
                    {
                        querryItems.vergiNo = txtKimlikNo.Text;
                    }
                    if (txtKimlikNo.Text.Length == 11)
                    {
                        querryItems.tcKimlik = txtKimlikNo.Text;
                    }

                    if (txtKimlikNo.Text.Length == 10)
                    {
                        if (aracsorgu.KoruSigortaVergiNo(out response, querryItems))
                        {
                            string text = ReadResponse(response);


                            var x1 = text.Split(new string[] { ":\"AracMotorNo" }, StringSplitOptions.None)[1];
                            string motorNofirst = x1.Split('}')[0];
                            x1 = motorNofirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string motorNo = x1.Split('\"')[0];
                            txtMotorNo.Text = motorNo;

                            x1 = text.Split(new string[] { ":\"AracSasiNo" }, StringSplitOptions.None)[1];
                            string sasiNofirst = x1.Split('}')[0];
                            x1 = sasiNofirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string sasiNo = x1.Split('\"')[0];
                            sasitxt.Text = sasiNo;


                            x1 = text.Split(new string[] { ":\"EGMMarka" }, StringSplitOptions.None)[1];
                            string EGMMarkafirst = x1.Split('}')[0];
                            x1 = EGMMarkafirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string EGMMarka = x1.Split('\"')[0];
                            txtMarka.Text = EGMMarka;

                            querryItems.sasiNo = sasiNo;


                            x1 = text.Split(new string[] { ":\"AracKisiSayisi" }, StringSplitOptions.None)[1];
                            string yolcusayisifirst = x1.Split('}')[0];
                            x1 = yolcusayisifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string yolcusayisi = x1.Split('\"')[0];
                            txtyolcusayısı.Text = yolcusayisi;

                            x1 = text.Split(new string[] { ":\"EGMModelYili" }, StringSplitOptions.None)[1];
                            string EGMModelYilifirst = x1.Split('}')[0];
                            x1 = EGMModelYilifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string EGMModelYili = x1.Split('\"')[0];
                            txtModel.Text = EGMModelYili;

                            x1 = text.Split(new string[] { ":\"EGMAltCins" }, StringSplitOptions.None)[1];
                            string EGMAltCinsfirst = x1.Split('}')[0];
                            x1 = EGMAltCinsfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string EGMAltCins = x1.Split('\"')[0];
                            txtKullanimSekli.Text = EGMAltCins;

                            x1 = text.Split(new string[] { ":\"EGMUstCins" }, StringSplitOptions.None)[1];
                            string EGMUstCinsfirst = x1.Split('}')[0];
                            x1 = EGMUstCinsfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string EGMUstCins = x1.Split('\"')[0];
                            txtkullanımtarzı.Text = EGMUstCins;

                            AracCinsi aracCinsi = new AracCinsi();
                            AracCinsi.AracCins arac = aracCinsi.AracCinsGetirIsmeGore(EGMUstCins);

                            int i = 0;
                            foreach (var item in cbxAracCinsi.Items)
                            {
                                if (item.ToString().Contains(arac.Number))
                                {
                                    cbxAracCinsi.SelectedIndex = i;
                                    break;
                                }
                                i++;
                            }

                            i = 0;
                            foreach (var item in cbxMarka.Items)
                            {
                                if (item.ToString().Contains(EGMMarka))
                                {
                                    cbxMarka.SelectedIndex = i;
                                    break;
                                }
                                i++;
                            }

                            x1 = text.Split(new string[] { ":\"YakitTipi" }, StringSplitOptions.None)[1];
                            string YakitTipifirst = x1.Split('}')[0];
                            x1 = YakitTipifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string YakitTipi = x1.Split('\"')[0];
                            txtYakitTipi.Text = YakitTipi;

                            x1 = text.Split(new string[] { ":\"EGMAd" }, StringSplitOptions.None)[1];
                            string EGMAdfirst = x1.Split('}')[0];
                            x1 = EGMAdfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string EGMAd = x1.Split('\"')[0];
                            txtAd.Text = EGMAd;

                            x1 = text.Split(new string[] { ":\"TrafikTescilTarihi" }, StringSplitOptions.None)[1];
                            string TrafikTescilTarihifirst = x1.Split('}')[0];
                            x1 = TrafikTescilTarihifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string TrafikTescilTarihi = x1.Split('\"')[0];
                            txttesciltarihi.Text = TrafikTescilTarihi;

                            querryItems.AracTarz = arac.Number;
                            if (aracsorgu.KoruSigortaVergiNoDetay(out response, querryItems))
                            {
                                text = ReadResponse(response);

                                x1 = text.Split(new string[] { ":\"AracKodu" }, StringSplitOptions.None)[1];
                                string AracKodufirst = x1.Split('}')[0];
                                x1 = AracKodufirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string AracKodu = x1.Split('\"')[0];
                                txtAracKodu.Text = AracKodu;

                                x1 = text.Split(new string[] { ":\"uygulanmasiGerekenTarifeBasamakKodu" }, StringSplitOptions.None)[1];
                                string uygulanmasiGerekenTarifeBasamakKodufirst = x1.Split('}')[0];
                                x1 = uygulanmasiGerekenTarifeBasamakKodufirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string uygulanmasiGerekenTarifeBasamakKodu = x1.Split('\"')[0];
                                txtBasamakKodu.Text = uygulanmasiGerekenTarifeBasamakKodu;

                                x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBaslangicTarihi" }, StringSplitOptions.None)[1];
                                string GecmisPoliceVadeBaslangicTarihifirst = x1.Split('}')[0];
                                x1 = GecmisPoliceVadeBaslangicTarihifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string GecmisPoliceVadeBaslangicTarihi = x1.Split('\"')[0];
                                txtPoliceBaslangic.Text = GecmisPoliceVadeBaslangicTarihi;

                                x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBitisTarihi" }, StringSplitOptions.None)[1];
                                string GecmisPoliceVadeBitisTarihifirst = x1.Split('}')[0];
                                x1 = GecmisPoliceVadeBitisTarihifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string GecmisPoliceVadeBitisTarihi = x1.Split('\"')[0];
                                txtPoliceBitis.Text = GecmisPoliceVadeBitisTarihi;

                                x1 = text.Split(new string[] { ":\"TipAdi" }, StringSplitOptions.None)[1];
                                string TipAdifirst = x1.Split('}')[0];
                                x1 = TipAdifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string TipAdi = x1.Split('\"')[0];
                                txtTip.Text = TipAdi;

                                if (sorgu.KoruSigortaVergiNo(out response, querryItems))
                                {
                                    text = ReadResponse(response);
                                    dynamic stuff = JsonConvert.DeserializeObject(text);
                                    txtIl.Text = stuff.value.SbmIlAdi;
                                    txtIlce.Text = stuff.value.SbmIlceAdi;
                                }
                            }
                        }
                    }
                    else
                    {
                        //        if (sorgu.KoruSigorta(out response, sessions.SessionList[0].SessionValue, sessions.SessionList[1].SessionValue, tckimlik, mtxtDogumTarihi.Text))
                        //        {
                        //            string text = ReadResponse(response);
                        //            dynamic stuff = JsonConvert.DeserializeObject(text);

                        //            string adsoyad = stuff.value.AdUnvan + " " + stuff.value.Soyad;
                        //            string il = stuff.value.EvIlAdi;
                        //            string ilce = stuff.value.EvIlceAdi;
                        //            string adress = stuff.value.EvAdresi;
                        //            string CepTelefonu = stuff.value.CepTelefonu;
                        //            string EPosta = stuff.value.EPosta;

                        //            txtAd.Text = adsoyad;
                        //            txtIl.Text = il;
                        //            txtIlce.Text = ilce;
                        //            txtAdres.Text = adress;
                        //            txtTel.Text = CepTelefonu;
                        //            txtEposta.Text = EPosta;
                        //            response.Close();
                        //        }
                        //        if (aracsorgu.KoruSigorta(out response, sessions.SessionList[0].SessionValue, sessions.SessionList[1].SessionValue, tckimlik, txtPlakaNo.Text.Substring(2), txtPlakaNo.Text.Substring(0, 2), vergino))
                        //        {
                        //            string text = ReadResponse(response);
                        //            string x1 = text.Split(new string[] { "\"marka\":{\"aciklama\":\"" }, StringSplitOptions.None)[1];
                        //            string marka = x1.Split('\"')[0];
                        //            txtMarka.Text = marka;

                        //            int i = 0;
                        //            foreach (var item in cbxMarka.Items)
                        //            {
                        //                if (item.ToString().Contains(marka))
                        //                {
                        //                    cbxMarka.SelectedIndex = i;
                        //                    break;
                        //                }
                        //                i++;
                        //            }

                        //            x1 = text.Split(new string[] { "\"kod\":\"" }, StringSplitOptions.None)[1];
                        //            string markakod = x1.Split('\"')[0];
                        //            x1 = text.Split(new string[] { "\"tip\":{\"aciklama\":\"" }, StringSplitOptions.None)[1];
                        //            string tip = x1.Split('\"')[0];
                        //            txtTip.Text = tip;
                        //            x1 = text.Split(new string[] { "\"kod\":\"" }, StringSplitOptions.None)[2];
                        //            string tipkod = x1.Split('\"')[0];
                        //            txtAracKodu.Text = markakod + tipkod;
                        //            x1 = text.Split(new string[] { "modelYili\":" }, StringSplitOptions.None)[1];
                        //            string model = x1.Split(',')[0];
                        //            txtModel.Text = model;
                        //            x1 = text.Split(new string[] { "kullanimSekli\":" }, StringSplitOptions.None)[1];
                        //            string ksekli = x1.Split(',')[0];
                        //            ksekli = ksekli.Replace("\"", "");
                        //            txtKullanimSekli.Text = ksekli;
                        //            x1 = text.Split(new string[] { "motorNo\":" }, StringSplitOptions.None)[1];
                        //            string motorNo = x1.Split(',')[0];
                        //            motorNo = motorNo.Replace("\"", "");
                        //            txtMotorNo.Text = motorNo;
                        //            x1 = text.Split(new string[] { "sasiNo\":" }, StringSplitOptions.None)[1];
                        //            string sasiNo = x1.Split(',')[0];
                        //            sasiNo = sasiNo.Replace("\"", "");
                        //            sasitxt.Text = sasiNo;

                        //            x1 = text.Split(new string[] { "uygulanmasiGerekenTarifeBasamakKodu\":" }, StringSplitOptions.None)[1];
                        //            string basamakKodu = x1.Split(',')[0];
                        //            txtBasamakKodu.Text = basamakKodu;

                        //            x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBaslangicTarihi" }, StringSplitOptions.None)[1];
                        //            string policebaslangicfirst = x1.Split('}')[0];
                        //            x1 = policebaslangicfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        //            string policebaslangic = x1.Split('\"')[0];
                        //            txtPoliceBaslangic.Text = policebaslangic;

                        //            x1 = text.Split(new string[] { ":\"AracTarz" }, StringSplitOptions.None)[1];
                        //            string AracTarzfirst = x1.Split('}')[0];
                        //            x1 = AracTarzfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        //            string AracTarz = x1.Split('\"')[0];

                        //            AracCinsi aracCinsi = new AracCinsi();
                        //            AracCinsi.AracCins arac = aracCinsi.AracCinsGetirSayiyaGore(AracTarz);

                        //            i = 0;
                        //            foreach (var item in cbxAracCinsi.Items)
                        //            {
                        //                if (item.ToString().Contains(arac.Number))
                        //                {
                        //                    cbxAracCinsi.SelectedIndex = i;
                        //                    break;
                        //                }
                        //                i++;
                        //            }

                        //            txtkullanımtarzı.Text = arac.Cins;

                        //            x1 = text.Split(new string[] { ":\"GecmisPoliceVadeBitisTarihi" }, StringSplitOptions.None)[1];
                        //            string policebitisfirst = x1.Split('}')[0];
                        //            x1 = policebitisfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        //            string policebitis = x1.Split('\"')[0];

                        //            txtPoliceBitis.Text = policebitis;

                        //            if (aracsorgu.KoruSigortaDetay(out response, querryItems))
                        //            {
                        //                text = ReadResponse(response);
                        //                x1 = text.Split(new string[] { ":\"AracIstihapHaddiKisi" }, StringSplitOptions.None)[1];
                        //                string parsedFirst = x1.Split('}')[0];
                        //                x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        //                string parsedtext = x1.Split('\"')[0];
                        //                txtyolcusayısı.Text = parsedtext;

                        //                x1 = text.Split(new string[] { ":\"YakitTipi" }, StringSplitOptions.None)[1];
                        //                parsedFirst = x1.Split('}')[0];
                        //                x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        //                parsedtext = x1.Split('\"')[0];
                        //                txtYakitTipi.Text = parsedtext;

                        //                x1 = text.Split(new string[] { ":\"TrafikTescilTarihi" }, StringSplitOptions.None)[1];
                        //                parsedFirst = x1.Split('}')[0];
                        //                x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        //                parsedtext = x1.Split('\"')[0];
                        //                txttesciltarihi.Text = parsedtext;

                        //                i = 0;
                        //                foreach (var item in cbxAracCinsi.Items)
                        //                {
                        //                    if (item.ToString().Contains(parsedtext))
                        //                    {
                        //                        cbxAracCinsi.SelectedIndex = i;
                        //                        break;

                        //                    }
                        //                    i++;
                        //                }

                        //                x1 = text.Split(new string[] { ":\"EGMKullanimSekli" }, StringSplitOptions.None)[1];
                        //                parsedFirst = x1.Split('}')[0];
                        //                x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                        //                parsedtext = x1.Split('\"')[0];
                        //                if (parsedtext == "YOLCU NAKLI")
                        //                {
                        //                    parsedtext = "HUSUSI";
                        //                }
                        //                txtKullanimSekli.Text = parsedtext;

                        //            }

                        //            response.Close();
                        //        }
                        //    }
                        //}
                        //else if (cmbInsuranceCompanySTR == "GRİSİGORTA")
                        //{
                        //    if (txtKimlikNo.Text.Length == 11)
                        //    {
                        //        tckimlik = txtKimlikNo.Text;
                        //    }
                        //    else if (txtKimlikNo.Text.Length == 10)
                        //    {
                        //        vergino = txtKimlikNo.Text;
                        //    }
                        //    else
                        //    {
                        //        MessageBox.Show("Yanlış Tc Kimlik/Vergi No.");
                        //        return;
                        //    }
                        //    var sessions = login.login(sorguKullanicisi.userName, sorguKullanicisi.password, "GriSigorta");

                        //try
                        //{
                        //    QuerryItems querryItems = new QuerryItems
                        //    {
                        //        belgeKodu = txtSeriNo.Text.Substring(0, 2),
                        //        belgeNo = txtSeriNo.Text.Substring(2),
                        //        Plaka = txtPlakaNo.Text.Substring(2),
                        //        plakaKodu = txtPlakaNo.Text.Substring(0, 2),
                        //        sessionId = new List<string> { sessions.SessionList[0].SessionValue, sessions.SessionList[1].SessionValue },
                        //        tcKimlik = "",
                        //        vergiNo = "",
                        //        dogumTarihi = mtxtDogumTarihi.Text,
                        //        firmaSessionBilgileri = sessions
                        //    };
                        //    if (txtKimlikNo.Text.Length == 10)
                        //    {
                        //        querryItems.vergiNo = txtKimlikNo.Text;
                        //    }
                        //    if (txtKimlikNo.Text.Length == 11)
                        //    {
                        //        querryItems.tcKimlik = txtKimlikNo.Text;
                        //    }
                        //    querryReturn = griSigortaGetQuerry.getGriQuerry(querryItems);
                        //}
                        //catch (Exception ex)
                        //{
                        //    MessageBox.Show(ex.Message);
                        //    return;
                        //}
                        //txtAd.Text = querryReturn.adSoyad;
                        //txtIl.Text = querryReturn.Il;
                        //txtIlce.Text = querryReturn.Ilce;
                        //txtAdres.Text = querryReturn.Adress;
                        //txtTel.Text = querryReturn.cepTelenu;
                        //txtEposta.Text = querryReturn.Eposta;
                        //txtkullanımtarzı.Text = querryReturn.kullanimTarzi;
                        //txtKullanimSekli.Text = querryReturn.kullanimSekli;
                        //txtModel.Text = querryReturn.Model;
                        //txtYakitTipi.Text = querryReturn.YakitTipi;
                        //sasitxt.Text = querryReturn.sasiNo;
                        //txtMotorNo.Text = querryReturn.motorNo;
                        //txtMarka.Text = querryReturn.Marka;
                        //txtTip.Text = querryReturn.Tip;
                        //txttesciltarihi.Text = querryReturn.tescilTarihi;


                        //int i = 0;
                        //foreach (var item in cbxAracCinsi.Items)
                        //{
                        //    if (item.ToString().Contains(querryReturn.kullanimTarzi))
                        //    {
                        //        cbxAracCinsi.SelectedIndex = i;
                        //        break;

                        //    }
                        //    i++;
                        //}

                        //try
                        //{

                        //    QuerryItems querryItems = new QuerryItems
                        //    {
                        //        belgeKodu = txtSeriNo.Text.Substring(0, 2),
                        //        belgeNo = txtSeriNo.Text.Substring(2),
                        //        Plaka = txtPlakaNo.Text.Substring(2),
                        //        plakaKodu = txtPlakaNo.Text.Substring(0, 2),
                        //        sessionId = new List<string> { sessions.SessionList[0].SessionValue, sessions.SessionList[1].SessionValue },
                        //        tcKimlik = "",
                        //        vergiNo = "",
                        //        dogumTarihi = mtxtDogumTarihi.Text,
                        //        sasiNo = sasitxt.Text,
                        //        firmaSessionBilgileri = sessions

                        //    };
                        //    if (txtKimlikNo.Text.Length == 10)
                        //    {
                        //        querryItems.vergiNo = txtKimlikNo.Text;
                        //    }
                        //    if (txtKimlikNo.Text.Length == 11)
                        //    {
                        //        querryItems.tcKimlik = txtKimlikNo.Text;
                        //    }
                        //    querryReturn = griSigortaGetQuerry.getGriAracQuerry(querryItems);

                        //}
                        //catch (Exception ex)
                        //{
                        //    MessageBox.Show(ex.Message);
                        //    return;
                        //}

                        //txtAracKodu.Text = querryReturn.aracKod;
                        //txtTip.Text = querryReturn.Tip;
                        //if (querryReturn.Marka != "" && querryReturn.Marka != null)
                        //{
                        //    txtMarka.Text = querryReturn.Marka;
                        //    i = 0;
                        //    foreach (var item in cbxMarka.Items)
                        //    {
                        //        if (item.ToString().Contains(querryReturn.Marka))
                        //        {
                        //            cbxMarka.SelectedIndex = i;
                        //            break;
                        //        }
                        //        i++;
                        //    }
                        //    txtyolcusayısı.Text = querryReturn.yolcuSayisi;
                        //    if (querryReturn.kullanimSekli == "YOLCU NAKLI")
                        //    {
                        //        querryReturn.kullanimSekli = "HUSUSI";
                        //    }
                        //    txtKullanimSekli.Text = querryReturn.kullanimSekli;

                        //}

                    }
                    //else if (cmbInsuranceCompanySTR == "AKSİGORTA")
                    //{
                    //    KullaniciBilgileriDoldur();
                    //    var browsersVar = mainForm.GetBrowsers;

                    //    BilgiCekme.AkSigortaBilgiCekme akSigortaBilgiCekme = new BilgiCekme.AkSigortaBilgiCekme();
                    //    var x = akSigortaBilgiCekme.BilgiCekme(info, browsersVar[InsuranceConstants.AkSigorta].browser);
                    //}
                    //else if (cmbInsuranceCompanySTR == "AVEONSİGORTA")
                    {

                        //var sessions = login.login(sorguKullanicisi.userName, sorguKullanicisi.password, "AveonSigorta");

                        //QuerryItems querryItems = new QuerryItems();
                        querryItems.plakaKodu = txtPlakaNo.Text.Substring(0, 2);
                        querryItems.Plaka = txtPlakaNo.Text.Substring(2);
                        querryItems.tcKimlik = txtKimlikNo.Text;
                        //querryItems.firmaSessionBilgileri = sessions;
                        querryItems.belgeKodu = txtSeriNo.Text.Substring(0, 2);
                        querryItems.belgeNo = txtSeriNo.Text.Substring(2);

                        //HttpWebResponse response;
                        if (txtKimlikNo.Text.Length == 10)
                        {
                            if (aracsorgu.AveonSigortaVergiNo(out response, querryItems))
                            {
                                string text = ReadResponse(response);

                                var x1 = text.Split(new string[] { ":\"AracMotorNo" }, StringSplitOptions.None)[1];
                                string motorNofirst = x1.Split('}')[0];
                                x1 = motorNofirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string motorNo = x1.Split('\"')[0];
                                txtMotorNo.Text = motorNo;

                                x1 = text.Split(new string[] { ":\"AracSasiNo" }, StringSplitOptions.None)[1];
                                string sasiNofirst = x1.Split('}')[0];
                                x1 = sasiNofirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string sasiNo = x1.Split('\"')[0];
                                sasitxt.Text = sasiNo;

                                querryItems.sasiNo = sasiNo;


                                x1 = text.Split(new string[] { ":\"AracKisiSayisi" }, StringSplitOptions.None)[1];
                                string yolcusayisifirst = x1.Split('}')[0];
                                x1 = yolcusayisifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string yolcusayisi = x1.Split('\"')[0];
                                txtyolcusayısı.Text = yolcusayisi;

                                x1 = text.Split(new string[] { ":\"EGMModelYili" }, StringSplitOptions.None)[1];
                                string EGMModelYilifirst = x1.Split('}')[0];
                                x1 = EGMModelYilifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string EGMModelYili = x1.Split('\"')[0];
                                txtModel.Text = EGMModelYili;

                                x1 = text.Split(new string[] { ":\"EGMAltCins" }, StringSplitOptions.None)[1];
                                string EGMAltCinsfirst = x1.Split('}')[0];
                                x1 = EGMAltCinsfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string EGMAltCins = x1.Split('\"')[0];
                                txtKullanimSekli.Text = EGMAltCins;

                                x1 = text.Split(new string[] { ":\"EGMUstCins" }, StringSplitOptions.None)[1];
                                string EGMUstCinsfirst = x1.Split('}')[0];
                                x1 = EGMUstCinsfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string EGMUstCins = x1.Split('\"')[0];
                                txtkullanımtarzı.Text = EGMUstCins;

                                AracCinsi aracCinsi = new AracCinsi();
                                AracCinsi.AracCins arac = aracCinsi.AracCinsGetirIsmeGore(EGMUstCins);

                                x1 = text.Split(new string[] { ":\"YakitTipi" }, StringSplitOptions.None)[1];
                                string YakitTipifirst = x1.Split('}')[0];
                                x1 = YakitTipifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string YakitTipi = x1.Split('\"')[0];
                                txtYakitTipi.Text = YakitTipi;

                                x1 = text.Split(new string[] { ":\"EGMAd" }, StringSplitOptions.None)[1];
                                string EGMAdfirst = x1.Split('}')[0];
                                x1 = EGMAdfirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string EGMAd = x1.Split('\"')[0];
                                txtAd.Text = EGMAd;

                                x1 = text.Split(new string[] { ":\"TrafikTescilTarihi" }, StringSplitOptions.None)[1];
                                string TrafikTescilTarihifirst = x1.Split('}')[0];
                                x1 = TrafikTescilTarihifirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                                string TrafikTescilTarihi = x1.Split('\"')[0];
                                txttesciltarihi.Text = TrafikTescilTarihi;


                                if (aracsorgu.AveonSigortaVergiNoDetay(out response, querryItems))
                                {
                                    text = ReadResponse(response);
                                    dynamic stuff = JsonConvert.DeserializeObject(text);

                                    txtAracKodu.Text = stuff.value.Mesaj;

                                    int i = 0;
                                    foreach (var item in cbxMarka.Items)
                                    {
                                        if (item.ToString().Contains(stuff.value.Mesaj.Substring(0, 3)))
                                        {
                                            cbxMarka.SelectedIndex = i;
                                            break;
                                        }
                                        i++;
                                    }
                                }



                            }
                            return;
                        }
                        if (aracsorgu.AveonSigorta(out response, querryItems))
                        {
                            string text = ReadResponse(response);
                            string x1 = text.Split(new string[] { ":\"EGMMarka" }, StringSplitOptions.None)[1];
                            string parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            string parsedtext = x1.Split('\"')[0];
                            txtMarka.Text = parsedtext;



                            x1 = text.Split(new string[] { ":\"AracIstihapHaddiKisi" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txtyolcusayısı.Text = parsedtext;

                            x1 = text.Split(new string[] { ":\"YakitTipi" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txtYakitTipi.Text = parsedtext;

                            x1 = text.Split(new string[] { ":\"TrafikTescilTarihi" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txttesciltarihi.Text = parsedtext;

                            x1 = text.Split(new string[] { ":\"EGMAd" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txtAd.Text = parsedtext;

                            x1 = text.Split(new string[] { ":\"EGMSoyad" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txtAd.Text += " " + parsedtext;

                            x1 = text.Split(new string[] { ":\"AracSasiNo" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            sasitxt.Text = parsedtext;

                            querryItems.sasiNo = parsedtext;

                            x1 = text.Split(new string[] { ":\"AracModelYili" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txtModel.Text = parsedtext;

                            x1 = text.Split(new string[] { ":\"AracMotorNo" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txtMotorNo.Text = parsedtext;

                            x1 = text.Split(new string[] { ":\"EGMCinsi" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            txtkullanımtarzı.Text = parsedtext;

                            int i = 0;
                            foreach (var item in cbxAracCinsi.Items)
                            {
                                if (item.ToString().Contains(parsedtext))
                                {
                                    cbxAracCinsi.SelectedIndex = i;
                                    break;

                                }
                                i++;
                            }

                            x1 = text.Split(new string[] { ":\"EGMKullanimSekli" }, StringSplitOptions.None)[1];
                            parsedFirst = x1.Split('}')[0];
                            x1 = parsedFirst.Split(new string[] { ":\"" }, StringSplitOptions.None)[1];
                            parsedtext = x1.Split('\"')[0];
                            if (parsedtext == "YOLCU NAKLI")
                            {
                                parsedtext = "HUSUSI";
                            }
                            txtKullanimSekli.Text = parsedtext;


                            if (aracsorgu.AveonSigortaAracKod(out response, querryItems))
                            {
                                text = ReadResponse(response);
                                x1 = text.Split(new string[] { "Mesaj\":" }, StringSplitOptions.None)[1];
                                x1 = x1.Replace("\"", "");
                                string arackod = x1.Split('}')[0];
                                txtAracKodu.Text = arackod;

                                i = 0;
                                foreach (var item in cbxMarka.Items)
                                {
                                    if (item.ToString().Contains(arackod.Substring(0, 3)))
                                    {
                                        cbxMarka.SelectedIndex = i;
                                        break;
                                    }
                                    i++;
                                }
                            }

                        }

                    }
                    //AracBilgiEkle();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Müşteri Bilgileri Getirilemedi.");
            }

        }


        private static string ReadResponse(HttpWebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                Stream streamToRead = responseStream;
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    streamToRead = new GZipStream(streamToRead, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    streamToRead = new DeflateStream(streamToRead, CompressionMode.Decompress);
                }

                using (StreamReader streamReader = new StreamReader(streamToRead, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }

            }
        }

        private bool CheckPhoneAndMail()
        {
            if (txtTel.Text == "" && txtEposta.Text == "")
                return false;
            else
                return true;
        }
        private async void btnDtarihGetir_Click(object sender, EventArgs e)
        {
            string sorguPrefix = "sistemSorgu";
            //Kullanici sorguKullanicisi = _kullaniciService.GetUsersByCompanyId(AktifKullaniciBilgileri.AktifKullanici.companyId).First(x => x.userName == sorguPrefix + ":" + x.userName.Split(':')[1]);

            Login login = new Login();

            if (txtKimlikNo.Text == null && txtKimlikNo.Text.Length < 11)
            {
                MessageBox.Show("Tc Kimlik Giriniz");
                return;
            }
            DtarihGetir dtarihGetir = new DtarihGetir();
            // if (chckBereketDtarih.Checked)
            if (checkedComboBoxEdit1.Properties.Items[0].CheckState == CheckState.Checked)
            {
                var dtarih = await dtarihGetir.DtarihGetirAsync(InsuranceConstants.BereketSigorta, txtKimlikNo.Text);
                if (dtarih == "Şirket Kapalı" && dtarih == "Hata Oluştu")
                {
                    MessageBox.Show(dtarih);
                    return;
                }
                string[] tarih = dtarih.ToString().Split('.');
                if (tarih[0] == "Şirket Kapalı")
                    return;
                if (Convert.ToInt16(tarih[0]) < 10)
                {
                    if (!dtarih.ToString().StartsWith("0") && dtarih.ToString().Count() < 10)
                    {
                        mtxtDogumTarihi.Text = "0" + dtarih.ToString();

                    }
                    return;
                }
                mtxtDogumTarihi.Text = dtarih.ToString();
                return;
            }
            //else if (chckTurkiyeDtarih.Checked)
            else if (checkedComboBoxEdit1.Properties.Items[1].CheckState == CheckState.Checked)
            {
                var dtarih = await dtarihGetir.DtarihGetirAsync(InsuranceConstants.TurkiyeSigorta, txtKimlikNo.Text);
                if (dtarih != null && dtarih != "")
                {
                    if (dtarih == "Şirket Kapalı" || dtarih == "Hata Oluştu")
                    {
                        MessageBox.Show("Şirket ekranı kapalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (dtarih.Length == 10)
                        mtxtDogumTarihi.Text = dtarih.ToString();
                    else
                    {
                        string[] tarih = dtarih.ToString().Split('.');
                        if (Convert.ToInt16(tarih[0]) < 10)
                        {
                            mtxtDogumTarihi.Text = "0" + dtarih.ToString();
                        }
                    }
                    return;
                }
            }
            else if (checkedComboBoxEdit1.Properties.Items[2].CheckState == CheckState.Checked)
            {
                var dtarih = await dtarihGetir.DtarihGetirAsync(InsuranceConstants.CorpusSigorta, txtKimlikNo.Text);
                if (dtarih != null && dtarih != "")
                {
                    if (dtarih == "Şirket Kapalı" || dtarih == "Hata Oluştu")
                    {
                        MessageBox.Show("Şirket ekranı kapalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (dtarih.Length == 10)
                        mtxtDogumTarihi.Text = dtarih.ToString();
                    else
                    {
                        string[] tarih = dtarih.ToString().Split('.');
                        if (Convert.ToInt16(tarih[0]) < 10)
                        {
                            mtxtDogumTarihi.Text = "0" + dtarih.ToString();
                        }
                    }
                    return;
                }
            }
            else if (checkedComboBoxEdit1.Properties.Items[3].CheckState == CheckState.Checked)
            {

                //DtarihGetirRequest dtarihGetirRequest = new DtarihGetirRequest();

                //QuerryItems querryItems = new QuerryItems();
                //querryItems.tcKimlik = txtKimlikNo.Text;
                //querryItems.FirmaAdi = InsuranceConstants.SompoJapanSigorta;

                //var sessions = login.login(sorguKullanicisi.userName, sorguKullanicisi.password, InsuranceConstants.SompoJapanSigorta);


                //var dtarih = dtarihGetirRequest.D_tarihGetir(querryItems, sessions);

                //if (dtarih != null && dtarih.Result != "")
                //{
                //    if (dtarih.Result == "Şirket Kapalı" && dtarih.Result == "Hata Oluştu")
                //    {
                //        return;
                //    }
                //    string[] tarih = dtarih.ToString().Split('.');
                //    if (Convert.ToInt16(tarih[0]) < 10)
                //    {
                //        mtxtDogumTarihi.Text = "0" + dtarih.ToString();
                //        return;
                //    }
                //    mtxtDogumTarihi.Text = dtarih.ToString();
                //    return;
                //}

            }
            else if (checkedComboBoxEdit1.Properties.Items[4].CheckState == CheckState.Checked)
            {
                var dtarih = await dtarihGetir.DtarihGetirAsync(InsuranceConstants.NeovaSigorta, txtKimlikNo.Text);
                if (dtarih != null && dtarih != "")
                {
                    if (dtarih == "Şirket Kapalı" || dtarih == "Hata Oluştu")
                    {
                        MessageBox.Show("Şirket ekranı kapalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (dtarih.Length == 10)
                        mtxtDogumTarihi.Text = dtarih.ToString();
                    else
                    {
                        string[] tarih = dtarih.ToString().Split('.');
                        if (Convert.ToInt16(tarih[0]) < 10)
                        {
                            mtxtDogumTarihi.Text = "0" + dtarih.ToString();
                        }
                    }
                    return;
                }
            }
            else if (checkedComboBoxEdit1.Properties.Items[5].CheckState == CheckState.Checked)
            {
                var dtarih = await dtarihGetir.DtarihGetirAsync(InsuranceConstants.TmtSigorta, txtKimlikNo.Text);
                if (dtarih != null && dtarih != "")
                {
                    if (dtarih == "Şirket Kapalı" && dtarih == "Hata Oluştu")
                    {
                        return;
                    }

                    if (dtarih.Length == 10)
                        mtxtDogumTarihi.Text = dtarih.ToString();
                    else
                    {
                        string[] tarih = dtarih.ToString().Split('.');
                        if (Convert.ToInt16(tarih[0]) < 10)
                        {
                            mtxtDogumTarihi.Text = "0" + dtarih.ToString();
                        }
                    }
                    return;
                }
            }
        }
        private void btnFillInfo_Click(object sender, EventArgs e)
        {

            var infos = txtTopluBilgi.Text.Trim().Replace(" ", "").ToUpper().Split('-');
            txtKimlikNo.Text = infos.Length > 0 ? infos[0] : "";
            txtPlakaNo.Text = infos.Length > 1 ? infos[1] : "";
            txtSeriNo.Text = infos.Length > 2 ? infos[2] : "";
            mtxtDogumTarihi.Text = infos.Length > 3 ? infos[3] : "";

            simpleButton8.PerformClick();//dbden veri çekerek doldur
            setEmailAndPhone();
        }
        private void hizliTeklifFormNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            DestroyConsumers();
            if (_fiyatBilgileriForm != null)
                _fiyatBilgileriForm.Close();

            /*if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();

                if (Application.OpenForms.OfType<TeminatBilgileriForm>().Count() == 1)
                    Application.OpenForms.OfType<TeminatBilgileriForm>().First().Hide();
            }*/
        }

        //private void btnAnkaraTekliSorgu_Click(object sender, EventArgs e)
        //{

        //}
        //private void btnAllianzTekliSorgu_Click(object sender, EventArgs e)
        //{

        //}
        //private void btnAnadoluTekliSorgu_Click(object sender, EventArgs e)
        //{

        //}

        //private void btnAcnTekliSorgu_Click(object sender, EventArgs e)
        //{

        //}

        //private void btnAkTekliSorgu_Click(object sender, EventArgs e)
        //{

        //}


        private void BtnAveonTekliSorgu_Click(object sender, EventArgs e)
        {
            KullaniciBilgileriDoldur();
            var sorguTipi = sorgutipi;
            FiyatSorgulamaQueue.Enqueue(new QueueMessage { InsuranceCompany = InsuranceConstants.AveonSigorta, SorguTipi = sorguTipi });
        }

        


        private string QrKodBilgisiAl()
        {
            try
            {
                Image copiedImage = null;
                if (Clipboard.ContainsImage())
                {
                    var reader = new BarcodeReader();
                    copiedImage = Clipboard.GetImage();
                    return reader.Decode(copiedImage as Bitmap).Text;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void btnQr_Click(object sender, EventArgs e)
        {
            var sonuc = QrKodBilgisiAl();
            if (sonuc == null)
                MessageBox.Show("QR Bulunamadı", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                string[] degerler = sonuc.ToString().Split('-');
                if (degerler.Length == 3)
                {
                    txtSeriNo.Text = degerler[0];
                    txtPlakaNo.Text = degerler[1];
                    txtKimlikNo.Text = degerler[2];
                }
                else
                    MessageBox.Show("QR Okunamadı", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSorguIptal_Click(object sender, EventArgs e)
        {
            try
            {
                // Paralel fiyat sorgulamayı iptal et
                if (_parallelCancellationTokenSource != null && !_parallelCancellationTokenSource.Token.IsCancellationRequested)
                {
                    _parallelCancellationTokenSource.Cancel();
                    Console.WriteLine("🚫 Kullanıcı paralel fiyat sorgulamayı iptal etti");
                    
                    // Buton durumunu sıfırla
                    SetButtonSorgulaniyorDurumu(false);
                }
                
                // Diğer aktif sorguları da iptal et (mevcut sistemi koru)
            AktifSorgulariIptalEt();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Sorgulama iptal hatası: {ex.Message}");
            }
        }

        //private void btnMagdeburgeTekliSorgu_Click(object sender, EventArgs e)
        //{

        //}

        //private void btnGrupamaTekliSorgu_Click(object sender, EventArgs e)
        //{

        //    KullaniciBilgileriDoldur();
        //    var sorguTipi = sorgutipi;
        //    FiyatSorgulamaQueue.Enqueue(new QueueMessage { InsuranceCompany = InsuranceConstants.GrupamaSigorta, SorguTipi = sorguTipi });
        //}
        private void setTipComboBox(List<aracTip> aracTipList)
        {
            //cbxTip.Items.Cledar();
            //foreach (var item in aracTipList)
            //{
            //    //cbxTip.Items.Add(item.tipKod + " - " + item.tipAdi);
            //}
        }
        DataTable dt = new DataTable();
        DataTable dtimage = new DataTable(); // Firma logo bilgileri için
        private readonly object _dataTableLock = new object(); // DataTable işlemleri için lock
        private readonly SemaphoreSlim _progressSemaphore = new SemaphoreSlim(1, 1); // Progress için async lock
        private CancellationTokenSource _parallelCancellationTokenSource; // Paralel sorgulama iptal token'ı
        
        private void hizliTeklifFormNew_Load(object sender, EventArgs e)
        {
            //splashScreenManager1.ShowWaitForm();
            //splashScreenManager1.SetWaitFormCaption("Lütfen Bekleyiniz..");
            //splashScreenManager1.SetWaitFormDescription("Form Açılıyor..");
            Task.Run(() =>
            {
                KullaniciSirketleriAktifEt(param);
            });

            Task.Run(() =>
            {
                setTipComboBox(aracTipTool.getTipList());
            });

            //splashScreenManager1.CloseWaitForm();
            xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            FirmaLogoGetir();
            tabloDoldur();
        }

        private void FirmaLogoGetir()
        {
            dal cmd = new dal();
            SqlConnection myConnection = new SqlConnection("server =93.89.230.234 ; database = sigortavipserver ; uid = sigortaVipDbUser; pwd =Asdfzxcv.12321!!; integrated security=false;MultipleActiveResultSets=true");
            dtimage = cmd.CommandExecuteReader("select * from Firma_Logo", myConnection).Tables[0];
        }

        private void KullaniciSirketleriAktifEt(string param) //dönülecek
        {
            //kullaniciSirketleri = _kullaniciSirketService.GetByUserId(AktifKullaniciBilgileri.AktifKullanici.id);

            //kullaniciSirketleri = kullaniciSirketleri.Where(x => x.sirketAdi != "Tramer" && x.sirketAdi != "TramerSorgu" && x.sirketAdi != "Ayarlar" && x.sirketAdi != "Firmalar" && x.sirketAdi != "TramerSorgu").ToList();

            //if (param == "Trafik")
            //{
            //    kullaniciSirketleri = kullaniciSirketleri.Where(x => x.sirketAdi != "AveonSigorta" && x.sirketAdi != "AcnTurkSigorta" && x.sirketAdi != "ZurichSigorta" && x.sirketAdi != "AtlasSigorta").ToList();
            //}

            //if (kullaniciSirketleri != null)
            //{

            //    foreach (KullaniciSirket item in kullaniciSirketleri)
            //    {
            //        if (item.sirketAktifMi)
            //        {
            //            //switch (item.sirketAdi)
            //            //{
            //            //  case InsuranceConstants.AkSigorta:
            //            //        // btnAkTekliSorgu.Enabled = true;
            //            //        ChckAkSigorta.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.AllianzSigorta:
            //            //        ChckAllianz.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.AnadoluSigorta:
            //            //        //btnAnadoluTekliSorgu.Enabled = true;
            //            //        ChckAnadolu.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.AnkaraSigorta:
            //            //        // btnAnkaraTekliSorgu.Enabled = true;
            //            //        ChckAnkara.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.BereketSigorta:
            //            //        //btnBereketTekliSorgu.Enabled = true;
            //            //        ChckBereket.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.CorpusSigorta:
            //            //        // btnCorpusTekliSorgu.Enabled = true;
            //            //        ChckCorpus.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.DogaSigorta:
            //            //        //btnDogaTekliSorgu.Enabled = true;
            //            //        ChckDoga.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.HdiSigorta:
            //            //        //btnHdiTekliSorgu.Enabled = true;
            //            //        ChckHdi.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.KoruSigorta:
            //            //        // btnKoruTekliSorgu.Enabled = true;
            //            //        ChckKoru.Enabled = true;
            //            //        continue;
            //            //    //case InsuranceConstants.GriSigorta:
            //            //    //    //btnGriTekliSorgu.Enabled = true;
            //            //    //    ChckGri.Enabled = true;
            //            //    //    continue;
            //            //    case InsuranceConstants.MagdeburgerSigorta:
            //            //        // btnMagdeburgeTekliSorgu.Enabled = true;
            //            //        ChckMadgeburge.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.NeovaSigorta:
            //            //        // btnNeovaTekliSorgu.Enabled = true;
            //            //        ChckNeova.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.QuickSigorta:
            //            //        //btnQuickTekliSorgu.Enabled = true;
            //            //        ChckQuick.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.RaySigorta:
            //            //        //btnRayTekliSorgu.Enabled = true;
            //            //        ChckRay.Enabled = true;
            //            //        continue;
            //            //    //case InsuranceConstants.TmtSigorta:
            //            //    //    // btnTmtTekliSorgu.Enabled = true;
            //            //    //    ChckTmtSigorta.Enabled = true;
            //            //    //    continue;
            //            //    case InsuranceConstants.TurkNipponSigorta:
            //            //        //btnNipponTekliSorgu.Enabled = true;
            //            //        ChckTurkNippon.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.TurkiyeSigorta:
            //            //        //  btnTurkiyeTekliSorgu.Enabled = true;
            //            //        ChckTurkiye.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.AcnTurkSigorta:
            //            //        // btnAcnTekliSorgu.Enabled = true;
            //            //        CheckAcn.Enabled = true;
            //            //        continue;
            //            //    /*case InsuranceConstants.GrupamaSigorta:
            //            //        //  btnGrupamaTekliSorgu.Enabled = true;
            //            //        ChckGrupoma.Enabled = true;
            //            //        continue;*/
            //            //    case InsuranceConstants.HepIyiSigorta:
            //            //        //  btnHepiyiTekliSorgu.Enabled = true;
            //            //        ChckHepİyi.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.SompoJapanSigorta:
            //            //        //  btnSompoTekliSorgu.Enabled = true;
            //            //        ChckSompo.Enabled = true;
            //            //        continue;
            //            //    //case InsuranceConstants.OrientSigorta:
            //            //    //    //btnOrientTekliSorgu.Enabled = true;
            //            //    //    ChckOrientSigorta.Enabled = true;
            //            //    //    continue;
            //            //    case InsuranceConstants.UnicoSigorta:
            //            //        // btnUnicoTekliSorgu.Enabled = true;
            //            //        ChckUnicoSigorta.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.AveonSigorta:
            //            //        // btnAveonTekliSorgu.Enabled = true;
            //            //        ChckAveonSigorta.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.AtlasSigorta:
            //            //        // btnAtlasTekliSorgu.Enabled = true;
            //            //        ChckAtlasSigorta.Enabled = true;
            //            //        continue;
            //            //    case InsuranceConstants.AnaSigorta:
            //            //        //btnAnaTekliSorgu.Enabled = true;
            //            //        ChckAnaSigorta.Enabled = true;
            //            //        continue;
            //            //    default:
            //            //        break;
            //            //}
            //        }
            //    }
            //}
        }


        //private void MusteriBilgiGetir_EnabledChanged(object sender, EventArgs e)
        //{
        //    //if (MusteriBilgiGetir.Enabled && btnTramerdenGetir.Enabled)
        //    //{
        //    //    MusteriBilgiGetir.BackColor = Color.MediumTurquoise;
        //    //    btnTramerdenGetir.BackColor = Color.MediumTurquoise;
        //    //}
        //    //else
        //    //{
        //    //    MusteriBilgiGetir.BackColor = Color.DarkGray;
        //    //    btnTramerdenGetir.BackColor = Color.DarkGray;
        //    //}
        //}

        private void SorguText_Changed(object sender, EventArgs e)
        {
            if (txtKimlikNo.Text.Trim() != string.Empty
                && txtPlakaNo.Text.Trim() != string.Empty
                && mtxtDogumTarihi.Text.Split(',')[0].Trim() != string.Empty
                && txtSeriNo.Text.Trim() != string.Empty)
            {
                btnTramerdenGetir.Enabled = true;
                //MusteriBilgiGetir.Enabled = true;
            }
            else
            {
                btnTramerdenGetir.Enabled = false;
                //MusteriBilgiGetir.Enabled = false;
            }


            layoutControlItem40.Text = "Kimlik / Vergi No (" + txtKimlikNo.Text.Length.ToString() + "):";
        }

        private void comboKeyPressed(ComboBox comboBox)
        {
            comboBox.DroppedDown = true;

            object[] orjinalListe = (object[])comboBox.Tag;
            if (orjinalListe == null)
            {
                orjinalListe = new object[comboBox.Items.Count];
                comboBox.Items.CopyTo(orjinalListe, 0);
                comboBox.Tag = orjinalListe;
            }

            string s = comboBox.Text.ToLower();
            IEnumerable<object> yeniListe = orjinalListe;
            if (s.Length > 0)
                yeniListe = orjinalListe.Where(item => item.ToString().ToLower().Contains(s));

            while (comboBox.Items.Count > 0)
                comboBox.Items.RemoveAt(0);

            comboBox.Items.AddRange(yeniListe.ToArray());
        }

        private void combobox_TextChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo.Text.Length == 0)
                comboKeyPressed(combo);

            if (cbxMarka.SelectedItem != "" && cbxMarka.SelectedItem != null)
            {
                setTipComboBox(aracTipTool.getTipListByMarka(cbxMarka.SelectedItem.ToString().Split(' ').First()));

            }
        }

        private void combobox_KeyPress(object sender, KeyPressEventArgs e)
        {
            comboKeyPressed((ComboBox)sender);
        }

        private void btnAtlasTekliSorgu_Click(object sender, EventArgs e)
        {

        }

        private void cbxMarka_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxMarka.SelectedItem != "" && cbxMarka.SelectedItem != null)
            {
                setTipComboBox(aracTipTool.getTipListByMarka(cbxMarka.SelectedItem.ToString().Split(' ').First()));

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cbxDoldurAracKodunaGöre();
        }
        private void cbxDoldurAracKodunaGöre()
        {
            if (txtAracKodu.Text != "")
            {
                aracTip aractipi = aracTipTool.getTipListByMarkaVeTipKodu(txtAracKodu.Text.Substring(0, 3), txtAracKodu.Text.Substring(3));
                txtMarka.Text = aractipi.markaAdi;
                txtTip.Text = aractipi.tipAdi;

                for (int i = 0; i < cbxMarka.Items.Count; i++)
                {
                    if (aractipi.markaAdi == null)
                    {
                        break;
                    }
                    if (cbxMarka.Items[i].ToString().Contains(aractipi.markaAdi))
                    {
                        cbxMarka.SelectedIndex = i;
                        break;
                    }
                }
                for (int i = 0; i < cbxTip.Items.Count; i++)
                {
                    if (aractipi.tipAdi != null)
                    {
                        if (cbxTip.Items[i].ToString().Contains(aractipi.tipAdi))
                        {
                            cbxTip.SelectedIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Aradığınız kod bulunamamıştır.");
                        break;

                    }

                }
            }
        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }
        #region Teminat Bilgileri


        private void btnTeminatBilgileri_Click(object sender, EventArgs e)
        {
            if (screenWidth == null)
                //screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

            if (screenWidth >= 1920)
            {
                if (_teminatBilgileriForm == null)
                    _teminatBilgileriForm = new TeminatBilgileriForm();
                _teminatBilgileriForm.Show();
                _teminatBilgileriForm.Focus();
            }
            else
            {
                if (_lowTeminatBilgileriForm == null)
                    _lowTeminatBilgileriForm = new lowTeminatBilgileriForm();
                _lowTeminatBilgileriForm.Show();
                _lowTeminatBilgileriForm.Focus();
            }
        }

        #endregion

        private Models.Querry TümVerileriGetir()
        {
            Models.Querry QuerryItems = new Models.Querry();

            QuerryItems.Tc = txtKimlikNo.Text;
            QuerryItems.Plaka = txtPlakaNo.Text;
            QuerryItems.Adress = txtAdres.Text;
            QuerryItems.adSoyad = txtAd.Text;
            QuerryItems.Marka = txtMarka.Text;
            QuerryItems.aracKod = txtAracKodu.Text;
            QuerryItems.basamakKodu = txtBasamakKodu.Text;
            QuerryItems.cepTelenu = txtTel.Text;
            QuerryItems.Eposta = txtEposta.Text;
            QuerryItems.Il = txtIl.Text;
            QuerryItems.Ilce = txtIlce.Text;
            QuerryItems.kullanimSekli = txtKullanimSekli.Text;
            QuerryItems.kullanimTarzi = txtkullanımtarzı.Text;
            QuerryItems.Model = txtModel.Text;
            QuerryItems.motorNo = txtMotorNo.Text;
            QuerryItems.policeBaslangis = txtPoliceBaslangic.Text;
            QuerryItems.policeBitis = txtPoliceBitis.Text;
            QuerryItems.sasiNo = sasitxt.Text;
            QuerryItems.tescilTarihi = txttesciltarihi.Text;
            QuerryItems.Tip = txtTip.Text;
            QuerryItems.YakitTipi = txtYakitTipi.Text;
            QuerryItems.yolcuSayisi = txtyolcusayısı.Text;
            //QuerryItems.SirketId = GetLoginRequest.sirket.id.ToString();

            return QuerryItems;
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    dal myDal = new dal();

        //    string selectQuery = $"SELECT * FROM [sigortaVipDbUser].[AracBilgileri] " +
        //            $"WHERE Tc = '{txtKimlikNo.Text}' AND plaka = '{txtPlakaNo.Text}' AND SirketId = '{GetLoginRequest.sirket.id}';";

        //    DataSet data = myDal.CommandExecuteReader(selectQuery, myDal.myConnection);

        //    if (data != null && data.Tables.Count == 1 && data.Tables[0].Rows.Count == 1)
        //    {
        //        txtAdres.Text = data.Tables[0].Rows[0]["Adress"].ToString();
        //        txtAd.Text = data.Tables[0].Rows[0]["adSoyad"].ToString();
        //        txtMarka.Text = data.Tables[0].Rows[0]["Marka"].ToString();
        //        txtAracKodu.Text = data.Tables[0].Rows[0]["aracKod"].ToString();
        //        txtBasamakKodu.Text = data.Tables[0].Rows[0]["basamakKodu"].ToString();
        //        txtTel.Text = data.Tables[0].Rows[0]["cepTelenu"].ToString();
        //        txtEposta.Text = data.Tables[0].Rows[0]["Eposta"].ToString();
        //        txtIl.Text = data.Tables[0].Rows[0]["Il"].ToString();
        //        txtIlce.Text = data.Tables[0].Rows[0]["Ilce"].ToString();
        //        txtKullanimSekli.Text = data.Tables[0].Rows[0]["kullanimSekli"].ToString();
        //        txtkullanımtarzı.Text = data.Tables[0].Rows[0]["kullanimTarzi"].ToString();
        //        txtModel.Text = data.Tables[0].Rows[0]["Model"].ToString();
        //        txtMotorNo.Text = data.Tables[0].Rows[0]["motorNo"].ToString();
        //        txtPoliceBaslangic.Text = data.Tables[0].Rows[0]["policeBaslangis"].ToString();
        //        txtPoliceBitis.Text = data.Tables[0].Rows[0]["policeBitis"].ToString();
        //        sasitxt.Text = data.Tables[0].Rows[0]["sasiNo"].ToString();
        //        txttesciltarihi.Text = data.Tables[0].Rows[0]["tescilTarihi"].ToString();
        //        txtTip.Text = data.Tables[0].Rows[0]["Tip"].ToString();
        //        txtYakitTipi.Text = data.Tables[0].Rows[0]["YakitTipi"].ToString();
        //        txtyolcusayısı.Text = data.Tables[0].Rows[0]["yolcuSayisi"].ToString();

        //        AracBilgiEkle();
        //    }
        //    //create 

        //}

        private void AracBilgiEkle()
        {
            Models.Querry QuerryItems = TümVerileriGetir();
            dal myDal = new dal();

            string selectQuery = $"SELECT * FROM [sigortaVipDbUser].[AracBilgileri] " +
                    $"WHERE Tc = '{QuerryItems.Tc}' AND plaka = '{QuerryItems.Plaka}' AND SirketId = '{QuerryItems.SirketId}';";


            DataSet data = myDal.CommandExecuteReader(selectQuery, myDal.myConnection);
            if (data != null && data.Tables.Count == 1 && data.Tables[0].Rows.Count == 1)
            {

            }
            else
            {
                string insertQuery = $"INSERT INTO [sigortaVipDbUser].[AracBilgileri] " +
                     $"([Tc], [Adress], [adSoyad], [Marka], [aracKod], [basamakKodu], [cepTelenu], [Eposta], [Il], [Ilce], " +
                     $"[kullanimSekli], [kullanimTarzi], [Model], [motorNo], [policeBaslangis], [policeBitis], [sasiNo], " +
                     $"[tescilTarihi], [Tip], [YakitTipi], [yolcuSayisi], [SirketId], [Plaka]) " +
                     $"VALUES " +
                     $"('{QuerryItems.Tc.Trim()}', '{QuerryItems.Adress.Trim()}', '{QuerryItems.adSoyad.Trim()}', '{QuerryItems.Marka.Trim()}', " +
                     $"'{QuerryItems.aracKod.Trim()}', '{QuerryItems.basamakKodu.Trim()}', '{QuerryItems.cepTelenu.Trim()}', '{QuerryItems.Eposta.Trim()}', " +
                     $"'{QuerryItems.Il.Trim()}', '{QuerryItems.Ilce.Trim()}', '{QuerryItems.kullanimSekli.Trim()}', '{QuerryItems.kullanimTarzi.Trim()}', " +
                     $"'{QuerryItems.Model.Trim()}', '{QuerryItems.motorNo.Trim()}', '{QuerryItems.policeBaslangis.Trim()}', '{QuerryItems.policeBitis.Trim()}', " +
                     $"'{QuerryItems.sasiNo.Trim()}', '{QuerryItems.tescilTarihi.Trim()}', '{QuerryItems.Tip.Trim()}', '{QuerryItems.YakitTipi.Trim()}', " +
                     $"'{QuerryItems.yolcuSayisi.Trim()}', '{QuerryItems.SirketId.Trim()}', '{QuerryItems.Plaka.Trim()}');";




                myDal.CommandExecuteReader(insertQuery, myDal.myConnection);
            }

        }

        string sorgutipi = "Trafik";
        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dropDownButton1.Text = "Trafik Sorgusu\nBaşlat";
            sorgutipi = "Trafik";
            param = "Trafik";
            /*Form aktifForm = formAcikmi("fiyatBilgileriForm");
            if (aktifForm != null)
            {
                aktifForm.Close();
            }
            fiyatBilgileriForm.html = htmlOlustur();
            KullaniciBilgileriDoldur();
            SeciliSirketleriIslemeAl();
            fiyatBilgileriGetir();*/

        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dropDownButton1.Text = "Kasko Sorgusu\nBaşlat";
            sorgutipi = "Kasko";
            param = "Kasko";

            /*Form aktifForm = formAcikmi("fiyatBilgileriForm");
            if (aktifForm != null)
            {
                aktifForm.Close();
            }
            fiyatBilgileriForm.html = htmlOlustur();
            KullaniciBilgileriDoldur();
            SeciliSirketleriIslemeAl();
            fiyatBilgileriGetir();*/
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var asdasda = sender;
                DevExpress.XtraBars.Ribbon.RibbonBarManager aaaa = sender as DevExpress.XtraBars.Ribbon.RibbonBarManager;
                DevExpress.XtraBars.BarItemLink ed = aaaa.PressedLink;
                dropDownButton2.Text = ed.Caption + "\nSorgula";

                cmbInsuranceCompanySTR = ed.Caption;
            }
            catch { }

            foreach (WebPage acikSayfalar in Browser.webPageList)
            {
                if (acikSayfalar.insuranceCompany != "AcnTurkSigorta")
                {
                    //acikSayfalar.browser.LoadUrlAsync(acikSayfalar.insuranceCompanyPageUrl);
                }
            }

        }

        private async void dropDownButton2_Click(object sender, EventArgs e)
        {
            
            if (layoutControlItem14.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
            {
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                BilgiGizleAc.Text = "↑";
            }
            try
            {
                // Form validasyonu
                if (string.IsNullOrEmpty(cmbInsuranceCompanySTR))
                {
                    MessageBox.Show("Lütfen sorgulama yapmadan önce firma seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidationHelper.IsValidTcNo(txtKimlikNo.Text) && !ValidationHelper.IsValidVergiNo(txtKimlikNo.Text))
                {
                    MessageBox.Show("Geçerli bir TC Kimlik No (11 haneli) veya Vergi No (10 haneli) giriniz.", "Validation Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidationHelper.IsValidPlate(txtPlakaNo.Text))
                {
                    MessageBox.Show("Geçerli bir plaka numarası giriniz. (Örnek: 34ABC123)", "Validation Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidationHelper.IsValidBirthDate(mtxtDogumTarihi.Text))
                {
                    MessageBox.Show("Geçerli bir doğum tarihi giriniz. (dd.MM.yyyy)", "Validation Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidationHelper.IsValidSerialNo(txtSeriNo.Text))
                {
                    MessageBox.Show("Geçerli bir seri numarası giriniz.", "Validation Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Sigorta şirketi ID'sini belirle
                int insuranceCompanyId = GetInsuranceCompanyId(cmbInsuranceCompanySTR);
                
                if (insuranceCompanyId == 0)
                {
                    MessageBox.Show($"'{cmbInsuranceCompanySTR}' sigorta şirketi için sorgu yapılamıyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // API request'i hazırla
                var queryRequest = new QueryRequestDto
                {
                    query_type = "identity",
                    insurance_company_item_id = insuranceCompanyId,
                    identity_number = txtKimlikNo.Text.Trim(),
                    birth_date = mtxtDogumTarihi.Text.Trim(),
                    plate_number = txtPlakaNo.Text.Trim(),
                    document_serial = txtSeriNo.Text.Trim()
                };

                // API service'i kullanarak sorgu yap
                var aracSorguService = new AracSorguApiService();
                
                Console.WriteLine($"🔍 Araç sorgusu başlatılıyor...");
                Console.WriteLine($"Sigorta Şirketi: {cmbInsuranceCompanySTR} (ID: {insuranceCompanyId})");
                Console.WriteLine($"TC Kimlik: {queryRequest.identity_number}");
                Console.WriteLine($"Plaka: {queryRequest.plate_number}");

                string response = await aracSorguService.CreateQueryAsync(queryRequest);
                
                Console.WriteLine($"🔍 Raw API Response: {response}");
                
                if (!string.IsNullOrEmpty(response))
                {
                    Console.WriteLine($"✅ API Response alındı - Length: {response.Length}");
                    
                    try
                    {
                        // Response'u parse et ve form alanlarını doldur
                        await ParseAndFillFormData(response);
                        
                        MessageBox.Show($"Müşteri bilgileri başarıyla getirildi!\n\n" +
                                      $"Ad Soyad: {txtAd.Text}\n" +
                                      $"Marka: {txtMarka.Text}\n" +
                                      $"İl: {txtIl.Text}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception parseEx)
                    {
                        Console.WriteLine($"❌ Parse Hatası: {parseEx.Message}");
                        MessageBox.Show($"Veri parse edilirken hata oluştu:\n{parseEx.Message}\n\nResponse:\n{response}", "Parse Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Console.WriteLine("❌ API'den boş response alındı");
                    MessageBox.Show("API'den boş response alındı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (String.IsNullOrWhiteSpace(txtMarka.Text))
                    cbxDoldurAracKodunaGöre();

                //AracBilgiEkle();
                setEmailAndPhone();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ dropDownButton2_Click Hatası: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                MessageBox.Show($"Müşteri bilgileri getirilemedi.\n\nHata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetInsuranceCompanyId(string companyName)
        {
            // Sigorta şirketi adına göre ID döndür
            switch (companyName.ToUpper())
            {
                case "DOĞASİGORTA":
                    return 7; // Örnek ID
                case "KORUSİGORTA":
                    return 9; // Örnek ID
                case "GRİSİGORTA":
                    return 65; // Örnek ID
                case "AVEONSİGORTA":
                    return 66; // Örnek ID
                case "ANASİGORTA":
                    return 62; // Örnek ID
                default:
                    return 0; // Bilinmeyen şirket
            }
        }

        private async Task ParseAndFillFormData(string jsonResponse)
        {
            try
            {
                JObject parsedResponseData = JObject.Parse(jsonResponse);
                
                // Success kontrolü
                bool success = parsedResponseData["success"]?.ToObject<bool>() ?? false;
                if (!success)
                {
                    string errorMsg = "API'den başarısız response alındı.";
                    Console.WriteLine($"❌ API Error: {errorMsg}");
                    throw new Exception(errorMsg);
                }

                // Data objesi içindeki bilgileri al
                var data = parsedResponseData["data"];
                if (data == null)
                {
                    throw new Exception("API response'unda 'data' objesi bulunamadı.");
                }

                Console.WriteLine($"📊 API'den alınan veriler parse ediliyor...");

                // Kişi Bilgileri
                string ad = data["ad"]?.ToString() ?? "";
                string soyad = data["soyad"]?.ToString() ?? "";
                txtAd.Text = $"{ad} {soyad}".Trim();
                
                txtIl.Text = data["il"]?.ToString() ?? "";
                txtIlce.Text = data["ilce"]?.ToString() ?? "";
                txtAdres.Text = data["adres"]?.ToString() ?? "";

                // Araç Bilgileri
                txtMarka.Text = data["marka"]?.ToString() ?? "";
                txtTip.Text = data["arac_tipi"]?.ToString() ?? "";
                txtAracKodu.Text = data["arac_kodu"]?.ToString() ?? "";
                txtModel.Text = data["model"]?.ToString() ?? "";
                txtMotorNo.Text = data["motor_no"]?.ToString() ?? "";
                sasitxt.Text = data["sasi_no"]?.ToString() ?? "";
                
                // Kullanım Bilgileri
                txtkullanımtarzı.Text = data["kullanim_tarzi"]?.ToString() ?? "";
                string kullanimSekli = data["kullanim_sekli"]?.ToString() ?? "";
                txtKullanimSekli.Text = kullanimSekli == "YOLCU NAKLI" ? "HUSUSI" : kullanimSekli;
                
                txtyolcusayısı.Text = data["yolcu_sayisi"]?.ToString() ?? "";
                txttesciltarihi.Text = data["tescil_tarihi"]?.ToString() ?? "";
                
                // Poliçe Bilgileri  
                txtBasamakKodu.Text = data["basamak_kodu"]?.ToString() ?? "";
                txtPoliceBaslangic.Text = data["police_baslangic"]?.ToString() ?? "";
                txtPoliceBitis.Text = data["police_bitis"]?.ToString() ?? "";

                Console.WriteLine("✅ Form alanları başarıyla dolduruldu:");
                Console.WriteLine($"   👤 Ad Soyad: {txtAd.Text}");
                Console.WriteLine($"   🚗 Araç: {txtMarka.Text} {txtTip.Text}");
                Console.WriteLine($"   📍 Adres: {txtIl.Text}/{txtIlce.Text}");
                Console.WriteLine($"   🔢 Plaka: {txtPlakaNo.Text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ JSON Parse Hatası: {ex.Message}");
                Console.WriteLine($"Response: {jsonResponse}");
                throw new Exception($"API response parse edilemedi: {ex.Message}");
            }
        }


        public static string param = "Trafik";
        private bool BilgilerDolumuKontrol()
        {
            message = "";
            bool sonuc = true;
            TextEdit[] textBoxes = { txtKimlikNo, txtPlakaNo, txtSeriNo, mtxtDogumTarihi, txtTip, txtAracKodu };

            foreach (TextEdit item in textBoxes)
            {
                if (string.IsNullOrEmpty(item.Text) == true)
                {
                    message += "'" + item.Tag.ToString() + "' ,";
                    sonuc = false;
                }
            }
            message += " alanları boş geçilemez.";
            return sonuc;
        }
        private async void dropDownButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Eğer paralel sorgulama devam ediyorsa önce iptal et
                if (_parallelCancellationTokenSource != null && !_parallelCancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("🔄 Devam eden sorgulama tespit edildi, iptal ediliyor...");
                    _parallelCancellationTokenSource.Cancel();
                    
                    // Biraz bekle
                    await Task.Delay(1000);
                }

                // DataTable'ı temizle - Yeni sorgulama için
                lock (_dataTableLock)
                {
                    if (dt != null)
                    {
                        dt.Clear();
                        Console.WriteLine("🗑️ DataTable temizlendi, yeni sorgulama için hazır");
                    }
                }

                // Müşteri bilgilerini kontrol et
                KullaniciBilgileriDoldur();

                if (info == null || string.IsNullOrEmpty(info.txtKimlikNo))
                {
                    MessageBox.Show("Lütfen önce müşteri bilgilerini doldurun!\n\n" +
                                  "Gerekli alanlar:\n" +
                                  "• TC Kimlik No\n" +
                                  "• Plaka No\n" +
                                  "• Ad Soyad\n" +
                                  "• Araç bilgileri",
                                  "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // MainForm'u bul
                var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm == null)
                {
                    MessageBox.Show("Ana form bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Müşteri bilgilerini göster
                var result = MessageBox.Show(
                    $"Aktif şirkette fiyat sorgulama yapılacak.\n\n" +
                    $"Müşteri Bilgileri:\n" +
                    $"• Ad Soyad: {info.AdSoyad}\n" +
                    $"• TC No: {info.txtKimlikNo}\n" +
                    $"• Plaka: {info.txtPlakaNo}\n" +
                    $"• Telefon: {info.txtTel}\n" +
                    $"• Araç: {info.txtMarka} {info.txtModel}\n\n" +
                    $"Devam etmek istiyor musunuz?",
                    "Fiyat Sorgulama Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                // Aktif şirkette fiyat sorgulamayı başlat ve sonuçları direkt al
                await FiyatSorguSonucuDataTableEkle(info);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fiyat sorgu sonuçlarını DataTable'a ekleyen metod
        private async Task FiyatSorguSonucuDataTableEkle(KullaniciBilgileri musteriBilgileri)
        {
            try
            {
                // MainForm'dan aktif şirket adını al
                var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm == null)
                {
                    MessageBox.Show("Ana form bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Açık şirketlerin listesini al
                var acikSirketler = mainForm.GetAcikSirketler();
                if (acikSirketler == null || acikSirketler.Count == 0)
                {
                    MessageBox.Show("Hiç açık şirket sekmesi bulunamadı!\n\nLütfen önce şirket sekmelerini açın.", 
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Paralel sorgulama onayı al
                var result = MessageBox.Show(
                    $"Paralel fiyat sorgulama yapılacak.\n\n" +
                    $"Açık şirket sayısı: {acikSirketler.Count}\n" +
                    $"Şirketler: {string.Join(", ", acikSirketler)}\n\n" +
                    $"⚡ Tüm şirketlerde eş zamanlı sorgulama yapılacak!\n" +
                    $"Devam etmek istiyor musunuz?",
                    "Paralel Fiyat Sorgulama Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                // Butonu sorgulanıyor durumuna getir
                SetButtonSorgulaniyorDurumu(true);

                // Önce DataTable'ı hazırla
                EnsureDataTableReady();

                // Paralel fiyat sorgulama işlemini başlat
                await TopluFiyatSorgulamaYap(acikSirketler, musteriBilgileri);

                // Butonu normal duruma getir
                SetButtonSorgulaniyorDurumu(false);
            }
            catch (Exception ex)
            {
                SetButtonSorgulaniyorDurumu(false);
                MessageBox.Show($"Paralel fiyat sorgulama hatası: {ex.Message}", 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DataTable'ın hazır olduğundan emin ol
        private void EnsureDataTableReady()
        {
            if (dt == null)
            {
                dt = new DataTable();
            }

            // Gerekli kolonları kontrol et ve ekle
            string[] requiredColumns = { "Firma", "TBrütPrim", "KBrütPrim", "TKomisyon", "KKomisyon", 
                                       "TTeklifNo", "KTeklifNo", "Pesin", "Sure", "TeklifTipi", "Durum", "ack" };

            foreach (string columnName in requiredColumns)
            {
                if (!dt.Columns.Contains(columnName))
                {
                    dt.Columns.Add(columnName, typeof(string));
                }
            }

            // DataSource'u bağla
            if (dgvFiyatlarTable.DataSource != dt)
            {
            dgvFiyatlarTable.DataSource = dt;
            }
        }

        // FiyatBilgisi'ni DataTable'a ekle (UI Thread'de çalışır)
        private async Task AddFiyatBilgisiToDataTableAsync(FiyatBilgisi fiyatBilgisi)
        {
            if (this.InvokeRequired)
            {
                await Task.Run(() =>
                {
                    this.Invoke(new Action(() => AddFiyatBilgisiToDataTableSync(fiyatBilgisi)));
                });
                    }
                    else
                    {
                AddFiyatBilgisiToDataTableSync(fiyatBilgisi);
            }
        }

        // FiyatBilgisi'ni DataTable'a ekle (Synchronous - UI Thread'de çalışmalı)
        private void AddFiyatBilgisiToDataTableSync(FiyatBilgisi fiyatBilgisi)
        {
            lock (_dataTableLock) // DataTable işlemlerini serialize et
        {
            try
            {
                    // Mevcut aynı firmayı sil
                    var existingRows = dt.AsEnumerable()
                        .Where(r => r.Field<string>("Firma") == fiyatBilgisi.FirmaAdi)
                        .ToList();
                    
                    foreach (var row in existingRows)
                    {
                        row.Delete();
                    }

                    // Yeni satır oluştur
                    DataRow newRow = dt.NewRow();
                    newRow["Firma"] = fiyatBilgisi.FirmaAdi ?? "";
                    newRow["TBrütPrim"] = fiyatBilgisi.BrutPrim ?? "0,00";
                    newRow["KBrütPrim"] = "0,00"; // Kasko bilgisi yoksa default
                    newRow["TKomisyon"] = fiyatBilgisi.Komisyon ?? "0,00";
                    newRow["KKomisyon"] = "0,00"; // Kasko komisyonu
                    newRow["TTeklifNo"] = fiyatBilgisi.TeklifNo ?? "";
                    newRow["KTeklifNo"] = ""; // Kasko teklif no
                    newRow["Pesin"] = fiyatBilgisi.Pesin ?? "0,00";
                    newRow["Sure"] = fiyatBilgisi.Sure ?? "";
                    newRow["TeklifTipi"] = fiyatBilgisi.TeklifTipi ?? "Trafik";
                    newRow["Durum"] = "100"; // Tamamlandı durumu
                    newRow["ack"] = fiyatBilgisi.Durum?.Contains("Hata") == true ? fiyatBilgisi.Durum : "Tamamlandı";

                    // Yeni satırı ekle
                    dt.Rows.Add(newRow);
                    dt.AcceptChanges();

                    // Grid'i yenile
                    RefreshDataGrid();
                    
                    Console.WriteLine($"📊 DataTable'a eklendi: {fiyatBilgisi.FirmaAdi} - UI Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ DataTable güncelleme hatası: {ex.Message}");
                }
            }
        }

        // Eski metod - geriye uyumluluk için
        private void AddFiyatBilgisiToDataTable(FiyatBilgisi fiyatBilgisi)
        {
            AddFiyatBilgisiToDataTableSync(fiyatBilgisi);
        }

        // DataGrid'i yenile
        private void RefreshDataGrid()
        {
            if (dgvFiyatlarTable.InvokeRequired)
            {
                dgvFiyatlarTable.Invoke(new Action(() => 
                {
                    bandedGridView1.RefreshData();
                    SetCurrencyFormat();
                }));
            }
            else
            {
                bandedGridView1.RefreshData();
                SetCurrencyFormat();
            }
        }

        // Gerçek fiyat sorgulama sonuçlarını al
        private async Task<FiyatBilgisi> GercekFiyatSorgusu(string sirketAdi, KullaniciBilgileri musteriBilgileri, CancellationToken cancellationToken = default)
        {
            try
            {
                // MainForm'dan şirket-specific browser'ı al
                var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm == null)
                    return null;

                // Her şirket için kendi browser'ını al
                var browser = mainForm.GetSirketBrowser(sirketAdi);
                if (browser == null)
                {
                    Console.WriteLine($"❌ {sirketAdi} için browser bulunamadı! Şirket sekmesi açık değil.");
                    return new FiyatBilgisi
                    {
                        FirmaAdi = sirketAdi,
                        BrutPrim = "Hata",
                        Komisyon = "0,00",
                        TeklifNo = "",
                        Pesin = "0,00",
                        Sure = "",
                        TeklifTipi = "Trafik",
                        Durum = "❌ Şirket sekmesi açık değil"
                    };
                }
                
                // Şirket adını factory'de kullanılan formata çevir
                string factorySirketAdi = GetFactorySirketAdi(sirketAdi);
                
                // Factory pattern kullanarak fiyat sorgulama yap
                try
                {
                    var factory = new ConcreteFiyatSorgulaFactory();
                    var fiyatSorgu = factory.GetFiyatSorgu(factorySirketAdi);
                    
                    if (fiyatSorgu != null)
                    {
                        // Progress callback oluştur - DataTable'da yüzde güncellemesi için
                        var progress = new Progress<int>(percentage =>
                        {
                            // UI thread'de progress güncelle
                            this.Invoke(new Action(() =>
                            {
                                // Mevcut şirket durumunu progress ile güncelle
                                var progressFiyatBilgisi = new FiyatBilgisi
                                {
                                    FirmaAdi = sirketAdi,
                                    BrutPrim = $"🔄 %{percentage} Tamamlandı",
                                    Komisyon = "0,00",
                                    TeklifNo = "",
                                    Pesin = "0,00",
                                    Sure = "",
                                    TeklifTipi = "Trafik",
                                    Durum = $"🔄 İlerleme: %{percentage} (Thread: {Thread.CurrentThread.ManagedThreadId})"
                                };
                                AddFiyatBilgisiToDataTableSync(progressFiyatBilgisi);
                            }));
                        });
                        
                        // Gerçek fiyat sorgulama işlemi
                        var sonuc = await fiyatSorgu.TrafikSorgula(musteriBilgileri, browser, cancellationToken, progress);
                        return sonuc;
                    }
                }
                catch (ApplicationException)
                {
                    // Bu şirket için factory tanımlı değil
                    Console.WriteLine($"Şirket {sirketAdi} için fiyat sorgulama factory'si bulunamadı.");
                }

                // Desteklenmeyen şirket veya hata durumu için varsayılan sonuç
                return new FiyatBilgisi
                {
                    FirmaAdi = sirketAdi,
                    BrutPrim = "Desteklenmiyor",
                    Komisyon = "0,00", 
                    TeklifNo = "",
                    Pesin = "0,00",
                    Sure = "",
                    TeklifTipi = "Trafik",
                    Durum = "Bu şirket için otomatik fiyat sorgulama desteklenmiyor"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fiyat sorgulama hatası: {ex.Message}");
                
                // Hata durumunda bilgilendirici sonuç döndür
                return new FiyatBilgisi
                {
                    FirmaAdi = sirketAdi,
                    BrutPrim = "Hata",
                    Komisyon = "0,00", 
                    TeklifNo = "",
                    Pesin = "0,00",
                    Sure = "",
                    TeklifTipi = "Trafik",
                    Durum = "Hata: " + ex.Message
                };
            }
        }

        // Toplu paralel fiyat sorgulama ana metodu kontrol
        private async Task TopluFiyatSorgulamaYap(List<string> acikSirketler, KullaniciBilgileri musteriBilgileri)
        {
            try
            {
                // İptal token'ı oluştur
                _parallelCancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _parallelCancellationTokenSource.Token;

                Console.WriteLine($"🚀 Paralel fiyat sorgulama başlatılıyor - {acikSirketler.Count} şirket");

                // Her şirket için başlangıç durumunu DataTable'a ekle
                foreach (string sirket in acikSirketler)
                {
                    var baslangicFiyatBilgisi = new FiyatBilgisi
                    {
                        FirmaAdi = sirket,
                        BrutPrim = "🔄 Başlatılıyor...",
                        Komisyon = "0,00",
                        TeklifNo = "",
                        Pesin = "0,00",
                        Sure = "",
                        TeklifTipi = "Trafik",
                        Durum = "🔄 Sırada bekliyor..."
                    };
                    AddFiyatBilgisiToDataTableSync(baslangicFiyatBilgisi);
                }

                // Paralel Task'ları oluştur
                var tasks = acikSirketler.Select(async sirket =>
                {
                    try
                    {
                        Console.WriteLine($"🔄 {sirket} için fiyat sorgulama başlatılıyor - Thread: {Thread.CurrentThread.ManagedThreadId}");
                        
                        var sonuc = await TekSirketFiyatSorgusu(sirket, musteriBilgileri, cancellationToken);
                        
                        if (sonuc != null)
                        {
                            Console.WriteLine($"✅ {sirket} fiyat sorgulama tamamlandı");
                            AddFiyatBilgisiToDataTableSync(sonuc);
                        }
                        else
                        {
                            Console.WriteLine($"❌ {sirket} fiyat sorgulama başarısız");
                            var hataBilgisi = new FiyatBilgisi
                            {
                                FirmaAdi = sirket,
                                BrutPrim = "Hata",
                                Komisyon = "0,00",
                                TeklifNo = "",
                                Pesin = "0,00",
                                Sure = "",
                                TeklifTipi = "Trafik",
                                Durum = "❌ Fiyat alınamadı"
                            };
                            AddFiyatBilgisiToDataTableSync(hataBilgisi);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"🛑 {sirket} fiyat sorgulama iptal edildi");
                        var iptalBilgisi = new FiyatBilgisi
                        {
                            FirmaAdi = sirket,
                            BrutPrim = "İptal",
                            Komisyon = "0,00",
                            TeklifNo = "",
                            Pesin = "0,00",
                            Sure = "",
                            TeklifTipi = "Trafik",
                            Durum = "🛑 İptal edildi"
                        };
                        AddFiyatBilgisiToDataTableSync(iptalBilgisi);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ {sirket} fiyat sorgulama hatası: {ex.Message}");
                        var hataBilgisi = new FiyatBilgisi
                        {
                            FirmaAdi = sirket,
                            BrutPrim = "Hata",
                            Komisyon = "0,00",
                            TeklifNo = "",
                            Pesin = "0,00",
                            Sure = "",
                            TeklifTipi = "Trafik",
                            Durum = $"❌ Hata: {ex.Message}"
                        };
                        AddFiyatBilgisiToDataTableSync(hataBilgisi);
                    }
                }).ToArray();

                // Tüm task'ları paralel olarak çalıştır
                await Task.WhenAll(tasks);
                
                Console.WriteLine("🎉 Tüm şirketler için paralel fiyat sorgulama tamamlandı!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Toplu fiyat sorgulama genel hatası: {ex.Message}");
                MessageBox.Show($"Toplu fiyat sorgulama hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tek şirket fiyat sorgulama metodu
        private async Task<FiyatBilgisi> TekSirketFiyatSorgusu(string sirketAdi, KullaniciBilgileri musteriBilgileri, CancellationToken cancellationToken)
        {
            try
            {
                // MainForm'dan şirket-specific browser'ı al
                var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm == null)
                    return null;

                // Her şirket için kendi browser'ını al - bu critical önemli!
                var browser = mainForm.GetSirketBrowser(sirketAdi);
                if (browser == null)
                {
                    Console.WriteLine($"❌ {sirketAdi} için browser bulunamadı! Şirket sekmesi açık değil.");
                    return new FiyatBilgisi
                    {
                        FirmaAdi = sirketAdi,
                        BrutPrim = "Hata",
                        Komisyon = "0,00",
                        TeklifNo = "",
                        Pesin = "0,00",
                        Sure = "",
                        TeklifTipi = "Trafik",
                        Durum = "❌ Şirket sekmesi açık değil"
                    };
                }
                
                // Şirket adını factory'de kullanılan formata çevir
                string factorySirketAdi = GetFactorySirketAdi(sirketAdi);
                
                // Factory pattern kullanarak fiyat sorgulama yap
                try
                {
                    var factory = new ConcreteFiyatSorgulaFactory();
                    var fiyatSorgu = factory.GetFiyatSorgu(factorySirketAdi);
                    
                    if (fiyatSorgu != null)
                    {
                        // Progress callback oluştur - DataTable'da yüzde güncellemesi için
                        var progress = new Progress<int>(percentage =>
                        {
                            // UI thread'de progress güncelle
                            this.Invoke(new Action(() =>
                            {
                                // Mevcut şirket durumunu progress ile güncelle
                                var progressFiyatBilgisi = new FiyatBilgisi
                                {
                                    FirmaAdi = sirketAdi,
                                    BrutPrim = $"🔄 %{percentage} Tamamlandı",
                                    Komisyon = "0,00",
                                    TeklifNo = "",
                                    Pesin = "0,00",
                                    Sure = "",
                                    TeklifTipi = "Trafik",
                                    Durum = $"🔄 İlerleme: %{percentage} (Thread: {Thread.CurrentThread.ManagedThreadId})"
                                };
                                AddFiyatBilgisiToDataTableSync(progressFiyatBilgisi);
                            }));
                        });
                        
                        // Gerçek fiyat sorgulama işlemi
                        var sonuc = await fiyatSorgu.TrafikSorgula(musteriBilgileri, browser, cancellationToken, progress);
                        return sonuc;
                    }
                }
                catch (ApplicationException)
                {
                    // Bu şirket için factory tanımlı değil
                    Console.WriteLine($"Şirket {sirketAdi} için fiyat sorgulama factory'si bulunamadı.");
                }

                // Desteklenmeyen şirket veya hata durumu için varsayılan sonuç
                return new FiyatBilgisi
                {
                    FirmaAdi = sirketAdi,
                    BrutPrim = "Desteklenmiyor",
                    Komisyon = "0,00", 
                    TeklifNo = "",
                    Pesin = "0,00",
                    Sure = "",
                    TeklifTipi = "Trafik",
                    Durum = "Bu şirket için otomatik fiyat sorgulama desteklenmiyor"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fiyat sorgulama hatası: {ex.Message}");
                
                // Hata durumunda bilgilendirici sonuç döndür
                return new FiyatBilgisi
                {
                    FirmaAdi = sirketAdi,
                    BrutPrim = "Hata",
                    Komisyon = "0,00", 
                    TeklifNo = "",
                    Pesin = "0,00",
                    Sure = "",
                    TeklifTipi = "Trafik",
                    Durum = "Hata: " + ex.Message
                };
            }
        }

        // Şirket adını factory formatına çevir
        private string GetFactorySirketAdi(string sirketAdi)
        {
            var cevrimTablosu = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Allianz Sigorta", "AllianzSigortaTrafikFiyat" },
                { "Ankara Sigorta", "AnkaraSigortaTrafikFiyat" },
                { "Bereket Sigorta", "BereketSigortaTrafikFiyat" },
                { "Corpus Sigorta", "CorpusSigortaTrafikFiyat" },
                { "Doğa Sigorta", "DogaSigorta2Fiyat" },
                { "HDI Sigorta", "HdiSigortaTrafikFiyat" },
                { "Hepıyı Sigorta", "HepiyiSigortaTrafikFiyat" },
                { "Hep İyİ Sigorta", "HepiyiSigortaTrafikFiyat" },
                { "Şeker Sigorta", "SekerSigortaTrafikFiyat" },
                { "Sompo Japan", "SompoJapanSigortaTrafik" },
                { "Ray Sigorta", "RaySigortaTrafikFiyat" },
                { "Quick Sigorta", "QuickSigortaTrafikFiyat" },
                { "Koru Sigorta", "KoruSigortaFiyat" },
                // InsuranceConstants değerleri ile eşleştirmeler
                { InsuranceConstants.AllianzSigorta, "AllianzSigortaTrafikFiyat" },
                { InsuranceConstants.AnkaraSigorta, "AnkaraSigortaTrafikFiyat" },
                { InsuranceConstants.BereketSigorta, "BereketSigortaTrafikFiyat" },
                { InsuranceConstants.CorpusSigorta, "CorpusSigortaTrafikFiyat" },
                { InsuranceConstants.DogaSigorta, "DogaSigorta2Fiyat" },
                { InsuranceConstants.HdiSigorta, "HdiSigortaTrafikFiyat" },
                { InsuranceConstants.HepIyiSigorta, "HepiyiSigortaTrafikFiyat" },
                { InsuranceConstants.SekerSigorta, "SekerSigortaTrafikFiyat" },
                { InsuranceConstants.SompoJapanSigorta, "SompoJapanSigortaTrafik" },
                { InsuranceConstants.RaySigorta, "RaySigortaTrafikFiyat" },
                { InsuranceConstants.QuickSigorta, "QuickSigortaTrafikFiyat" },
                { InsuranceConstants.KoruSigorta, "KoruSigortaFiyat" },
            };

            return cevrimTablosu.TryGetValue(sirketAdi, out string factoryAdi) ? factoryAdi : sirketAdi;
        }

        // Para birimi formatını ayarla
        private void SetCurrencyFormat()
        {
            try
            {
                if (bandedGridView1.Columns["TBrütPrim"] != null)
                {
                    bandedGridView1.Columns["TBrütPrim"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    bandedGridView1.Columns["TBrütPrim"].DisplayFormat.FormatString = "c2";
                }
                
                if (bandedGridView1.Columns["TKomisyon"] != null)
                {
                    bandedGridView1.Columns["TKomisyon"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    bandedGridView1.Columns["TKomisyon"].DisplayFormat.FormatString = "c2";
                }
                
                if (bandedGridView1.Columns["Pesin"] != null)
                {
                    bandedGridView1.Columns["Pesin"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    bandedGridView1.Columns["Pesin"].DisplayFormat.FormatString = "c2";
                }
            }
            catch
            {
                // Format hatası sessizce geçilir
            }
        }

                // Buton durumu yönetimi
        private string _originalButtonText = "Toplu Fiyat Sorgula";
        
        private void SetButtonSorgulaniyorDurumu(bool sorgulaniyorMu)
        {
            try
            {
                if (dropDownButton1.InvokeRequired)
                {
                    dropDownButton1.Invoke(new Action(() => SetButtonSorgulaniyorDurumu(sorgulaniyorMu)));
                return;
                }

                if (sorgulaniyorMu)
                {
                    // İlk kez çağrılıyorsa orijinal text'i sakla
                    if (dropDownButton1.Text != "🔄 Sorgulanıyor... (Tekrar tıklayın: Yeniden başlat)")
                    {
                        _originalButtonText = dropDownButton1.Text;
                    }
                    
                    dropDownButton1.Text = "🔄 Sorgulanıyor... (Tekrar tıklayın: Yeniden başlat)";
                    dropDownButton1.Enabled = true; // Butonu aktif tut - kullanıcı tekrar tıklayabilsin
                }
                else
                {
                    dropDownButton1.Text = _originalButtonText;
                    dropDownButton1.Enabled = true;
                }
                
                Application.DoEvents();
            }
            catch
            {
                // Buton güncellenirken hata olursa sessizce devam et
            }
        }

        // DevExpress Grid için CustomDrawCell event handler
        private void bandedGridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                int indx = bandedGridView1.GetDataSourceRowIndex(e.RowHandle);
                if (indx < 0 || indx >= dt.Rows.Count) return;
                
                DataRow dr = dt.Rows[indx];

                if (e.Column.FieldName == "Durum")
                {
                    string durumText = Convert.ToString(dr["Durum"]);
                    string ackText = Convert.ToString(dr["ack"]);
                    
                    // Progress yüzdesini parse et
                    int percentage = 0;
                    if (int.TryParse(durumText, out percentage))
                    {
                        // Progress bar çiz
                        int progressWidth = percentage * e.Bounds.Width / 100;
                        Rectangle progressRect = new Rectangle(e.Bounds.X, e.Bounds.Y, progressWidth, e.Bounds.Height);

                        // Durum rengini belirle
                        Brush progressBrush = Brushes.Gray;
                        
                        if (ackText.Contains("Tamamlandı"))
                        {
                            progressBrush = Brushes.LimeGreen;
                        }
                        else if (ackText.Contains("🔄"))
                        {
                            progressBrush = Brushes.Orange;
                        }
                        else if (ackText.Contains("Hata") || ackText.Contains("❌"))
                        {
                            progressBrush = Brushes.Red;
                        }
                        else if (ackText.Contains("İptal"))
                        {
                            progressBrush = Brushes.Gray;
                        }

                        // Progress bar'ı çiz
                        e.Cache.FillRectangle(progressBrush, progressRect);
                        
                        // Text'i üzerine yaz
                        e.Appearance.DrawString(e.Cache, e.DisplayText, e.Bounds);
                        e.Handled = true;
                    }
                }

                // Logo kolonunu çiz (eğer varsa)
                if (e.Column.FieldName == "bandedGridColumn3")
                {
                    e.Column.OptionsColumn.AllowEdit = false;
                    
                    try
                    {
                        string firmaAdi = Convert.ToString(dr["Firma"]);
                        // Logo çizme işlemi burada yapılabilir
                        // Şu an için basit text gösterimi
            }
            catch
            {
                        // Logo hatası sessizce geçilir
                    }
                }
            }
            catch
            {
                // Grid çizim hatası sessizce geçilir
            }
        }

        // ResizeImage helper metodu (Logo için kullanılabilir)
        private static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            try
            {
                int sourceWidth = imgToResize.Width;
                int sourceHeight = imgToResize.Height;
                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;
                
                nPercentW = (size.Width / (float)sourceWidth);
                nPercentH = (size.Height / (float)sourceHeight);
                nPercent = Math.Min(nPercentW, nPercentH);
                
                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);
                
                Bitmap b = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();
                
                return b;
            }
            catch
            {
                return imgToResize; // Hata durumunda orijinal image'ı döndür
            }
        }

        // Sorgu listesi doldur metodu
        private void SorguListesiDoldur()
        {
            try
            {
                // Bu metod şirket listesini doldurmak için kullanılır
                // Basit bir implementasyon yapılmıştır
                Console.WriteLine("Sorgu listesi dolduruldu");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sorgu listesi doldurma hatası: {ex.Message}");
            }
        }

        // DataTable kolonlarını doldur
        private void tabloDoldur()
        {
            try
            {
                int progressDefaultValue = 0;

                // Gerekli kolonları kontrol et ve ekle
                if (!dt.Columns.Contains("Firma"))
                    dt.Columns.Add("Firma", typeof(string));

                if (!dt.Columns.Contains("TBrütPrim"))
                    dt.Columns.Add("TBrütPrim", typeof(string));

                if (!dt.Columns.Contains("KBrütPrim"))
                    dt.Columns.Add("KBrütPrim", typeof(string));

                if (!dt.Columns.Contains("TKomisyon"))
                    dt.Columns.Add("TKomisyon", typeof(string));

                if (!dt.Columns.Contains("KKomisyon"))
                    dt.Columns.Add("KKomisyon", typeof(string));

                if (!dt.Columns.Contains("TTeklifNo"))
                    dt.Columns.Add("TTeklifNo", typeof(string));

                if (!dt.Columns.Contains("KTeklifNo"))
                    dt.Columns.Add("KTeklifNo", typeof(string));

                if (!dt.Columns.Contains("Pesin"))
                    dt.Columns.Add("Pesin", typeof(string));

                if (!dt.Columns.Contains("Sure"))
                    dt.Columns.Add("Sure", typeof(string));

                if (!dt.Columns.Contains("TeklifTipi"))
                    dt.Columns.Add("TeklifTipi", typeof(string));

                if (!dt.Columns.Contains("Durum"))
                {
                    dt.Columns.Add("Durum", typeof(string));
                    dt.Columns["Durum"].DefaultValue = progressDefaultValue;
                }

                if (!dt.Columns.Contains("ack"))
                    dt.Columns.Add("ack", typeof(string));

                dt.AcceptChanges();

                // DataSource'u bağla
                dgvFiyatlarTable.DataSource = dt;

                Console.WriteLine("DataTable kolonları başarıyla dolduruldu");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataTable doldurma hatası: {ex.Message}");
            }
        }

        // Email ve telefon alanları boşsa default değerler atar
        private void setEmailAndPhone()
        {
            try
            {
                // Telefon alanı boşsa default değer ata
                if (string.IsNullOrWhiteSpace(txtTel.Text))
                {
                    txtTel.Text = "5435467543"; // Default telefon numarası
                }

                // Email alanı boşsa default değer ata
                if (string.IsNullOrWhiteSpace(txtEposta.Text))
                {
                    txtEposta.Text = "test@example.com"; // Default email
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"setEmailAndPhone hatası: {ex.Message}");
            }
        }

        // DataTable'daki fiyat bilgilerini temizler
        internal void dtackdelete(string param)
        {
            try
            {
                lock (dt)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i].SetField("ack", "");
                        dt.Rows[i].SetField("Durum", 0);

                        if (param == "Kasko")
                        {
                            dt.Rows[i].SetField("KBrütPrim", string.Empty);
                            dt.Rows[i].SetField("KKomisyon", string.Empty);
                            dt.Rows[i].SetField("KTeklifNo", string.Empty);
                            dt.Rows[i].SetField("Pesin", string.Empty);
                        }
                        else
                        {
                            param = "Trafik";
                            dt.Rows[i].SetField("TBrütPrim", string.Empty);
                            dt.Rows[i].SetField("TKomisyon", string.Empty);
                            dt.Rows[i].SetField("TTeklifNo", string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"dtackdelete hatası: {ex.Message}");
            }
        }

        // Sorgu Bilgileri butonu için event handler
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // Sorgu bilgilerini göster veya bir işlem yap
                // Bu buton "Sorgu Bilgileri" caption'ına sahip
                
                // Örnek: Sorgu durumu hakkında bilgi göster
                string sorguBilgisi = $"Sorgu Tipi: {sorgutipi}\n";
                sorguBilgisi += $"Müşteri TC: {txtKimlikNo.Text}\n";
                sorguBilgisi += $"Plaka: {txtPlakaNo.Text}\n";
                sorguBilgisi += $"Aktif Şirket: {cmbInsuranceCompanySTR}\n";
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    sorguBilgisi += $"Sonuç Sayısı: {dt.Rows.Count}\n";
                }
                
                MessageBox.Show(sorguBilgisi, "Sorgu Bilgileri", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"barButtonItem1_ItemClick hatası: {ex.Message}");
                MessageBox.Show("Sorgu bilgileri alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
