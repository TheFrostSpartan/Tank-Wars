using GameController;
using Model;
using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TankWars;

namespace Server
{
    class Program
    {
        //Dictionaries to hold every object the dictionary must keep track of
        private static Dictionary<int, Wall> walls = new Dictionary<int, Wall>();
        private static Dictionary<int, Tank> tanks = new Dictionary<int, Tank>();
        private static Dictionary<int, Beam> beams = new Dictionary<int, Beam>();
        private static Dictionary<int, Powerup> powerups = new Dictionary<int, Powerup>();
        private static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
        private static Dictionary<long, SocketState> connectedClients = new Dictionary<long, SocketState>();
        private static Dictionary<int, Tank> allTanksEver = new Dictionary<int, Tank>();

        //Objects to keep track of data for the server
        private static DatabaseController mainDataControl = new DatabaseController();
        private static Stopwatch gameTime = new Stopwatch();
        private static World serverWorld = new World();

        //Constants that define the rules of the world
        private static int MSFrameRate;
        private static int FramesPerShot;
        private static int RespawnRate;
        private static int StartingHitPoints;
        private static int ProjectileSpeed;
        private static double EngineStrength;
        private static int MaxPowerups;
        private static int MaxPowerupDelay;
        private static int shotCount = 0;

        static void Main(string[] args)
        {
            //Set all of the constants to values in the settings document
            XDocument temp = XDocument.Load(@"..\..\..\Resources\Resources\settings.xml");
            serverWorld.worldSize = (int)temp.Root.Element("UniverseSize");
            MSFrameRate = (int)temp.Root.Element("MSPerFrame");
            FramesPerShot = (int)temp.Root.Element("FramesPerShot");
            RespawnRate = (int)temp.Root.Element("RespawnRate");
            StartingHitPoints = (int)temp.Root.Element("StartingHitPoints");
            ProjectileSpeed = (int)temp.Root.Element("ProjectileSpeed");
            EngineStrength = (double)temp.Root.Element("EngineStrength");
            MaxPowerups = (int)temp.Root.Element("MaxPowerups");
            MaxPowerupDelay = (int)temp.Root.Element("MaxPowerupDelay");

            //Counter to assign every wall a unique ID
            int wallID = 0;

            //Create all of the walls from the settings document
            foreach (XElement t in temp.Root.Elements("Wall"))
            {
                Vector2D P1 = new Vector2D((double)t.Element("p1").Element("x"), (double)t.Element("p1").Element("y"));
                Vector2D P2 = new Vector2D((double)t.Element("p2").Element("x"), (double)t.Element("p2").Element("y"));
                Wall tempWall = new Wall(P1, P2);
                tempWall.ID = wallID;
                walls.Add(tempWall.ID, tempWall);
                wallID++;
            }

            //Spawn every powerup into the world
            for(int i = 0; i < MaxPowerups; i++)
            {
                powerRespawn();
            }

            //Start the server
            Networking.StartServer(ClientHandler, 11000);
            Console.WriteLine("Server is running: Accepting new Clients");

            //Start a new thread for the update method
            Thread loopThread = new Thread(() => UpdateLoop());
            loopThread.Start();

            //Start the http server
            Networking.StartServer(HandleHttpConnection, 80);
            Console.WriteLine("Database is running");

            //Time the game until a key is pressed
            gameTime.Start();
            Console.ReadLine();
            gameTime.Stop();

            //Upload the game to the database
            mainDataControl.sendGameToDatabase(allTanksEver, gameTime);
        }

        /// <summary>
        /// Callback method for http Connections
        /// </summary>
        /// <param name="obj">The socketstate the connection is on</param>
        private static void HandleHttpConnection(SocketState obj)
        {
            //Change the callback method then ask for more data
            obj.OnNetworkAction = ServeHttpRequest;
            Networking.GetData(obj);
        }

        /// <summary>
        /// Second callback method for after the server has made a connection
        /// </summary>
        /// <param name="state"></param>
        private static void ServeHttpRequest(SocketState state)
        {
            string request = state.GetData();
            
            //If the request is for an individual player, create their table
            if (request.Contains("GET /games?player="))
            {
                String playerNameRequest = request.Substring(21, request.IndexOf("\n") - 34);
                String dataToSend = mainDataControl.individualTableCreator(playerNameRequest);
                Networking.SendAndClose(state.TheSocket, dataToSend);
            }
            //If the request is for all games, generate all the tables
            else if (request.Contains("GET /games"))
            {
                String dataToSend = mainDataControl.allGamesTableCreator();
                Networking.SendAndClose(state.TheSocket, dataToSend);
            }
            //If the request is for anything else, send them to the homepage
            else
            {
                Networking.SendAndClose(state.TheSocket, mainDataControl.sendToHomePage());
            }
        }

