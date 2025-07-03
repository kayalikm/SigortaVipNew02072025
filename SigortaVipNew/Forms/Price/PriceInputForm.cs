using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using SigortaVipNew.Models;

namespace SigortaVipNew.Forms.Price
{
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
} 