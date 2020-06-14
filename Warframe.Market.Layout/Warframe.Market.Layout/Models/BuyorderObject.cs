using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout
{
    public class BuyorderObject
    {
        public string item { get; set; }
        public double sma { get; set; }
        public double loffer { get; set; }
        public double hoffer { get; set; }
        public double dev { get; set; }

        public BuyorderObject(string item, double sma, double loffer, double hoffer, double dev)
        {
            this.item = item;
            this.sma = sma;
            this.loffer = loffer;
            this.hoffer = hoffer;
            this.dev = dev;
        }
    }
}
