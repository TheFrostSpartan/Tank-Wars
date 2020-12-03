using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace Model
{
    public class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location { get; set; }

        [JsonProperty(PropertyName = "died")]
        public bool died { get; set; }

        [JsonIgnoreAttribute]
        public int respawnTimer { get; set; }

        public Powerup()
        {
            respawnTimer = -1;
        }
    }
}
