using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Warframe.Market.Layout
{
    public class Config
    {
        public enum calc : int
        {
            MinMax = 0,
            avg = 1,
            wa = 2,
            median = 3,
            moving_avg = 4
        }

        public string username { get; set; }
        public DateTime lastupdate { get; set; }
        public int minMA { get; set; }
        public int minVolume { get; set; }
        public int minDevAlert { get; set; }
        public double minMAAlert { get; set; }
        public List<ListItem> itemList { get; set; }
        public int hotdeals { get; set; }
        public int buyorderdev { get; set; }
        public int setdeviation { get; set; }
        public bool setsExcluded { get; set; }
        public bool modsExcluded { get; set; }
        public bool relicsExcluded { get; set; }
        public bool hdsetsExcluded { get; set; }
        public calc calculationType { get; set; }

        public void saveCfg()
        {
            string jsonString = new JavaScriptSerializer().Serialize(this);
            File.WriteAllText("config.cfg", jsonString);
        }

        public void loadCfg()
        {
            try
            {
                if (!File.Exists("config.cfg"))
                {
                    this.username = "Your Username";
                    this.minMAAlert = 10;
                    this.minDevAlert = 70;
                    this.lastupdate = DateTime.Today;
                    this.itemList = new List<ListItem>();
                    this.hotdeals = 5;
                    this.minVolume = 0;
                    this.minMA = 10;
                    this.calculationType = calc.MinMax;
                    this.relicsExcluded = true;
                    this.modsExcluded = true;
                    this.setsExcluded = true;
                    this.buyorderdev = 5;
                    this.hdsetsExcluded = true;
                    File.WriteAllText("config.cfg", new JavaScriptSerializer().Serialize(this));
                }
                Config cfg = new JavaScriptSerializer().Deserialize<Config>(File.ReadAllText("config.cfg"));
                this.username = cfg.username;
                this.minMAAlert = cfg.minMAAlert;
                this.minDevAlert = cfg.minDevAlert;
                this.lastupdate = cfg.lastupdate;
                this.itemList = cfg.itemList;
                this.hotdeals = cfg.hotdeals;
                this.minMA = cfg.minMA;
                this.minVolume = cfg.minVolume;
                this.calculationType = cfg.calculationType;
                this.relicsExcluded = cfg.relicsExcluded;
                this.modsExcluded = cfg.modsExcluded;
                this.setsExcluded = cfg.setsExcluded;
                this.buyorderdev = cfg.buyorderdev;
                this.hdsetsExcluded = cfg.hdsetsExcluded;
            }
            catch
            {
                MessageBox.Show("Fatal error while initializing config.");
                Environment.Exit(1);
            }
        }
    }
}