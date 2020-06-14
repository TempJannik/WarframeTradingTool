using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout
{
    public class UndercutObject
    {
        public string item { get; set; }
        public double sma { get; set; }
        public double yoffer { get; set; }
        public double toffer { get; set; }
        public double dev { get; set; }

        public UndercutObject(string item, double sma, double yoffer, double toffer, double dev)
        {
            this.item = item;
            this.sma = sma;
            this.yoffer = yoffer;
            this.toffer = toffer;
            this.dev = dev;
        }
    }
}
