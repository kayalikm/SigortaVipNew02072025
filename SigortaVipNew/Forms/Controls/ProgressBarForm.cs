using System;
using System.Drawing;
using System.Windows.Forms;

namespace SigortaVipNew.Forms.Controls
{
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
} 