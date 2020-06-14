using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout.Responses
{
    public class En
    {
        public string thumb { get; set; }
        public string url_name { get; set; }
        public string item_name { get; set; }
        public string id { get; set; }
    }

    public class Items
    {
        public List<En> en { get; set; }
    }

    public class ItemsResponse
    {
        public Payload payload { get; set; }
    }
}
