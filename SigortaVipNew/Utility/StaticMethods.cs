using System.Windows.Forms;

namespace SigortaVip.Utility
{
    public static class StaticMethods
    {
        public static void setDefault(ComboBox[] comboBox)
        {
            int temp = 0;
            int maxWidth = 0;
            foreach (ComboBox cbx in comboBox)
            {
                if (cbx.Items.Count > 0)
                {
                    cbx.SelectedIndex = 0;

                    foreach (var item in cbx.Items)
                    {
                        temp = TextRenderer.MeasureText(item.ToString(), cbx.Font).Width;
                        if (temp > maxWidth)
                            maxWidth = temp;
                    }
                    cbx.DropDownWidth = maxWidth;
                }
            }
        }
    }
}
