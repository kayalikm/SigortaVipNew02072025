using System;
using System.Windows.Forms;

namespace SigortaVip.Forms.LowResolutionForms.TeminatBilgileriForm
{
    public partial class lowTeminatBilgileriForm : Form
    {
        lowTeminatBilgileri _lowTeminatBilgileri;
        public lowTeminatBilgileriForm()
        {
            InitializeComponent();
            _lowTeminatBilgileri = new lowTeminatBilgileri();
        }

        private void lowTeminatBilgileriForm_Load(object sender, EventArgs e)
        {
            pnlMain.Controls.Add(_lowTeminatBilgileri);
        }

        private void lowTeminatBilgileriForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
