using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SigortaVip.Models;

namespace SigortaVipNew.Forms.Company
{
    public class CompanyDetailForm : DevExpress.XtraEditors.XtraForm
    {
        private DevExpress.XtraEditors.TextEdit txtCompanyName;
        private DevExpress.XtraEditors.TextEdit txtCompanyUrl;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraEditors.CheckEdit chkIsFavorite;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        
        private InsuranceCompanyInfo company;
        
        public CompanyDetailForm(InsuranceCompanyInfo company)
        {
            this.company = company;
            InitializeComponents();
            LoadCompanyData();
        }
        
        private void InitializeComponents()
        {
            this.Text = "Şirket Bilgileri";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(550, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Ana panel
            var mainPanel = new DevExpress.XtraEditors.PanelControl();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(10);
            this.Controls.Add(mainPanel);
            
            // Form düzeni
            var layoutControl = new DevExpress.XtraLayout.LayoutControl();
            layoutControl.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(layoutControl);
            
            var layoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControl.Root = layoutGroup;
            
            // Şirket adı
            txtCompanyName = new DevExpress.XtraEditors.TextEdit();
            layoutControl.Controls.Add(txtCompanyName);
            var nameItem = new DevExpress.XtraLayout.LayoutControlItem();
            nameItem.Control = txtCompanyName;
            nameItem.Text = "Şirket Adı:";
            layoutGroup.AddItem(nameItem);
            
            // Şirket URL
            txtCompanyUrl = new DevExpress.XtraEditors.TextEdit();
            layoutControl.Controls.Add(txtCompanyUrl);
            var urlItem = new DevExpress.XtraLayout.LayoutControlItem();
            urlItem.Control = txtCompanyUrl;
            urlItem.Text = "Web Adresi:";
            layoutGroup.AddItem(urlItem);
            
            // Açıklama
            txtDescription = new DevExpress.XtraEditors.MemoEdit();
            txtDescription.Properties.ScrollBars = ScrollBars.Both;
            txtDescription.Height = 100;
            layoutControl.Controls.Add(txtDescription);
            var descItem = new DevExpress.XtraLayout.LayoutControlItem();
            descItem.Control = txtDescription;
            descItem.Text = "Açıklama:";
            descItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            descItem.MinSize = new Size(0, 100);
            descItem.MaxSize = new Size(0, 100);
            layoutGroup.AddItem(descItem);
            
            // Favori mi?
            chkIsFavorite = new DevExpress.XtraEditors.CheckEdit();
            chkIsFavorite.Text = "Favori Şirket";
            layoutControl.Controls.Add(chkIsFavorite);
            var favoriteItem = new DevExpress.XtraLayout.LayoutControlItem();
            favoriteItem.Control = chkIsFavorite;
            favoriteItem.Text = "";
            layoutGroup.AddItem(favoriteItem);
            
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
            
            // Kaydet butonu
            btnSave = new DevExpress.XtraEditors.SimpleButton();
            btnSave.Text = "Kaydet";
            btnSave.Width = 100;
            btnSave.Height = 35;
            btnSave.Location = new Point(buttonPanel.Width - 120, 15);
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 114, 206);
            btnSave.Appearance.ForeColor = System.Drawing.Color.White;
            btnSave.Click += BtnSave_Click;
            buttonPanel.Controls.Add(btnSave);
            
            // Kontrollerin sıralaması
            mainPanel.Controls.SetChildIndex(buttonPanel, 0);
        }
        
        private void LoadCompanyData()
        {
            txtCompanyName.Text = company.CompanyName;
            txtCompanyUrl.Text = company.CompanyUrl;
            txtDescription.Text = company.Description;
            chkIsFavorite.Checked = company.IsFavorite;
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Lütfen şirket adını girin.", "Eksik Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyName.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtCompanyUrl.Text))
            {
                MessageBox.Show("Lütfen web adresini girin.", "Eksik Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCompanyUrl.Focus();
                return;
            }
            
            // Bilgileri kaydet
            company.CompanyName = txtCompanyName.Text;
            company.CompanyUrl = txtCompanyUrl.Text;
            company.Description = txtDescription.Text;
            company.IsFavorite = chkIsFavorite.Checked;
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 