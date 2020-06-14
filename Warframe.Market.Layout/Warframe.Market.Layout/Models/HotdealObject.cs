using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout
{
    public class HotdealObject
    {
        public string item { get; set; }
        public double sma { get; set; }
        public double loffer { get; set; }
        public double devplat { get; set; }

        public HotdealObject(string item, double sma, double loffer, double devplat)
        {
            this.item = item;
            this.sma = sma;
            this.loffer = loffer;
            this.devplat = devplat;
        }
    }
}
