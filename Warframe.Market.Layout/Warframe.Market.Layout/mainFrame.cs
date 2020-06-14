using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warframe.Market.Layout
{
    public partial class MainFrame : Form
    {
        public static Config cfg;
        public static bool muted = false;
        public List<UndercutObject> lastList;
        public MainFrame()
        {
            InitializeComponent();
            cfg = new Config();
            cfg.loadCfg();
            HotDealsSlider.Value = cfg.hotdeals;
            PercentLabel.Text = HotDealsSlider.Value+" Platinum";
            lastList = new List<UndercutObject>();
            if (!Request.ProfileExists(cfg.username))
            {
                MessageBox.Show("Your current username wasn't found, please change it in the settings for the scan to start.","Warning");
            }
            Thread t = new Thread(() => HandleLoops());
            t.IsBackground = true;
            t.Start();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if(!Program.settingsOpen)
            {
                Program.settingsOpen = true;
                settings settingsForm = new settings();
                settingsForm.Show();
            }
        }

        private void SoundButton_Click(object sender, EventArgs e)
        {
            if(!muted)
            {
                muted = true;
                SoundButton.Iconimage = Properties.Resources.icons8_kein_ton_50;
            }else
            {
                muted = false;
                SoundButton.Iconimage = Properties.Resources.icons8_hohe_lautstärke_50;
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(2);
        }

        private void HandleLoops()
        {
            while(true)
            {
                while(!Program.scanRunning)
                {
                    if(Request.ProfileExists(cfg.username))
                    {
                        CheckForUndercuts();
                        CheckForOvercuts();
                    }
                    
                    var list = CheckForDeals();
                    CheckForBuyOrders(list);
                    CheckForSetProfits();
                }
                Thread.Sleep(5000);
            }
        }

        private void CheckForBuyOrders(List<HotdealObject> list)
        {
            List<BuyorderObject> blist = new List<BuyorderObject>();
            int checkedItems = 0;
            if (list == null)
                return;
            foreach (var item in cfg.itemList)
            {
                if (Program.buyorderdealsPaused)
                    return;
               while (Program.scanRunning || Program.paused)
                    Thread.Sleep(500);
               try
               {
                    checkedItems++;
                    int currPercent = (int)(((double)checkedItems / (double)cfg.itemList.Count()) * 100.00);
                    this.Invoke((MethodInvoker)delegate {
                        buyProgress.Value = currPercent;
                    });

                    var buyOrders = Request.GetItemBuyOrders(item.url_name).Where(x=> x.user.status.Equals("ingame")).OrderByDescending(x => x.platinum);
                    Console.WriteLine("Looking for "+item.name);
                    var hotdealsobj = list.First(x => x.item.Equals(item.name));
                    if (hotdealsobj == null)
                     continue;
                    Console.WriteLine("Found");
                    double highestBuy = 0;
                    if(buyOrders.Count() != 0)
                        highestBuy = buyOrders.FirstOrDefault().platinum;
                    double lowestSell = hotdealsobj.loffer;
                    double difference = hotdealsobj.sma - highestBuy;
                    if (difference > cfg.buyorderdev)
                    {
                        blist.Add(new BuyorderObject(item.name, hotdealsobj.sma, lowestSell, highestBuy, difference));
                    }
               }catch(Exception e)
               {
                    Console.WriteLine("Exception: " + e.Message + "\nStacktrace: " + e.StackTrace);
                }
            }

            this.Invoke((MethodInvoker)delegate {
                buyChart.Rows.Clear();
            });
            if (blist.Count != 0)
            {
                foreach (var item in blist)
                {
                    this.Invoke((MethodInvoker)delegate {
                        buyChart.Rows.Add(item.item, item.sma, item.hoffer, item.loffer, Math.Round(item.dev, 2));
                    });
                }
            }
            AutoSizeRows(buyChart);
        }

        private void CheckForSetProfits()
        {
            List<SetObject> shownSets = new List<SetObject>();
            var allItems = cfg.itemList.Where(x => x.name.EndsWith(" Set"));
            string[] exceptions = new string[] { "Dual Kamas", "Akstiletto", "Venka", "Akjagara", "Spira", "Hikou"};
            int checkedItems = 0;
            foreach(var possibleSet in allItems)
            {
                if (Program.setprofitsPaused)
                    return;
                while (Program.paused || Program.scanRunning)
                    Thread.Sleep(500);

                try
                {
                    checkedItems++;
                    int currPercent = (int)(((double)checkedItems / (double)allItems.Count()) * 100.00);
                    this.Invoke((MethodInvoker)delegate {
                        setProgress.Value = currPercent;
                    });
                    bool minimumNotMet = false;
                    double currPlatinumPrice = 0;
                    var setInfo = Request.getSetInfo(possibleSet.url_name);
                    foreach (var setItem in setInfo.items_in_set)
                    {
                        if (setItem.url_name.Contains("_set"))
                            continue;
                        var orders = Request.GetItemSellOrders(setItem.url_name).Where(x => x.user.status.Equals("ingame"));
                        if (orders.Count() != 0)
                        {
                            foreach(var ex in exceptions)
                            {
                                if (setItem.url_name.Contains(ex))
                                    if(!setItem.url_name.Contains("blueprint"))
                                        currPlatinumPrice += orders.OrderBy(x => x.platinum).Skip(1).FirstOrDefault().platinum;
                            }
                            
                            currPlatinumPrice += orders.OrderBy(x => x.platinum).FirstOrDefault().platinum;
                        }
                        else
                        {
                            minimumNotMet = true;
                            break;
                        }

                    }
                    if (minimumNotMet)
                        continue;
                    var setSellList = Request.GetItemSellOrders(possibleSet.url_name).Where(x => x.user.status.Equals("ingame"));
                    if (setSellList.Count() != 0)
                    {
                        double setPrice = setSellList.OrderBy(x => x.platinum).FirstOrDefault().platinum;
                        if (currPlatinumPrice < setPrice)
                        {
                            double dev = -((double)currPlatinumPrice / (double)setPrice - 1.0) * 100.00;
                            if ((setPrice - currPlatinumPrice) > cfg.setdeviation)
                            {
                                shownSets.Add(new SetObject(possibleSet.name, Request.CalculateSMA(possibleSet.url_name), currPlatinumPrice, setPrice, setPrice - currPlatinumPrice));
                            }
                        }
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message + "\nStacktrace: " + e.StackTrace);
                }
            }

            this.Invoke((MethodInvoker)delegate {
                SetChart.Rows.Clear();
            });
            if (shownSets.Count != 0)
            {
                foreach (var item in shownSets)
                {
                    this.Invoke((MethodInvoker)delegate {
                        SetChart.Rows.Add(item.item, item.combined, item.lowestprice,item.sma ,Math.Round(item.dev, 2));
                    });
                }
            }

            AutoSizeRows(SetChart);
        }

        private void CheckForOvercuts()
        {
            List<UndercutObject> updList = new List<UndercutObject>();
            var userBuyOrders = Request.GetProfileBuyOrders(cfg.username);
            foreach (var order in userBuyOrders)
            {
                if (Program.scanRunning)
                    return;
                while (Program.paused)
                    Thread.Sleep(500);
                try
                {
                    var orderList = Request.GetItemBuyOrders(order.item.url_name).Where(x => x.user.status.Equals("ingame")).OrderByDescending(x => x.platinum);
                    if (orderList.Count() > 1)
                    {
                        if (orderList.FirstOrDefault().user.ingame_name != cfg.username)
                        {
                            var highestOffer = orderList.FirstOrDefault();
                            string item = order.item.en.item_name;
                            double newSma = Request.CalculateSMA(order.item.url_name);
                            double yoffer = order.platinum;
                            double toffer = highestOffer.platinum;
                            double dev = ((double)yoffer / (double)toffer - 1.0) * 100.00;
                            updList.Add(new UndercutObject(item, newSma, yoffer, toffer, dev));
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message + "\nStacktrace: " + e.StackTrace);
                }
            }

            this.Invoke((MethodInvoker)delegate {
                OvercutChart.Rows.Clear();
            });
            if (updList.Count != 0)
            {
                foreach (var item in updList)
                {
                    this.Invoke((MethodInvoker)delegate {
                        OvercutChart.Rows.Add(item.item, item.sma, item.yoffer, item.toffer, Math.Round(item.dev, 2));
                    });
                }
                if (listsIdentical(updList, lastList))
                    Sound.PlayUndercutSound();
                lastList = updList;
            }
            AutoSizeRows(OvercutChart);
        }

        private void CheckForUndercuts()
        {
            List<UndercutObject> updList = new List<UndercutObject>();
            var userSellOrders = Request.GetProfileSellOrders(cfg.username);
            foreach(var order in userSellOrders)
            {
                if (Program.scanRunning)
                    return;
                while (Program.paused)
                    Thread.Sleep(500);
                try
                {
                    var orderList = Request.GetItemSellOrders(order.item.url_name).Where(x => x.user.status.Equals("ingame")).OrderBy(x => x.platinum);
                    if (orderList.Count() > 1)
                    {
                        if (orderList.FirstOrDefault().user.ingame_name != cfg.username)
                        {
                            var lowestOffer = orderList.FirstOrDefault();
                            string item = order.item.en.item_name;
                            double newSma = Request.CalculateSMA(order.item.url_name);
                            double yoffer = order.platinum;
                            double toffer = lowestOffer.platinum;
                            double dev = ((double)yoffer / (double)toffer - 1.0) * 100.00;
                            updList.Add(new UndercutObject(item, newSma, yoffer, toffer, dev));
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message + "\nStacktrace: " + e.StackTrace);
                }
            }

            this.Invoke((MethodInvoker)delegate {
                UndercutChart.Rows.Clear();
            });
            if (updList.Count != 0)
            {

                foreach(var item in updList)
                {
                    this.Invoke((MethodInvoker)delegate {
                        UndercutChart.Rows.Add(item.item, item.sma, item.yoffer, item.toffer, Math.Round(item.dev, 2));
                    });
                }
                if(listsIdentical(updList, lastList))
                    Sound.PlayUndercutSound();
                lastList = updList;
            }
            AutoSizeRows(UndercutChart);
        }

        private bool listsIdentical(List<UndercutObject> list1, List<UndercutObject> list2)
        {
            bool alright = true;
            foreach(var item in list1)
            {
                if (list2.Where(x=> x.item.Equals(item.item)).Count() == 0)
                {
                    alright = false;
                }
            }
            foreach (var item in list2)
            {
                if (list1.Where(x => x.item.Equals(item.item)).Count() == 0)
                {
                    alright = false;
                }
            }

            return alright;
        }

        private List<HotdealObject> CheckForDeals()
        {
            List<HotdealObject> objList = new List<HotdealObject>();
            List<HotdealObject> continList = new List<HotdealObject>();
            int checkedItems = 0;
            var itemList = cfg.itemList;
            //if(cfg.hdsetsExcluded)
                //itemList = itemList.Where(x => !x.name.EndsWith(" Set")).ToList();

            foreach(var item in itemList)
            {
                while (Program.paused || Program.scanRunning)
                    Thread.Sleep(500);
                if (Program.hotdealsPaused)
                    return null;
                try
                {
                    checkedItems++;
                    int currPercent = 0;
                    currPercent = (int)(((double)checkedItems / (double)itemList.Count()) * 100.00);
                    this.Invoke((MethodInvoker)delegate {
                        ProgressBar.Value = currPercent;
                    });
                    var orderList = Request.GetItemSellOrders(item.url_name).Where(x => x.user.status.Equals("ingame")).OrderBy(x => x.platinum);
                    if (orderList.Count() != 0)
                    {
                        var lowestOffer = orderList.FirstOrDefault();
                        double lowestPlatinum = lowestOffer.platinum;
                        double newSma = Request.CalculateSMA(item.url_name);
                        double devplatinum = newSma - lowestPlatinum;
                        continList.Add(new HotdealObject(item.name, newSma, lowestPlatinum, devplatinum));
                        if (devplatinum > cfg.hotdeals)
                        {
                            objList.Add(new HotdealObject(item.name, newSma, lowestPlatinum, devplatinum));
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: "+e.Message + "\nStacktrace: "+e.StackTrace);
                }
            }
            this.Invoke((MethodInvoker)delegate {
                HotDealsChart.Rows.Clear();
            });
            if(objList.Count != 0)
            {
                foreach (var item in objList)
                {
                    this.Invoke((MethodInvoker)delegate {
                        HotDealsChart.Rows.Add(item.item, item.loffer,item.sma, item.devplat);
                    });
                }
                Sound.PlayDealsSound();
            }

            AutoSizeRows(HotDealsChart);
            return continList;
        }

        private void AutoSizeRows(Bunifu.Framework.UI.BunifuCustomDataGrid grid)
        {
            try
            {
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    this.Invoke((MethodInvoker)delegate {
                        if(i == 0)
                            grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        else
                            grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    });
                }
                this.Invoke((MethodInvoker)delegate
                {
                    grid.Sort(grid.Columns[grid.Columns.Count - 1], ListSortDirection.Descending);
                });
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + "\nStacktrace: " + e.StackTrace);
            }
        }

        private void UndercutChart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 0)
            {
                var itemlist = Request.itemsList();
                string urlname = itemlist.FirstOrDefault(x => x.item_name.Equals(UndercutChart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())).url_name;
                string link = "https://warframe.market/items/" + urlname;
                System.Diagnostics.Process.Start(link);
            }
        }

        private void HotDealsSlider_ValueChanged(object sender, EventArgs e)
        {
            PercentLabel.Text = HotDealsSlider.Value + " Platinum";
        }

        private void HotDealsSlider_ValueChangeComplete(object sender, EventArgs e)
        {
            cfg.hotdeals = HotDealsSlider.Value;
            cfg.saveCfg();
        }

        private void HotDealsChart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                var itemlist = Request.itemsList();
                string urlname = itemlist.FirstOrDefault(x => x.item_name.Equals(HotDealsChart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())).url_name;
                string link = "https://warframe.market/items/" + urlname;
                System.Diagnostics.Process.Start(link);
            }
        }

        private void BunifuCustomLabel4_Click(object sender, EventArgs e)
        {

        }

        private void Startstopbtn_Click(object sender, EventArgs e)
        {
            if(Program.paused)
            {
                Program.paused = false;
                startstopbtn.Iconimage = Properties.Resources.icons8_pause_50;
                startlabel.Text = "Scanning...";
            }else
            {
                Program.paused = true;
                startstopbtn.Iconimage = Properties.Resources.icons8_spielen_50;
                startlabel.Text = "Not Scanning";
            }
        }

        private void SetChart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                var itemlist = Request.itemsList();
                string urlname = itemlist.FirstOrDefault(x => x.item_name.Equals(SetChart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())).url_name;
                string link = "https://warframe.market/items/" + urlname;
                System.Diagnostics.Process.Start(link);
            }
        }

        private void BuySlider_ValueChangeComplete(object sender, EventArgs e)
        {
            buyLabel.Text = buySlider.Value.ToString() + " Platinum";
        }

        private void BuySlider_ValueChanged(object sender, EventArgs e)
        {
            cfg.buyorderdev = buySlider.Value;
            cfg.saveCfg();
        }

        private void SetSlider_ValueChanged_1(object sender, EventArgs e)
        {
            newSetLabel.Text = setSlider.Value.ToString() + " Platinum";
        }

        private void SetSlider_ValueChangeComplete_1(object sender, EventArgs e)
        {
            cfg.setdeviation = setSlider.Value;
            cfg.saveCfg();
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            if(Program.hotdealsPaused)
            {
                Program.hotdealsPaused = false;
                bunifuFlatButton1.Iconimage = Properties.Resources.icons8_pause_50;
            }else
            {
                Program.hotdealsPaused = true;
                bunifuFlatButton1.Iconimage = Properties.Resources.icons8_spielen_50;
            }
        }

        private void BunifuFlatButton2_Click(object sender, EventArgs e)
        {
            if (Program.buyorderdealsPaused)
            {
                Program.buyorderdealsPaused = false;
                bunifuFlatButton2.Iconimage = Properties.Resources.icons8_pause_50;
            }
            else
            {
                Program.buyorderdealsPaused = true;
                bunifuFlatButton2.Iconimage = Properties.Resources.icons8_spielen_50;
            }
        }

        private void BunifuFlatButton3_Click(object sender, EventArgs e)
        {
            if (Program.setprofitsPaused)
            {
                Program.setprofitsPaused = false;
                bunifuFlatButton3.Iconimage = Properties.Resources.icons8_pause_50;
            }
            else
            {
                Program.setprofitsPaused = true;
                bunifuFlatButton3.Iconimage = Properties.Resources.icons8_spielen_50;
            }
        }
    }
}
