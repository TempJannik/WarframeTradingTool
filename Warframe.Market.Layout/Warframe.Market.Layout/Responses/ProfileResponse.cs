using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe.Market.Layout.Responses
{
    public class ProfileResponse
    {
        public Payload payload { get; set; }
    }

    public class Profile
    {
        public DateTime last_seen { get; set; }
        public string platform { get; set; }
        public string about { get; set; }
        public string region { get; set; }
        public string ingame_name { get; set; }
        public string status { get; set; }
        public List<object> achievements { get; set; }
        public bool banned { get; set; }
        public string about_raw { get; set; }
        public bool own_profile { get; set; }
        public string avatar { get; set; }
        public object background { get; set; }
        public int reputation { get; set; }
        public string id { get; set; }
    }
}
