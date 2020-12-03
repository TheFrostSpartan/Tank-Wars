using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class Constants
    {
        //The main players ID
        public static int playerID = -1;

        //Number of turrets currently in game
        public static int playerTurretCounter = 0;

        //Number of tanks currently in game
        public static int playerTankCounter = 0;
        
        //The main players name
        public static string playerName;

        //The worldsize
        public static int worldSize = 0;

        //Boolean for knowing whether the client has recieved the walls
        public static bool wallsRec = false;

        //Boolean for knowing whether this is the first frame or not
        public static bool firstFrame = true;

        //Integers for Tank width and height
        public static int tankWidth = 30;
        public static int tankHeight = 50;

        //The main socket that the client is connected on
        public static SocketState mainSocket;

        public static bool enabled = true;


        //Dictionaries that will hold every object in the world 
        public static Dictionary<int, Explosion> explosions = new Dictionary<int, Explosion>();
        public static Dictionary<int, Wall> walls = new Dictionary<int, Wall>();
        public static Dictionary<int, Tank> tanks = new Dictionary<int, Tank>();
        public static Dictionary<int, Beam> beams = new Dictionary<int, Beam>();
        public static Dictionary<int, Powerup> powerups = new Dictionary<int, Powerup>();
        public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    }
}
