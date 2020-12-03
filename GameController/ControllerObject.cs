using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace GameController
{
    public class ControllerObject
    {
        [JsonProperty(PropertyName = "moving")]
        private string moveDirection = "none";

        [JsonProperty(PropertyName = "fire")]
        private string fireStatus = "none";

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D turretDirection = new Vector2D(1,1);

        public ControllerObject()
        {
        }

        public string getFireStatus()
        {
            return fireStatus;
        }

        public string getMoveDirection()
        {
            return moveDirection;
        }

        public Vector2D getTurretDirection()
        {
            return turretDirection;
        }

        //Method used for changing the move direction variable
        public void moveTank(string dir)
        {
            switch (dir)
            {
                case "up":
                    moveDirection = dir;
                    break;

                case "down":
                    moveDirection = dir;
                    break;

                case "left":
                    moveDirection = dir;
                    break;

                case "right":
                    moveDirection = dir;
                    break;

                case "none":
                    moveDirection = dir;
                    break;
            }
        }

        //Method for changing the fireTank variable
        public void fireTank(string type)
        {
            switch (type)
            {
                case "none":
                    fireStatus = type;
                    break;

                case "main":
                    fireStatus = type;
                    break;

                case "alt":
                    fireStatus = type;
                    break;
            }
        }

        //Method for changing the turretDirection variable
        public void turretDir(double x, double y)
        {
            turretDirection = new Vector2D(x, y);
            turretDirection.Normalize();
        }
    }
}
