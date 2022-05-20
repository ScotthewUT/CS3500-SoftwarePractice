// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 6 December 2019

using Newtonsoft.Json;
using System;

namespace TankWars
{
    /// <summary>
    /// GameObject base class from which the following subclasses inherit:
    /// Tank, Wall, Powerup, Projectile, & Beam.
    /// </summary>
    public class GameObject
    {
        private int _ID;
        private Vector2D _location;

        /// <summary>
        /// Default constructor for GameObject. Creates a game object with empty id and location.
        /// </summary>
        public GameObject() : this(0, new Vector2D(0, 0))
        {
        }

        public GameObject(int id, Vector2D loc)
        {
            _ID = id;
            _location = loc;
        }

        public int ID
        {
            get { return _ID; }
            protected set { _ID = value; }
        }

        public Vector2D Location
        {
            get { return _location; }
            set { _location = value; }
        }
    }


    /// <summary>
    /// Represents a Tank object in the game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank : GameObject
    {
        [JsonProperty(PropertyName = "tank")]
        private int _ID { get { return this.ID; } set { this.ID = value; } }

        [JsonProperty(PropertyName = "loc")]
        private Vector2D _location { get { return this.Location; } set { this.Location = value; } }

        [JsonProperty(PropertyName = "bdir")]
        private Vector2D _orientation = new Vector2D(0, -1);

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D _aiming = new Vector2D(0, -1);

        [JsonProperty(PropertyName = "name")]
        private string _name;

        [JsonProperty(PropertyName = "hp")]
        private int _hitPoints;

        [JsonProperty(PropertyName = "score")]
        private int _score = 0;

        [JsonProperty(PropertyName = "died")]
        private bool _died = false;

        [JsonProperty(PropertyName = "dc")]
        private bool _disconnected = false;

        [JsonProperty(PropertyName = "join")]
        private bool _joined = false;

        private Vector2D _velocity = new Vector2D(0, 0);
        private int _beamCount = 0;
        private int _shots = 0;
        private int _hits = 0;
        private int _frameCount = 0;

        /// <summary>
        /// Empty default constructor required for JSON.
        /// </summary>
        public Tank()
        {
        }

        /// <summary>
        /// Overloaded constructor for creating a temporary tank during startup.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public Tank(int posX, int posY)
        {
            _location = new Vector2D(posX, posY);
        }

        /// <summary>
        /// Primary constructor for creating new Tanks.
        /// </summary>
        /// <param name="id">Unique ID</param>
        /// <param name="loc">Location</param>
        /// <param name="name">Player's Name</param>
        public Tank(int id, Vector2D loc, string name, int hp)
        {
            _ID = id;
            _location = loc;
            _name = name;
            _joined = true;
            _hitPoints = hp;
        }

        public Vector2D Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        public Vector2D Aiming
        {
            get { return _aiming; }
            set { _aiming = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int HP
        {
            get { return _hitPoints; }
            set { _hitPoints = value; }
        }

        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public bool Died
        {
            get { return _died; }
            set { _died = value; }
        }

        public bool Disconnected
        {
            get { return _disconnected; }
            set { _disconnected = value; }
        }

        public bool Joined
        {
            get { return _joined; }
            set { _joined = value; }
        }

        public Vector2D Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public int BeamCount
        {
            get { return _beamCount; }
            set { _beamCount = value; }
        }

        public int Shots
        {
            get { return _shots; }
            set { _shots = value; }
        }

        public int Hits
        {
            get { return _hits; }
            set { _hits = value; }
        }

        // Calculate Accuracy based on hits compared to shots.
        public int Accuracy
        {
            get
            {
                if (_shots == 0)
                    return 0;
                return (int)(((double)_hits / (double)_shots) * 100);
            }
        }

        public int FrameCount
        {
            get { return _frameCount; }
            set { _frameCount = value; }
        }
    }


    /// <summary>
    /// Represents a Projectile object in the game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile : GameObject
    {
        [JsonProperty(PropertyName = "proj")]
        private int _ID { get { return this.ID; } set { this.ID = value; } }

        [JsonProperty(PropertyName = "loc")]
        private Vector2D _location { get { return this.Location; } set { this.Location = value; } }

        [JsonProperty(PropertyName = "dir")]
        private Vector2D _orientation;

        [JsonProperty(PropertyName = "died")]
        private bool _died = false;

        [JsonProperty(PropertyName = "owner")]
        private int _shooter;

        private Vector2D _velocity;


        /// <summary>
        /// Empty default constructor required for JSON.
        /// </summary>
        public Projectile()
        {
        }

        /// <summary>
        /// Primary constructor for creating a new Projectile.
        /// </summary>
        /// <param name="id">Unique ID</param>
        /// <param name="loc">Location</param>
        /// <param name="dir">Orientation</param>
        /// <param name="owner">ID of Tank that fired this Projectile</param>
        /// <param name="speed">Speed of projectile</param>
        public Projectile(int id, Vector2D loc, Vector2D dir, int owner, float speed)
        {
            _ID = id;
            _location = loc;
            _orientation = dir;
            _shooter = owner;
            _velocity = new Vector2D(dir);
            _velocity.Scale(speed);
        }

        public Vector2D Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        public bool Dead
        {
            get { return _died; }
            set { _died = value; }
        }

        public int Shooter
        {
            get { return _shooter; }
        }

        public Vector2D Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
    }


    /// <summary>
    /// Represents a Beam object in the game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam : GameObject
    {
        [JsonProperty(PropertyName = "beam")]
        private int _ID { get { return this.ID; } set { this.ID = value; } }

        [JsonProperty(PropertyName = "org")]
        private Vector2D _origin;

        [JsonProperty(PropertyName = "dir")]
        private Vector2D _direction;

        [JsonProperty(PropertyName = "owner")]
        private int _shooter;

        /// <summary>
        /// Empty default constructor required for JSON.
        /// </summary>
        public Beam()
        {
        }

        /// <summary>
        /// Primary constructor for creating new Beams.
        /// </summary>
        /// <param name="id">Unique ID</param>
        /// <param name="org">Beam's origin point.</param>
        /// <param name="dir">Direction</param>
        /// <param name="owner">ID of the Tank that fired this Beam</param>
        public Beam(int id, Vector2D org, Vector2D dir, int owner)
        {
            _ID = id;
            _origin = org;
            _direction = dir;
            _shooter = owner;
        }

        public Vector2D Origin
        {
            get { return _origin; }
        }

        public Vector2D Direction
        {
            get { return _direction; }
        }

        public int Shooter
        {
            get { return _shooter; }
        }
    }


    /// <summary>
    /// Represents a Wall object in the game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall : GameObject
    {
        [JsonProperty(PropertyName = "wall")]
        private int _ID { get { return this.ID; } set { this.ID = value; } }

        [JsonProperty(PropertyName = "p1")]
        private Vector2D _start;

        [JsonProperty(PropertyName = "p2")]
        private Vector2D _end;

        /// <summary>
        /// Empty default constructor required for JSON.
        /// </summary>
        public Wall()
        {
        }

        /// <summary>
        /// Primary constructor for creating new Walls.
        /// </summary>
        /// <param name="id">Unique ID</param>
        /// <param name="p1">One endpoint of the Wall</param>
        /// <param name="p2">Other endpoint of the Wall</param>
        public Wall(int id, Vector2D p1, Vector2D p2)
        {
            this.ID = id;
            _start = p1;
            _end = p2;
        }

        public Vector2D StartPoint
        {
            get { return _start; }
        }

        public Vector2D EndPoint
        {
            get { return _end; }
        }
    }


    /// <summary>
    ///  Represents a Powerup object in the game.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup : GameObject
    {
        [JsonProperty(PropertyName = "power")]
        private int _ID { get { return this.ID; } set { this.ID = value; } }

        [JsonProperty(PropertyName = "loc")]
        private Vector2D _location { get { return this.Location; } set { this.Location = value; } }

        [JsonProperty(PropertyName = "died")]
        private bool _died = false;

        /// <summary>
        /// Empty default constructor required for JSON.
        /// </summary>
        public Powerup()
        {
        }

        /// <summary>
        /// Primary constructor for creating new Powerups.
        /// </summary>
        /// <param name="id">Unique ID</param>
        /// <param name="loc">Location</param>
        public Powerup(int id, Vector2D loc)
        {
            _ID = id;
            _location = loc;
        }

        /// <summary>
        /// Overloaded constructor for creating a temporary Powerup with a specified location.
        /// </summary>
        /// <param name="posX">The x-position.</param>
        /// <param name="posY">The y-position.</param>
        public Powerup(int posX, int posY)
        {
            _location = new Vector2D(posX, posY);
        }

        public bool PickedUp
        {
            get { return _died; }
            set { _died = value; }
        }
    }


    /// <summary>
    /// Represents a Command object in the game.
    /// This is not a GameObject type as it is only used
    /// to store commands coming in from the clients.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Command
    {
        [JsonProperty(PropertyName = "moving")]
        private string _moving = "none";

        [JsonProperty(PropertyName = "fire")]
        private string _fire = "none";

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D _tdir = new Vector2D(0, -1);

        private int _owner;

        /// <summary>
        /// Empty default constructor required for JSON.
        /// </summary>
        public Command()
        {
        }

        public string Moving
        {
            get { return _moving; }
        }

        public string Fire
        {
            get { return _fire; }
        }

        public Vector2D TurretDirection
        {
            get { return _tdir; }

        }
        public int OwnerID
        {
            get { return _owner; }
            set { _owner = value; }
        }
    }


    /// <summary>
    /// Tracks and calculates frames per second.
    /// </summary>
    public class FPS
    {
        private float _period;
        private int _frameCounter;
        private long _lastClock;

        /// <summary>
        /// Primary constructor for FPS object.
        /// </summary>
        /// <param name="minPeriod">Minimum duration between reports in seconds.</param>
        public FPS(float minPeriod)
        {
            _period = minPeriod * 1000;
            _frameCounter = 0;
        }

        /// <summary>
        /// Used to increment the frame counter:  yourFPS.Frame++;
        /// </summary>
        public int Frame
        {
            get { return _frameCounter; }
            set { _frameCounter = value; }
        }

        /// <summary>
        /// Begins the FPS calculator.
        /// </summary>
        public void Start()
        {
            _lastClock = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Resets the FPS calculator.
        /// </summary>
        public void Reset()
        {
            _frameCounter = 0;
            _lastClock = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Reports the FPS over the previous period.
        /// </summary>
        /// <param name="fps">Sets value of FPS to nearest 100th of a frame.</param>
        /// <returns>True, if the specified period has passed; otherwise false.</returns>
        public bool Report(out float fps)
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long elapsed = now - _lastClock;
            fps = 0.0f;
            if (elapsed < _period)
                return false;
            fps = (float)Math.Round((decimal)_frameCounter / elapsed * 1000, 2);
            _frameCounter = 0;
            _lastClock = now;
            return true;
        }
    }
}
