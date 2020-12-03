using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TankWars;

namespace DrawPan
{
    public class DrawingPanel : Panel
    {

        private World theWorld;

        //Load the tank sprites that will be required
        Image blueTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\BlueTank.png");
        Image darkTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\DarkTank.png");
        Image greenTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\GreenTank.png");
        Image lightGreenTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\LightGreenTank.png");
        Image orangeTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\OrangeTank.png");
        Image purpleTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\PurpleTank.png");
        Image redTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\RedTank.png");
        Image yellowTank = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\YellowTank.png");

        //Load the turret sprites that will be required
        Image blueTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\BlueTurret.png");
        Image darkTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\DarkTurret.png");
        Image greenTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\GreenTurret.png");
        Image lightGreenTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\LightGreenTurret.png");
        Image orangeTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\OrangeTurret.png");
        Image purpleTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\PurpleTurret.png");
        Image redTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\RedTurret.png");
        Image yellowTurret = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\YellowTurret.png");

        //Load the wall and background sprites that will be requried
        Image wallSprite = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\WallSprite.png");
        Image background = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\Background.png");

        //Load the shot sprites that will be required
        Image blueShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-blue.png");
        Image darkShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-grey.png");
        Image greenShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-green.png");
        Image lightGreenShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-white.png");
        Image orangeShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-brown.png");
        Image purpleShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-violet.png");
        Image redShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-red.png");
        Image yellowShot = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\shot-yellow.png");

        //Load the explosions that will be required
        Image explosion1and3 = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\Explosion1and3.png");
        Image explosion2 = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\Explosion2.png");
        Image explosion4 = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\Explosion4.png");
        Image explosion5 = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\Explosion5.png");
        Image explosion6 = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\Explosion6.png");
        Image explosion7 = Image.FromFile(@"..\..\..\Resources\Resources\Sprites\Explosion7.png");

        /// <summary>
        /// Constructor for the drawing panel
        /// </summary>
        public DrawingPanel(World w)
        {
            DoubleBuffered = true;
            theWorld = w;
        }

        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        //The drawer for the tank that will be used as the ObjectDelegate
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = Constants.tanks[(int)o];
            switch (t.color)
            {
                case "blue":
                    e.Graphics.DrawImage(blueTank, -30, -30, 60, 60);
                    break;

                case "dark":
                    e.Graphics.DrawImage(darkTank, -30, -30, 60, 60);
                    break;

                case "lightGreen":
                    e.Graphics.DrawImage(lightGreenTank, -30, -30, 60, 60);
                    break;

                case "green":
                    e.Graphics.DrawImage(greenTank, -30, -30, 60, 60);
                    break;

                case "orange":
                    e.Graphics.DrawImage(orangeTank, -30, -30, 60, 60);
                    break;

                case "purple":
                    e.Graphics.DrawImage(purpleTank, -30, -30, 60, 60);
                    break;

                case "red":
                    e.Graphics.DrawImage(redTank, -30, -30, 60, 60);
                    break;

                case "yellow":
                    e.Graphics.DrawImage(yellowTank, -30, -30, 60, 60);
                    break;
            }
        }

        //The drawer for the GUI (text and healthbar) that will be used as the ObjectDelegate
        private void GUIDrawer(object o, PaintEventArgs e)
        {
            Tank t = Constants.tanks[(int)o];

            e.Graphics.DrawString(t.name + ": " + t.score, DefaultFont, Brushes.White, -25, 32);

            if(t.hitPoints == 3)
            {
                e.Graphics.FillRectangle(Brushes.Green, -21, -42, 42, 8);
            }
            else if(t.hitPoints == 2)
            {
                e.Graphics.FillRectangle(Brushes.GreenYellow, -21, -42, 28, 8);
            }
            else if(t.hitPoints == 1)
            {
                e.Graphics.FillRectangle(Brushes.DarkRed, -21, -42, 14, 8);
            }
        }

