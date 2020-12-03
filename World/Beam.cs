using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace Model
{
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "org")]
        public Vector2D origin { get; set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D direction { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public int tankID { get; set; }


        [JsonIgnoreAttribute]
        public int timeout { get; set; }

        public Beam()
        {
            timeout = 20;
        }
    }
}
