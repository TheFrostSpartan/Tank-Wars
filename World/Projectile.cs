using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace Model
{
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location { get; set; }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D orientation { get;  set; }

        [JsonProperty(PropertyName = "died")]
        public bool died { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public int tankID { get; set; }

        public Projectile()
        {
            died = false;
        }
    }
}
