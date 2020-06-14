using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout.Responses
{
    public class User
    {
        public string ingame_name { get; set; }
        public DateTime? last_seen { get; set; }
        public double reputation_bonus { get; set; }
        public double reputation { get; set; }
        public string region { get; set; }
        public string avatar { get; set; }
        public string status { get; set; }
        public string id { get; set; }
    }

    public class Order
    {
        public DateTime creation_date { get; set; }
        public bool visible { get; set; }
        public int quantity { get; set; }
        public User user { get; set; }
        public DateTime last_update { get; set; }
        public double platinum { get; set; }
        public string order_type { get; set; }
        public string region { get; set; }
        public string platform { get; set; }
        public string id { get; set; }
    }

    class ItemOrderResponse
    {
        public Payload payload { get; set; }
    }
}
