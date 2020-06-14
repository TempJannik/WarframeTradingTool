using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout.Responses
{
    public class Ko
    {
        public string item_name { get; set; }
    }

    public class De
    {
        public string item_name { get; set; }
    }

    public class Ru
    {
        public string item_name { get; set; }
    }

    public class Sv
    {
        public string item_name { get; set; }
    }

    public class Fr
    {
        public string item_name { get; set; }
    }

    public class Zh
    {
        public string item_name { get; set; }
    }

    public class Item
    {
        public Ko ko { get; set; }
        public string sub_icon { get; set; }
        public De de { get; set; }
        public Ru ru { get; set; }
        public string url_name { get; set; }
        public Sv sv { get; set; }
        public List<string> tags { get; set; }
        public string icon { get; set; }
        public string thumb { get; set; }
        public Fr fr { get; set; }
        public En en { get; set; }
        public string id { get; set; }
        public Zh zh { get; set; }
    }

    public class SellOrder
    {
        public bool visible { get; set; }
        public string id { get; set; }
        public DateTime creation_date { get; set; }
        public string region { get; set; }
        public DateTime last_update { get; set; }
        public int quantity { get; set; }
        public string platform { get; set; }
        public Item item { get; set; }
        public string order_type { get; set; }
        public double platinum { get; set; }
    }

    public class BuyOrder
    {
        public bool visible { get; set; }
        public string order_type { get; set; }
        public string platform { get; set; }
        public string id { get; set; }
        public string region { get; set; }
        public int quantity { get; set; }
        public Item item { get; set; }
        public int platinum { get; set; }
        public DateTime creation_date { get; set; }
        public DateTime last_update { get; set; }
    }

    public class Payload
    {
        public List<SellOrder> sell_orders { get; set; }
        public List<BuyOrder> buy_orders { get; set; }
        public Items items { get; set; }
        public StatisticsLive statistics_live { get; set; }
        public StatisticsClosed statistics_closed { get; set; }
        public Profile profile { get; set; }
        public List<Order> orders { get; set; }
        [JsonProperty("Item")]
        public SetItem item { get; set; }
        public string error { get; set; }
    }

    public class OrdersResponse
    {
        public Payload payload { get; set; }
    }
}
