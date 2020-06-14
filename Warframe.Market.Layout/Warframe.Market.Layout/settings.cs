using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warframe.Market.Layout.Responses;

namespace Warframe.Market.Layout
{
    public partial class settings : Form
    {
        public settings()
        {
            InitializeComponent();
            UsernameBox.Text = MainFrame.cfg.username;
            DeviationSlider.Value = MainFrame.cfg.minDevAlert;
            devlabel.Text = MainFrame.cfg.minDevAlert + "%";
            MABox.Text = MainFrame.cfg.minMAAlert.ToString();
            LastUpdate.Text = CalcLastUpdate(MainFrame.cfg.lastupdate).ToString();
            MinVBox.Text = MainFrame.cfg.minVolume.ToString();
            MinMAB.Text = MainFrame.cfg.minMA.ToString();
            bunifuDropdown1.selectedIndex = (int)MainFrame.cfg.calculationType;
            relicBtn.Checked = MainFrame.cfg.hdsetsExcluded;
            if (CalcLastUpdate(MainFrame.cfg.lastupdate) >= 14)
                MessageBox.Show("It has been over 14 days since your last update.\nIt's recommended you update your list regularly.","Notice");
        }

        public int CalcLastUpdate(DateTime lastupdate)
        {
            DateTime today = DateTime.Today;
            int daysSince = today.Subtract(lastupdate).Days;
            return daysSince;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Program.settingsOpen = false;
            this.Close();
        }

        private void Savebutton_Click(object sender, EventArgs e)
        {
            if(Request.ProfileExists(UsernameBox.Text))
            {
                MainFrame.cfg.username = UsernameBox.Text;
                MainFrame.cfg.saveCfg();
                Program.paused = false;
            }else
            {
                MessageBox.Show("Username not found","Error");
            }
        }

        private void DeviationSlider_ValueChangeComplete(object sender, EventArgs e)
        {
            if(MainFrame.cfg.hotdeals > DeviationSlider.Value)
            {
                DeviationSlider.Value = MainFrame.cfg.hotdeals;
                devlabel.Text = DeviationSlider.Value + "%";
                MessageBox.Show("The minimum deviation for alerts can't be lower than the minimum deviation for hot deals to show.","Error");
            }else
            {
                MainFrame.cfg.minDevAlert = DeviationSlider.Value;
                MainFrame.cfg.saveCfg();
            }
            
        }

        private void MABox_OnValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (MABox.Text.Length == 1)
                    return;
                if (MABox.Text[MABox.Text.Length - 1] != '.')
                {
                    double newNum = double.Parse(MABox.Text);
                    if(newNum < 10)
                    {
                        MessageBox.Show("The minimum must be over 10!", "Error");
                        MABox.Text = MainFrame.cfg.minMAAlert.ToString();
                    }else
                    {
                        MainFrame.cfg.minMAAlert = double.Parse(MABox.Text);
                        MainFrame.cfg.saveCfg();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Incorrect format!","Error");
                MABox.Text = MainFrame.cfg.minMAAlert.ToString();
            }
        }

        private void MinMAB_OnValueChanged(object sender, EventArgs e)
        {
            if (!MinMAB.Text.Equals(""))
            {
                MainFrame.cfg.minMA = int.Parse(MinMAB.Text);
                MainFrame.cfg.saveCfg();
            }
        }

        private void MinVBox_OnValueChanged(object sender, EventArgs e)
        {
            if (!MinVBox.Text.Equals(""))
            {
                MainFrame.cfg.minVolume = int.Parse(MinVBox.Text);
                MainFrame.cfg.saveCfg();
            }
        }

        private void ViewListButton_Click(object sender, EventArgs e)
        {
            ListView view = new ListView();
            view.Show();
        }

        private void DeviationSlider_ValueChanged(object sender, EventArgs e)
        {
            devlabel.Text = DeviationSlider.Value + "%";
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (Program.scanRunning)
                return;
            ExitButton.Enabled = false;
            Progressbar.Value = 0;
            Program.scanRunning = true;
            new Thread(() => UpdateList()).Start();
        }

