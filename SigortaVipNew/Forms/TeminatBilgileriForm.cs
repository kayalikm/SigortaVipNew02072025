using Sigorta_Re;
using System;
using System.Windows.Forms;

namespace SigortaVip.Forms
{
    public partial class TeminatBilgileriForm : Form
    {
        teminatBilgileri _teminatBilgileri;
        public TeminatBilgileriForm()
        {
            InitializeComponent();
            _teminatBilgileri = new teminatBilgileri();
        }

        private void TeminatBilgileriForm_Load(object sender, EventArgs e)
        {
            pnlMain.Controls.Add(_teminatBilgileri);
        }

        private void TeminatBilgileriForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
