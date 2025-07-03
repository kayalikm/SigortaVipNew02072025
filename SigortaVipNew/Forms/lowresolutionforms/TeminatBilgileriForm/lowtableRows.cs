using System.Windows.Forms;

namespace SigortaVip.Forms.LowResolutionForms.TeminatBilgileriForm
{
    public partial class lowtableRows : UserControl
    {
        public lowtableRows()
        {
            InitializeComponent();
            tblHeaderRow.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }
    }
}
