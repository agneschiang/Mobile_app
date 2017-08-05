using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Identification
{ 
    public class Shapedetail
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "Tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "Probability")]
        public float Probability { get; set; }

        [JsonProperty(PropertyName = "Colour")]
        public string Colour { get; set; }
    }
}
