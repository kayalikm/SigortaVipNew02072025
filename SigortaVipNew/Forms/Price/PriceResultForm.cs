using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace SigortaVipNew.Forms.Price
{
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