// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 6 December 2019

using System;
using System.Collections.Generic;
using System.Drawing;

namespace TankWars
{
    /// <summary>
    /// Representation of a TankWars game world. This is the model element of MVC.
    /// The World is basically a container that organizes a set of Collections which
    /// in turn hold the game world's objects (e.g. tanks, walls, etc).
    /// </summary>
    public class World
    {
        private readonly string _PLAYER;                    // The name of the client's player.
        private readonly int _ID;                           // The Tank ID of the client's player.
        private readonly int _MAPSIZE;                      // The height & width of the world.
        private Dictionary<int, Wall> _walls;               // The collection of walls,
        private Dictionary<int, Tank> _tanks;               //   "            "  tanks,
        private Dictionary<int, Tank> _disconnectedTanks;   //   "            "  tanks that have left,
        private Dictionary<int, Powerup> _powerups;         //   "            "  power-ups,
        private Dictionary<int, Projectile> _projectiles;   //   "            "  projectiles, &
        private Dictionary<int, Beam> _beams;               //   "            "  beams in the game.
        private int _nextWallID = 0;                        // Tracks unique Wall IDs.
        private int _nextPowrID = 0;                        // Tracks unique Powerup IDs.
        private int _nextProjID = 0;                        // Tracks unique Projectile IDs.
        private int _nextBeamID = 0;                        // Tracks unique Beam IDs.
        private Dictionary<string, Object> _settings;       // Settings for world from file.

        /// <summary>
        /// Default constructor that calls the string constructor.
        /// </summary>
        public World() : this("")
        {
        }

        /// <summary>
        /// Simple constructor that takes in a player name.
        /// </summary>
        /// <param name="name">The player's name.</param>
        public World(string name) : this(name, 0, 1200)
        {
        }

        /// <summary>
        /// Simple constructor for constructing a world with no settings.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="size"></param>
        public World(string name, int id, int size) : this(name, id, size, new Dictionary<string,object>())
        {
        }

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="id">The player's tank ID.</param>
        /// <param name="size">The map's height & width in pixels.</param>
        public World(string name, int id, int size, Dictionary<string,Object> settings)
        {
            _PLAYER = name;
            _ID = id;
            _MAPSIZE = size;
            _walls = new Dictionary<int, Wall>();
            _tanks = new Dictionary<int, Tank>();
            _disconnectedTanks = new Dictionary<int, Tank>();
            _powerups = new Dictionary<int, Powerup>();
            _projectiles = new Dictionary<int, Projectile>();
            _beams = new Dictionary<int, Beam>();
            _settings = settings;
        }


        /// <summary>
        /// The name of this player.
        /// </summary>
        public string PlayerName
        {
            get => _PLAYER;
        }

        /// <summary>
        /// The height & width of the map in pixels.
        /// </summary>
        public int Size
        {
            get => _MAPSIZE;
        }

        /// <summary>
        /// The collection of Walls in the game.
        /// </summary>
        public Dictionary<int, Wall> Walls
        {
            get => _walls;
        }

        /// <summary>
        /// The collection of Tanks in the game.
        /// </summary>
        public Dictionary<int, Tank> Tanks
        {
            get => _tanks;
        }

        /// <summary>
        /// The collection of Tanks that joined at one point
        /// while the server was up, but are now disconnected
        /// </summary>
        public Dictionary<int, Tank> DisconnectedTanks
        {
            get => _disconnectedTanks;
        }

        /// <summary>
        /// The collection of Powerups in the game.
        /// </summary>
        public Dictionary<int, Powerup> Powerups
        {
            get => _powerups;
        }

        /// <summary>
        /// The collection of Projectiles in the game.
        /// </summary>
        public Dictionary<int, Projectile> Projectiles
        {
            get => _projectiles;
        }

        /// <summary>
        /// The collection of Beams in the game.
        /// </summary>
        public Dictionary<int, Beam> Beams
        {
            get => _beams;
        }

        /// <summary>
        /// Returns the tank controlled by the player
        /// </summary>
        public Tank GetPlayer
        {
            get
            {
                if (!_tanks.TryGetValue(_ID, out Tank tank))
                    // If no tank exists for the player ID, make a temporary one.
                    tank = new Tank(0, 0);
                return tank;
            }
        }


        /// <summary>
        /// Adds a Wall to the client's World.
        /// </summary>
        /// <param name="newWall">The new Wall (w/ unique ID) which to add.</param>
        public void AddWall(Wall newWall)
        {
            _walls[newWall.ID] = newWall;
        }


        /// <summary>
        /// Adds a Wall to the server's World.
        /// </summary>
        /// <param name="p1">Start point of the wall</param>
        /// <param name="p2">End Point of the wall</param>
        public void AddWall(Vector2D p1, Vector2D p2)
        {
            Wall newWall = new Wall(_nextWallID, p1, p2);
            _nextWallID++;
            AddWall(newWall);
        }


