using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout.Responses
{
    public class FEEhour
{
    public DateTime datetime { get; set; }
    public int volume { get; set; }
    public double min_price { get; set; }
    public double max_price { get; set; }
    public int open_price { get; set; }
    public int closed_price { get; set; }
    public double avg_price { get; set; }
    public double wa_price { get; set; }
    public double median { get; set; }
    public double moving_avg { get; set; }
    public int donch_top { get; set; }
    public int donch_bot { get; set; }
    public string id { get; set; }
}

public class NNNday
{
    public DateTime datetime { get; set; }
    public int volume { get; set; }
    public double min_price { get; set; }
    public double max_price { get; set; }
    public int open_price { get; set; }
    public int closed_price { get; set; }
    public double avg_price { get; set; }
    public double wa_price { get; set; }
    public double median { get; set; }
    public double moving_avg { get; set; }
    public int donch_top { get; set; }
    public int donch_bot { get; set; }
    public string id { get; set; }
}
    public class FEHours
    {
        public DateTime datetime { get; set; }
        public int volume { get; set; }
        public double min_price { get; set; }
        public double max_price { get; set; }
        public double avg_price { get; set; }
        public double wa_price { get; set; }
        public double median { get; set; }
        public string order_type { get; set; }
        public double moving_avg { get; set; }
        public string id { get; set; }
    }

    public class NinetyDays
    {
        public DateTime datetime { get; set; }
        public int volume { get; set; }
        public double min_price { get; set; }
        public double max_price { get; set; }
        public double avg_price { get; set; }
        public double wa_price { get; set; }
        public double median { get; set; }
        public string order_type { get; set; }
        public double moving_avg { get; set; }
        public int? mod_rank { get; set; }
        public string id { get; set; }
    }

    public class StatisticsLive
    {
        [JsonProperty("48hours")]
        public List<FEHours> fhours { get; set; }
        [JsonProperty("90days")]
        public List<NinetyDays> ndays { get; set; }
    }

    public class StatisticsClosed
    {
        [JsonProperty("48hours")]
        public List<FEEhour> fhours { get; set; }
        [JsonProperty("90days")]
        public List<NNNday> ndays { get; set; }
    }

    public class StatisticsResponse
    {
        public Payload payload { get; set; }
    }
}
