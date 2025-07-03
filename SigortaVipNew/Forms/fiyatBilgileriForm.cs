using CefSharp.WinForms;
//using DevExpress.DataAccess.Sql;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
//using iTextSharp.text;
//using iTextSharp.text.html.simpleparser;
//using iTextSharp.text.pdf;
//using sigortaVip.Business.Abstract;
//using sigortaVip.Business.DependencyResolvers.Ninject;
//using sigortaVip.Entities.Concrete;
using SigortaVip.Business;
using SigortaVip.FiyatSorgulamaFactory;
using SigortaVip.FiyatSorgulamaFactory.Interface;
using SigortaVip.Models;
using SigortaVip.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Rectangle = System.Drawing.Rectangle;

namespace SigortaVip.Forms
{
    public partial class fiyatBilgileriForm : Form
    {
        public ConcurrentQueue<QueueMessage> _fiyatSorgulamaQueue;
        private List<Thread> _activeThreadList;
        public ConcurrentQueue<QueueMessage> _EkPaketFiyatSorgulamaQueue = null;
        //private IKullaniciSirketService _kullaniciSirketService;
        private FiyatSorgulaFactory _sorgulamaFactory;
        private KullaniciBilgileri _mevcutKullaniciBilgileri = null;
        public static KullaniciBilgileri _kullaniciBilgileri;
        private CancellationTokenSource _cancellationTokenSource;
        private NotifyIcon _notifyIcon;
        private bool _onlyOneNotify;
        private int row_index = 0;
        public static string html = "";
        public static bool _sorgulamaAktif = false;


        private Thread kontrolThread;

        public fiyatBilgileriForm(KullaniciBilgileri kullaniciBilgileri, ConcurrentQueue<QueueMessage> fiyatSorgulamaQueue)
        {
            InitializeComponent();
            //_kullaniciSirketService = InstanceFactory.GetInstance<IKullaniciSirketService>();
            _kullaniciBilgileri = kullaniciBilgileri;
            _fiyatSorgulamaQueue = fiyatSorgulamaQueue;
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            _onlyOneNotify = true;

            _cancellationTokenSource = new CancellationTokenSource();
            bilgileriDoldur();

            if (kullaniciBilgileri != null && fiyatSorgulamaQueue != null)
            {
                kontrolThread = new Thread(KontrolThreadIslem);
                kontrolThread.Start();
            }

            if (_mevcutKullaniciBilgileri != null && _kullaniciBilgileri != null) //yeni sorgu tespiti
            {
                if (_mevcutKullaniciBilgileri.txtKimlikNo == _kullaniciBilgileri.txtKimlikNo && _mevcutKullaniciBilgileri.txtPlakaNo == _kullaniciBilgileri.txtPlakaNo)
                {

                }
                else
                {

                }
            }
            _mevcutKullaniciBilgileri = _kullaniciBilgileri;

            tabloDoldur();
            Setup();
        }
        private void KontrolThreadIslem()
        {
            while (true)
            {
                if (_EkPaketFiyatSorgulamaQueue != null)
                {
                    _fiyatSorgulamaQueue = null;
                    _fiyatSorgulamaQueue = _EkPaketFiyatSorgulamaQueue;

                    break;
                }
                Thread.Sleep(1000);
            }
        }
        private void bilgileriDoldur()
        {
            if (_kullaniciBilgileri != null)
            {
                tbxAdSoyad.Text = _kullaniciBilgileri.AdSoyad;
                tbxTC.Text = _kullaniciBilgileri.txtKimlikNo;
                tbxPlaka.Text = _kullaniciBilgileri.txtPlakaNo;
            }
        }

        public static DataTable dt = new DataTable();
        private void tabloDoldur()
        {
            int progressDefaultValue = 0;
            // DataGridViewProgressColumn progressColumn = new DataGridViewProgressColumn();
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

            dgvFiyatlarTable.DataSource = dt;

            if (_fiyatSorgulamaQueue != null)
            {
                foreach (var item in _fiyatSorgulamaQueue)
                {
                    item.RowIndex = AddRow(new FiyatBilgisi { FirmaAdi = item.InsuranceCompany, TeklifTipi = item.SorguTipi }, dgvFiyatlarTable, progressDefaultValue);
                }
            }
            //else
            //{
            //    if (_kullaniciBilgileri == null)
            //    {
            //        List<KullaniciSirket> kullaniciSirketleri = _kullaniciSirketService.GetByUserId(AktifKullaniciBilgileri.AktifKullanici.id).OrderByDescending(x => x.sirketAdi).ToList();

            //        if (kullaniciSirketleri != null)
            //        {
            //            foreach (KullaniciSirket item in kullaniciSirketleri)
            //            {
            //                if (item.sirketAktifMi)
            //                {
            //                    AddRow(new FiyatBilgisi { FirmaAdi = item.sirketAdi, TeklifTipi = null }, dgvFiyatlarTable, progressDefaultValue);
            //                }
            //            }
            //        }
            //    }
            //}
            
        }

        public  void Setup()
        {
            try
            {
                _sorgulamaFactory = new ConcreteFiyatSorgulaFactory();
            }
            catch { }
            BuildQueue();
            BuildConsumer();

        }

        private async void BuildConsumer()
        {

            try
            {
                Thread thread = new Thread(RunConsumerThread);
                thread.Name = "Toplu Fiyat Sorgu Thread";
                thread.Start();
                _activeThreadList.Add(thread);
            }
            catch { }
        }

