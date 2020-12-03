using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {

        [JsonProperty(PropertyName = "tank")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location { get; set; }

        [JsonProperty(PropertyName = "bdir")]
        public Vector2D orientation { get; set; }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D aiming { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "hp")]
        public int hitPoints { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int score { get; set; }

        [JsonProperty(PropertyName = "died")]
        public bool died { get; set; }

        [JsonProperty(PropertyName = "dc")]
        public bool disconnected { get; set; }

        [JsonProperty(PropertyName = "join")]
        public bool joined { get; set; }

        [JsonIgnoreAttribute]
        public string color { get; set; }

        [JsonIgnoreAttribute]
        public Vector2D tankVelocity { get; set; }

        [JsonIgnoreAttribute]
        public int shotTimer { get; set; }

        [JsonIgnoreAttribute]
        public int railgunShots { get; set; }

        [JsonIgnoreAttribute]
        public int timeToRespawn { get; set; }

        [JsonIgnoreAttribute]
        public int tankShotsHit { get; set; }

        [JsonIgnoreAttribute]
        public int tankTotalShots { get; set; }

        public Tank()
        {
            aiming = new Vector2D(0, -1);
            orientation = new Vector2D(0, -1);
            aiming = new Vector2D(0, -1);
            tankVelocity = new Vector2D(0, 0);
            died = false;
            disconnected = false;
            location = new Vector2D(0, 0);
            shotTimer = 0;
            timeToRespawn = 0;
            railgunShots = 0;
            tankTotalShots = 0;
            tankShotsHit = 0;
        }
    }
}