        /// <summary>
        /// Builds a new Tank at a random spawn point.
        /// </summary>
        /// <param name="id">Tank ID</param>
        /// <param name="name">Tank name</param>
        /// <param name="name">Tank HP</param>
        /// <returns></returns>
        public Tank GetNewTank(int id, string name, int hp)
        {
            Tank newTank = new Tank(id, SpawnPoint('T'), name, hp);
            _tanks[newTank.ID] = newTank;
            return newTank;
        }


        /// <summary>
        ///  Adds, removes, or updates a Tank in the World.
        /// </summary>
        /// <param name="tank">The Tank to update.</param>
        public void TankUpdate(Tank tank)
        {
            _tanks[tank.ID] = tank;
        }


        /// <summary>
        /// Generates a new Powerup at a random spawn point.
        /// </summary>
        /// <returns></returns>
        public Powerup GetNewPowerup()
        {
            Powerup newPow = new Powerup(_nextPowrID, SpawnPoint('P'));
            _nextPowrID++;
            _powerups[newPow.ID] = newPow;
            return newPow;
        }


        /// <summary>
        /// Adds, removes, or updates a Powerup in the World.
        /// </summary>
        /// <param name="pow">The Powerup to update.</param>
        public void PowerUpdate(Powerup pow)
        {
            _powerups[pow.ID] = pow;
        }


        /// <summary>
        /// Adds, removes, or updates a Projectile in the World.
        /// </summary>
        /// <param name="proj">The Projectile to update.</param>
        public void ProjUpdate(Projectile proj)
        {
            _projectiles[proj.ID] = proj;
        }


        /// <summary>
        /// Creates a new Projectile with given origin, direction, and owner,
        /// and adds it to the world.
        /// </summary>
        /// <param name="orig">Vector represeting origin of the projectile</param>
        /// <param name="dir">Vector2D representing direction of the projectile</param>
        /// <param name="owner">int representing id of tank that shot the projectile</param>
        /// <param name="speed">Speed of the projectile</param>
        public void NewProjectile(Vector2D orig, Vector2D dir, int owner, float speed)
        {
            Projectile newProj = new Projectile(_nextProjID, orig, dir, owner, speed);
            _nextProjID++;
            ProjUpdate(newProj);
        }


        /// <summary>
        /// Adds a Beam to the World.
        /// </summary>
        /// <param name="beam">The new Beam to add.</param>
        public void NewBeam(Beam beam)
        {
            _beams.Add(beam.ID, beam);
        }

        /// <summary>
        /// Creates a new Beam with given origin, direction, and owner,
        /// and adds it to the world.
        /// </summary>
        /// <param name="orig">Vector represeting origin of the beam</param>
        /// <param name="dir">Vector2D representing direction of the beam</param>
        /// <param name="owner">int representing id of tank that shot the beam</param>
        public void NewBeam(Vector2D orig, Vector2D dir, int owner)
        {
            Beam newBeam = new Beam(_nextBeamID, orig, dir, owner);
            _nextBeamID++;
            NewBeam(newBeam);
        }


        /// <summary>
        /// Clears all Beams from the World.
        /// </summary>
        public void ClearBeams()
        {
            _beams.Clear();
        }


        /// <summary>
        /// Generates a spawn location for Tanks and Powerups.
        /// </summary>
        /// <param name="type">Type of spawning GameObject: 'T' for Tank / 'P' for Powerup.</param>
        /// <returns>The location of the spawn point.</returns>
        public Vector2D SpawnPoint(char type)
        {
            int x, y;
            Random rand = new Random();
            int buffer = Constants.SPAWN_BUFFER;

            bool collision;
            do
            {   // Reset loop condition.
                collision = false;

                // Pick a random x-y position at least the buffer width inside the map edges.
                x = rand.Next(-_MAPSIZE / 2 + buffer, _MAPSIZE / 2 - buffer);
                y = rand.Next(-_MAPSIZE / 2 + buffer, _MAPSIZE / 2 - buffer);

                // Determine if we're spawning a Tank or Powerup.
                GameObject spawn;
                if (type == 'T' || type == 't')
                    spawn = new Tank(x, y);
                else if (type == 'P' || type == 'p')
                    spawn = new Powerup(x, y);
                else
                    throw new ArgumentException("Valid argment is either 'T' for Tank or 'P' for Powerup.");

                // Loop over game objects to verify no collisions exist at that point.
                // If collisions exist, find a new point.

                foreach (Wall wall in _walls.Values)
                {
                    if (RectangleCollision(spawn, wall, true))
                    {
                        collision = true;
                        break;
                    }
                }
                if (collision)
                    continue;

                foreach (Tank tank in _tanks.Values)
                {
                    if (tank.HP < 1 || tank.Disconnected)
                        continue;

                    if (RectangleCollision(spawn, tank, true))
                    {
                        collision = true;
                        break;
                    }
                }
                if (collision)
                    continue;

                foreach (Powerup pow in _powerups.Values)
                {
                    if (pow.PickedUp)
                        continue;

                    if (RectangleCollision(spawn, pow, true))
                    {
                        collision = true;
                        break;
                    }
                }

            } while (collision);

            return new Vector2D(x, y);
        }