        private async void BuildQueue()
        {
            try
            {
                if (_fiyatSorgulamaQueue == null)
                    _fiyatSorgulamaQueue = new ConcurrentQueue<QueueMessage>();
                if (_activeThreadList == null)
                    _activeThreadList = new List<Thread>();
            }
            catch { }
        }
        internal void dtackdelete(string param)
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

        DevExpress.XtraSplashScreen.IOverlaySplashScreenHandle handle = null;
        void overlayfonk(bool show)
        {
            try
            {

                if (show && handle == null)
                {
                    //handle = DevExpress.XtraSplashScreen.SplashScreenManager.ShowOverlayForm(dgvFiyatlarTable);
                }
                else
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseOverlayForm(handle);
                    handle = null;
                }
            }
            catch { }
        }
        private async void SorguBaslat(IFiyatSorgu fiyatSorgu, QueueMessage message, int rowIndex)
        {
            //try
            //{
            //    overlayfonk(true);
            //    Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            //    progress.ProgressChanged += ReportProgress;
            //    ProgressReportModel report = new ProgressReportModel();

            //    if (rowIndex == null)
            //    {
            //        report.rowIndex = AddRow(new FiyatBilgisi { FirmaAdi = message.InsuranceCompany, TeklifTipi = message.SorguTipi }, dgvFiyatlarTable, report.Percentage);
            //    }
            //    else
            //    {
            //        report.rowIndex = message.RowIndex;
            //    }

            //    MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            ////    var browsersVar = mainForm.GetBrowsers;

            //    if (browsersVar.TryGetValue(message.InsuranceCompany, out (ChromiumWebBrowser browser, TabPage tabPage) value))
            //    {
            //        var resp = message.SorguTipi == "Trafik"
            //        ? await fiyatSorgu.TrafikSorgula(_kullaniciBilgileri, browsersVar[message.InsuranceCompany].browser, _cancellationTokenSource.Token, progress, report).ConfigureAwait(false)
            //        : await fiyatSorgu.KaskoSorgula(_kullaniciBilgileri, browsersVar[message.InsuranceCompany].browser, _cancellationTokenSource.Token, progress, report).ConfigureAwait(false);

            //        if (resp != null)
            //        {
            //            resp.TeklifTipi = message.SorguTipi;
            //                switch (resp.Durum)
            //            {
            //                case "Tamamlandı":
            //                case "Tamamlandı (Teminatlı)":
            //                    resp.DurumRengi = Color.FromArgb(102, 255, 102);
            //                    break;
            //                case "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır":
            //                    resp.DurumRengi = Color.Black;
            //                    break;
            //                default:
            //                    resp.DurumRengi = Color.Red;
            //                    break;
            //            }

            //            if (resp.Durum.Contains("Tamamlandı"))
            //            {
            //                resp.DurumRengi = Color.FromArgb(102, 255, 102);
            //            }
            //            UpdateRow(resp.fiyatRowIndex, resp);
            //        }
            //    }
            //}
        //    catch (OperationCanceledException)
        //    {
        //        if (_onlyOneNotify)
        //        {
        //            _notifyIcon.Visible = true;
        //            _notifyIcon.Text = "SigortaVip Uyarı";
        //            _notifyIcon.BalloonTipTitle = "Sorgular İptal Edildi";
        //            _notifyIcon.BalloonTipText = "Toplu fiyat sorgusu iptal edildi";
        //            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
        //            _notifyIcon.ShowBalloonTip(2000);

        //            _notifyIcon.MouseDoubleClick += new MouseEventHandler(notify_Icon_MouseDoubleClick);

        //            _onlyOneNotify = false;
        //        }
        //    }
        //    catch
        //    { }

        //    overlayfonk(false);
        }