        //The drawer for the turret that will be used as the ObjectDelegate
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = Constants.tanks[(int)o];
            switch (t.color)
            {
                case "blue":
                    e.Graphics.DrawImage(blueTurret, -25, -25, 50, 50);
                    break;

                case "dark":
                    e.Graphics.DrawImage(darkTurret, -25, -25, 50, 50);
                    break;

                case "lightGreen":
                    e.Graphics.DrawImage(lightGreenTurret, -25, -25, 50, 50);
                    break;

                case "green":
                    e.Graphics.DrawImage(greenTurret, -25, -25, 50, 50);
                    break;

                case "orange":
                    e.Graphics.DrawImage(orangeTurret, -25, -25, 50, 50);
                    break;

                case "purple":
                    e.Graphics.DrawImage(purpleTurret, -25, -25, 50, 50);
                    break;

                case "red":
                    e.Graphics.DrawImage(redTurret, -25, -25, 50, 50);
                    break;

                case "yellow":
                    e.Graphics.DrawImage(yellowTurret, -25, -25, 50, 50);
                    break;
            }
        }

        //The drawer for the projectile that will be used as the ObjectDelegate
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile t = Constants.projectiles[(int)o];

            switch (Constants.tanks[t.tankID].color)
            {
                case "blue":
                    e.Graphics.DrawImage(blueShot, -15, -15, 30, 30);
                    break;

                case "dark":
                    e.Graphics.DrawImage(darkShot, -15, -15, 30, 30);
                    break;

                case "lightGreen":
                    e.Graphics.DrawImage(lightGreenShot, -15, -15, 30, 30);
                    break;

                case "green":
                    e.Graphics.DrawImage(greenShot, -15, -15, 30, 30);
                    break;

                case "orange":
                    e.Graphics.DrawImage(orangeShot, -15, -15, 30, 30);
                    break;

                case "purple":
                    e.Graphics.DrawImage(purpleShot, -15, -15, 30, 30);
                    break;

                case "red":
                    e.Graphics.DrawImage(redShot, -15, -15, 30, 30);
                    break;

                case "yellow":
                    e.Graphics.DrawImage(yellowShot, -15, -15, 30, 30);
                    break;
            }
        }

        //The drawer for the beam that will be used as the ObjectDelegate
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beam b = Constants.beams[(int)o];

            Pen whitePen = Pens.White;
            Point originPointCenter = new Point(0, 0);
            Point originPointSide1 = new Point(1, 0);
            Point originPointSide2 = new Point(-1, 0);

            Point directionPointSide1 = new Point(1, -1600);
            Point directionPointCenter = new Point(0, -1600);
            Point directionPointSide2 = new Point(-1, -1600);
            if(b.timeout > 0)
            {
                if(b.timeout > 16)
                {
                    e.Graphics.DrawLine(whitePen, originPointSide1, directionPointSide1);
                    e.Graphics.DrawLine(whitePen, originPointSide2, directionPointSide2);
                }
                e.Graphics.DrawLine(whitePen, originPointCenter, directionPointCenter);
            }
            b.timeout--;
        }

        //The drawer for the explosion that will be used as the ObjectDelegate
        private void ExplosionDrawer(object o, PaintEventArgs e)
        {
            Explosion t = Constants.explosions[(int)o];

            if (t.timeout > 28)
            {
                e.Graphics.DrawImage(explosion1and3, -60, -60, 120, 120);
            }
            else if (t.timeout <= 28 && t.timeout > 24)
            {
                e.Graphics.DrawImage(explosion2, -60, -60, 120, 120);
            }
            else if (t.timeout <= 24 && t.timeout > 20)
            {
                e.Graphics.DrawImage(explosion1and3, -60, -60, 120, 120);
            }
            else if (t.timeout <= 20 && t.timeout > 16)
            {
                e.Graphics.DrawImage(explosion4, -60, -60, 120, 120);
            }
            else if (t.timeout <= 16 && t.timeout > 12)
            {
                e.Graphics.DrawImage(explosion5, -60, -60, 120, 120);
            }
            else if (t.timeout <= 12 && t.timeout > 8)
            {
                e.Graphics.DrawImage(explosion6, -60, -60, 120, 120);
            }
            else if (t.timeout <= 8 && t.timeout > 4)
            {
                e.Graphics.DrawImage(explosion7, -60, -60, 120, 120);
            }

            t.timeout--;
        }

        //The drawer for the wall that will be used as the ObjectDelegate
        private void WallDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(wallSprite, -25, -25,50,50);
        }

        //The drawer for the powerup that will be used as the ObjectDelegate
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            Brush orangeBrush = Brushes.Orange;
            e.Graphics.FillEllipse(orangeBrush, -8, -8, 16, 16);

            Brush greenBrush = Brushes.Green;
            e.Graphics.FillEllipse(greenBrush, -5, -5, 10, 10);
        }

        /// <summary>
        /// The method that will be used to draw everything in the frame
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Constants.tanks.ContainsKey(Constants.playerID))
            {
                return;
            }
            //Lock the world while everything is drawing
            lock (theWorld)
            {
                Tank theTank = Constants.tanks[Constants.playerID];

                double playerX = theTank.location.GetX();
                double playerY = theTank.location.GetY();

                // Before anything else draw the background
                e.Graphics.DrawImage(background, (int)(-playerX - 200), (int)(-playerY - 200), Constants.worldSize, Constants.worldSize);

                // calculate view/world size ratio
                double ratio = (double)800 / (double)Constants.worldSize;
                int halfSizeScaled = (int)(Constants.worldSize / 2.0 * ratio);

                double inverseTranslateX = -WorldSpaceToImageSpace(Constants.worldSize, playerX) + halfSizeScaled;
                double inverseTranslateY = -WorldSpaceToImageSpace(Constants.worldSize, playerY) + halfSizeScaled;

                e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

                //Draw every tank in the constants tanks dictionary
                foreach (int t in Constants.tanks.Keys)
                {
                    Tank currTank = Constants.tanks[t];

                    //If the tank is disconnected, draw an explosion
                    if (currTank.disconnected == true)
                    {
                        if (!Constants.explosions.ContainsKey(t))
                        {
                            Constants.explosions.Add(t, new Explosion(currTank.location, t));
                        }
                        continue;
                    }
                    // If the tank has zero hit points, draw an explosion
                    if (currTank.hitPoints == 0)
                    {
                        if(!Constants.explosions.ContainsKey(t))
                        {
                            Constants.explosions.Add(t, new Explosion(currTank.location, t));
                        }
                        continue;
                    }

                    //Draw the tank while ensuring the turret ispointing a valid direction
                    DrawObjectWithTransform(e, t, Constants.worldSize, currTank.location.GetX(), currTank.location.GetY(), currTank.orientation.ToAngle(), TankDrawer);
                    if(!float.IsNaN(currTank.aiming.ToAngle()) && !float.IsInfinity(currTank.aiming.ToAngle()))
                    {
                        DrawObjectWithTransform(e, t, Constants.worldSize, currTank.location.GetX(), currTank.location.GetY(), currTank.aiming.ToAngle(), TurretDrawer);
                    }
                    else
                    {
                        DrawObjectWithTransform(e, t, Constants.worldSize, currTank.location.GetX(), currTank.location.GetY(), 0, TurretDrawer);
                    }
                    DrawObjectWithTransform(e, t, Constants.worldSize, currTank.location.GetX(), currTank.location.GetY(), 0, GUIDrawer);
                }

                //A list to hold all repeated keys
                List<int> keys = new List<int>();

                //Draw all the explosions
                foreach (int t in Constants.explosions.Keys)
                {
                    Explosion currEx = Constants.explosions[t];
                    if (currEx.timeout < 4)
                    {
                        keys.Add(t);
                        continue;
                    }
                    DrawObjectWithTransform(e, t, Constants.worldSize, currEx.location.GetX(), currEx.location.GetY(), 0, ExplosionDrawer);
                }
                //Clear all the explosions
                foreach (int t in keys)
                {
                    if(Constants.tanks[t].hitPoints > 0)
                    {
                        Constants.explosions.Remove(t);
                    }
                }
                keys.Clear();

                //Draw all the projectiles
                foreach (int t in Constants.projectiles.Keys)
                {
                    Projectile currProjectile = Constants.projectiles[t];
                    if (currProjectile.died)
                    {
                        keys.Add(t);
                        continue;
                    }
                    DrawObjectWithTransform(e, t, Constants.worldSize, currProjectile.location.GetX(), currProjectile.location.GetY(), currProjectile.orientation.ToAngle(), ProjectileDrawer);
                }
                //Clear all the projectiles
                foreach(int t in keys)
                {
                    Constants.projectiles.Remove(t);
                }
                keys.Clear();
                
                // Draw all the powerups
                foreach (int t in Constants.powerups.Keys)
                {
                    Powerup currPowerup = Constants.powerups[t];
                    if (currPowerup.died)
                    {
                        keys.Add(t);
                        continue;
                    }
                    DrawObjectWithTransform(e, t, Constants.worldSize, currPowerup.location.GetX(), currPowerup.location.GetY(), 0, PowerupDrawer);
                }
                // Clear all the powerups
                foreach (int t in keys)
                {
                    Constants.powerups.Remove(t);
                }
                keys.Clear();

                //Draw all the beams
                foreach (int t in Constants.beams.Keys)
                {
                    Beam currbeam = Constants.beams[t];
                    DrawObjectWithTransform(e, t, Constants.worldSize, currbeam.origin.GetX(), currbeam.origin.GetY(), currbeam.direction.ToAngle(), BeamDrawer);
                }

                //Draw all the walls
                foreach (int t in Constants.walls.Keys)
                {
                    Wall currWall = Constants.walls[t];
                    if (currWall.endpointOne.GetX() == currWall.endpointTwo.GetX())
                    {
                        if (currWall.endpointOne.GetY() < currWall.endpointTwo.GetY())
                        {
                            for (double i = currWall.endpointOne.GetY(); i <= currWall.endpointTwo.GetY(); i += 50)
                            {
                                DrawObjectWithTransform(e, t, Constants.worldSize, currWall.endpointOne.GetX(), i, 0, WallDrawer);
                            }
                        }
                        else
                        {
                            for (double i = currWall.endpointOne.GetY(); i >= currWall.endpointTwo.GetY(); i -= 50)
                            {
                                DrawObjectWithTransform(e, t, Constants.worldSize, currWall.endpointOne.GetX(), i, 0, WallDrawer);
                            }
                        }

                    }
                    else
                    {
                        if (currWall.endpointOne.GetX() < currWall.endpointTwo.GetX())
                        {
                            for (double i = currWall.endpointOne.GetX(); i <= currWall.endpointTwo.GetX(); i += 50)
                            {
                                DrawObjectWithTransform(e, t, Constants.worldSize, i, currWall.endpointOne.GetY(), 0, WallDrawer);
                            }
                        }
                        else
                        {
                            for (double i = currWall.endpointOne.GetX(); i >= currWall.endpointTwo.GetX(); i -= 50)
                            {
                                DrawObjectWithTransform(e, t, Constants.worldSize, i, currWall.endpointOne.GetY(), 0, WallDrawer);
                            }
                        }
                    }
                }
            }
        }
    }
}