        /// <summary>
        /// Collisions detection method.
        /// </summary>
        /// <param name="obj">The moving GameObject.</param>
        /// <param name="hitObj">Set to the GameObject collided with, if any.</param>
        /// <returns>True, if a collision was detected; otherwise false.</returns>
        public bool Collision(GameObject obj, out GameObject hitObj)
        {
            hitObj = null;

            if (obj is Tank)
            {
                foreach (Wall wall in _walls.Values)
                {
                    if (RectangleCollision((Tank)obj, wall, false))
                    {
                        hitObj = wall;
                        return true;
                    }
                }

                foreach (Powerup pow in _powerups.Values)
                {
                    if (pow.PickedUp)
                        continue;

                    if (RectangleCollision((Tank)obj, pow, false))
                    {
                        hitObj = pow;
                        return true;
                    }
                }
            }

            else if (obj is Projectile)
            {
                foreach (Wall wall in _walls.Values)
                {
                    if (RectangleCollision((Projectile)obj, wall, false))
                    {
                        hitObj = wall;
                        return true;
                    }
                }

                foreach (Tank tank in _tanks.Values)
                {
                    if (tank.HP < 1 || tank.ID == ((Projectile)obj).Shooter || tank.Disconnected)
                        continue;

                    if (RectangleCollision((Projectile)obj, tank, false))
                    {
                        hitObj = tank;
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Helper method uses Rectangle.IntersectsWith to detect collisions.
        /// </summary>
        /// <param name="moving">A Tank, Projectile, or Powerup that is either spawning or moving.</param>
        /// <param name="stationary">A Tank, Powerup, or Wall that might be collided with.</param>
        /// <param name="spawning">Whether or not this Tank or Powerup is spawning and should use the buffer.</param>
        /// <returns>True if a collision was detected; otherwise false.</returns>
        /// <exception cref="ArgumentException">Only certain GameObject pairs are valid arguments.</exception>
        private bool RectangleCollision(GameObject moving, GameObject stationary, bool spawning)
        {
            int buffer = 0;
            if (spawning)
                buffer = Constants.SPAWN_BUFFER;

            if (moving is Tank)
            {
                Tank movTank = (Tank)moving;
                int x = (int)movTank.Location.GetX() - ((int)_settings["TANK_SIZE"] / 2) - buffer;
                int y = (int)movTank.Location.GetY() - ((int)_settings["TANK_SIZE"] / 2) - buffer;
                int size = (int)_settings["TANK_SIZE"] + buffer;
                Rectangle moveRec = new Rectangle(x, y, size, size);
                if (stationary is Wall)
                {
                    Wall wall = (Wall)stationary;
                    if (wall.StartPoint.GetX() < wall.EndPoint.GetX())
                        x = (int)wall.StartPoint.GetX() - ((int)_settings["WALL_SIZE"]) / 2;
                    else
                        x = (int)wall.EndPoint.GetX() - (int)_settings["WALL_SIZE"] / 2;
                    if (wall.StartPoint.GetY() < wall.EndPoint.GetY())
                        y = (int)wall.StartPoint.GetY() - (int)_settings["WALL_SIZE"] / 2;
                    else
                        y = (int)wall.EndPoint.GetY() - (int)_settings["WALL_SIZE"] / 2;
                    int width = Math.Abs((int)(wall.StartPoint.GetX() - wall.EndPoint.GetX())) + (int)_settings["WALL_SIZE"];
                    int height = Math.Abs((int)(wall.StartPoint.GetY() - wall.EndPoint.GetY())) + (int)_settings["WALL_SIZE"];
                    Rectangle wallRec = new Rectangle(x, y, width, height);
                    if (moveRec.IntersectsWith(wallRec))
                        return true;
                    return false;
                }
                if (stationary is Tank)
                {
                    Tank staTank = (Tank)stationary;
                    x = (int)staTank.Location.GetX() - ((int)_settings["TANK_SIZE"] / 2);
                    y = (int)staTank.Location.GetY() - ((int)_settings["TANK_SIZE"] / 2);
                    size = (int)_settings["TANK_SIZE"];
                    Rectangle staRec = new Rectangle(x, y, size, size);
                    if (moveRec.IntersectsWith(staRec))
                        return true;
                    return false;
                }
                if (stationary is Powerup)
                {
                    Powerup pow = (Powerup)stationary;
                    double distance = (movTank.Location - pow.Location).Length();
                    if (distance < (int)_settings["TANK_SIZE"] / 2 + buffer)
                        return true;
                    return false;
                }
                throw new ArgumentException("If 'moving' is Tank, 'stationary' must be Wall, Tank, or Powerup.");
            }

            if (moving is Projectile)
            {
                Projectile proj = (Projectile)moving;
                if (stationary is Tank)
                {
                    Tank staTank = (Tank)stationary;
                    double distance = (proj.Location - staTank.Location).Length();
                    if (distance < (int)_settings["TANK_SIZE"] / 2)
                        return true;
                    return false;
                }
                if (stationary is Wall)
                {
                    int x = (int)proj.Location.GetX() - (Constants.PROJ_SIZE / 2);
                    int y = (int)proj.Location.GetY() - (Constants.PROJ_SIZE / 2);
                    int size = Constants.PROJ_SIZE;
                    Rectangle projRec = new Rectangle(x, y, size, size);

                    Wall wall = (Wall)stationary;
                    if (wall.StartPoint.GetX() < wall.EndPoint.GetX())
                        x = (int)wall.StartPoint.GetX() - (int)_settings["WALL_SIZE"] / 2;
                    else
                        x = (int)wall.EndPoint.GetX() - (int)_settings["WALL_SIZE"] / 2;
                    if (wall.StartPoint.GetY() < wall.EndPoint.GetY())
                        y = (int)wall.StartPoint.GetY() - (int)_settings["WALL_SIZE"] / 2;
                    else
                        y = (int)wall.EndPoint.GetY() - (int)_settings["WALL_SIZE"] / 2;
                    int width = Math.Abs((int)(wall.StartPoint.GetX() - wall.EndPoint.GetX())) + (int)_settings["WALL_SIZE"];
                    int height = Math.Abs((int)(wall.StartPoint.GetY() - wall.EndPoint.GetY())) + (int)_settings["WALL_SIZE"];
                    Rectangle wallRec = new Rectangle(x, y, width, height);
                    if (projRec.IntersectsWith(wallRec))
                        return true;
                    return false;
                }
                throw new ArgumentException("If 'moving' is Projectile, 'stationary' must be Tank or Wall");
            }

            if (moving is Powerup)
            {
                Powerup movPow = (Powerup)moving;
                if (stationary is Tank)
                {
                    Tank staTank = (Tank)stationary;
                    double distance = (movPow.Location - staTank.Location).Length();
                    if (distance < (int)_settings["TANK_SIZE"] / 2 + buffer)
                        return true;
                    return false;
                }
                if (stationary is Powerup)
                {
                    Powerup staPow = (Powerup)stationary;
                    double distance = (movPow.Location - staPow.Location).Length();
                    if (distance < Constants.POWR_SIZE / 2 + buffer)
                        return true;
                    return false;
                }
                if (stationary is Wall)
                {
                    int x = (int)movPow.Location.GetX() - (Constants.POWR_SIZE / 2) - buffer;
                    int y = (int)movPow.Location.GetY() - (Constants.POWR_SIZE / 2) - buffer;
                    int size = Constants.POWR_SIZE;
                    Rectangle powRec = new Rectangle(x, y, size, size);

                    Wall wall = (Wall)stationary;
                    if (wall.StartPoint.GetX() < wall.EndPoint.GetX())
                        x = (int)wall.StartPoint.GetX() - (int)_settings["WALL_SIZE"] / 2;
                    else
                        x = (int)wall.EndPoint.GetX() - (int)_settings["WALL_SIZE"] / 2;
                    if (wall.StartPoint.GetY() < wall.EndPoint.GetY())
                        y = (int)wall.StartPoint.GetY() - (int)_settings["WALL_SIZE"] / 2;
                    else
                        y = (int)wall.EndPoint.GetY() - (int)_settings["WALL_SIZE"] / 2;
                    int width = Math.Abs((int)(wall.StartPoint.GetX() - wall.EndPoint.GetX())) + (int)_settings["WALL_SIZE"];
                    int height = Math.Abs((int)(wall.StartPoint.GetY() - wall.EndPoint.GetY())) + (int)_settings["WALL_SIZE"];
                    Rectangle wallRec = new Rectangle(x, y, width, height);
                    if (powRec.IntersectsWith(wallRec))
                        return true;
                    return false;
                }
                throw new ArgumentException("If 'moving' is Prowerup, 'stationary' must be Tank, Powerup, or Wall.");
            }
            throw new ArgumentException("The 1st argument, 'moving', must be a Tank, Projectile, or Powerup.");
        }
    }
}