        public static void SQLMusteriBilgilerWrite(string firmaad, string ack, string brutPrim, string komisyon, string teklifno, string tip)
        {
            if (string.IsNullOrEmpty(tbxTC.Text) || string.IsNullOrEmpty(tbxPlaka.Text) || string.IsNullOrEmpty(firmaad) || string.IsNullOrEmpty(tip))
                return;


            dal myDal = new dal();
            string sql = "IF NOT EXISTS (select TCKimlik from [sigortaVipDbUser].[FirmaFiyatlari] where TCKimlik='" + tbxTC.Text + "' and Plaka='" + tbxPlaka.Text + "' and FirmaAdi='" + firmaad + "' and KaskoTrafik='" + tip + "')   insert into [sigortaVipDbUser].[FirmaFiyatlari] ([FirmaAdi],[Aciklama],[BrutPrim],[Komisyon],[TeklifNo],[KaskoTrafik],[TCKimlik],[Plaka],[IsimSoyisim]) values ('" + firmaad + "','" + ack + "','" + brutPrim.ToString().Replace(",", ".") + "','" + komisyon.ToString().Replace(",", ".") + "','" + teklifno + "','" + tip + "','" + tbxTC.Text + "','" + tbxPlaka.Text + "','" + tbxAdSoyad.Text + "') ELSE update [sigortaVipDbUser].[FirmaFiyatlari] set [Aciklama]='" + ack + "',[BrutPrim]='" + brutPrim.ToString().Replace(",", ".") + "' ,[Komisyon]='" + komisyon.ToString().Replace(",", ".") + "',[TeklifNo]='" + teklifno + "' where TCKimlik='" + tbxTC.Text + "' and Plaka='" + tbxPlaka.Text + "' and FirmaAdi='" + firmaad + "' and KaskoTrafik='" + tip + "'";

            myDal.CommandExecuteNonQuery(sql, myDal.myConnection);
        }
        public static async void UpdateRow(int rowIndex, FiyatBilgisi row)
        {
            if (row == null || rowIndex < 0) return;

            var aaaa = row;
            try
            {

                if (dgvFiyatlarTable.InvokeRequired)
                    _ = dgvFiyatlarTable.BeginInvoke(new Action(() => UpdateRow(rowIndex, row)));

                try
                {
                    lock (dt)
                    {

                        DataRow dtROWS = dt.Rows[rowIndex];

                        if (Convert.ToString(dtROWS["Firma"]) != Convert.ToString(row.FirmaAdi))
                        {
                            rowIndex = dt.Rows.IndexOf(dt.AsEnumerable().Where(w => Convert.ToString(w["Firma"]) == row.FirmaAdi).FirstOrDefault());
                            dtROWS = dt.Rows[rowIndex];
                        }
                        lock (dtROWS)
                        {
                            if (dtROWS != null)
                            {

                                dtROWS["Firma"] = row.FirmaAdi;
                                if (row.TeklifTipi == "Trafik" && row.BrutPrim != null)
                                {
                                    string formatedValue = FormatCurrencyValue(row.BrutPrim);
                                    dtROWS["TBrütPrim"] = formatedValue;
                                }
                                else if (row.BrutPrim != null)
                             
                                {
                                    string formatedValue = FormatCurrencyValue(row.BrutPrim);
                                    dtROWS["KBrütPrim"] = formatedValue;
                                }
                                else if (row.Pesin != null)

                                {
                                    string formatedValue = FormatCurrencyValue(row.Pesin);
                                    dtROWS["Pesin"] = formatedValue;
                                }


                                if (row.TeklifTipi == "Trafik" && row.Komisyon != null)
                                   
                                {
                                    string formatedValue = FormatCurrencyValue(row.Komisyon);
                                    dtROWS["TKomisyon"] = formatedValue;
                                }

                                else if (row.Komisyon != null)
                                
                                {
                                    string formatedValue = FormatCurrencyValue(row.Komisyon);
                                    dtROWS["KKomisyon"] = formatedValue;
                                }
                                if (row.TeklifTipi == "Trafik" && row.TeklifNo != null)
                                    dtROWS["TTeklifNo"] = row.TeklifNo;
                                else if (row.TeklifNo != null)
                                    dtROWS["KTeklifNo"] = row.TeklifNo;

                                if (row.Sure != null)
                                    dtROWS["Sure"] = row.Sure;
                                if (row.TeklifTipi != null)
                                    dtROWS["TeklifTipi"] = row.TeklifTipi;
                                
                                if (row.Durum != null)
                                    dtROWS["ack"] = row.Durum;
                                dtROWS.AcceptChanges();


                            }
                        }
                    }
                }
                catch { }

            }
            catch (ArgumentOutOfRangeException)
            {

            }
            catch
            {

            }

            string firmaad = "", ack = "", teklifno = "0", tip = "";
            string brutPrim = "", komisyon = "";
            try
            {

                var bbb = aaaa;

                if (row.FirmaAdi != null && !string.IsNullOrEmpty(row.FirmaAdi))
                    firmaad = row.FirmaAdi;
                if (row.Durum != null && !string.IsNullOrEmpty(row.Durum))
                    ack = row.Durum.Replace("'", "");
                if (row.TeklifNo != null && !string.IsNullOrEmpty(row.TeklifNo))
                    teklifno = row.TeklifNo;
                if (row.TeklifTipi != null && !string.IsNullOrEmpty(row.TeklifTipi))
                    tip = row.TeklifTipi;

                if (row.BrutPrim != null && !string.IsNullOrEmpty(row.BrutPrim))
                    brutPrim = row.BrutPrim;
                if (row.Komisyon != null && !string.IsNullOrEmpty(row.Komisyon))
                    komisyon = row.Komisyon;
                if (row.Pesin != null && !string.IsNullOrEmpty(row.Pesin))
                    komisyon = row.Pesin;


                SQLMusteriBilgilerWrite(firmaad, ack, brutPrim, komisyon, teklifno, tip);
            }
            catch (Exception)
            {

            }
        }
        Regex rgx = new Regex(@"-?\d+(?:\.\d+)?");
        private void UpdateRowPercentage(int rowIndex, int percentage)
        {
          
            try
            {

                if (rowIndex < 0) return;
                lock (dt)
                {
                    DataRow dr = dt.Rows[rowIndex];

                    lock (dr)
                    {
                        dr["Durum"] = percentage;
                        dr.AcceptChanges();
                    }
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
            catch
            {
                return;
            }
        }

        private void notify_Icon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _notifyIcon.Visible = false;
        }

        private async void RunConsumerThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(100);
                    try
                    {
                        bool isDequeue = _fiyatSorgulamaQueue.TryDequeue(out QueueMessage message);
                        if (isDequeue)
                        {
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]: consumed a value: {message.InsuranceCompany}, {message.SorguTipi} | queue size {_fiyatSorgulamaQueue.Count}");
                            IFiyatSorgu fiyatSorgu = _sorgulamaFactory.GetFiyatSorgu(message.InsuranceCompany);

                            message.RowIndex = dt.Rows.IndexOf(dt.AsEnumerable().Where(w => Convert.ToString(w["Firma"]) == message.InsuranceCompany).FirstOrDefault());

                            SorguBaslat(fiyatSorgu, message, message.RowIndex);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        // ignored
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
            }
            catch { }
        }

        private async void ReportProgress(object sender, ProgressReportModel e)
        {
            if (this.row_index != 0)
            {
                UpdateRowPercentage(this.row_index, e.Percentage);

            }
            else
            {
                UpdateRowPercentage(e.rowIndex, e.Percentage);

            }

        }