        /// <summary>
        /// Loop to call update every frame
        /// </summary>
        static void UpdateLoop()
        {
            Stopwatch watch = new Stopwatch();

            //Until the program is closed, every MSFrameRate frames call update()
            while (true)
            {
                watch.Start();
                while (watch.ElapsedMilliseconds < MSFrameRate) { }
                watch.Reset();
                Update();
                
            }
        }

        /// <summary>
        /// Callback method for when a client connects to the server
        /// </summary>
        /// <param name="sock"></param>
        static void ClientHandler(SocketState sock)
        {
            //Change the callback method to RecieveName then ask for more data
            Console.WriteLine("Client " + (int)sock.ID + " Connected");
            sock.OnNetworkAction = ReceiveName;
            Networking.GetData(sock);
        }

        /// <summary>
        /// Callback method for after the server receives the name
        /// </summary>
        /// <param name="sock"></param>
        static void ReceiveName(SocketState sock)
        {
            //Create the players tank and assign its name and ID
            Tank mainPlayer = new Tank();
            mainPlayer.name = sock.GetData().Trim();
            mainPlayer.ID = (int)sock.ID;

            //Add the tank to both tank classes
            lock(tanks)
            {
                allTanksEver.Add(mainPlayer.ID, mainPlayer);
                tanks.Add(mainPlayer.ID, mainPlayer);
            }

            //Assign the health of the tank and spawn the tank
            mainPlayer.hitPoints = StartingHitPoints;
            tankRespawn(mainPlayer);

            //Change the callback to CommandRequestHandler and clear the sock's data
            sock.OnNetworkAction = CommandRequestHandler;
            sock.ClearData();

            //Get the startup data, serialize it, and send it to the client
            string startupData = mainPlayer.ID + "\n" + serverWorld.worldSize + "\n";
            foreach (Wall w in walls.Values)
            {
                startupData += JsonConvert.SerializeObject(w) + "\n";
            }
            startupData += GetWorldData();
            Networking.Send(sock.TheSocket, startupData);

            //Add the client to the connectedClients list and ask for more data 
            lock (connectedClients)
            {
                connectedClients.Add(sock.ID, sock);
            }
            Console.WriteLine(mainPlayer.name + " has Joined the Game");
            mainPlayer.joined = true;
            Networking.GetData(sock);
        }

        /// <summary>
        /// Gets all of the objects in the world in one serialized string
        /// </summary>
        /// <returns>String of every object</returns>
        private static string GetWorldData()
        {
            //The return data and the list of objects to be removed
            string startupData = "";
            List<object> removal = new List<object>();

            //If any tanks have disconnected, remove them, otherwise serialize them 
            //and add them to the end of the result
            lock(tanks)
            {
                foreach (Tank t in tanks.Values)
                {
                    if (t.disconnected)
                    {
                        removal.Add(t);
                        Console.WriteLine(t.name + " (ID: " + t.ID + ") Has Disconnected");
                    }
                    t.location = new Vector2D(t.location.GetX(), t.location.GetY());
                    startupData += JsonConvert.SerializeObject(t) + "\n";
                }
            }
            //If any tanks are in removal, remove them from the tanks list
            lock (tanks)
            {
                foreach (Tank t in removal)
                {
                    tanks.Remove(t.ID);
                }
            }
            //Serialize all beams in the game
            foreach (int t in beams.Keys)
            {
                string serializedBeam = JsonConvert.SerializeObject(beams[t]);
                startupData += serializedBeam + "\n";
            }
            //Serialize all powerups in the game
            foreach (int t in powerups.Keys)
            {
                string serializedPower = JsonConvert.SerializeObject(powerups[t]);
                startupData += serializedPower + "\n";
            }
            //Serialize all projectiles in the game
            foreach (int t in projectiles.Keys)
            {
                string serializedProj = JsonConvert.SerializeObject(projectiles[t]);
                startupData += serializedProj + "\n";
            }

            return startupData;
        }

