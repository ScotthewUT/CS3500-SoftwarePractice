// AUTHORS: Scott Crowley (u1178178), David Gillespie (u0720569), & CS3500 Faculty
// VERSION: 25 November 2019

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TankWars
{
    public class DrawingPanel : Panel
    {
        private World _world;
        private ClientController _controller;
        private Dictionary<Beam, long> _beamDrawTimer;

        public DrawingPanel(World world, ClientController controller)
        {
            DoubleBuffered = true;
            _world = world;
            _controller = controller;
            _beamDrawTimer = new Dictionary<Beam, long>();
        }


        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size.</param>
        /// <param name="w">The worldspace coordinat.</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }


        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

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
        /// Delegate method for drawing tank bodies with appropriate color.
        /// </summary>
        /// <param name="o">The Tank (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics.</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)
            Rectangle tankRec = new Rectangle(-(Constants.TANK_SIZE / 2), -(Constants.TANK_SIZE / 2),
                                                           Constants.TANK_SIZE, Constants.TANK_SIZE);

            // Divide the 8 available colors among the players.
            string imageName = "tank" + ((Tank)o).ID % 8;
            Image tankImage = Constants.SPRITES[imageName];

            e.Graphics.DrawImage(tankImage, tankRec);
        }

        /// <summary>
        /// Delegate method for drawing Health Bars with appropriate color.
        /// </summary>
        /// <param name="o">The Tank (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics.</param>
        private void HealthDrawer(object o, PaintEventArgs e)
        {
            int HP = ((Tank)o).HP;

            int healthBarHeight = 10;
            int healthBarPadding = 10;
            int healthBarWidth = Constants.TANK_SIZE - (healthBarPadding * 2);

            Rectangle healthRec = new Rectangle(-(Constants.TANK_SIZE / 2) + healthBarPadding,
                                     -(Constants.TANK_SIZE / 2) - (healthBarHeight + healthBarPadding),
                                                           HP * (healthBarWidth / 3), healthBarHeight);
            SolidBrush brush;
            switch (HP)
            {
                case (1):
                    brush = new SolidBrush(Color.Red);
                    break;
                case (2):
                    brush = new SolidBrush(Color.Yellow);
                    break;
                default:
                    brush = new SolidBrush(Color.Green);
                    break;
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.FillRectangle(brush, healthRec);
        }

        /// <summary>
        /// Delegate method for drawing Player Stats with appropriate color.
        /// </summary>
        /// <param name="o">The Tank (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics.</param>
        private void StatsDrawer(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int statsPadding = 10; // Offset stats slightly beyond tank.
            int statsY = (Constants.TANK_SIZE / 2) + (statsPadding);

            SolidBrush drawBrush = new SolidBrush(Color.White);
            Font drawFont = new Font("Arial", 12);
            string stats = tank.Name + ":" + tank.Score;
            SizeF statsSize = e.Graphics.MeasureString(stats, drawFont);
            e.Graphics.DrawString(stats, drawFont, drawBrush, -statsSize.Width / 2, statsY);
        }


        /// <summary>
        /// Delegate method for drawing tank turrents with appropriate color.
        /// </summary>
        /// <param name="o">The Tank (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics.</param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle turrRec = new Rectangle(-(Constants.TURR_SIZE / 2), -(Constants.TURR_SIZE / 2),
                                                           Constants.TURR_SIZE, Constants.TURR_SIZE);

            string imageName = "turret" + ((Tank)o).ID % 8;
            Image turrImage = Constants.SPRITES[imageName];
            e.Graphics.DrawImage(turrImage, turrRec);
        }


        /// <summary>
        /// Delegate method for drawing tank turrents with appropriate color.
        /// </summary>
        /// <param name="o">The Tank (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics.</param>
        private void SkullDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle skullRec = new Rectangle(-(Constants.TANK_SIZE / 2), -(Constants.TANK_SIZE / 2),
                                                            Constants.TANK_SIZE, Constants.TANK_SIZE);

            e.Graphics.DrawImage(Constants.SPRITES["skull"], skullRec);
        }


        /// <summary>
        /// Delegate method for drawing individual wall segments.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void WallSegmentDrawer(object o, PaintEventArgs e)
        {
            int width = Constants.WALL_SIZE;
            int height = Constants.WALL_SIZE;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle wallSegment = new Rectangle(-(width / 2), -(height / 2), width, height);

            e.Graphics.DrawImage(Constants.SPRITES["wall"], wallSegment);
        }


        /// <summary>
        /// Delegate method for drawing Walls.
        /// </summary>
        /// <param name="o">The Wall (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall wall = o as Wall;

            int width = Constants.WALL_SIZE;
            int height = Constants.WALL_SIZE;

            Vector2D diff = wall.StartPoint - wall.EndPoint;
            int numSegmentsX = Math.Abs((int) diff.GetX() / width);
            int numSegmentsY = Math.Abs((int)diff.GetY() / height);

            IEnumerable<int> placements;
            int  directionModifierX;
            int directionModifierY;

            // Get number of wall segments based on if the wall is vertical or horizontal
            if (numSegmentsX == 0)
            { // Vertical Wall
                placements = Enumerable.Range(0, numSegmentsY+1) // Generate range of segements
                    .Select(y => (int)(y * Math.Sign(diff.GetY()))); // Extract direction from diff
                directionModifierX = 0;
                directionModifierY = 1;
            }
            else
            { // Horizontal Wall
                placements = Enumerable.Range(0, numSegmentsX+1) // Generate range of segements
                    .Select(x => (int)(x * Math.Sign(diff.GetX()))); // Extract direction from diff
                directionModifierX = 1;
                directionModifierY = 0;
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Loop over each segment offsetting it by the segment number multiplied by the width of the segment.
            foreach (int segment in placements)
            {
                DrawObjectWithTransform(e, wall, this.Size.Width, (wall.StartPoint.GetX() -
                                      directionModifierX * segment * width), // Segment number offset from start
                                      (wall.StartPoint.GetY() -
                                      directionModifierY * segment * width), // Segment number offset from start
                                                      0, WallSegmentDrawer);
            }
        }


        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void PowerupDrawer(object o, PaintEventArgs e)
        {
            int width = Constants.POWR_SIZE;
            int height = Constants.POWR_SIZE;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (SolidBrush yellowBrush = new SolidBrush(Color.Yellow))
            using (SolidBrush greenBrush = new SolidBrush(Color.DarkGreen))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle outerCircle = new Rectangle(-(width / 2), -(height / 2), width, height);
                Rectangle innerCircle = new Rectangle(-((width/2)/ 2), -((height/2)/ 2), width/2, height/2);

                e.Graphics.FillEllipse(yellowBrush, outerCircle);
                e.Graphics.FillEllipse(greenBrush, innerCircle);
            }
        }

        /// <summary>
        /// Delegate method for drawing Projectiles with appropriate color.
        /// </summary>
        /// <param name="o">The Projectile (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics.</param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle projRec = new Rectangle(-(Constants.PROJ_SIZE / 2), -(Constants.PROJ_SIZE / 2),
                                                           Constants.PROJ_SIZE, Constants.PROJ_SIZE);

            string imageName = "shot" + ((Projectile)o).Shooter % 8;
            Image turrImage = Constants.SPRITES[imageName];
            e.Graphics.DrawImage(turrImage, projRec);
        }


        /// <summary>
        /// Delegate method for drawing Beams with appropriate color.
        /// </summary>
        /// <param name="o">The Beam (as an Object) to draw.</param>
        /// <param name="e">The PaintEventArgs to access the graphics.</param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            // The longest the beam needs to be is the diagonal of the map.
            int beam_length = (int) Math.Sqrt(2 * (_world.Size * _world.Size));

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle beamRec = new Rectangle(-(Constants.BEAM_SIZE / 2) , -(Constants.BEAM_SIZE / 2)
                                                      + 30, Constants.BEAM_SIZE, Constants.VIEW_SIZE);

            string imageName = "beam" + ((Beam)o).Shooter % 8;
            Image beamImage = Constants.SPRITES[imageName];
            e.Graphics.DrawImage(beamImage, beamRec);
        }


        /// <summary>
        /// This method is invoked when the DrawingPanel needs to be redrawn.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.DarkOliveGreen),
                                                                        0, 0, _world.Size, _world.Size);
            _world = _controller.GetWorld();

            int offset = (Constants.VIEW_SIZE - _world.Size) / 2;
            double playerX = _world.GetPlayer.Location.GetX() + offset; // Player's world-space x-coordinate.
            double playerY = _world.GetPlayer.Location.GetY() + offset; // Player's world-space y-coordinate.

            // Calculate view/world size ratio.
            double ratio = (double)Constants.VIEW_SIZE / (double)_world.Size;
            int halfSizeScaled = (int)(_world.Size / 2.0 * ratio);

            double inverseTranslateX = -WorldSpaceToImageSpace(_world.Size, playerX) + halfSizeScaled;
            double inverseTranslateY = -WorldSpaceToImageSpace(_world.Size, playerY) + halfSizeScaled;

            e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);
            
            if (_controller.Connected)
                e.Graphics.DrawImage(Constants.SPRITES["background"], offset, offset, _world.Size, _world.Size);

            
            lock (_world)
            {   // Draw the walls.
                foreach (Wall wall in _world.Walls.Values)
                {
                    WallDrawer(wall, e);
                }
            }
            
            lock (_world)
            {   // Draw the tanks.
                foreach (Tank tank in _world.Tanks.Values)
                {
                    if (tank.HP > 0)
                    {   // Only draw the live tanks.
                        DrawObjectWithTransform(e, tank, this.Size.Width, tank.Location.GetX(), tank.Location.GetY(), tank.Orientation.ToAngle(), TankDrawer);
                        DrawObjectWithTransform(e, tank, this.Size.Width, tank.Location.GetX(), tank.Location.GetY(), tank.Aiming.ToAngle(), TurretDrawer);
                        DrawObjectWithTransform(e, tank, this.Size.Width, tank.Location.GetX(), tank.Location.GetY(), 0, HealthDrawer);
                        DrawObjectWithTransform(e, tank, this.Size.Width, tank.Location.GetX(), tank.Location.GetY(), 0, StatsDrawer);
                    }
                    else if (tank.HP == 0 && !tank.Disconnected)
                        // Let players know tank is dead.
                        DrawObjectWithTransform(e, tank, this.Size.Width, tank.Location.GetX(), tank.Location.GetY(), 0, SkullDrawer);
                }
            }

            lock (_world)
            {   // Draw the projectiles.
                foreach (Projectile projectile in _world.Projectiles.Values)
                {   // Only draw active projectiles.
                    if (projectile.Dead == false)
                        DrawObjectWithTransform(e, projectile, this.Size.Width, projectile.Location.GetX(), projectile.Location.GetY(), projectile.Orientation.ToAngle(), ProjectileDrawer);
                }
            }

            lock (_world)
            {   // Draw the powerups
                foreach (Powerup pow in _world.Powerups.Values)
                {   // Only draw available powerups.
                    if (pow.PickedUp == false) 
                        DrawObjectWithTransform(e, pow, this.Size.Width, pow.Location.GetX(), pow.Location.GetY(), 0, PowerupDrawer);
                }
            }

            lock (_world)
            {   // Move the new Beams to _beamDrawTimer for drawing.
                foreach (Beam beam in _world.Beams.Values)
                {
                    _beamDrawTimer.Add(beam, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                }
                _world.ClearBeams();
            }

            // Draw the Beams.
            List<Beam> finishedBeams = new List<Beam>();
            foreach (Beam beam in _beamDrawTimer.Keys)
            {
                DrawObjectWithTransform(e, beam, this.Size.Width, beam.Origin.GetX(), beam.Origin.GetY(), beam.Direction.ToAngle()+180, BeamDrawer);

                long elapsed = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _beamDrawTimer[beam];
                if (elapsed > Constants.BEAM_LIFE)
                    finishedBeams.Add(beam);
            }
            // Remove the fully drawn beams from _beamDrawTimer.
            foreach (Beam beam in finishedBeams)
            {
                _beamDrawTimer.Remove(beam);
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }
    }
}