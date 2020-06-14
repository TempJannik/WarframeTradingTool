using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warframe.Market.Layout
{
    public partial class ListView : Form
    {
        public ListView()
        {
            InitializeComponent();
            int num = 0;
            var list = MainFrame.cfg.itemList.OrderByDescending(x=> x.moving_avg);
            foreach(var ingameitem in list)
            {
                num++;
                ListViewItem item = new ListViewItem(num.ToString());
                item.SubItems.Add(ingameitem.name);
                item.SubItems.Add(ingameitem.moving_avg.ToString());
                ItemListChart.Items.Add(item);
            }
            ItemListChart.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            ItemListChart.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