        public void UpdateList()
        {
            File.AppendAllText("debug.txt","\nStarting scan");
            List<ListItem> newItemList = new List<ListItem>();
            List<Responses.En> items = Request.itemsList();
            string itemList = "";
            foreach (var item in items)
                itemList += " "+item.item_name;
            File.AppendAllText("debug.txt", "\nAll items: "+Environment.NewLine+itemList);
            if (MainFrame.cfg.relicsExcluded)
            {
                items = items.Where(x => !x.item_name.ToLower().StartsWith("axi ")).Where(x => !x.item_name.ToLower().StartsWith("meso ")).Where(x => !x.item_name.ToLower().StartsWith("neo ")).Where(x => !x.item_name.ToLower().StartsWith("lith ")).ToList();
                itemList = "";
                foreach (var item in items)
                    itemList += " " + item.item_name;
                File.AppendAllText("debug.txt", Environment.NewLine+Environment.NewLine+Environment.NewLine+"\nRelics excluded, new item list: " + itemList);
            }
                
            if (MainFrame.cfg.setsExcluded)
            {
                items = items.Where(x => !x.item_name.ToLower().EndsWith(" set")).ToList();
                itemList = "";
                foreach (var item in items)
                    itemList += " " + item.item_name;
                File.AppendAllText("debug.txt", Environment.NewLine + Environment.NewLine + Environment.NewLine + "\nSets excluded, new item list: " + itemList);
            }
                
            int currPercent = 0;
            int itemsToCheck = items.Count;
            int checkedItems = 0;

            foreach (var item in items)
            {
                try
                {
                    string response = Request.DLString($"items/{item.url_name}/statistics");
                    StatisticsResponse statResponse = new StatisticsResponse();
                    List<FEEhour> newListClosed = new List<FEEhour>();
                    statResponse = JsonConvert.DeserializeObject<Responses.StatisticsResponse>(response);
                    newListClosed = statResponse.payload.statistics_closed.fhours.OrderByDescending(x => x.datetime).ToList();
                    if (MainFrame.cfg.modsExcluded)
                    {
                        if(statResponse.payload.statistics_live.ndays.Count != 0)
                        {
                            if (statResponse.payload.statistics_live.ndays.FirstOrDefault().mod_rank != null)
                            {
                                File.AppendAllText("debug.txt", Environment.NewLine+ $"\nMods excluded, {item.item_name} will not be in the list");
                                continue;
                            }
                               
                        }
                    }
                    
                    if (newListClosed.Count() != 0)
                    {
                        int volume = 0;
                        foreach (var FEHItem in newListClosed)
                        {
                            volume += FEHItem.volume;
                        }
                        double newSma = Request.CalculateSMA(item.url_name, statResponse);
                        if (newSma > MainFrame.cfg.minMA && volume > MainFrame.cfg.minVolume)
                        {
                            ListItem fullItem = new ListItem();
                            fullItem.url_name = item.url_name;
                            fullItem.name = item.item_name;
                            fullItem.moving_avg = newSma;
                            newItemList.Add(fullItem);
                        }else
                        {
                            File.AppendAllText("debug.txt", Environment.NewLine + $"\n{item.item_name} has been excluded as it does not meet the min. requirements. SMA: {newSma} Volume: {volume}");
                        }
                    }
                    checkedItems++;
                    currPercent = (int)(((double)checkedItems / (double)itemsToCheck) * 100.00);
                    this.Invoke((MethodInvoker)delegate {
                        Progressbar.Value = currPercent;
                    });
                }
                catch(Exception e)
                {
                    MessageBox.Show("Inner Exception occured: "+e.Message+ "\nStacktrace: "+e.StackTrace);
                }
                
            }
            MainFrame.cfg.itemList = newItemList;
            MainFrame.cfg.lastupdate = DateTime.Today;
            MainFrame.cfg.saveCfg();
            //MessageBox.Show("List updated, now checking for hot deals on " + newItemList.Count + " items.","Success");
            this.Invoke((MethodInvoker)delegate {
                ExitButton.Enabled = true;
                LastUpdate.Text = "0";
                Progressbar.Value = 0;
            });
            Program.scanRunning = false;
        }

        private void BunifuThinButton21_Click(object sender, EventArgs e)
        {
            Blacklist view = new Blacklist();
            view.Show();
        }

        private void BunifuDropdown1_onItemSelected(object sender, EventArgs e)
        {
            MainFrame.cfg.calculationType = (Config.calc)bunifuDropdown1.selectedIndex;
            MainFrame.cfg.saveCfg();
        }

        private void RelicBtn_OnChange(object sender, EventArgs e)
        {
            MainFrame.cfg.hdsetsExcluded = !MainFrame.cfg.hdsetsExcluded;
            MainFrame.cfg.saveCfg();
        }
    }
}
