using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Warframe.Market.Layout
{
    public class Request
    {
        public static string DLString(string apiR)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Platform", "pc");
                client.Headers.Add("Language", "en");
                client.Proxy = null;
                string requestString = "https://api.warframe.market/v1/" + apiR;
                string result = client.DownloadString(requestString);
                Thread.Sleep(333);
                return result;
            }
        }

        public static double CalculateSMA(string url_name, Responses.StatisticsResponse resp = null)
        {
            Responses.StatisticsResponse statResponse = null;
            if (resp == null)
            {
                string response = DLString($"items/{url_name}/statistics");
                statResponse = JsonConvert.DeserializeObject<Responses.StatisticsResponse>(response);
            }
            else
                statResponse = resp;
            
            var newListClosed = statResponse.payload.statistics_closed.fhours.OrderByDescending(x => x.datetime);
            List<double> medianList = new List<double>();
            foreach (var FEHItem in newListClosed)
            {
                switch (MainFrame.cfg.calculationType)
                {
                    case Config.calc.MinMax:
                        medianList.Add(((FEHItem.min_price + FEHItem.max_price) / 2));
                        break;
                    case Config.calc.avg:
                        medianList.Add(FEHItem.avg_price);
                        break;
                    case Config.calc.median:
                        medianList.Add(FEHItem.median);
                        break;
                    case Config.calc.wa:
                        medianList.Add(FEHItem.wa_price);
                        break;
                    case Config.calc.moving_avg:
                        medianList.Add(FEHItem.moving_avg);
                        break;
                }
            }
            medianList.Sort();
            double newSma = medianList.ElementAtOrDefault((medianList.Count() - 1) / 2);
            return newSma;
        }

        public static List<Responses.En> itemsList()
        {
            string itemsJson = DLString("items");
            Responses.ItemsResponse response = JsonConvert.DeserializeObject<Responses.ItemsResponse>(itemsJson);
            return response.payload.items.en;
        }

        public static Responses.SetItem getSetInfo(string setname)
        {
            string json = DLString("items/"+setname);
            Responses.ItemResponse response = JsonConvert.DeserializeObject<Responses.ItemResponse>(json);
            return response.payload.item;
        }

        private static Responses.Profile getProfile(string username)
        {
            try
            {
                string profileJson = DLString("profile/" + username);
                Responses.ProfileResponse response = JsonConvert.DeserializeObject<Responses.ProfileResponse>(profileJson);
                return response.payload.profile;
            }
            catch
            {
                return null;
            }
        }

        public static bool ProfileExists(string username)
        {
            if (getProfile(username) == null)
                return false;
            else
                return true;
        }

        private static Responses.OrdersResponse GetProfileOrders(string username)
        {
            string profileJson = DLString($"profile/{username}/orders");
            Responses.OrdersResponse response = JsonConvert.DeserializeObject<Responses.OrdersResponse>(profileJson);
            return response;
        }

        public static List<Responses.SellOrder> GetProfileSellOrders(string username)
        {
            var profile = GetProfileOrders(username);
            return profile.payload.sell_orders;
        }

        public static List<Responses.BuyOrder> GetProfileBuyOrders(string username)
        {
            var profile = GetProfileOrders(username);
            return profile.payload.buy_orders;
        }

        public static List<Responses.Order> GetItemSellOrders(string item)
        {
            string itemsJson = DLString($"items/{item}/orders");
            var parsedJson = JsonConvert.DeserializeObject<Responses.ItemOrderResponse>(itemsJson);
            return parsedJson.payload.orders.Where(x => x.order_type.Equals("sell")).ToList();
        }

        public static List<Responses.Order> GetItemBuyOrders(string item)
        {
            string itemsJson = DLString($"items/{item}/orders");
            var parsedJson = JsonConvert.DeserializeObject<Responses.ItemOrderResponse>(itemsJson);
            return parsedJson.payload.orders.Where(x => x.order_type.Equals("buy")).ToList();
        }
    }
}
