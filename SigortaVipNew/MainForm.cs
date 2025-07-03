using CefSharp;
using CefSharp.WinForms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;
using DevExpress.XtraTab;
using DevExpress.XtraWaitForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SigortaVip.Models;
using SigortaVipNew.Forms.Controls;
using SigortaVipNew.Forms.Company;
using SigortaVipNew.Forms.Price;
using SigortaVipNew.Models;

namespace SigortaVipNew
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Dictionary<string, ChromiumWebBrowser> browserTabs = new Dictionary<string, ChromiumWebBrowser>();
        private List<InsuranceCompanyInfo> insuranceCompanies;
        private DevExpress.XtraBars.BarButtonItem btnAddCompany;
        private DevExpress.XtraBars.BarButtonItem btnEditCompany;
        private DevExpress.XtraBars.BarButtonItem btnOpenAll;
        private DevExpress.XtraBars.BarButtonItem btnSearch;
        private DevExpress.XtraBars.PopupMenu contextMenu;
        private NavBarGroup companiesGroup;
        private Dictionary<string, string> collectedPrices = new Dictionary<string, string>();
        private bool isCollectingPrices = false;
        private int completedPriceQueries = 0;
        private int totalPriceQueries = 0;
        private bool isClosingTab = false;

        // Fiyat hesaplama parametreleri sınıfı
        public class PriceCalculationParams
        {
            public string VehicleType { get; set; } = "Otomobil";  // Araç tipi
            public int ModelYear { get; set; } = DateTime.Now.Year;  // Model yılı
            public string MakeModel { get; set; } = "Toyota Corolla";  // Marka/Model
            public bool HasDamageRecord { get; set; } = false;  // Hasar kaydı var mı
            public string InsuranceType { get; set; } = "Trafik Sigortası";  // Sigorta tipi
            public decimal EstimatedValue { get; set; } = 300000;  // Tahmini değer
            public string LicensePlate { get; set; } = "";  // Plaka
            public string IdentityNumber { get; set; } = "";  // TC Kimlik No
        }

        // Fiyat toplamak için gerekli bilgileri toplayan form
        public class PriceInputForm : DevExpress.XtraEditors.XtraForm
        {
            private DevExpress.XtraEditors.GroupControl vehicleGroup;
            private DevExpress.XtraEditors.GroupControl personGroup;
            private DevExpress.XtraEditors.GroupControl insuranceGroup;
            
            private DevExpress.XtraEditors.ComboBoxEdit cmbVehicleType;
            private DevExpress.XtraEditors.TextEdit txtMakeModel;
            private DevExpress.XtraEditors.SpinEdit spnModelYear;
            private DevExpress.XtraEditors.CheckEdit chkHasDamage;
            private DevExpress.XtraEditors.SpinEdit spnValue;
            private DevExpress.XtraEditors.TextEdit txtLicensePlate;
            private DevExpress.XtraEditors.TextEdit txtIdentityNumber;
            private DevExpress.XtraEditors.ComboBoxEdit cmbInsuranceType;
            
            private DevExpress.XtraEditors.SimpleButton btnCancel;
            private DevExpress.XtraEditors.SimpleButton btnCalculate;
            
            public PriceCalculationParams CalculationParams { get; private set; }
            
            public PriceInputForm()
            {
                CalculationParams = new PriceCalculationParams();
                InitializeComponent();
                LoadDefaults();
            }
            
            private void InitializeComponent()
            {
                this.Text = "Fiyat Hesaplama Parametreleri";
                this.StartPosition = FormStartPosition.CenterParent;
                this.Size = new Size(550, 550);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                
                // Ana panel
                var mainPanel = new DevExpress.XtraEditors.PanelControl();
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.Padding = new Padding(10);
                this.Controls.Add(mainPanel);
                
                // Araç bilgileri grubu
                vehicleGroup = new DevExpress.XtraEditors.GroupControl();
                vehicleGroup.Text = "Araç Bilgileri";
                vehicleGroup.Dock = DockStyle.Top;
                vehicleGroup.Height = 180;
                vehicleGroup.Padding = new Padding(5);
                mainPanel.Controls.Add(vehicleGroup);
                
                var vehicleLayout = new DevExpress.XtraLayout.LayoutControl();
                vehicleLayout.Dock = DockStyle.Fill;
                vehicleGroup.Controls.Add(vehicleLayout);
                
                var vehicleRoot = new DevExpress.XtraLayout.LayoutControlGroup();
                vehicleRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
                vehicleLayout.Root = vehicleRoot;
                
                // Araç tipi
                cmbVehicleType = new DevExpress.XtraEditors.ComboBoxEdit();
                cmbVehicleType.Properties.Items.AddRange(new string[] { "Otomobil", "SUV", "Ticari Araç", "Kamyonet", "Motosiklet" });
                vehicleLayout.Controls.Add(cmbVehicleType);
                var vehicleTypeItem = new DevExpress.XtraLayout.LayoutControlItem();
                vehicleTypeItem.Control = cmbVehicleType;
                vehicleTypeItem.Text = "Araç Tipi:";
                vehicleRoot.AddItem(vehicleTypeItem);
                
                // Marka/Model
                txtMakeModel = new DevExpress.XtraEditors.TextEdit();
                vehicleLayout.Controls.Add(txtMakeModel);
                var makeModelItem = new DevExpress.XtraLayout.LayoutControlItem();
                makeModelItem.Control = txtMakeModel;
                makeModelItem.Text = "Marka/Model:";
                vehicleRoot.AddItem(makeModelItem);
                
                // Model yılı
                spnModelYear = new DevExpress.XtraEditors.SpinEdit();
                spnModelYear.Properties.MinValue = 1990;
                spnModelYear.Properties.MaxValue = DateTime.Now.Year;
                vehicleLayout.Controls.Add(spnModelYear);
                var modelYearItem = new DevExpress.XtraLayout.LayoutControlItem();
                modelYearItem.Control = spnModelYear;
                modelYearItem.Text = "Model Yılı:";
                vehicleRoot.AddItem(modelYearItem);
                
                // Hasar kaydı
                chkHasDamage = new DevExpress.XtraEditors.CheckEdit();
                chkHasDamage.Text = "Hasar Kaydı Var";
                vehicleLayout.Controls.Add(chkHasDamage);
                var hasDamageItem = new DevExpress.XtraLayout.LayoutControlItem();
                hasDamageItem.Control = chkHasDamage;
                hasDamageItem.Text = "";
                vehicleRoot.AddItem(hasDamageItem);
                
                // Tahmini değer
                spnValue = new DevExpress.XtraEditors.SpinEdit();
                spnValue.Properties.Mask.EditMask = "n0";
                spnValue.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                spnValue.Properties.Mask.UseMaskAsDisplayFormat = true;
                spnValue.Properties.MinValue = 10000;
                spnValue.Properties.MaxValue = 10000000;
                spnValue.Properties.Increment = 5000;
                vehicleLayout.Controls.Add(spnValue);
                var valueItem = new DevExpress.XtraLayout.LayoutControlItem();
                valueItem.Control = spnValue;
                valueItem.Text = "Tahmini Değer (TL):";
                vehicleRoot.AddItem(valueItem);
                
                // Kişi bilgileri grubu
                personGroup = new DevExpress.XtraEditors.GroupControl();
                personGroup.Text = "Kişi Bilgileri";
                personGroup.Dock = DockStyle.Top;
                personGroup.Height = 120;
                personGroup.Padding = new Padding(5);
                personGroup.Top = vehicleGroup.Bottom + 10;
                mainPanel.Controls.Add(personGroup);
                
                var personLayout = new DevExpress.XtraLayout.LayoutControl();
                personLayout.Dock = DockStyle.Fill;
                personGroup.Controls.Add(personLayout);
                
                var personRoot = new DevExpress.XtraLayout.LayoutControlGroup();
                personRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
                personLayout.Root = personRoot;
                
                // TC Kimlik No
                txtIdentityNumber = new DevExpress.XtraEditors.TextEdit();
                txtIdentityNumber.Properties.MaxLength = 11;
                personLayout.Controls.Add(txtIdentityNumber);
                var identityItem = new DevExpress.XtraLayout.LayoutControlItem();
                identityItem.Control = txtIdentityNumber;
                identityItem.Text = "TC Kimlik No:";
                personRoot.AddItem(identityItem);
                
                // Plaka
                txtLicensePlate = new DevExpress.XtraEditors.TextEdit();
                txtLicensePlate.Properties.MaxLength = 10;
                personLayout.Controls.Add(txtLicensePlate);
                var plateItem = new DevExpress.XtraLayout.LayoutControlItem();
                plateItem.Control = txtLicensePlate;
                plateItem.Text = "Plaka:";
                personRoot.AddItem(plateItem);
                
                // Sigorta bilgileri grubu
                insuranceGroup = new DevExpress.XtraEditors.GroupControl();
                insuranceGroup.Text = "Sigorta Bilgileri";
                insuranceGroup.Dock = DockStyle.Top;
                insuranceGroup.Height = 100;
                insuranceGroup.Padding = new Padding(5);
                insuranceGroup.Top = personGroup.Bottom + 10;
                mainPanel.Controls.Add(insuranceGroup);
                
                var insuranceLayout = new DevExpress.XtraLayout.LayoutControl();
                insuranceLayout.Dock = DockStyle.Fill;
                insuranceGroup.Controls.Add(insuranceLayout);
                
                var insuranceRoot = new DevExpress.XtraLayout.LayoutControlGroup();
                insuranceRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
                insuranceLayout.Root = insuranceRoot;
                
                // Sigorta tipi
                cmbInsuranceType = new DevExpress.XtraEditors.ComboBoxEdit();
                cmbInsuranceType.Properties.Items.AddRange(new string[] {
                    "Trafik Sigortası", 
                    "Kasko", 
                    "Sağlık Sigortası", 
                    "DASK", 
                    "Konut Sigortası"
                });
                insuranceLayout.Controls.Add(cmbInsuranceType);
                var insuranceTypeItem = new DevExpress.XtraLayout.LayoutControlItem();
                insuranceTypeItem.Control = cmbInsuranceType;
                insuranceTypeItem.Text = "Sigorta Tipi:";
                insuranceRoot.AddItem(insuranceTypeItem);
                
                // Butonlar için panel
                var buttonPanel = new DevExpress.XtraEditors.PanelControl();
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Height = 60;
                buttonPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                mainPanel.Controls.Add(buttonPanel);
                
                // İptal butonu
                btnCancel = new DevExpress.XtraEditors.SimpleButton();
                btnCancel.Text = "İptal";
                btnCancel.Width = 100;
                btnCancel.Height = 35;
                btnCancel.Location = new Point(buttonPanel.Width - 230, 15);
                btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnCancel.Click += (s, e) => {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                };
                buttonPanel.Controls.Add(btnCancel);
                
                // Hesapla butonu
                btnCalculate = new DevExpress.XtraEditors.SimpleButton();
                btnCalculate.Text = "Fiyat Hesapla";
                btnCalculate.Width = 120;
                btnCalculate.Height = 35;
                btnCalculate.Location = new Point(buttonPanel.Width - 120, 15);
                btnCalculate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnCalculate.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
                btnCalculate.Appearance.ForeColor = System.Drawing.Color.White;
                btnCalculate.Click += BtnCalculate_Click;
                buttonPanel.Controls.Add(btnCalculate);
                
                // Olayları bağla
                this.Load += (s, e) => {
                    // Form yüklendiğinde odaklanma
                    cmbVehicleType.Select();
                };
            }
            
            private void LoadDefaults()
            {
                cmbVehicleType.SelectedItem = CalculationParams.VehicleType;
                txtMakeModel.Text = CalculationParams.MakeModel;
                spnModelYear.Value = CalculationParams.ModelYear;
                chkHasDamage.Checked = CalculationParams.HasDamageRecord;
                spnValue.Value = (decimal)CalculationParams.EstimatedValue;
                txtLicensePlate.Text = CalculationParams.LicensePlate;
                txtIdentityNumber.Text = CalculationParams.IdentityNumber;
                cmbInsuranceType.SelectedItem = CalculationParams.InsuranceType;
            }
            
            private void BtnCalculate_Click(object sender, EventArgs e)
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtMakeModel.Text))
                {
                    MessageBox.Show("Lütfen araç marka/modelini girin.", "Eksik Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMakeModel.Focus();
                    return;
                }
                
                // TC Kimlik No kontrolü (opsiyonel)
                if (!string.IsNullOrEmpty(txtIdentityNumber.Text) && txtIdentityNumber.Text.Length != 11)
                {
                    MessageBox.Show("TC Kimlik No 11 haneli olmalıdır.", "Hatalı Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtIdentityNumber.Focus();
                    return;
                }
                
                // Bilgileri modele aktar
                CalculationParams.VehicleType = cmbVehicleType.Text;
                CalculationParams.MakeModel = txtMakeModel.Text;
                CalculationParams.ModelYear = (int)spnModelYear.Value;
                CalculationParams.HasDamageRecord = chkHasDamage.Checked;
                CalculationParams.EstimatedValue = spnValue.Value;
                CalculationParams.LicensePlate = txtLicensePlate.Text;
                CalculationParams.IdentityNumber = txtIdentityNumber.Text;
                CalculationParams.InsuranceType = cmbInsuranceType.Text;
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public MainForm()
        {
            InitializeComponent();
            InitializeCef();
            // Browser dictionary'sini başlangıçta temizle
            browserTabs = new Dictionary<string, ChromiumWebBrowser>();
            LoadCompanies();
            
            // Temalar ve görsel ayarlarını uygula
            ApplyVisualStyle();
            
            SetupUI();
            SetupRibbonItems();
            SetupKeyboardShortcuts();

            // Form kapanmasını engelle (X butonuna basılsa bile)
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapatma olayından geldiyse ve bir sekme kapatma işlemi değilse
            if (e.CloseReason == CloseReason.UserClosing && !isClosingTab)
            {
                // Normal kapanma onayı
                if (MessageBox.Show("Uygulamayı kapatmak istiyor musunuz?", "Çıkış Onayı",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void InitializeCef()
        {
            try
            {
            var settings = new CefSettings();

                // Sistem proxy ayarlarını kullan (HTTP 407 hatası için)
                settings.CefCommandLineArgs.Add("enable-system-proxy", "1");
                settings.CefCommandLineArgs.Add("no-proxy-server", "0");

                // Cef daha önce başlatılmadıysa başlat
                if (!Cef.IsInitialized.GetValueOrDefault())
                {
                Cef.Initialize(settings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tarayıcı başlatılırken hata oluştu: {ex.Message}", 
                    "Başlatma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCompanies()
        {
            LoginScriptMappings loginScriptMappings = new LoginScriptMappings();
            loginScriptMappings.LoadMappings();
            insuranceCompanies = loginScriptMappings.GetInsuranceCompanies();
        }

        private void ApplyVisualStyle()
        {
            // Modern tema uygula
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(DevExpress.LookAndFeel.SkinStyle.Office2019Colorful);
            
            // Form özellikleri
            this.Text = "Sigorta VIP";
            // İkon ayarlamasını geçici olarak kaldırıldı - hata veriyor
            // this.Icon = DevExpress.Utils.ResourceImageHelper.CreateIconFromResources("DevExpress.Images.Office2013.About.png", typeof(DevExpress.Utils.ResourceImageHelper).Assembly);
            
            // Ribbon renk ayarları
            ribbon.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2019;
            ribbon.ColorScheme = DevExpress.XtraBars.Ribbon.RibbonControlColorScheme.Blue;
        }

        private void SetupUI()
        {
            // NavBarControl'ün görünümünü ayarla - sabit genişlik ve düğmesiz
            navPane.OptionsNavPane.ExpandedWidth = 220;
            navPane.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            navPane.OptionsNavPane.ShowExpandButton = false; // Genişletme düğmesi kaldırıldı
            navPane.OptionsNavPane.ShowOverflowButton = false; // Taşma düğmesi kaldırıldı
            navPane.OptionsNavPane.ShowOverflowPanel = false; // Taşma paneli kaldırıldı
            navPane.OptionsNavPane.ShowSplitter = false; // Bölücü çizgiyi kaldır
            navPane.PaintStyleKind = NavBarViewKind.NavigationPane;
            navPane.StoreDefaultPaintStyleName = true;
            navPane.Dock = DockStyle.Left;

            // Görsel stil ayarları
            navPane.Appearance.Background.BackColor = Color.FromArgb(240, 240, 240);
            navPane.Appearance.GroupHeader.Font = new Font("Segoe UI Semibold", 10F);
            navPane.Appearance.GroupHeader.Options.UseFont = true;
            navPane.Appearance.Item.Font = new Font("Segoe UI", 9.75F);
            navPane.Appearance.Item.Options.UseFont = true;

            // Mevcut grupları temizle
            navPane.Groups.Clear();

            // Şirketler grubu
            companiesGroup = new NavBarGroup("Sigorta Firmaları");
            companiesGroup.Expanded = true;
            companiesGroup.SmallImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocontact2_16x16.png");
            companiesGroup.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocontact2_32x32.png");
            navPane.Groups.Add(companiesGroup);

            // Şirketleri NavBar'a ekle
            LoadCompaniesToNavBar(insuranceCompanies);

            // Sekme kontrolü kurulumu
            tabControl.CloseButtonClick += TabControl_CloseButtonClick;
            tabControl.AppearancePage.Header.Font = new Font("Segoe UI", 9.75F);
            tabControl.AppearancePage.Header.Options.UseFont = true;
            
            // Anasayfa sekmesinin kapatma butonunu gizle
            tabMainPage.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
            tabMainPage.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/navigation/home_16x16.png");

            // Ana sayfaya hoşgeldiniz içeriği ekle
            CreateWelcomePage(tabMainPage);

            // NavBar öğeleri için sağ tık menüsü
            contextMenu = new DevExpress.XtraBars.PopupMenu(components);
            btnEditCompany = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Düzenle");
            btnEditCompany.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_16x16.png");
            btnEditCompany.ItemClick += BtnEditCompany_ItemClick;
            contextMenu.AddItem(btnEditCompany);

            navPane.MouseUp += NavPane_MouseUp;
        }

        private void CreateWelcomePage(XtraTabPage page)
        {
            // Mevcut içeriği temizle
            page.Controls.Clear();

            // Layout panel oluştur
            var layoutPanel = new DevExpress.XtraEditors.PanelControl
            {
                Dock = DockStyle.Fill,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
                Appearance = { BackColor = Color.White }
            };
            page.Controls.Add(layoutPanel);

            // Logo/resim alanı
            var logoPanel = new DevExpress.XtraEditors.PanelControl
            {
                Dock = DockStyle.Top,
                Height = 150,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };
            layoutPanel.Controls.Add(logoPanel);

            // Logo resmi
            var logoPictureEdit = new DevExpress.XtraEditors.PictureEdit
            {
                Dock = DockStyle.Fill,
                Properties = { 
                    SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom,
                    ShowMenu = false 
                }
            };
            // DevExpress'ten örnek logo
            logoPictureEdit.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/support/info_32x32.png");
            logoPanel.Controls.Add(logoPictureEdit);

            // Başlık
            var lblTitle = new DevExpress.XtraEditors.LabelControl
            {
                Text = "Sigorta VIP Uygulamasına Hoşgeldiniz",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 114, 198),
                Appearance = { TextOptions = { HAlignment = HorzAlignment.Center } },
                AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None,
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(0, 20, 0, 0)
            };
            layoutPanel.Controls.Add(lblTitle);

            // Alt başlık
            var lblSubtitle = new DevExpress.XtraEditors.LabelControl
            {
                Text = "Sigorta işlemlerinizi hızlı ve kolay bir şekilde yönetin",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Appearance = { TextOptions = { HAlignment = HorzAlignment.Center } },
                AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None,
                Dock = DockStyle.Top,
                Height = 30
            };
            layoutPanel.Controls.Add(lblSubtitle);

            // Kısayol bilgileri
            var shortcutPanel = new DevExpress.XtraEditors.PanelControl
            {
                Dock = DockStyle.Top,
                Height = 120,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
                Margin = new Padding(50)
            };
            layoutPanel.Controls.Add(shortcutPanel);

            // Kısayol bilgileri ekle
            var lblShortcuts = new DevExpress.XtraEditors.LabelControl
            {
                Text = "Klavye Kısayolları:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(30, 10)
            };
            shortcutPanel.Controls.Add(lblShortcuts);

            var shortcuts = new[]
            {
                new { Key = "CTRL+K", Desc = "Şirket Ara", Img = "search" },
                new { Key = "CTRL+W", Desc = "Aktif Sekmeyi Kapat", Img = "close" },
                new { Key = "CTRL+R", Desc = "Sayfayı Yenile", Img = "refresh" }
            };

            for (int i = 0; i < shortcuts.Length; i++)
            {
                var sc = shortcuts[i];
                
                var keyImg = new DevExpress.XtraEditors.PictureEdit
                {
                    Image = DevExpress.Images.ImageResourceCache.Default.GetImage($"images/actions/{sc.Img}_16x16.png"),
                    Location = new Point(35, 40 + i * 25),
                    Properties = { 
                        BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
                        ShowMenu = false
                    },
                    Size = new Size(16, 16)
                };
                
                var lblKey = new DevExpress.XtraEditors.LabelControl
                {
                    Text = sc.Key,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 114, 198),
                    Location = new Point(60, 40 + i * 25)
                };
                
                var lblDesc = new DevExpress.XtraEditors.LabelControl
                {
                    Text = sc.Desc,
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.FromArgb(80, 80, 80),
                    Location = new Point(130, 40 + i * 25)
                };
                
                shortcutPanel.Controls.Add(keyImg);
                shortcutPanel.Controls.Add(lblKey);
                shortcutPanel.Controls.Add(lblDesc);
            }

            // Tarih ve saat
            var clockTimer = new System.Windows.Forms.Timer();
            var lblDateTime = new DevExpress.XtraEditors.LabelControl
            {
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(150, 150, 150),
                Dock = DockStyle.Bottom,
                Height = 30,
                Appearance = { TextOptions = { HAlignment = HorzAlignment.Far } },
                Padding = new Padding(0, 0, 10, 10)
            };
            layoutPanel.Controls.Add(lblDateTime);
            
            // Zamanı güncelle
            void UpdateTime(object s, EventArgs e) 
            { 
                lblDateTime.Text = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss"); 
            }
            
            clockTimer.Interval = 1000;
            clockTimer.Tick += UpdateTime;
            clockTimer.Start();
            UpdateTime(null, null);
        }

        private void LoadCompaniesToNavBar(List<InsuranceCompanyInfo> companies)
        {
            // Önce mevcut öğeleri temizle
            companiesGroup.ItemLinks.Clear();

            // Şirketleri NavBar'a ekle
            foreach (var company in companies)
            {
                var navItem = new NavBarItem
                {
                    Caption = company.CompanyName,
                    Tag = company
                };
                
                // Şirket adının baş harfini al
                char firstLetter = company.CompanyName.Length > 0 ? 
                    company.CompanyName[0] : 'S';
                
                // Şirket için ikon belirle
                navItem.SmallImage = GetCompanyIcon(company.CompanyName);
                
                navItem.LinkClicked += NavItem_LinkClicked;
                companiesGroup.ItemLinks.Add(navItem);
            }
        }

        private Image GetCompanyIcon(string companyName)
        {
            // Şirket adı bazı kelimeler içeriyorsa özel ikonlar göster
            string nameLower = companyName.ToLower();
            
            if (nameLower.Contains("aksigorta") || nameLower.Contains("ak sigorta"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocustomer_16x16.png");
            else if (nameLower.Contains("allianz"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bosale_16x16.png");
            else if (nameLower.Contains("anadolu"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/boresource_16x16.png");
            else if (nameLower.Contains("mapfre"))
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/boproduct_16x16.png");
            else
                return DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocontact_16x16.png");
        }

        private void NavPane_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                NavBarHitInfo hitInfo = navPane.CalcHitInfo(e.Location);
                if (hitInfo.InLink)
                {
                    navPane.SelectedLink = hitInfo.Link;
                    contextMenu.ShowPopup(navPane.PointToScreen(e.Location));
                }
            }
        }

        private void BtnEditCompany_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (navPane.SelectedLink != null)
            {
                var company = (InsuranceCompanyInfo)navPane.SelectedLink.Item.Tag;
                EditCompany(company);
            }
        }

        private void EditCompany(InsuranceCompanyInfo company)
        {
            using (var form = new CompanyDetailForm(company))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Update UI to reflect changes
                    foreach (NavBarItemLink link in companiesGroup.ItemLinks)
                    {
                        if (link.Item.Tag == company)
                        {
                            link.Item.Caption = company.CompanyName;
                        }
                    }

                    // Update tab if open
                    string tabKey = $"tab_{company.CompanyName}";
                    foreach (XtraTabPage page in tabControl.TabPages)
                    {
                        if (page.Tag != null && page.Tag.ToString() == tabKey)
                        {
                            page.Text = company.CompanyName;
                            // Reload if URL changed
                            var browser = browserTabs[tabKey];
                            if (browser.Address != company.CompanyUrl)
                            {
                                browser.Load(company.CompanyUrl);
                            }
                        }
                    }
                }
            }
        }

        private void SetupRibbonItems()
        {
            // Ana sayfa ribbon grubuna "Toplu Aç" butonu ekle
            var operationsGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup("İşlemler");
            ribbonPage1.Groups.Add(operationsGroup);
            
            btnOpenAll = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Tüm Şirketleri Aç");
            btnOpenAll.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/open_16x16.png");
            btnOpenAll.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/open_32x32.png");
            btnOpenAll.ItemClick += BtnOpenAll_ItemClick;
            ribbon.Items.Add(btnOpenAll);
            operationsGroup.ItemLinks.Add(btnOpenAll);

            // Şirket arama butonu
            btnSearch = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Şirket Ara");
            btnSearch.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/search_16x16.png");
            btnSearch.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/search_32x32.png");
            btnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut(Keys.Control | Keys.K);
            btnSearch.ItemClick += BtnSearch_ItemClick;
            ribbon.Items.Add(btnSearch);
            operationsGroup.ItemLinks.Add(btnSearch);

            // Sayfa yenileme butonu
            var refreshBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Sayfayı Yenile");
            refreshBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/refresh_16x16.png");
            refreshBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/refresh_32x32.png");
            refreshBtn.ItemShortcut = new DevExpress.XtraBars.BarShortcut(Keys.Control | Keys.R);
            refreshBtn.ItemClick += (s, e) => RefreshCurrentTab();
            ribbon.Items.Add(refreshBtn);
            operationsGroup.ItemLinks.Add(refreshBtn);

            // Sekme kapatma butonu
            var closeTabBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Sekmeyi Kapat");
            closeTabBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/close_16x16.png");
            closeTabBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/close_32x32.png");
            closeTabBtn.ItemShortcut = new DevExpress.XtraBars.BarShortcut(Keys.Control | Keys.W);
            closeTabBtn.ItemClick += (s, e) => CloseCurrentTab();
            ribbon.Items.Add(closeTabBtn);
            operationsGroup.ItemLinks.Add(closeTabBtn);

            // Fiyat toplama butonu
            var getPricesBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Fiyat Topla");
            getPricesBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocustomer_16x16.png");
            getPricesBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/business%20objects/bocustomer_32x32.png");
            getPricesBtn.ItemClick += (s, e) => StartPriceCollection(insuranceCompanies);
            ribbon.Items.Add(getPricesBtn);
            operationsGroup.ItemLinks.Add(getPricesBtn);

            // Add company management ribbon
            var ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage("Şirket Yönetimi");
            ribbon.Pages.Add(ribbonPage);

            var ribbonGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup("Şirketler");
            ribbonPage.Groups.Add(ribbonGroup);

            btnAddCompany = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Yeni Şirket");
            btnAddCompany.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/new_16x16.png");
            btnAddCompany.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/new_32x32.png");
            btnAddCompany.ItemClick += BtnAddCompany_ItemClick;
            ribbon.Items.Add(btnAddCompany);
            ribbonGroup.ItemLinks.Add(btnAddCompany);

            // Şirket düzenleme butonu
            var editCompanyBtn = new DevExpress.XtraBars.BarButtonItem(ribbon.Manager, "Şirketi Düzenle");
            editCompanyBtn.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_16x16.png");
            editCompanyBtn.ImageOptions.LargeImage = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_32x32.png");
            editCompanyBtn.ItemClick += (s, e) => 
            {
                XtraTabPage page = tabControl.SelectedTabPage;
                if (page != null && page != tabMainPage)
                {
                    var tabKey = page.Tag?.ToString();
                    if (!string.IsNullOrEmpty(tabKey))
                    {
                        string companyName = tabKey.Substring(4); // "tab_" kısmını çıkar
                        var company = insuranceCompanies.FirstOrDefault(c => c.CompanyName == companyName);
                        if (company != null)
                        {
                            EditCompany(company);
                        }
                    }
                }
            };
            ribbon.Items.Add(editCompanyBtn);
            ribbonGroup.ItemLinks.Add(editCompanyBtn);
        }

        private void SetupKeyboardShortcuts()
        {
            // Klavye kısayolları için
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.K)
                {
                    e.Handled = true;
                    ShowCompanySearchDialog();
                }
                else if (e.Control && e.KeyCode == Keys.W)
                {
                    e.Handled = true;
                    CloseCurrentTab();
                }
                else if (e.Control && e.KeyCode == Keys.R)
                {
                    e.Handled = true;
                    RefreshCurrentTab();
                }
            };
        }

        private void CloseCurrentTab()
        {
            // Sekme kapatma bayrağını aktifleştir
            isClosingTab = true;
            
            try
            {
                // Aktif sekmeyi al
                XtraTabPage currentPage = tabControl.SelectedTabPage;
                
                // Ana sayfa sekmesi değilse kapat
                if (currentPage != null && currentPage != tabMainPage)
                {
                    var tabKey = currentPage.Tag?.ToString();
                    if (!string.IsNullOrEmpty(tabKey) && browserTabs.ContainsKey(tabKey))
                    {
                        try
                        {
                            // Önce browser'ı remove et, sonra dispose et
                            var browser = browserTabs[tabKey];
                            browserTabs.Remove(tabKey);
                            
                            // Browser'ı dispose et
                            if (browser != null && !browser.IsDisposed)
                            {
                                browser.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Browser kapatılırken hata: {ex.Message}");
                        }
                    }
                    
                    try
                    {
                        // Sekmeyi seçme durumunu önce ana sayfaya geçirerek değiştir
                        tabControl.SelectedTabPage = tabMainPage;
                        
                        // Ardından kaldır
                        tabControl.TabPages.Remove(currentPage);
                        currentPage.Dispose();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Sekme kaldırılırken hata: {ex.Message}");
                    }
                }
            }
            finally
            {
                // Sekme kapatma bayrağını sıfırla
                isClosingTab = false;
            }
        }

        private void RefreshCurrentTab()
        {
            XtraTabPage currentPage = tabControl.SelectedTabPage;
            
            // Ana sayfa değilse ve içerisinde browser varsa yenile
            if (currentPage != null && currentPage != tabMainPage)
            {
                var tabKey = currentPage.Tag?.ToString();
                if (!string.IsNullOrEmpty(tabKey) && browserTabs.ContainsKey(tabKey))
                {
                    var browser = browserTabs[tabKey];
                    if (browser != null && !browser.IsDisposed)
                    {
                        // Sayfayı tamamen yenile (önbellekten değil)
                        browser.GetBrowser().Reload(true);
                        
                        // Opsiyonel: Yenileniyor mesajı göster
                        browser.LoadingStateChanged += (s, e) => {
                            if (e.IsLoading)
                            {
                                // Yükleniyor
                                this.Invoke(new Action(() => {
                                    this.Text = $"{currentPage.Text} - Yenileniyor...";
                                }));
                            }
                            else
                            {
                                // Yükleme tamamlandı
                                this.Text = "Sigorta VIP";
                            }
                        };
                    }
                }
            }
        }

        private void BtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowCompanySearchDialog();
        }

        private void ShowCompanySearchDialog()
        {
            using (var searchForm = new CompanySearchForm(insuranceCompanies))
            {
                if (searchForm.ShowDialog() == DialogResult.OK && searchForm.SelectedCompany != null)
                {
                    // Seçilen şirketin sayfasına git
                    OpenCompanyTab(searchForm.SelectedCompany);
                }
            }
        }

        private void BtnAddCompany_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var newCompany = new InsuranceCompanyInfo
            {
                CompanyName = "Yeni Şirket",
                CompanyUrl = "https://www.example.com",
                Description = "Yeni şirket açıklaması",
                LastAccessTime = DateTime.Now,
                IsFavorite = false
            };

            using (var form = new CompanyDetailForm(newCompany))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    insuranceCompanies.Add(newCompany);

                    // Add to navigation
                    var navItem = new NavBarItem
                    {
                        Caption = newCompany.CompanyName,
                        Tag = newCompany
                    };
                    navItem.LinkClicked += NavItem_LinkClicked;
                    companiesGroup.ItemLinks.Add(navItem);
                }
            }
        }

        private void NavItem_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            var navItem = e.Link.Item;
            var company = (InsuranceCompanyInfo)navItem.Tag;
            OpenCompanyTab(company);
        }

        private void OpenCompanyTab(InsuranceCompanyInfo company)
        {
            // Check if tab already exists
            string tabKey = $"tab_{company.CompanyName}";
            
            // If tab exists, just select it
            foreach (XtraTabPage page in tabControl.TabPages)
            {
                if (page.Tag != null && page.Tag.ToString() == tabKey)
                {
                    tabControl.SelectedTabPage = page;
                    return;
                }
            }

            try
            {
                // Create new tab
                var tabPage = new XtraTabPage
                {
                    Text = company.CompanyName,
                    Tag = tabKey,
                    ShowCloseButton = DevExpress.Utils.DefaultBoolean.True
                };

                // Yükleniyor mesajını göster
                var loadingLabel = new LabelControl
                {
                    Text = "Sayfa yükleniyor...",
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    Appearance = { TextOptions = { HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center } },
                Dock = DockStyle.Fill
            };
                tabPage.Controls.Add(loadingLabel);
                tabControl.TabPages.Add(tabPage);
                tabControl.SelectedTabPage = tabPage;

                // Browser ayarlarını özelleştir
                var browserSettings = new BrowserSettings
                {
                    DefaultEncoding = "UTF-8",
                    Javascript = CefState.Enabled,
                    LocalStorage = CefState.Enabled,
                    //ApplicationCache = CefState.Enabled,
                    ImageLoading = CefState.Enabled,
                    //WebSecurity = CefState.Disabled
                };

                // Her zaman yeni browser oluştur - mevcut olsa bile
                if (browserTabs.ContainsKey(tabKey))
                {
                    // Eski browser'ı dictionary'den kaldır
                    browserTabs.Remove(tabKey);
                }

                // Yeni browser oluştur
                var browser = new ChromiumWebBrowser(string.Empty)
                {
                    Dock = DockStyle.Fill,
                    BrowserSettings = browserSettings
                };

                // Hata ayıklama işleyicileri
                browser.LoadError += Browser_LoadError;
                browser.ConsoleMessage += Browser_ConsoleMessage;
                browser.FrameLoadEnd += Browser_FrameLoadEnd;

                // Tarayıcıyı saklayalım
                browserTabs.Add(tabKey, browser);

                // Yükleniyor mesajını kaldır ve tarayıcıyı ekle
                Task.Run(() => 
                {
                    Thread.Sleep(500); // Kısa bir gecikme
                    if (!this.IsDisposed && !tabPage.IsDisposed)
                    {
                        this.Invoke(new Action(() => 
                        {
                            if (!this.IsDisposed && !tabPage.IsDisposed)
                            {
                                tabPage.Controls.Remove(loadingLabel);
                                tabPage.Controls.Add(browserTabs[tabKey]);

                                // Siteyi yükle - direkt URL yerine önce boş sayfa, sonra navigate
                                browserTabs[tabKey].Load(company.CompanyUrl);
                            }
                        }));
                    }
                });

                // Update last access time
                company.LastAccessTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sayfa açılırken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                var browser = (ChromiumWebBrowser)sender;
                var browserTabKey = GetTabKeyFromBrowser(browser);
                var companyName = GetCompanyNameFromTabKey(browserTabKey);
                
                // Sayfa başarıyla yüklendiyse
                if (e.HttpStatusCode == 200)
                {
                    // Sigorta firmasına özel JavaScript kodlarını çalıştırmak için
                    if (isCollectingPrices && !string.IsNullOrEmpty(companyName))
                    {
                        ExecutePriceCollectionScript(browser, companyName);
                    }
                    else
                    {
                        // Normal sayfa yüklemesi için genel düzenlemeler
                        this.Invoke(new Action(() =>
                        {
                            // Sayfa kodunda iyileştirmeler
                            browser.ExecuteScriptAsync(@"
                                document.body.style.zoom = '100%';
                                // DevTools'u devre dışı bırak
                                document.addEventListener('keydown', function(e) {
                                    if (e.keyCode === 123 || (e.ctrlKey && e.shiftKey && e.keyCode === 73)) {
                                        e.preventDefault();
                                    }
                                });
                            ");
                        }));
                    }
                }
                else
                {
                    // Hata durumunda
                    this.Invoke(new Action(() =>
                    {
                        // Sayfa yüklenemediğinde isCollectingPrices modunda ise hata bildir
                        if (isCollectingPrices && !string.IsNullOrEmpty(companyName))
                        {
                            UpdatePriceCollection(companyName, "HATA: Sayfa yüklenemedi", false);
                        }
                        
                        // Sayfa kodunda iyileştirmeler yapabilmemiz için
                        browser.ExecuteScriptAsync(@"
                            document.body.style.zoom = '100%';
                            // Diğer JavaScript düzeltmeleri
                        ");
                    }));
                }
            }
        }

        private string GetTabKeyFromBrowser(ChromiumWebBrowser browser)
        {
            foreach (var pair in browserTabs)
            {
                if (pair.Value == browser)
                {
                    return pair.Key;
                }
            }
            return string.Empty;
        }

        private string GetCompanyNameFromTabKey(string tabKey)
        {
            if (string.IsNullOrEmpty(tabKey) || !tabKey.StartsWith("tab_"))
            {
                return string.Empty;
            }
            return tabKey.Substring(4); // "tab_" kısmını çıkarıyoruz
        }

        private void ExecutePriceCollectionScript(ChromiumWebBrowser browser, string companyName)
        {
            var script = GetPriceScriptForCompany(companyName);
            if (!string.IsNullOrEmpty(script))
            {
                this.Invoke(new Action(() =>
                {
                    UpdateStatusMessage($"{companyName} için fiyat sorgulanıyor...");
                    // İlk olarak sayfa elementlerinin yüklenmesi için kısa bir bekleme
                    Task.Delay(1500).ContinueWith(_ =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            // JavaScripti çalıştır ve sonucu al
                            browser.EvaluateScriptAsync(script).ContinueWith(t =>
                            {
                                if (!t.IsFaulted && t.Result.Success && t.Result.Result != null)
                                {
                                    string price = t.Result.Result.ToString();
                                    UpdatePriceCollection(companyName, price, true);
                                }
                                else
                                {
                                    UpdatePriceCollection(companyName, "HATA: Fiyat alınamadı", false);
                                }
                            });
                        }));
                    });
                }));
            }
            else
            {
                UpdatePriceCollection(companyName, "HATA: Script bulunamadı", false);
            }
        }

        private string GetPriceScriptForCompany(string companyName)
        {
            string scriptFormat = "";
            string companyNameLower = companyName.ToLower();
            
            // Şirkete göre özel script oluştur
            if (companyNameLower.Contains("aksigorta"))
            {
                scriptFormat = @"
                    (function() {{
                        try {{
                            // Aksigorta sitesindeki fiyat elementini bul
                            var priceElement = document.querySelector('.policy-price') || 
                                    document.querySelector('.price-value') || 
                                    document.querySelector('.amount');
                            
                            if(priceElement) {{
                                return priceElement.innerText.trim();
                            }}
                            
                            // Form doldurma işlemleri
                            // Örnek: Araç Tipini seç
                            var vehicleTypeSelect = document.querySelector('select[id*=""VehicleType""]');
                            if (vehicleTypeSelect) {{
                                vehicleTypeSelect.value = '{0}';
                                vehicleTypeSelect.dispatchEvent(new Event('change'));
                            }}
                            
                            // Marka/Model
                            var makeModelInput = document.querySelector('input[id*=""MakeModel""]');
                            if (makeModelInput) {{
                                makeModelInput.value = '{1}';
                                makeModelInput.dispatchEvent(new Event('input'));
                            }}
                            
                            // Model Yılı
                            var yearSelect = document.querySelector('select[id*=""Year""]');
                            if (yearSelect) {{
                                yearSelect.value = '{2}';
                                yearSelect.dispatchEvent(new Event('change'));
                            }}
                            
                            // Butonları tıkla
                            var calculateButton = document.querySelector('button[id*=""Calculate""]') || 
                                                document.querySelector('button[id*=""Submit""]') ||
                                                document.querySelector('button.primary-button');
                            if (calculateButton) {{
                                calculateButton.click();
                                return 'HESAPLANIYOR...';
                            }}
                            
                            return 'Fiyat bulunamadı veya form alanları tespit edilemedi';
                        }} catch(e) {{
                            return 'HATA: ' + e.message;
                        }}
                    }})();
                ";
                
                // Şirketin API anahtarı, kullanıcı adı ve şifresini kontrol et
                var company = insuranceCompanies.FirstOrDefault(c => c.CompanyName == companyName);
                if (company != null)
                {
                    // Fiyat hesaplama parametrelerini al
                    var parameters = CalculationParams;
                    
                    // Script'i düzenle
                    return string.Format(scriptFormat, 
                        parameters.VehicleType,
                        parameters.MakeModel,
                        parameters.ModelYear.ToString());
                }
            }
            else if (companyNameLower.Contains("allianz"))
            {
                scriptFormat = @"
                    (function() {{
                        try {{
                            // Allianz sitesindeki fiyat elementini bul
                            var priceElement = document.querySelector('.premium-price') || 
                                    document.querySelector('.total-price') || 
                                    document.querySelector('.price-display');
                            
                            if(priceElement) {{
                                return priceElement.innerText.trim();
                            }}
                            
                            // Form doldurma işlemleri
                            // TC Kimlik ve Plaka bilgisini gir
                            var plateInput = document.querySelector('input[id*=""licensePlate""]');
                            if (plateInput) {{
                                plateInput.value = '{0}';
                                plateInput.dispatchEvent(new Event('input'));
                            }}
                            
                            var tcknInput = document.querySelector('input[id*=""identityNumber""]');
                            if (tcknInput) {{
                                tcknInput.value = '{1}';
                                tcknInput.dispatchEvent(new Event('input'));
                            }}
                            
                            // Devam butonuna tıkla
                            var continueButton = document.querySelector('button[id*=""continue""]');
                            if (continueButton) {{
                                continueButton.click();
                                return 'DEVAM EDİLİYOR...';
                            }}
                            
                            return 'Fiyat bulunamadı veya form alanları tespit edilemedi';
                        }} catch(e) {{
                            return 'HATA: ' + e.message;
                        }}
                    }})();
                ";
                
                // Parametre değerlerini al
                var company = insuranceCompanies.FirstOrDefault(c => c.CompanyName == companyName);
                if (company != null)
                {
                    // Fiyat hesaplama parametrelerini al
                    var parameters = CalculationParams;
                    
                    return string.Format(scriptFormat, 
                        parameters.LicensePlate, 
                        parameters.IdentityNumber);
                }
            }
            // Diğer şirketler için benzer scriptler eklenebilir
            
            return scriptFormat;
        }

        private void UpdatePriceCollection(string companyName, string price, bool success)
        {
            this.Invoke(new Action(() =>
            {
                // Fiyatı kaydet
                if (collectedPrices.ContainsKey(companyName))
                {
                    collectedPrices[companyName] = price;
                }
                else
                {
                    collectedPrices.Add(companyName, price);
                }

                // Tamamlanan sorgu sayısını artır
                completedPriceQueries++;

                // Durum mesajını güncelle
                UpdateStatusMessage($"Fiyat alındı: {companyName} - {price}");

                // Tüm sorgular tamamlandı mı kontrol et
                if (completedPriceQueries >= totalPriceQueries)
                {
                    FinalizePriceCollection();
                }
            }));
        }

        private void UpdateStatusMessage(string message)
        {
            // Ribbon'da veya status bar'da gösterebilirsiniz
            this.Text = $"Sigorta VIP - {message}";
        }

        private void FinalizePriceCollection()
        {
            isCollectingPrices = false;
            
            // Sonuçları göster
            var resultForm = new PriceResultForm(collectedPrices);
            resultForm.ShowDialog();
            
            // Temizlik
            collectedPrices.Clear();
            completedPriceQueries = 0;
            totalPriceQueries = 0;
            
            // Durum çubuğunu sıfırla
            this.Text = "Sigorta VIP";
        }

        private void StartPriceCollection(IEnumerable<InsuranceCompanyInfo> companies)
        {
            // Önceki işlem devam ediyorsa uyarı ver
            if (isCollectingPrices)
            {
                MessageBox.Show("Fiyat toplama işlemi zaten devam ediyor.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Fiyat hesaplama formunu göster
            using (var inputForm = new PriceInputForm())
            {
                if (inputForm.ShowDialog() != DialogResult.OK)
                {
                    return; // Kullanıcı iptal ettiyse çık
                }
                
                var calculationParams = inputForm.CalculationParams;
                CalculationParams = calculationParams; // Kopya oluştur
                
                // Fiyat toplama işlemini başlat
                isCollectingPrices = true;
                collectedPrices.Clear();
                completedPriceQueries = 0;
                
                var selectedCompanies = companies.ToList();
                totalPriceQueries = selectedCompanies.Count;
                
                if (totalPriceQueries == 0)
                {
                    MessageBox.Show("Seçili şirket bulunamadı.", "Uyarı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    isCollectingPrices = false;
                    return;
                }

                // İlerleme formu göster
                using (var progress = new ProgressBarForm())
                {
                    progress.Caption = "Fiyat Bilgileri Alınıyor";
                    progress.Description = $"Şirketlerden {calculationParams.VehicleType} {calculationParams.MakeModel} aracı için fiyat bilgileri alınıyor...";
                    progress.Show(this);
                    
                    // Asenkron olarak çalıştır
                    Task.Run(() =>
                    {
                        int total = selectedCompanies.Count;
                        int current = 0;
                        
                        // Şirketleri maksimum 3'er 3'er gruplar halinde aç (performans için)
                        var companyGroups = selectedCompanies
                            .Select((company, index) => new { Company = company, Index = index })
                            .GroupBy(x => x.Index / 3)
                            .Select(g => g.Select(x => x.Company).ToList())
                            .ToList();
                        
                        foreach (var group in companyGroups)
                        {
                            // Her gruptaki şirketleri aç
                            foreach (var company in group)
                            {
                                current++;
                                
                                // UI thread'de işlem yap
                                this.Invoke(new Action(() =>
                                {
                                    // İlerleme durumunu güncelle
                                    progress.Description = $"({current}/{total}) {company.CompanyName} için {calculationParams.InsuranceType} fiyatı alınıyor...";
                                    
                                    // Şirketi aç ve script çalıştır
                                    OpenCompanyTab(company);
                                }));
                                
                                // Kısa bir bekleme
                                Thread.Sleep(300);
                            }
                            
                            // Grubun işlemi bitene kadar bekle (maksimum 10 saniye)
                            for (int i = 0; i < 20; i++)
                            {
                                if (completedPriceQueries >= current)
                                    break;
                                
                                Thread.Sleep(500);
                            }
                        }
                        
                        // Tüm işlemler bittiğinde
                        this.Invoke(new Action(() =>
                        {
                            if (progress != null && !progress.IsDisposed)
                            {
                                progress.Close();
                            }
                        }));
                    });
                }
            }
        }

        private void Browser_LoadError(object sender, LoadErrorEventArgs e)
        {
            if (e.ErrorCode != CefErrorCode.None && e.Frame.IsMain)
            {
                var browser = (ChromiumWebBrowser)sender;
                var errorUrl = e.FailedUrl;
                
                // HTTPS hatası olup olmadığını kontrol et
                if (errorUrl.StartsWith("https://") && 
                    (e.ErrorCode == CefErrorCode.ConnectionFailed || 
                     e.ErrorCode == CefErrorCode.CertificateTransparencyRequired || 
                     e.ErrorCode == CefErrorCode.InsecureResponse ||
                     (int)e.ErrorCode < -200))
                {
                    string newUrl = errorUrl.Replace("https://", "http://");
                    
                    // URL'yi değiştirip tekrar yükle
                    this.Invoke(new Action(() => {
                        browser.Load(newUrl);
                    }));
                    return;
                }

                // Genel hata sayfası
                this.Invoke(new Action(() => {
                    var htmlContent = $@"
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <style>
                                body {{ font-family: Arial, sans-serif; margin: 50px; text-align: center; }}
                                h2 {{ color: #666; }}
                                p {{ color: #888; }}
                                .container {{ max-width: 600px; margin: 0 auto; }}
                                .error-code {{ background: #f8f8f8; padding: 10px; border-radius: 4px; }}
                                button {{ padding: 10px 20px; background: #0078d7; color: white; 
                                        border: none; border-radius: 4px; cursor: pointer; }}
                                button:hover {{ background: #0063b1; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Sayfa Yüklenemedi</h2>
                                <p>URL: <strong>{e.FailedUrl}</strong></p>
                                <p>Hata: <span class='error-code'>{(int)e.ErrorCode} ({e.ErrorCode})</span></p>
                                <p>{e.ErrorText}</p>
                                <p>Bu sorun genellikle geçici bağlantı hatalarından kaynaklanır.</p>
                                <button onclick='window.location.reload();'>Tekrar Dene</button>
                                <button onclick=""window.location.href = '{e.FailedUrl.Replace("https://", "http://")}'"">HTTP Bağlantı Dene</button>
                            </div>
                        </body>
                        </html>";

                    browser.LoadHtml(htmlContent, e.FailedUrl);
                }));
            }
        }

        private void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            // Geliştirme amaçlı konsol çıktısı
            System.Diagnostics.Debug.WriteLine($"CEF Console [{e.Level}]: {e.Message} ({e.Source}:{e.Line})");
        }

        private void TabControl_CloseButtonClick(object sender, EventArgs e)
        {
            // Sekme kapatma bayrağını aktifleştir
            isClosingTab = true;
            
            try
            {
                // Hangi sekmenin kapatılmak istendiğini tespit edelim
                // Sender muhtemelen XtraTabControl'dür
                XtraTabPage pageToClose = null;
                
                try
                {
                    // Gelen olay argümanının tipini kontrol edelim
                    var control = sender as DevExpress.XtraTab.XtraTabControl;
                    
                    if (control != null)
                    {
                        // Aktif sekmeyi hedef alalım
                        pageToClose = control.SelectedTabPage;
                    }
                    else
                    {
                        // Eğer sender XtraTabControl değilse, argümanın özelliklerini kontrol et
                        var properties = e.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            if (prop.Name == "Page" && prop.PropertyType == typeof(XtraTabPage))
                            {
                                pageToClose = prop.GetValue(e) as XtraTabPage;
                                break;
                            }
                        }

                        // Eğer prop bulunamadıysa, aktif sekmeyi kullan
                        if (pageToClose == null)
                        {
                            pageToClose = tabControl.SelectedTabPage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Herhangi bir hata durumunda şu anki aktif sekmeyi kullan
                    System.Diagnostics.Debug.WriteLine($"Sekme tespitinde hata: {ex.Message}");
                    pageToClose = tabControl.SelectedTabPage;
                }
                
                // Hata ayıklama
                System.Diagnostics.Debug.WriteLine($"Kapatılmaya çalışılan sekme: {pageToClose?.Text}");
                
                if (pageToClose != null && pageToClose != tabMainPage) // Ana sayfa sekmesi kapatılamaz
                {
                    var tabKey = pageToClose.Tag?.ToString();
                    if (!string.IsNullOrEmpty(tabKey) && browserTabs.ContainsKey(tabKey))
                    {
                        try
                        {
                            // Önce browser'ı remove et, sonra dispose et
                            var browser = browserTabs[tabKey];
                            browserTabs.Remove(tabKey);
                            
                            // Browser'ı dispose et
                            if (browser != null && !browser.IsDisposed)
                            {
                                browser.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Hata mesajını logla ama uygulamanın çökmesine izin verme
                            System.Diagnostics.Debug.WriteLine($"Browser kapatılırken hata: {ex.Message}");
                        }
                    }
                    
                    try
                    {
                        // Sekmeyi seçme durumunu önce ana sayfaya geçirerek değiştir
                        tabControl.SelectedTabPage = tabMainPage;
                        
                        // Ardından kaldır
                        tabControl.TabPages.Remove(pageToClose);
                        pageToClose.Dispose();
                    }
                    catch (Exception ex)
                    {
                        // Hata mesajını logla ama uygulamanın çökmesine izin verme
                        System.Diagnostics.Debug.WriteLine($"Sekme kaldırılırken hata: {ex.Message}");
                    }
                }
                else if (pageToClose == tabMainPage)
                {
                    // Ana sayfa sekmesi kapatılmaya çalışılırsa hiçbir şey yapma
                    try
                    {
                        // Eğer Cancel özelliği varsa, onu true yap
                        var cancelProp = e.GetType().GetProperty("Cancel");
                        if (cancelProp != null && cancelProp.PropertyType == typeof(bool))
                        {
                            cancelProp.SetValue(e, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Reflection başarısız olursa, sessizce devam et
                        System.Diagnostics.Debug.WriteLine($"Cancel özelliği ayarlanırken hata: {ex.Message}");
                    }
                }
            }
            finally
            {
                // Sekme kapatma bayrağını sıfırla
                isClosingTab = false;
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var tabPage = tabControl.SelectedTabPage;
            if (tabPage != null && tabPage != tabMainPage)
            {
                var browser = tabPage.Controls[0] as ChromiumWebBrowser;
                if (browser != null)
                {
                    // Önce sayfayı boşalt, sonra yeniden yükle
                    browser.GetBrowser().Reload(true);
                }
            }
        }

        private void btnHome_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            tabControl.SelectedTabPage = tabMainPage;
        }

        private void BtnOpenAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kullanıcıya onay sor
            var result = MessageBox.Show(
                $"Tüm sigorta şirketleri ({insuranceCompanies.Count} adet) açılacak. Devam etmek istiyor musunuz?",
                "Tüm Şirketleri Aç",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                // İlerleme çubuğu oluştur
                using (var progress = new ProgressBarForm())
                {
                    progress.Caption = "Sigorta Firmaları Açılıyor";
                    progress.Description = "Lütfen bekleyin...";
                    progress.Show(this);
                    
                    // Tüm şirketleri açma işlemi için task oluştur
                    Task.Run(() =>
                    {
                        int total = insuranceCompanies.Count;
                        int current = 0;
                        
                        foreach (var company in insuranceCompanies)
                        {
                            current++;
                            
                            // UI thread'de işlem yap
                            this.Invoke(new Action(() =>
                            {
                                // İlerleme durumunu güncelle
                                progress.Description = $"({current}/{total}) {company.CompanyName} açılıyor...";
                                
                                // Şirketi aç
                                OpenCompanyTab(company);
                                
                                // Son şirketi açtıysak ilerleme çubuğunu kapat
                                if (current == total)
                                {
                                    progress.Close();
                                }
                                
                                // Kısa bir bekletme ekle, tarayıcıların üst üste açılmasını önle
                                Thread.Sleep(500);
                            }));
                        }
                    });
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Normal kapanmayı işleyen MainForm_FormClosing metodumuz var
            // Burada sadece kaynak temizliği yapalım
            try
            {
                // Tüm tarayıcıları temizle
                foreach (var browser in browserTabs.Values)
                {
                    if (browser != null && !browser.IsDisposed)
                    {
                        try
                        {
                            browser.Dispose();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Browser dispose edilirken hata: {ex.Message}");
                        }
                    }
                }
                
                // Dictionary'i temizle
                browserTabs.Clear();
                
                try
        {
            if (Cef.IsInitialized == true)
            {
                Cef.Shutdown();
            }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CEF kapatılırken hata: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form kapatılırken genel hata: {ex.Message}");
            }
            
            base.OnFormClosing(e);
        }

        private void tabMainPage_Paint(object sender, PaintEventArgs e)
        {

        }

        // Bu değişken alanını ekleyelim
        private PriceCalculationParams CalculationParams = new PriceCalculationParams();
    }

    // Basit ilerleme formu
    public class ProgressBarForm : Form
    {
        private Label lblCaption;
        private Label lblDescription;
        private System.Windows.Forms.ProgressBar progressBar;

        public string Caption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = value; }
        }

        public string Description
        {
            get { return lblDescription.Text; }
            set { lblDescription.Text = value; }
        }

        public ProgressBarForm()
        {
            this.Size = new Size(400, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "İşlem";

            lblCaption = new Label();
            lblCaption.AutoSize = false;
            lblCaption.TextAlign = ContentAlignment.MiddleCenter;
            lblCaption.Dock = DockStyle.Top;
            lblCaption.Height = 30;
            lblCaption.Font = new Font(lblCaption.Font.FontFamily, 12, FontStyle.Bold);
            this.Controls.Add(lblCaption);

            lblDescription = new Label();
            lblDescription.AutoSize = false;
            lblDescription.TextAlign = ContentAlignment.MiddleCenter;
            lblDescription.Dock = DockStyle.Top;
            lblDescription.Height = 30;
            lblDescription.Top = lblCaption.Bottom;
            this.Controls.Add(lblDescription);

            progressBar = new System.Windows.Forms.ProgressBar();
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Dock = DockStyle.Top;
            progressBar.Height = 30;
            progressBar.Top = lblDescription.Bottom + 10;
            progressBar.MarqueeAnimationSpeed = 30;
            this.Controls.Add(progressBar);
        }
    }

    // Şirket arama formu
    public class CompanySearchForm : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraEditors.SearchControl txtSearch;
        private DevExpress.XtraGrid.GridControl gridCompanies;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private List<InsuranceCompanyInfo> allCompanies;
        private List<InsuranceCompanyInfo> filteredCompanies;

        public InsuranceCompanyInfo SelectedCompany { get; private set; }

        public CompanySearchForm(List<InsuranceCompanyInfo> companies)
        {
            this.allCompanies = companies;
            this.filteredCompanies = new List<InsuranceCompanyInfo>(companies);
            InitializeComponents();
            PopulateCompanyList();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(600, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Şirket Ara (CTRL+K)";
            this.KeyPreview = true;
            this.KeyDown += (s, e) => 
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter && gridView.SelectedRowsCount > 0)
                {
                    SelectCompanyAndClose();
                }
            };

            // Panel oluştur
            var panelSearch = new DevExpress.XtraEditors.PanelControl();
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Height = 50;
            panelSearch.Padding = new Padding(10, 10, 10, 10);
            this.Controls.Add(panelSearch);

            // Arama kontrolü
            txtSearch = new DevExpress.XtraEditors.SearchControl();
            txtSearch.Dock = DockStyle.Fill;
            txtSearch.Properties.NullValuePrompt = "Şirket adı yazın...";
            txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            txtSearch.Properties.ShowSearchButton = true;
            txtSearch.Properties.ShowClearButton = true;
            txtSearch.Properties.Buttons[0].Caption = "Ara";
            txtSearch.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
            txtSearch.Properties.Client = gridCompanies;
            txtSearch.Properties.EditValueChanging += (s, e) => FilterCompanies(e.NewValue?.ToString());
            panelSearch.Controls.Add(txtSearch);

            // Grid kontrolü
            gridCompanies = new DevExpress.XtraGrid.GridControl();
            gridCompanies.Dock = DockStyle.Fill;
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridCompanies.MainView = gridView;
            
            // Grid görünümünü yapılandır
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsCustomization.AllowColumnMoving = false;
            gridView.OptionsCustomization.AllowFilter = false;
            gridView.OptionsCustomization.AllowGroup = false;
            gridView.OptionsCustomization.AllowSort = true;
            gridView.OptionsDetail.EnableMasterViewMode = false;
            gridView.OptionsFind.AlwaysVisible = false;
            gridView.OptionsMenu.EnableColumnMenu = false;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsView.ShowIndicator = false;
            gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView.DoubleClick += (s, e) => SelectCompanyAndClose();
            
            // Sütunları ekle
            var colName = gridView.Columns.Add();
            colName.Caption = "Şirket Adı";
            colName.FieldName = "CompanyName";
            colName.Visible = true;
            colName.Width = 300;
            
            var colLastAccess = gridView.Columns.Add();
            colLastAccess.Caption = "Son Erişim";
            colLastAccess.FieldName = "LastAccessTime";
            colLastAccess.Visible = true;
            colLastAccess.Width = 180;
            colLastAccess.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            colLastAccess.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            
            var colDescription = gridView.Columns.Add();
            colDescription.Caption = "Açıklama";
            colDescription.FieldName = "Description";
            colDescription.Visible = true;
            colDescription.Width = 300;
            
            this.Controls.Add(gridCompanies);

            // Butonlar için panel
            var buttonPanel = new DevExpress.XtraEditors.PanelControl();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);
            this.Controls.Add(buttonPanel);

            // İptal butonu
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnCancel.Text = "İptal";
            btnCancel.Width = 100;
            btnCancel.Height = 35;
            btnCancel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            btnCancel.Location = new Point(buttonPanel.Width - 230, 12);
            btnCancel.Click += (s, e) => 
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            buttonPanel.Controls.Add(btnCancel);

            // Tamam butonu
            btnOK = new DevExpress.XtraEditors.SimpleButton();
            btnOK.Text = "Seç";
            btnOK.Width = 100;
            btnOK.Height = 35;
            btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            btnOK.Appearance.ForeColor = System.Drawing.Color.White;
            btnOK.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            btnOK.Location = new Point(buttonPanel.Width - 120, 12);
            btnOK.Click += (s, e) => SelectCompanyAndClose();
            buttonPanel.Controls.Add(btnOK);

            // Kontrollerin sıralaması
            this.Controls.SetChildIndex(buttonPanel, 0);
            this.Controls.SetChildIndex(gridCompanies, 1);
            this.Controls.SetChildIndex(panelSearch, 2);

            // Başlangıçta arama kutusuna odaklan
            this.Load += (s, e) => txtSearch.Focus();
        }

        private void FilterCompanies(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredCompanies = new List<InsuranceCompanyInfo>(allCompanies);
            }
            else
            {
                searchText = searchText.ToLower();
                filteredCompanies = allCompanies
                    .Where(c => c.CompanyName.ToLower().Contains(searchText) || 
                               (c.Description != null && c.Description.ToLower().Contains(searchText)))
                    .ToList();
            }

            PopulateCompanyList();
        }

        private void PopulateCompanyList()
        {
            // Veri kaynağı olarak filtrelenmiş şirketleri kullan
            gridCompanies.DataSource = null;
            gridCompanies.DataSource = filteredCompanies;

            if (gridView.RowCount > 0)
            {
                gridView.FocusedRowHandle = 0;
            }
        }

        private void SelectCompanyAndClose()
        {
            if (gridView.SelectedRowsCount > 0)
            {
                int rowHandle = gridView.GetSelectedRows()[0];
                if (rowHandle >= 0)
                {
                    SelectedCompany = gridView.GetRow(rowHandle) as InsuranceCompanyInfo;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }

    // Fiyat sonuçlarını göstermek için yeni bir form
    public class PriceResultForm : DevExpress.XtraEditors.XtraForm
    {
        private Dictionary<string, string> prices;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        
        public PriceResultForm(Dictionary<string, string> prices)
        {
            this.prices = new Dictionary<string, string>(prices);
            InitializeComponents();
            LoadPriceData();
        }
        
        private void InitializeComponents()
        {
            this.Text = "Fiyat Karşılaştırma Sonuçları";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            
            // Grid kontrolü
            gridControl = new DevExpress.XtraGrid.GridControl();
            gridControl.Dock = DockStyle.Fill;
            
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridControl.MainView = gridView;
            
            // Grid görünümünü yapılandır
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsCustomization.AllowFilter = false;
            gridView.OptionsCustomization.AllowGroup = false;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.OptionsView.ShowIndicator = false;
            
            // Sütunları ekle
            var colCompany = gridView.Columns.Add();
            colCompany.Caption = "Şirket";
            colCompany.FieldName = "CompanyName";
            colCompany.Visible = true;
            colCompany.Width = 200;
            
            var colPrice = gridView.Columns.Add();
            colPrice.Caption = "Fiyat";
            colPrice.FieldName = "Price";
            colPrice.Visible = true;
            colPrice.Width = 150;
            
            var colStatus = gridView.Columns.Add();
            colStatus.Caption = "Durum";
            colStatus.FieldName = "Status";
            colStatus.Visible = true;
            colStatus.Width = 200;
            
            this.Controls.Add(gridControl);
            
            // Kapat butonu
            var closeButton = new DevExpress.XtraEditors.SimpleButton();
            closeButton.Text = "Kapat";
            closeButton.Width = 100;
            closeButton.Height = 30;
            closeButton.Dock = DockStyle.Bottom;
            closeButton.Click += (s, e) => this.Close();
            
            this.Controls.Add(closeButton);
            this.Controls.SetChildIndex(closeButton, 0);
        }
        
        private void LoadPriceData()
        {
            var priceList = new List<PriceInfo>();
            
            foreach (var item in prices)
            {
                string status = "Başarılı";
                if (item.Value.StartsWith("HATA:"))
                {
                    status = item.Value;
                }
                
                priceList.Add(new PriceInfo
                {
                    CompanyName = item.Key,
                    Price = status == "Başarılı" ? item.Value : "",
                    Status = status
                });
            }
            
            gridControl.DataSource = priceList;
        }
        
        private class PriceInfo
        {
            public string CompanyName { get; set; }
            public string Price { get; set; }
            public string Status { get; set; }
        }
    }
}