        private delegate int SafeCallDelegate(FiyatBilgisi row, GridControl fiyatTablosu, int durumYuzdesi);

        public static int AddRow(FiyatBilgisi row, GridControl dataGridView, int durumYuzdesi)
        {
            try
            {

                if (row == null)
                {
                    DataRow dr = dt.NewRow();

                    dr["Firma"] = "";
                    dr["TBrütPrim"] = "";
                    dr["KBrütPrim"] = "";
                    dr["TKomisyon"] = "";
                    dr["KKomisyon"] = "";
                    dr["TTeklifNo"] = "";
                    dr["KTeklifNo"] = "";
                    dr["Sure"] = "";
                    dr["Pesin"] = "";
                    dr["TeklifTipi"] = "";
                    dr["Durum"] = 10;
                    dr["ack"] = "";

                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                    return dt.Rows.IndexOf(dr);
                    /*satir = new object[]
                    {
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            durumYuzdesi
                    };*/
                }
                else
                {

                    bool contains = dt.AsEnumerable().Any(r => r.Field<string>("Firma") == row.FirmaAdi);
                    if (contains == false)
                    {

                        DataRow dr = dt.NewRow();

                        dr["Firma"] = row.FirmaAdi;

                        if (row.TeklifTipi == "Trafik")
                            dr["TBrütPrim"] = row.BrutPrim;
                        else
                            dr["KBrütPrim"] = row.BrutPrim;

                        if (row.TeklifTipi == "Trafik")
                            dr["TKomisyon"] = row.Komisyon;
                        else
                            dr["KKomisyon"] = row.Komisyon;

                        if (row.TeklifTipi == "Trafik")
                            dr["TTeklifNo"] = row.TeklifNo;
                        else
                            dr["KTeklifNo"] = row.TeklifNo;

                        dr["Sure"] = row.Sure;
                        dr["TeklifTipi"] = row.TeklifTipi;
                        dr["Durum"] = durumYuzdesi;
                        dr["ack"] = row.Durum;

                        dt.Rows.Add(dr);
                        dt.AcceptChanges();

                        return dt.Rows.IndexOf(dr);

                    }
                    else
                        return dt.Rows.IndexOf(dt.AsEnumerable().Where(r => r.Field<string>("Firma") == row.FirmaAdi).FirstOrDefault());
                }



            }
            catch { }

            return dt.Rows.Count - 1;


            
        }


