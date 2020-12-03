using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;
using NetworkUtil;
using System.Text.RegularExpressions;
using Model;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace GameController
{
    public class Controller
    {
        //The main object that we will send for controller commands
        private ControllerObject outputData = new ControllerObject();

        public delegate void ServerUpdateHandler();

        private event ServerUpdateHandler updateHandler;

        private World gameWorld = new World();

        public Controller()
        {
            gameWorld = new World();
        }

        public World getWorld()
        {
            return gameWorld;
        }

        // Used to handle updates
        public void registerUpdateHandler(ServerUpdateHandler handler)
        {
            updateHandler += handler;
        }

        // Method to move the tank
        public void moveTank(string dir)
        {
            outputData.moveTank(dir);
        }

        // Method to fire the tank
        public void fireTank(string type)
        {
            outputData.fireTank(type);
        }

        // Method to change the turret direction
        public void turretDir(Point loc)
        {
            outputData.turretDir(loc.X-400, loc.Y-400);
        }

        // Main method to connect to the server
        public bool connectToServer(string hostName, string name)
        {
            //Use the Networking method to connect to the server
            Networking.ConnectToServer(ServerConnected, hostName, 11000);
            Constants.playerName = name;
            return true;
        }

        // The callback for when the server is connected
        private void ServerConnected(SocketState sock)
        {
            if (sock.ErrorOccured)
            {
                MessageBox.Show("Error: Connection to server unsuccessful, please restart client");
                Constants.enabled = true;
                return;
            }

            Constants.enabled = false;
            Constants.mainSocket = sock;
            Networking.Send(Constants.mainSocket.TheSocket, Constants.playerName);

            //Change the onNetworkAction and get the data
            sock.OnNetworkAction = ReceiveMessage;
            Networking.GetData(sock);
        }

        // The callback method for when the data is recieved
        public void ReceiveMessage(SocketState sock)
        {
            if (sock.ErrorOccured)
            {
                MessageBox.Show("Error: Could not retrieve data, please restart client and reconnect");
                Constants.enabled = true;
                return;
            }

            //Process the data recieved from the server
            ProcessMessages(sock);
            if(updateHandler != null)
            {
                updateHandler();
            }

            //Send the controller, stop the tank from firing
            sendControllerToServer(outputData);
            fireTank("none");

            //Continue the loop
            Networking.GetData(sock);
        }

        //Method used to process the messages
        public void ProcessMessages(SocketState sock)
        {
            //Get the data and split it around \n
            string totalData = sock.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            int i = 1;

            // For every string, find out what it is and then deserialize it.
            foreach (string s in parts)
            {
                totalData = totalData.Substring(s.Length, totalData.Length - s.Length);
                
                //If it doesn't end with\n, it is incomplete data
                if (!s.EndsWith("\n"))
                {
                    break;
                }
                //If this is the first frame, then the first thing being sent is the playerID
                else if (i == 1 && Constants.firstFrame)
                {
                    Constants.playerID = int.Parse(s);
                    sock.RemoveData(0, sock.GetData().Length - totalData.Length);
                    i++;
                    Constants.firstFrame = false;
                }
                //If this is the second bit of data being processed, then this is the worldsize
                else if (i == 2)
                {
                    Constants.worldSize = int.Parse(s);
                    sock.RemoveData(0, sock.GetData().Length - totalData.Length);
                    i++;
                }
                //If s contains a wall, then deserialize it and add it to the walls dictionary
                else if (s.Contains("wall"))
                {
                    Wall newWall = JsonConvert.DeserializeObject<Wall>(s);
                    Constants.walls.Add(newWall.ID, newWall);
                    sock.RemoveData(0, sock.GetData().Length - totalData.Length);
                    Constants.wallsRec = true;
                }
                //If s is a beam, then deserialize it and add it to the beams dictionary or update it
                else if (s.Contains("beam"))
                {
                    Beam newBeam = JsonConvert.DeserializeObject<Beam>(s);
                    if (Constants.beams.ContainsKey(newBeam.ID))
                    {
                        Constants.beams.Remove(newBeam.ID);
                        Constants.beams.Add(newBeam.ID, newBeam);
                    }
                    else
                    {
                        Constants.beams.Add(newBeam.ID, newBeam);
                    }
                    sock.RemoveData(0, sock.GetData().Length - totalData.Length);
                    continue;
                }
                //If s is a tank, then deserialize it and update all of the tank's data.
                //If this is a new tank, then determine its color as well
                else if (s.Contains("tank"))
                {
                    Tank newTank = JsonConvert.DeserializeObject<Tank>(s);
                    if (Constants.tanks.ContainsKey(newTank.ID))
                    {
                        Constants.tanks[newTank.ID].hitPoints = newTank.hitPoints;
                        Constants.tanks[newTank.ID].died = newTank.died;
                        Constants.tanks[newTank.ID].location = newTank.location;
                        Constants.tanks[newTank.ID].orientation = newTank.orientation;
                        Constants.tanks[newTank.ID].aiming = newTank.aiming;
                        Constants.tanks[newTank.ID].score = newTank.score;
                    }
                    else
                    {
                        switch((Constants.tanks.Keys.Count)%8)
                        {
                            case 0:
                                newTank.color = "blue";
                                break;

                            case 1:
                                newTank.color = "dark";
                                break;

                            case 2:
                                newTank.color = "green";
                                break;

                            case 3:
                                newTank.color = "lightGreen";
                                break;

                            case 4:
                                newTank.color = "orange";
                                break;

                            case 5:
                                newTank.color = "purple";
                                break;

                            case 6:
                                newTank.color = "red";
                                break;

                            case 7:
                                newTank.color = "yellow";
                                break;
                        }
                        Constants.tanks.Add(newTank.ID, newTank);
                    }
                    sock.RemoveData(0, sock.GetData().Length - totalData.Length);
                    continue;
                }
                //If s is a powerup, then deserialize it and add it to the powerups dictionary
                else if (s.Contains("power"))
                {
                    Powerup newPowerup = JsonConvert.DeserializeObject<Powerup>(s);
                    if (Constants.powerups.ContainsKey(newPowerup.ID))
                    {
                        Constants.powerups[newPowerup.ID].location = newPowerup.location;
                        Constants.powerups[newPowerup.ID].died = newPowerup.died;
                    }
                    else
                    {
                        Constants.powerups.Add(newPowerup.ID, newPowerup);
                    }
                    sock.RemoveData(0, sock.GetData().Length - totalData.Length);
                    continue;
                }
                //If s is a projectile, then deserialize it and add it to the projectiles dictionary
                else if (s.Contains("proj"))
                {
                    Projectile newProj = JsonConvert.DeserializeObject<Projectile>(s);
                    if (Constants.projectiles.ContainsKey(newProj.ID))
                    {
                        Constants.projectiles[newProj.ID].location = newProj.location;
                        Constants.projectiles[newProj.ID].died = newProj.died;
                    }
                    else
                    {
                        Constants.projectiles.Add(newProj.ID, newProj);
                    }
                    sock.RemoveData(0, sock.GetData().Length - totalData.Length);
                    continue;
                }
            }
        }

        //Method to be used to send the controller to the server
        public void sendControllerToServer(ControllerObject output)
        {
            if(Constants.playerID != -1 && Constants.worldSize != 0 && Constants.wallsRec == true)
            {
                string message = JsonConvert.SerializeObject(output);
                message += "\n";
                NetworkUtil.Networking.Send(Constants.mainSocket.TheSocket, message);
            }
        }
    }
}
