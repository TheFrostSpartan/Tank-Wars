using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace Model
{
    public class Wall 
    {
        [JsonProperty(PropertyName = "wall")]
        public int ID { get; set; }

        [JsonProperty(PropertyName = "p1")]
        public Vector2D endpointOne { get; set; }

        [JsonProperty(PropertyName = "p2")]
        public Vector2D endpointTwo { get; set; }

        [JsonIgnoreAttribute]
        public double[] totalArea { get; private set; }


        public Wall()
        {
        }

        public Wall(Vector2D input1, Vector2D input2)
        {
            endpointOne = input1;
            endpointTwo = input2;

            totalArea = new double[5];
            if(endpointOne.GetX() > endpointTwo.GetX())
            {
                totalArea[3] = endpointOne.GetX() + 50;
                totalArea[1] = endpointTwo.GetX() - 50;

            }
            else
            {
                totalArea[1] = endpointOne.GetX() - 50;
                totalArea[3] = endpointTwo.GetX() + 50;
            }

            if (endpointOne.GetY() > endpointTwo.GetY())
            {
                totalArea[4] = endpointOne.GetY() + 50;
                totalArea[2] = endpointTwo.GetY() - 50;

            }
            else
            {
                totalArea[2] = endpointOne.GetY() - 50;
                totalArea[4] = endpointTwo.GetY() + 50;
            }

        }
    }
}