        internal void DestroyConsumers()
        {
            try
            {
                foreach (Thread thread in _activeThreadList)
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
                        catch
                        { }
                    }

                }
            }
            catch { }
        }

        private void fiyatBilgileriForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _cancellationTokenSource.Cancel();
                DestroyConsumers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on Destroy Consumers. Error: {ex.Message}");
            }
            try
            { overlayfonk(false); }
            catch { }
        }
        

        string param = "Trafik";
        DataTable dtimage = new DataTable();
        internal Dictionary<string, System.Drawing.Image> imageCache = new Dictionary<string, System.Drawing.Image>(StringComparer.OrdinalIgnoreCase);

        private void fiyatBilgileriForm_Load(object sender, EventArgs e)
        {
            dal cmd = new dal();
            SqlConnection myConnection = new SqlConnection("server =93.89.230.234 ; database = sigortavipserver ; uid = sigortaVipDbUser; pwd =Asdfzxcv.12321!!; integrated security=false;MultipleActiveResultSets=true");
            dtimage = cmd.CommandExecuteReader("select * from Firma_Logo", myConnection).Tables[0];
        }
        private void button3_Click(object sender, EventArgs e)
        {
            fiyatBilgileri fiyatBilgisi = getFiyatBilgileri();

            dal myDal = new dal();

            foreach (var item in fiyatBilgisi.FiyatList)
            {
                string sql = "INSERT INTO [sigortavipserver].[sigortaVipDbUser].[FiyatBilgileri] ([KimlikNo], [Plaka], [FirmaAdi], [BrutPrim], [Komisyon], [TeklifNo], [TeklifTipi], [Durum], [KBrutPrim], [KKomisyon], [KTeklifNo], [Pesin],AdSoyad) " +
                "VALUES ('" + fiyatBilgisi.TcVeyaVergiNo + "','" + fiyatBilgisi.Plaka + "','" + item.FirmaAdi + "','" + item.BrutPrim + "','" + item.Komisyon + "','" + item.TeklifNo + "','" + item.TeklifTipi + "','" + item.Durum + "','" + item.KBrutPrim + "','" + item.KKomisyon + "','" + item.KTeklifNo + "','" + item.Pesin + "','" + tbxAdSoyad.Text + "');";
                myDal.CommandExecuteReader(sql, myDal.myConnection);

            }

        }
        private fiyatBilgileri getFiyatBilgileri()
        {


            fiyatBilgileri fiyatBilgileri = new fiyatBilgileri();
            try
            {
                fiyatBilgileri.TcVeyaVergiNo = tbxTC.Text;
                fiyatBilgileri.Plaka = tbxPlaka.Text;

                fiyatBilgileri.FiyatList = new List<Fiyat>();


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    fiyatBilgileri.FiyatList.Add(new Fiyat
                    {
                        FirmaAdi = dr[0].ToString(),
                        BrutPrim = dr[1].ToString(),
                        Komisyon = dr[2].ToString(),
                        TeklifNo = dr[3].ToString(),
                        TeklifTipi = dr[5].ToString(),
                        KBrutPrim = dr[6].ToString(),
                        KKomisyon = dr[7].ToString(),
                        KTeklifNo = dr[8].ToString(),
                        Pesin = dr[9].ToString(),
                    });
                }
            }
            catch { }
            return fiyatBilgileri;
        }

        [Obsolete]
        private void button4_Click(object sender, EventArgs e)
        {
            string sirketHtml = "";

            fiyatBilgileri Fbilgi = getFiyatBilgileri();
            List<int> fiyatlar = new List<int>();

            foreach (var item in Fbilgi.FiyatList)
            {
                sirketHtml += @" <tr style=""background-color: rgb(232, 232, 232);"">
                               <td>" + item.FirmaAdi + @"</td>
                               <td>" + item.BrutPrim + @" </td>
                               <td>" + item.TeklifNo + @"</td>
                               <td><svg style=""color: red"" xmlns=""http://www.w3.org/2000/svg"" width=""16"" height=""16"" fill=""currentColor"" class=""bi bi-x-circle-fill"" viewBox=""0 0 16 16""> <path d=""M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM5.354 4.646a.5.5 0 1 0-.708.708L7.293 8l-2.647 2.646a.5.5 0 0 0 .708.708L8 8.707l2.646 2.647a.5.5 0 0 0 .708-.708L8.707 8l2.647-2.646a.5.5 0 0 0-.708-.708L8 7.293 5.354 4.646z"" fill=""red""></path> </svg> </td>
                               <td>-" + item.TeklifTipi + @"</td>
                             </tr>
                             <tr>";

                try
                {
                    fiyatlar.Add(Convert.ToInt32(item.BrutPrim));
                }
                catch (Exception)
                {

                }
            }
            int minFiyat = 0;
            try
            {
                minFiyat = fiyatlar.Min();

            }
            catch (Exception)
            {

            }
            html = html.Replace("{0}", sirketHtml.ToString()).Replace("{1}", Convert.ToString(minFiyat));

            string htmlContent = html;
            string outputFilePath = "TeklifBilgilendirmeFormu.pdf";

            ConvertHtmlToPdf(htmlContent, outputFilePath);

        }

        [Obsolete]
        public static void ConvertHtmlToPdf(string htmlContent, string pdfFilePath)
        {


            //using (FileStream fs = new FileStream(pdfFilePath, FileMode.Create))
            //{
            //    Document document = new Document();
            //    PdfWriter writer = PdfWriter.GetInstance(document, fs);

            //    document.Open();

            //    HTMLWorker htmlWorker = new HTMLWorker(document);
            //    htmlWorker.Parse(new StringReader(htmlContent));

            //    document.Close();
            //}
        }

        private void btnPlakaSorgu_Click(object sender, EventArgs e)
        {
            dal myDal = new dal();

            if (!string.IsNullOrEmpty(tbxPlaka.Text))
            {
                string plaka = tbxPlaka.Text;

                string selectQuery = $"SELECT [KimlikNo], [Plaka], [FirmaAdi], [BrutPrim], [Komisyon], [TeklifNo], [TeklifTipi], [Durum] " +
                                     $"FROM [sigortavipserver].[sigortaVipDbUser].[FiyatBilgileri] " +
                                     $"WHERE [Plaka] = '{plaka}'";

                DataSet data = myDal.CommandExecuteReader(selectQuery, myDal.myConnection);

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    FiyatBilgisi fiyatBilgisi = new FiyatBilgisi()
                    {
                        FirmaAdi = row["FirmaAdi"].ToString(),
                        Durum = row["Durum"].ToString(),
                        BrutPrim = row["BrutPrim"].ToString(),
                        Komisyon = row["Komisyon"].ToString(),
                        TeklifNo = row["TeklifNo"].ToString(),
                        TcKimlik = row["KimlikNo"].ToString(),
                        TeklifTipi = row["TeklifTipi"].ToString(),
                        DurumRengi = Color.Yellow
                    };

                    int rowIndex = AddRow(fiyatBilgisi, dgvFiyatlarTable, 100);
                }

                selectQuery = $"SELECT [KimlikNo], [Plaka], [FirmaAdi], [KBrutPrim], [KKomisyon], [KTeklifNo], [Pesin], [Durum] " +
                                     $"FROM [sigortavipserver].[sigortaVipDbUser].[FiyatBilgileri] " +
                                     $"WHERE [Plaka] = '{plaka}'";

                data = myDal.CommandExecuteReader(selectQuery, myDal.myConnection);

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    FiyatBilgisi fiyatBilgisi = new FiyatBilgisi()
                    {
                        FirmaAdi = row["FirmaAdi"].ToString(),
                        Durum = row["Durum"].ToString(),
                        BrutPrim = row["KBrutPrim"].ToString(),
                        Komisyon = row["KKomisyon"].ToString(),
                        TeklifNo = row["KTeklifNo"].ToString(),
                        TcKimlik = row["KimlikNo"].ToString(),
                        DurumRengi = Color.Yellow,
                    };

                    int rowIndex = AddRow(fiyatBilgisi, dgvFiyatlarTable, 100);
                }
            }
        }



       
        //şirket işlemlerini tekrar yaptıran bölüm************************************************************************************************************************************************************************************************************************************
        private async void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //int rowindx=bandedGridView1.GetDataSourceRowIndex(bandedGridView1.FocusedRowHandle);
            dt.AcceptChanges();
            int rowindx = bandedGridView1.GetFocusedDataSourceRowIndex();
            //MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();

            string sirketAdi = dt.Rows[rowindx]["Firma"].ToString();
            dt.Rows[rowindx]["ack"] = string.Empty;

            //if (e.Button.Caption == "Şirket Aç")
            //{
            //    var c = !Browser.webPageList.Any(x => x.insuranceCompany == sirketAdi);
            //    if (!Browser.webPageList.Any(x => x.insuranceCompany == sirketAdi))
            //    {
            //        foreach (ToolStripMenuItem item in MainForm.sirketMenusu.Items)
            //        {
            //            if (item.Name == "tsr" + sirketAdi)
            //            {
            //                item.PerformClick();
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (Browser.webPageList.Count > 0)
            //        {
            //            var result = ApiClient.getInstance().deleteCookie(Browser.webPageList.FirstOrDefault(x => x.insuranceCompany == sirketAdi).insuranceCompanyId);

            //        }

            //        foreach (ToolStripMenuItem item in MainForm.sirketMenusu.Items)
            //        {
            //            if (item.Name == "tsr" + sirketAdi)
            //            {
            //                item.PerformClick();
            //                MainForm.btnCloseTab.PerformClick();
            //            }
            //        }
            //        foreach (ToolStripMenuItem item in MainForm.sirketMenusu.Items)
            //        {
            //            if (item.Name == "tsr" + sirketAdi)
            //            {
            //                item.PerformClick();
            //            }
            //        }

            //    }
            //}
            //else if (e.Button.Caption == "Kasko Sorgula")
            //{
            //    Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();

            //    this.row_index = rowindx;
            //    progress.ProgressChanged += ReportProgress;
            //    ProgressReportModel report = new ProgressReportModel();

            //    var browsersVar = mainForm.GetBrowsers;

            //    IFiyatSorgu fiyatSorgu = _sorgulamaFactory.GetFiyatSorgu(sirketAdi);
            //    if (Browser.webPageList.Any(x => x.insuranceCompany == sirketAdi))
            //    {
            //        try
            //        {
            //            var resp = await fiyatSorgu.KaskoSorgula(_kullaniciBilgileri, browsersVar[sirketAdi].browser, _cancellationTokenSource.Token, progress, report).ConfigureAwait(false);

            //            if (resp != null)
            //            {
            //                resp.TeklifTipi = "Kasko";
            //                switch (resp.Durum)
            //                {
            //                    case "Tamamlandı":
            //                    case "Tamamlandı (Teminatlı)":
            //                        resp.DurumRengi = Color.FromArgb(102, 255, 102);
            //                        break;
            //                    case "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır":
            //                        resp.DurumRengi = Color.Black;
            //                        break;
            //                    default:
            //                        resp.DurumRengi = Color.Red;
            //                        break;
            //                }
            //                UpdateRow(rowindx, resp);
            //            }
            //        }
            //        catch
            //        {

            //        }


            //    }
            //    else
            //    {
            //        FiyatBilgisi fiyatBilgisi = new FiyatBilgisi();
            //        fiyatBilgisi.Durum = "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır";
            //        fiyatBilgisi.FirmaAdi = sirketAdi;
            //        fiyatBilgisi.TeklifTipi = "Kasko";
            //        UpdateRow(rowindx, fiyatBilgisi);

            //    }

            //}
            //else if (e.Button.Caption == "Trafik Sorgula")
            //{
            //    Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();

            //    this.row_index = rowindx;
            //    progress.ProgressChanged += ReportProgress;
            //    ProgressReportModel report = new ProgressReportModel();

            //    var browsersVar = mainForm.GetBrowsers;

            //    IFiyatSorgu fiyatSorgu = _sorgulamaFactory.GetFiyatSorgu(sirketAdi);
            //    if (Browser.webPageList.Any(x => x.insuranceCompany == sirketAdi))
            //    {
            //        var resp = await fiyatSorgu.TrafikSorgula(_kullaniciBilgileri, browsersVar[sirketAdi].browser, _cancellationTokenSource.Token, progress, report).ConfigureAwait(false);

            //        if (resp != null)
            //        {
            //            resp.TeklifTipi = "Trafik";
            //            switch (resp.Durum)
            //            {
            //                case "Tamamlandı":
            //                case "Tamamlandı (Teminatlı)":
            //                    resp.DurumRengi = Color.FromArgb(102, 255, 102);
            //                    break;
            //                case "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır":
            //                    resp.DurumRengi = Color.Black;
            //                    break;
            //                default:
            //                    resp.DurumRengi = Color.Red;
            //                    break;
            //            }
            //            if (resp.Durum.Contains("Tamamlandı"))
            //            {
            //                resp.DurumRengi = Color.FromArgb(102, 255, 102);
            //            }
            //            UpdateRow(rowindx, resp);
            //        }
            //    }
            //    else
            //    {
            //        FiyatBilgisi fiyatBilgisi = new FiyatBilgisi();
            //        fiyatBilgisi.Durum = "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır";
            //        fiyatBilgisi.FirmaAdi = sirketAdi;
            //        fiyatBilgisi.TeklifTipi = "Trafik";
            //        UpdateRow(rowindx, fiyatBilgisi);

            //    }
            //}
        }

        private void bandedGridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            //if (e.RowHandle < 0) return;
            //try
            //{
            //    int indx = bandedGridView1.GetDataSourceRowIndex(e.RowHandle);
            //    if (e.Column.FieldName == "Durum")
            //    {

            //        DataRow dr = dt.Rows[indx];

            //        if (Convert.ToString(dr["ack"]) == "Tamamlandı")
            //        {
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor = Color.Green;
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor2 = Color.Green;
            //        }
            //        else if (Convert.ToString(dr["ack"]) == "Tamamlandı (Teminatlı)")
            //        {
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor = Color.FromArgb(102, 255, 102);
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor2 = Color.FromArgb(102, 255, 102);
            //        }
            //        else if (Convert.ToString(dr["ack"]) == "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır")
            //        {
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor = Color.Black;
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor2 = Color.Black;
            //        }
            //        else if (string.IsNullOrEmpty(Convert.ToString(dr["ack"])))
            //        {
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor = Color.Yellow;
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor2 = Color.Yellow;
            //        }
            //        else
            //        {
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor = Color.Red;
            //            e.RepositoryItem.LookAndFeel.SkinMaskColor2 = Color.Red;
            //        }


            //    }
            //}
            //catch { }

            /* switch (resp.Durum)
             {
                 case "Tamamlandı":
                 case "Tamamlandı (Teminatlı)":
                     resp.DurumRengi = Color.FromArgb(102, 255, 102);
                     break;
                 case "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır":
                     resp.DurumRengi = Color.Black;
                     break;
                 default:
                     resp.DurumRengi = Color.Red;
                     break;
             }*/
        }

        //void DrawProgressBar(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        //{
        //    int percent = Convert.ToInt16(e.CellValue);

        //    int v = Convert.ToInt32(e.CellValue);
        //    v = v * e.Bounds.Width / 100;
        //    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, v, e.Bounds.Height);
        //    Brush b = Brushes.Green;
        //    if (percent < 25)
        //        b = Brushes.Red;
        //    else if (percent < 50)
        //        b = Brushes.Orange;
        //    else if (percent < 75)
        //        b = Brushes.YellowGreen;
        //    e.Cache.Paint.FillRectangle(e.Cache.Graphics, b, rect);
        //}

        private void bandedGridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            //   e.Graphics.FillRectangle
            try
            {
                int indx = bandedGridView1.GetDataSourceRowIndex(e.RowHandle);
                DataRow dr = dt.Rows[indx];

                if (e.Column.FieldName == "Durum")
                {

                    int v = Convert.ToInt32(dr["Durum"]);
                    v = v * e.Bounds.Width / 100;
                    Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, v, e.Bounds.Height);

                    if (Convert.ToString(dr["ack"]).Contains("Tamamlandı"))
                    {
                        e.Cache.FillRectangle(Brushes.Blue, rect);
                    }
                    else if (Convert.ToString(dr["ack"]) == "Tamamlandı (Teminatlı)")
                    {
                        e.Cache.FillRectangle(Brushes.LimeGreen, rect);
                    }
                    else if (Convert.ToString(dr["ack"]) == "Ekranınız Kapalı Olduğundan Fiyat Alınamamıştır")
                    {
                        e.Cache.FillRectangle(Brushes.Black, rect);
                    }
                    else if (string.IsNullOrEmpty(Convert.ToString(dr["ack"])))
                    {
                        e.Cache.FillRectangle(Brushes.Yellow, rect);
                    }
                    else
                    {
                        e.Cache.FillRectangle(Brushes.Red, rect);
                    }

                    e.Appearance.DrawString(e.Cache, e.DisplayText, e.Bounds);
                    e.Handled = true;

                }

                try
                {
                    if (e.Column.FieldName == "bandedGridColumn3")
                    {
                        e.Column.OptionsColumn.AllowEdit = false;
                        //Byte.Parse
                        byte[] imageData = dtimage.AsEnumerable().Where(w => w["FirmaAdi"].ToString() == dr["Firma"].ToString()).Select(s => (byte[])s["logoBinary"]).FirstOrDefault();
                        if (imageData != null)
                        {
                            MemoryStream ms = new MemoryStream(imageData);
                            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                            Size ss = new Size(1, 1);
                            img = ResizeImage(img, new Size(e.Column.Width, 30));


                            e.Cache.DrawImage(img, e.Bounds.Location);
                        }
                    }
                }
                catch
                { }
            }
            catch { }

        }
        private static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            // Get the image current width
            int sourceWidth = imgToResize.Width;
            // Get the image current height
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calculate width and height with new desired size
            nPercentW = (size.Width / (float)sourceWidth);
            nPercentH = (size.Height / (float)sourceHeight);
            nPercent = Math.Min(nPercentW, nPercentH);
            // New Width and Height
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        private void bandedGridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                    e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
            catch { }
        }

        private void tekrarSorguTRAFIKKASKO(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //hizliTeklifForm HızlıTeklifFORM = (hizliTeklifForm)Application.OpenForms["hizliTeklifForm"];
            //if (HızlıTeklifFORM == null) return;

            param = e.Item.Caption;
            dropDownButton2.Text = e.Item.Caption + " Tekrar Sorgula";
            //if (e.Item.Caption == "Trafik")
            //{

            //}//
            //else if (e.Item.Caption == "Kasko")
            //{
            //    //FONKSİYON
            //}
        }

        private void dropDownButton2_Click(object sender, EventArgs e)
        {
            try
            {
                this.dtackdelete(param);
            }
            catch { }

            foreach (WebPage acikSayfalar in Browser.webPageList)
            {
                if (acikSayfalar.insuranceCompany != "AcnTurkSigorta")
                {
                    acikSayfalar.browser.LoadUrlAsync(acikSayfalar.insuranceCompanyPageUrl);
                }
            }

            _kullaniciBilgileri = _kullaniciBilgileri;
            this._EkPaketFiyatSorgulamaQueue = _EkPaketFiyatSorgulamaQueue;

            var queueMessages = _fiyatSorgulamaQueue.Select(cb => new QueueMessage
            {
                InsuranceCompany = cb.InsuranceCompany,
                SorguTipi = param
            });


            foreach (var message in queueMessages)
            {
                _fiyatSorgulamaQueue.Enqueue(message);
            }



            Setup();
        }
        //raporlama 
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //trafik rapor
            //using (Reports.FiyatRapor ac = new Reports.FiyatRapor())
            //{
            //    CustomSqlQuery query = new CustomSqlQuery();
            //    query.Name = "sigortaVipDbUser_FiyatBilgileri";
            //    query.Sql = "select *,(select top 1 tescilTarihi from AracBilgileri where Tc=TCkimlik) as tescilTarihi,(select top 1 kullanimTarzi from AracBilgileri where Tc = TCkimlik) as kullanimTarzi ,(select top 1 Model + ' / ' + Marka from AracBilgileri where Tc = TCkimlik) as markamodel,(select top 1 Tip from AracBilgileri where Tc = TCkimlik) as Tip,(select top 1 aracKod from AracBilgileri where Tc = TCkimlik) as aracKod,(select top 1 motorno from AracBilgileri where Tc = TCkimlik) as motorno,(select top 1 sasiNo from AracBilgileri where Tc = TCkimlik) as sasiNo from sigortaVipDbUser.FirmaFiyatlari where TcKimlik = '" + tbxTC.Text + "' and Plaka = '" + tbxPlaka.Text + "' and KaskoTrafik='Trafik' and BrutPrim<>'' ";
            //    ac.sqlDataSource1.Queries.Clear();
            //    ac.sqlDataSource1.Queries.Add(query);
            //    ac.DataMember = query.Name;

            //    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";


            //    ac.ExportOptions.Pdf.Compressed = true;
            //    ac.ExportOptions.Pdf.ConvertImagesToJpeg = false;

            //    ac.ExportToPdf(path + tbxPlaka.Text + "_TRAFIK.pdf");
            //}
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //kasko rapor
            //using (Reports.FiyatRapor ac = new Reports.FiyatRapor())
            //{
            //    CustomSqlQuery query = new CustomSqlQuery();
            //    query.Name = "sigortaVipDbUser_FiyatBilgileri";
            //    query.Sql = "select *,(select top 1 tescilTarihi from AracBilgileri where Tc=TCkimlik) as tescilTarihi,(select top 1 kullanimTarzi from AracBilgileri where Tc = TCkimlik) as kullanimTarzi ,(select top 1 Model + ' / ' + Marka from AracBilgileri where Tc = TCkimlik) as markamodel,(select top 1 Tip from AracBilgileri where Tc = TCkimlik) as Tip,(select top 1 aracKod from AracBilgileri where Tc = TCkimlik) as aracKod,(select top 1 motorno from AracBilgileri where Tc = TCkimlik) as motorno,(select top 1 sasiNo from AracBilgileri where Tc = TCkimlik) as sasiNo from sigortaVipDbUser.FirmaFiyatlari where TcKimlik = '" + tbxTC.Text + "' and Plaka = '" + tbxPlaka.Text + "' and KaskoTrafik='Kasko' and BrutPrim<>'' ";
            //    ac.sqlDataSource1.Queries.Clear();
            //    ac.sqlDataSource1.Queries.Add(query);
            //    ac.DataMember = query.Name;

            //    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

            //    ac.ExportOptions.Pdf.Compressed = true;
            //    ac.ExportOptions.Pdf.ConvertImagesToJpeg = false;


            //    ac.ExportToPdf(path + tbxPlaka.Text + "_KASKO.pdf");
            //}
        }
        static string FormatCurrencyValue(string input)
        {
            // Düzensiz karakterleri temizle
            string cleanedInput = Regex.Replace(input, @"[^\d\.,]+", "");

            // Sayıyı kültüre uygun bir formatta çevir
          

            // Eğer çevrim başarısız olursa, orijinal değeri döndür
            return cleanedInput;
        }
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<string> Txtlist = new List<string>();
   

            /*TBrütPrim*/
            try
            {
                Txtlist.Add(tbxPlaka.Text + " plakalı aracınıza ait fiyat sonuçları aşağıda listelenmiştir.");

                DataRow[] dr1 = dt.AsEnumerable().Where(w => !string.IsNullOrEmpty(Convert.ToString(w["TBrütPrim"]))).ToArray();
                if (dr1 != null && dr1.Count() > 0)
                {
                    Txtlist.Add(Environment.NewLine + "--TRAFİK--");
                    for (int i = 0; i < dr1.Count(); i++)
                    {
                        string formatedValue = FormatCurrencyValue(Convert.ToString(dr1[i]["TBrütPrim"]));
                        Txtlist.Add(Convert.ToString(dr1[i]["Firma"]) + "  "  + formatedValue);
                    }
                }


                DataRow[] dr2 = dt.AsEnumerable().Where(w => !string.IsNullOrEmpty(Convert.ToString(w["KBrütPrim"]))).ToArray();
                if (dr2 != null && dr2.Count() > 0)
                {
                    Txtlist.Add(Environment.NewLine + Environment.NewLine + "--KASKO--");
                    for (int i = 0; i < dr2.Count(); i++)

                    {
                        string formatedValue = FormatCurrencyValue(Convert.ToString(dr2[i]["KBrütPrim"]));

                        Txtlist.Add(Convert.ToString(dr2[i]["Firma"]) + "  " + formatedValue);
                    }
                }
            }
            catch { }



            Clipboard.SetText(string.Join(Environment.NewLine, Txtlist));

        }

    }
}
