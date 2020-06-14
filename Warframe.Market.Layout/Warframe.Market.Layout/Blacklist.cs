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
    public partial class Blacklist : Form
    {
        public Blacklist()
        {
            InitializeComponent();
            relicBtn.Checked = MainFrame.cfg.relicsExcluded;
            ModsBtn.Checked = MainFrame.cfg.modsExcluded;
            SetsBtn.Checked = MainFrame.cfg.setsExcluded;
        }

        private void BunifuCustomLabel3_Click(object sender, EventArgs e)
        {

        }

        private void BunifuSeparator2_Load(object sender, EventArgs e)
        {

        }

        private void BunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RelicBtn_OnChange(object sender, EventArgs e)
        {
            MainFrame.cfg.relicsExcluded = !MainFrame.cfg.relicsExcluded;
            MainFrame.cfg.saveCfg();
        }

        private void ModsBtn_OnChange(object sender, EventArgs e)
        {
            MainFrame.cfg.modsExcluded = !MainFrame.cfg.modsExcluded;
            MainFrame.cfg.saveCfg();
        }

        private void SetsBtn_OnChange(object sender, EventArgs e)
        {
            MainFrame.cfg.setsExcluded = !MainFrame.cfg.setsExcluded;
            MainFrame.cfg.saveCfg();
        }
    }
}