        /// <summary>
        /// Callback method for any commands the player sends to the controller
        /// </summary>
        /// <param name="sock">The socketstate the connection is on</param>
        static void CommandRequestHandler(SocketState sock)
        {
            Tank main;

            //If the tank is not currently connected, don't listen to any commands from that client
            if (tanks.ContainsKey((int)sock.ID))
            {
                main = tanks[(int)sock.ID];
            }
            else
            {
                return;
            }

            // Split the command by its \n's 
            string[] commands = Regex.Split(sock.GetData(), @"(?<=[\n])");
            if(commands.Length == 1)
            {
                return;
            }

            //Deserialize the command
            ControllerObject command = JsonConvert.DeserializeObject<ControllerObject>(commands[commands.Length - 2]);

            //If there is nothing in command then ignore it
            if (command == null)
            {
                return;
            }

            //If the tank is currently alive and has made it this far, listen to its commands
            if (main.hitPoints != 0)
            {
                //Process the fire status of the tank and either fire a projectile, beam, or do nothing
                switch (command.getFireStatus())
                {
                    case "none":
                        break;

                    case "main":
                        if (main.shotTimer > 0)
                        {
                            break;
                        }
                        Projectile shot = new Projectile();
                        main.tankTotalShots++;
                        shot.orientation = main.aiming;
                        shot.location = main.location;
                        shot.tankID = main.ID;
                        shot.ID = shotCount;
                        shotCount++;
                        lock (projectiles)
                        {
                            projectiles.Add(shot.ID, shot);
                        }
                        main.shotTimer = FramesPerShot;
                        break;

                    case "alt":
                        if (main.railgunShots > 0)
                        {
                            main.railgunShots--;
                            Beam railBeam = new Beam();
                            main.tankTotalShots++;
                            railBeam.direction = main.aiming;
                            railBeam.origin = main.location;
                            railBeam.tankID = main.ID;
                            railBeam.ID = shotCount;
                            lock (beams)
                            {
                                beams.Add(railBeam.ID, railBeam);
                            }
                            shotCount++;
                        }

                        break;
                }

                //Variables used to track where the tank will be after the command is implemented
                double tankLocAfterMoveY;
                double tankLocAfterMoveX;

                //Process the tank's move command and update it's location based on the button that was pressed
                switch (command.getMoveDirection())
                {
                    case "none":
                        main.tankVelocity = new Vector2D(0, 0);
                        break;

                    case "up":
                        main.tankVelocity = new Vector2D(0, -EngineStrength);
                        tankLocAfterMoveY = main.location.GetY() + main.tankVelocity.GetY();
                        tankLocAfterMoveX = main.location.GetX() + main.tankVelocity.GetX();

                        //If the tank will hit a wall with this move, don't move it
                        if (collisionCheck(tankLocAfterMoveX, tankLocAfterMoveY))
                        {
                            main.tankVelocity = new Vector2D(0, 0);
                        }
                        //If the tank is at the edge of the world after this move, teleport it to the other side
                        else if(tankLocAfterMoveY < -serverWorld.worldSize/2)
                        {
                            tankLocAfterMoveY = serverWorld.worldSize / 2 - 5;
                            main.location = new Vector2D(main.location.GetX(), tankLocAfterMoveY);
                            main.orientation = new Vector2D(0, -1);
                        }
                        //If the tank won't hit a wall or the edge, move it
                        else
                        {
                            main.location = main.location + main.tankVelocity;
                            main.orientation = new Vector2D(0, -1);
                        }
                        break;

                    case "down":
                        main.tankVelocity = new Vector2D(0, EngineStrength);
                        tankLocAfterMoveY = main.location.GetY() + main.tankVelocity.GetY();
                        tankLocAfterMoveX = main.location.GetX() + main.tankVelocity.GetX();

                        //If the tank will hit a wall with this move, don't move it
                        if (collisionCheck(tankLocAfterMoveX, tankLocAfterMoveY))
                        {
                            main.tankVelocity = new Vector2D(0, 0);
                        }
                        //If the tank is at the edge of the world after this move, teleport it to the other side
                        else if (tankLocAfterMoveY > serverWorld.worldSize / 2)
                        {
                            tankLocAfterMoveY = -serverWorld.worldSize / 2 + 5;
                            main.location = new Vector2D(main.location.GetX(), tankLocAfterMoveY);
                            main.orientation = new Vector2D(0, 1);
                        }
                        //If the tank won't hit a wall or the edge, move it
                        else
                        {
                            main.location = main.location + main.tankVelocity;
                            main.orientation = new Vector2D(0, 1);
                        }
                        break;

                    case "left":
                        main.tankVelocity = new Vector2D(-EngineStrength, 0);
                        tankLocAfterMoveY = main.location.GetY() + main.tankVelocity.GetY();
                        tankLocAfterMoveX = main.location.GetX() + main.tankVelocity.GetX();

                        //If the tank will hit a wall with this move, don't move it
                        if (collisionCheck(tankLocAfterMoveX, tankLocAfterMoveY))
                        {
                            main.tankVelocity = new Vector2D(0, 0);
                        }
                        //If the tank is at the edge of the world after this move, teleport it to the other side
                        else if (tankLocAfterMoveX < -serverWorld.worldSize / 2)
                        {
                            tankLocAfterMoveX = serverWorld.worldSize / 2 - 5;
                            main.location = new Vector2D(tankLocAfterMoveX, main.location.GetY());
                            main.orientation = new Vector2D(-1, 0);
                        }
                        //If the tank won't hit a wall or the edge, move it
                        else
                        {
                            main.location = main.location + main.tankVelocity;
                            main.orientation = new Vector2D(-1, 0);
                        }
                        break;

                    case "right":
                        main.tankVelocity = new Vector2D(EngineStrength, 0);
                        tankLocAfterMoveY = main.location.GetY() + main.tankVelocity.GetY();
                        tankLocAfterMoveX = main.location.GetX() + main.tankVelocity.GetX();

                        //If the tank will hit a wall with this move, don't move it
                        if (collisionCheck(tankLocAfterMoveX, tankLocAfterMoveY))
                        {
                            main.tankVelocity = new Vector2D(0, 0);
                        }
                        //If the tank is at the edge of the world after this move, teleport it to the other side
                        else if (tankLocAfterMoveX > serverWorld.worldSize / 2)
                        {
                            tankLocAfterMoveX = -serverWorld.worldSize / 2 + 5;
                            main.location = new Vector2D(tankLocAfterMoveX, main.location.GetY());
                            main.orientation = new Vector2D(1, 0);
                        }
                        //If the tank won't hit a wall or the edge, move it
                        else
                        {
                            main.location = main.location + main.tankVelocity;
                            main.orientation = new Vector2D(1, 0);
                        }
                        break;
                }
                //Aim the turret the direction it should be facing
                main.aiming = command.getTurretDirection();
            }

            //Remove the data that has been processed and ask for more data
            sock.RemoveData(0, sock.GetData().Length);
            Networking.GetData(sock);
        }

