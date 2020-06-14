using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout.Responses
{
    public class ItemsInSet
    {
        public string id { get; set; }
        public string sub_icon { get; set; }
        public List<string> tags { get; set; }
        public string icon_format { get; set; }
        public bool set_root { get; set; }
        public Ru ru { get; set; }
        public int trading_tax { get; set; }
        public string thumb { get; set; }
        public string url_name { get; set; }
        public Zh zh { get; set; }
        public De de { get; set; }
        public Sv sv { get; set; }
        public En en { get; set; }
        public string icon { get; set; }
        public Fr fr { get; set; }
        public Ko ko { get; set; }
        public int mastery_level { get; set; }
    }

    public class SetItem
    {
        public string id { get; set; }
        public List<ItemsInSet> items_in_set { get; set; }
    }
    class ItemResponse
    {
        public Payload payload;
    }
}
