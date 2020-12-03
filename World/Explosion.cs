using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace Model
{
    public class Explosion
    {
        public int ID { get; private set; }

        public Vector2D location { get; set; }

        public int timeout { get; set; }

        public Explosion(Vector2D vec, int explosionID)
        {
            location = vec;
            ID = explosionID;
            timeout = 32;
        }
    }
}
