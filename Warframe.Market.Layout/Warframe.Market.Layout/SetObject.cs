using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout
{
    public class SetObject
    {
        public string item { get; set; }
        public double sma { get; set; }
        public double combined { get; set; }
        public double lowestprice { get; set; }
        public double dev { get; set; }

        public SetObject(string item, double sma, double combined, double lowestprice, double dev)
        {
            this.item = item;
            this.sma = sma;
            this.lowestprice = lowestprice;
            this.dev = dev;
            this.combined = combined;
        }
    }
}
