using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SigortaVip.Models;

namespace SigortaVipNew.Forms.Company
{
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
            btnCancel.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
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
            btnOK.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
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
} 