        /// <summary>
        /// Checks if the two variables cross with existing wall coordinates
        /// </summary>
        /// <param name="tankLocAfterMoveX">The X value to check</param>
        /// <param name="tankLocAfterMoveY">The Y value to check</param>
        /// <returns></returns>
        private static bool collisionCheck(double tankLocAfterMoveX, double tankLocAfterMoveY)
        {
            //Check every wall in the game and see if the coordinates are within their objects
            foreach (Wall w in walls.Values)
            {
                if ((tankLocAfterMoveX >= w.totalArea[1]) && (tankLocAfterMoveX <= w.totalArea[3]) && (tankLocAfterMoveY >= w.totalArea[2]) && (tankLocAfterMoveY <= w.totalArea[4]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Spawns a tank anywhere on the map that isn't a wall or off the edge
        /// </summary>
        /// <param name="t">The tank to spawn</param>
        private static void tankRespawn(Tank t)
        {
            //Reset the tank's hit points back to their starting value and calculate a random
            //X and Y to teleport it to
            t.hitPoints = StartingHitPoints;
            Random rand = new Random();
            double x = rand.Next(-(serverWorld.worldSize / 2), serverWorld.worldSize / 2);
            double y = rand.Next(-(serverWorld.worldSize / 2), serverWorld.worldSize / 2);
            
            //If the tank won't collide with a wall with the random values, spawn it there
            if (!collisionCheck(x, y))
            {
                t.location = new Vector2D(x, y);
                return;
            }

            //If the tank will collide with a wall, pick a random wall and spawn it anywhere next to it
            int randomWall = rand.Next(0, walls.Count - 1);
            Wall randWall = walls[randomWall];

            if (randWall.totalArea[1] < 50)
            {
                x = randWall.totalArea[1] + 101;
            }
            else
            {
                x = randWall.totalArea[1] - 1;
            }

            if(randWall.totalArea[2] < 50)
            {
                y = randWall.totalArea[2] + 101;
            }
            else
            {
                y = randWall.totalArea[2] - 1;
            }
            t.location = new Vector2D(x, y);
        }

        /// <summary>
        /// Spawns a powerup anywhere on the map that isn't a wall or off the edge
        /// </summary>
        private static void powerRespawn()
        {
            //Create a new powerup object and generate two random points within the world
            Powerup p = new Powerup();
            p.ID = shotCount;
            shotCount++;
            Random rand = new Random(p.ID);
            double x = rand.Next(-(serverWorld.worldSize / 2), serverWorld.worldSize / 2);
            double y = rand.Next(-(serverWorld.worldSize / 2), serverWorld.worldSize / 2);

            //If the two random points won't collide with any walls, spawn the powerup there
            if (!collisionCheck(x, y))
            {
                p.location = new Vector2D(x, y);
                lock (powerups)
                {
                    powerups.Add(p.ID, p);
                }
                return;
            }
            //If the random coordinates will collide with a wall, pick a random wall and spawn the powerup next to it
            else
            {
                int randomWall = rand.Next(0, walls.Count - 1);

                Wall randWall = walls[randomWall];

                if(randWall.endpointOne.GetX() < randWall.endpointTwo.GetX())
                {
                    x = rand.Next((int)randWall.endpointOne.GetX(), (int)randWall.endpointTwo.GetX());
                }
                else
                {
                    x = rand.Next((int)randWall.endpointTwo.GetX(), (int)randWall.endpointOne.GetX());
                }

                if (randWall.endpointOne.GetY() < randWall.endpointTwo.GetY())
                {
                    y = rand.Next((int)randWall.endpointOne.GetY(), (int)randWall.endpointTwo.GetY());
                }
                else
                {
                    y = rand.Next((int)randWall.endpointTwo.GetY(), (int)randWall.endpointOne.GetY());
                }


                if (x < -serverWorld.worldSize/2 + 55)
                {
                    x += 71;
                }
                else if(x > serverWorld.worldSize /2 - 55)
                {
                    x -= 71;
                }
                else
                {
                    x += 71;
                }

                if (y < -serverWorld.worldSize / 2 + 55)
                {
                    y += 71;
                }
                else if (y > serverWorld.worldSize / 2 - 55)
                {
                    y -= 71;
                }
                else
                {
                    y += 71;
                }

                p.location = new Vector2D(x, y);
                lock (powerups)
                {
                    powerups.Add(p.ID, p);
                }
            }
        }

        /// <summary>
        /// Checks if anything has collided with a tank
        /// </summary>
        /// <param name="tank">The tank to check</param>
        /// <param name="other">The other object to check</param>
        /// <returns></returns>
        private static bool objectCollisionCheck(Vector2D tank, Vector2D other)
        {
            double x = tank.GetX() - other.GetX();
            double y = tank.GetY() - other.GetY();

            if (x < 30 && x > -30 && y < 30 && y > -30)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends the client the world according to where every object is
        /// </summary>
        static void Update()
        {
            //Lists to hold objects that will be removed 
            List<SocketState> disconectedClients = new List<SocketState>();
            List<Object> expiredObjects = new List<object>();

            lock (projectiles)
            {
                //For every object in projectiles, Find where it is and update its location
                foreach (Projectile p in projectiles.Values)
                {
                    //Calculate the object's new location
                    p.orientation.Normalize();
                    Vector2D projAngle = new Vector2D(p.orientation.GetX() * ProjectileSpeed, p.orientation.GetY() * ProjectileSpeed);
                    p.location = p.location + projAngle;

                    //If the projectile has collided with anything or is dead, add it to the expired objects list
                    if (collisionCheck(p.location.GetX(), p.location.GetY()))
                    {
                        p.died = true;
                        expiredObjects.Add(p);
                    }
                    if (p.location.GetX() > serverWorld.worldSize / 2 || p.location.GetY() > serverWorld.worldSize / 2 || p.location.GetX() < -serverWorld.worldSize / 2 || p.location.GetY() < -serverWorld.worldSize / 2)
                    {
                        p.died = true;
                        expiredObjects.Add(p);
                    }
                }
            }

            lock (tanks)
            {
                //For every object in Tanks, find it's status and update the object
                foreach (Tank t in tanks.Values)
                {
                    //If the time to respawn counter is still going, drop it down by one
                    if (t.timeToRespawn > 0)
                    {
                        t.timeToRespawn--;
                    }
                    //If the timer is over, respawn the tank
                    else if (t.hitPoints == 0 && !t.disconnected)
                    {
                        tankRespawn(t);
                    }

                    //If the status of the tank is dead, set it to be alive
                    if (t.died)
                    {
                        t.died = false;
                    }
                    //If the tank is disconnected however, is has died
                    if (t.disconnected)
                    {
                        t.died = true;
                        t.hitPoints = 0;
                    }

                    t.shotTimer--;

                    lock (beams)
                    {
                        //Check to see if any beams currently on screen have hit a tank
                        foreach (Beam b in beams.Values)
                        {
                            //If the beam has hit a tank, the tank is dead
                            if (Intersects(b.origin, b.direction, t.location, 30) && t.hitPoints > 0)
                            {
                                t.hitPoints = 0;
                                t.died = true;
                                t.timeToRespawn = RespawnRate;
                                tanks[b.tankID].tankShotsHit++;
                                tanks[b.tankID].score++;
                            }
                        }
                    }

                    lock (projectiles)
                    {
                        //Check to see if any projectiles on screen have hit a tank
                        foreach (Projectile p in projectiles.Values)
                        {
                            //If the projectile hit a tank then lower the hitpoints by one for that tank
                            if (p.tankID != t.ID && objectCollisionCheck(t.location, p.location) && t.hitPoints > 0 && !p.died)
                            {
                                p.died = true;
                                expiredObjects.Add(p);
                                t.hitPoints -= 1;
                                tanks[p.tankID].tankShotsHit++;

                                //If the tank has no hitpoints left, it has died
                                if (t.hitPoints == 0)
                                {
                                    t.died = true;
                                    t.timeToRespawn = RespawnRate;
                                    tanks[p.tankID].score++;
                                }
                            }
                        }
                    }

                    lock (powerups)
                    {
                        //Check to see if any powerups on screen have hit a tank
                        foreach (Powerup p in powerups.Values)
                        {
                            //If a powerup has hit a tank, increase the beam amount for that tank and destroy the powerup
                            if (objectCollisionCheck(t.location, p.location) && t.hitPoints > 0 && !p.died)
                            {
                                Random rand = new Random();
                                p.died = true;
                                p.respawnTimer = rand.Next(10, MaxPowerupDelay);
                                t.railgunShots++;
                            }
                        }
                    }
                }
            }

            int powerupRespawnCount = 0;

            lock (powerups)
            {
                //Check all powerups to see if they can respawn
                foreach (Powerup p in powerups.Values)
                {
                    //If the timer is up for a powerup to respawn, add one to the count
                    if (p.respawnTimer == 0)
                    {
                        p.respawnTimer = -1;
                        powerupRespawnCount++;
                        expiredObjects.Add(p);
                    }
                    p.respawnTimer--;
                }
            }

            lock (powerups)
            {
                //Respawn all powerups
                for (int i = 0; i < powerupRespawnCount; i++)
                {
                    powerRespawn();
                }
            }

            //Send the data to all connected clients, if the send fails, add that client to the 
            //disconnectedClients list to be removed later
            String dataToSend = GetWorldData();
            lock (connectedClients)
            {
                foreach (SocketState sock in connectedClients.Values)
                {
                    if (!Networking.Send(sock.TheSocket, dataToSend))
                    {
                        lock (disconectedClients)
                        {
                            disconectedClients.Add(sock);
                        }
                    }
                }
            }

            lock (connectedClients)
            {
                //Remove the disconnected client and destroy its tank
                foreach (SocketState sock in disconectedClients)
                {
                    connectedClients.Remove(sock.ID);
                    tanks[(int)sock.ID].died = true;
                    tanks[(int)sock.ID].hitPoints = 0;
                    tanks[(int)sock.ID].disconnected = true;
                }
            }
            
            lock(beams)
            {
                //Destroy all active beams
                beams.Clear();
            }

            lock (expiredObjects)
            {
                //Remove all expired objects from their respective lists
                foreach (Object o in expiredObjects)
                {
                    if (o is Projectile)
                    {
                        lock (projectiles)
                        {
                            Projectile p = (Projectile)o;
                            projectiles.Remove(p.ID);
                        }
                    }

                    if (o is Powerup)
                    {
                        lock (powerups)
                        {
                            Powerup p = (Powerup)o;
                            powerups.Remove(p.ID);
                        }
                    }
                }
                expiredObjects.Clear();
            }
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substitute to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }
    }
}
