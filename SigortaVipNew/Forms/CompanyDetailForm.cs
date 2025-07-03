using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System;
using System.Windows.Forms;
using SigortaVip.Models;

namespace SigortaVipNew
{
    public partial class CompanyDetailForm : XtraForm
    {
        private InsuranceCompanyInfo _company;

        public CompanyDetailForm(InsuranceCompanyInfo company)
        {
            InitializeComponent();
            _company = company;
            LoadCompanyData();
        }

        private void InitializeComponent()
        {
            this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.txtCompanyName = new DevExpress.XtraEditors.TextEdit();
            this.txtCompanyUrl = new DevExpress.XtraEditors.TextEdit();
            this.memoDescription = new DevExpress.XtraEditors.MemoEdit();
            this.chkFavorite = new DevExpress.XtraEditors.CheckEdit();
            this.dtLastAccess = new DevExpress.XtraEditors.DateEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFavorite.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastAccess.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastAccess.Properties.CalendarTimeProperties)).BeginInit();
            this.SuspendLayout();
            
            // layoutControl
            this.layoutControl.Controls.Add(this.txtCompanyName);
            this.layoutControl.Controls.Add(this.txtCompanyUrl);
            this.layoutControl.Controls.Add(this.memoDescription);
            this.layoutControl.Controls.Add(this.chkFavorite);
            this.layoutControl.Controls.Add(this.dtLastAccess);
            this.layoutControl.Controls.Add(this.btnSave);
            this.layoutControl.Controls.Add(this.btnCancel);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(500, 300);
            this.layoutControl.TabIndex = 0;
            this.layoutControl.Text = "layoutControl";
            
            // Root
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(500, 300);
            this.Root.TextVisible = false;
            
            // txtCompanyName
            this.txtCompanyName.Location = new System.Drawing.Point(105, 12);
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(383, 20);
            this.txtCompanyName.StyleController = this.layoutControl;
            this.txtCompanyName.TabIndex = 4;
            
            // layoutControlItem1
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1.Control = this.txtCompanyName;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(480, 24);
            this.layoutControlItem1.Text = "Şirket Adı:";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(90, 13);
            
            // txtCompanyUrl
            this.txtCompanyUrl.Location = new System.Drawing.Point(105, 36);
            this.txtCompanyUrl.Name = "txtCompanyUrl";
            this.txtCompanyUrl.Size = new System.Drawing.Size(383, 20);
            this.txtCompanyUrl.StyleController = this.layoutControl;
            this.txtCompanyUrl.TabIndex = 5;
            
            // layoutControlItem2
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2.Control = this.txtCompanyUrl;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(480, 24);
            this.layoutControlItem2.Text = "Web Adresi:";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(90, 13);
            
            // memoDescription
            this.memoDescription.Location = new System.Drawing.Point(105, 60);
            this.memoDescription.Name = "memoDescription";
            this.memoDescription.Size = new System.Drawing.Size(383, 96);
            this.memoDescription.StyleController = this.layoutControl;
            this.memoDescription.TabIndex = 6;
            
            // layoutControlItem3
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3.Control = this.memoDescription;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(480, 100);
            this.layoutControlItem3.Text = "Açıklama:";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(90, 13);
            
            // chkFavorite
            this.chkFavorite.Location = new System.Drawing.Point(105, 160);
            this.chkFavorite.Name = "chkFavorite";
            this.chkFavorite.Properties.Caption = "";
            this.chkFavorite.Size = new System.Drawing.Size(383, 20);
            this.chkFavorite.StyleController = this.layoutControl;
            this.chkFavorite.TabIndex = 7;
            
            // layoutControlItem4
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4.Control = this.chkFavorite;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 148);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(480, 24);
            this.layoutControlItem4.Text = "Favori:";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(90, 13);
            
            // dtLastAccess
            this.dtLastAccess.EditValue = null;
            this.dtLastAccess.Location = new System.Drawing.Point(105, 184);
            this.dtLastAccess.Name = "dtLastAccess";
            this.dtLastAccess.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLastAccess.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLastAccess.Size = new System.Drawing.Size(383, 20);
            this.dtLastAccess.StyleController = this.layoutControl;
            this.dtLastAccess.TabIndex = 8;
            
            // layoutControlItem5
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5.Control = this.dtLastAccess;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 172);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(480, 24);
            this.layoutControlItem5.Text = "Son Erişim:";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(90, 13);
            
            // btnSave
            this.btnSave.Location = new System.Drawing.Point(297, 266);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 22);
            this.btnSave.StyleController = this.layoutControl;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Kaydet";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            
            // layoutControlItem6
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6.Control = this.btnSave;
            this.layoutControlItem6.Location = new System.Drawing.Point(285, 254);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(90, 26);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(90, 26);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(90, 26);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            
            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(387, 266);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 22);
            this.btnCancel.StyleController = this.layoutControl;
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "İptal";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // layoutControlItem7
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7.Control = this.btnCancel;
            this.layoutControlItem7.Location = new System.Drawing.Point(375, 254);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(105, 26);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(105, 26);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            
            // emptySpaceItem1
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 196);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(480, 58);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            
            // CompanyDetailForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.layoutControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompanyDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Şirket Detayları";
            
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCompanyUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFavorite.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastAccess.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastAccess.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
        }

        private void LoadCompanyData()
        {
            txtCompanyName.Text = _company.CompanyName;
            txtCompanyUrl.Text = _company.CompanyUrl;
            memoDescription.Text = _company.Description;
            chkFavorite.Checked = _company.IsFavorite;
            dtLastAccess.DateTime = _company.LastAccessTime;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Update company data
            _company.CompanyName = txtCompanyName.Text;
            _company.CompanyUrl = txtCompanyUrl.Text;
            _company.Description = memoDescription.Text;
            _company.IsFavorite = chkFavorite.Checked;
            _company.LastAccessTime = dtLastAccess.DateTime;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txtCompanyName;
        private DevExpress.XtraEditors.TextEdit txtCompanyUrl;
        private DevExpress.XtraEditors.MemoEdit memoDescription;
        private DevExpress.XtraEditors.CheckEdit chkFavorite;
        private DevExpress.XtraEditors.DateEdit dtLastAccess;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
} 