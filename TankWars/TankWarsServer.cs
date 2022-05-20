// AUTHORS: Scott Crowley (u1178178), David Gillespie (u0720569), & CS3500 Faculty
// VERSION: 6 December 2019

using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace TankWars
{
    /// <summary>
    /// Represents a TankWars game server. This is the controller element for MVC.
    /// </summary>
    class TankWarsServer
    {
        private Dictionary<string, Object> _settings;       // Settings from settings file.
        private Dictionary<long, SocketState> _clients;     // Dictionary of clients currently connected.
        private World _world;                               // Instance of the game World.
        private Queue<Command> _commandQueue;               // Storage for commands that come in from clients.
        private Queue<Tank> _joinedQueue;                   // Players that just joined the game.
        private Queue<GameObject> _removeQueue;             // GameObjects that need to be removed on the next frame.
        private Queue<Tank> _respawnQueue;                  // Tanks that are waiting respawn.
        private int _powerupCountDown;                      // Timer for next Powerup spawn.
        private FPS _fps;                                   // FPS caclulator.
        private TcpListener _tcp;                            // Server's Tcp Listener.

        /// <summary>
        /// Main entry point of the server program.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            
            // Initialize and start the server.
            TankWarsServer server = new TankWarsServer();
            server.StartServer();

            // Initialize and start webserver.
            WebServer webserver = new WebServer((int)server._settings["WEB_PORT"]);
            webserver.StartServer();

            // Begin the "frame loop" on a separate thread.
            Thread loop = new Thread(server.FrameLoop);
            loop.Start();
            // Console input signals end of game. Server closes.
            Console.ReadLine();
            loop.Abort();

            // Close game server
            server.StopServer();
            Console.WriteLine("Game Server stopped. Press any key to close Stats Webserver.");
            TimeSpan runtime = (DateTime.Now - startTime);
            uint duration = (uint)runtime.TotalSeconds;
            DatabaseController controller = new DatabaseController();
            GameModel game = controller.CreateGame(duration);

            // Get a dictionary of all tanks that were connected during the game by combining current and
            // disconnected tanks.
            Dictionary<int, Tank> allTanks = server._world.DisconnectedTanks
                                                .Concat(server._world.Tanks)
                                                .GroupBy(d => d.Key)
                                                .ToDictionary(d => d.Key, d => d.First().Value);
            Dictionary<string, Tank> dedupedTanks = new Dictionary<string, Tank>();

            // Loop over all tanks and combine any duplicately named tanks to account
            // for reconnections.
            foreach (Tank tank in allTanks.Values)
            {
                if (dedupedTanks.ContainsKey(tank.Name))
                {
                    Tank processedTank = dedupedTanks[tank.Name];
                    processedTank.Score += tank.Score;
                    processedTank.Shots += tank.Shots;
                    processedTank.Hits += tank.Hits;
                }
                else
                {
                    dedupedTanks.Add(tank.Name, tank);
                }
            }

            // Add each tank to the game record
            foreach (Tank tank in dedupedTanks.Values)
            {
                game.AddPlayer(tank.Name, (uint)tank.Score, (uint)tank.Accuracy);
            }
            // Save the game stats to DB.
            controller.SaveGameStats(game);

            // Input closes webserver, and exits application.
            Console.ReadLine();
            webserver.StopServer();
        }

        /// <summary>
        /// Primary constructor. Initializes the World model as well as the backing lists and queues.
        /// Gets server settings from the provided settings.xml file.
        /// </summary>
        public TankWarsServer()
        {   // Read in the settings.xml file.
            _settings = TankWarsServer.ReadSettings();
            // Initialize the client list.
            _clients = new Dictionary<long, SocketState>();
            // Create an empty World of size defined in settings.
            _world = new World("", 0, (int)_settings["WORLD_SIZE"], _settings);
            // Initialize the command, joined, & remove queues.
            _commandQueue = new Queue<Command>();
            _joinedQueue = new Queue<Tank>();
            _removeQueue = new Queue<GameObject>();
            _respawnQueue = new Queue<Tank>();
            _powerupCountDown = (int)_settings["POWR_SPAWN_MAX"];
            _fps = new FPS(2.0f);
            // Add all the Walls found in the settings file to the World.
            List<Dictionary<string, Vector2D>> walls = _settings["WALLS"] as List<Dictionary<string, Vector2D>>;
            foreach (Dictionary<string, Vector2D> wall in walls)
                _world.AddWall(wall["p1"], wall["p2"]);
        }


        /// <summary>
        /// Infinite loop thread that sends the World to clients every frame.
        /// </summary>
        private void FrameLoop()
        {
            _fps.Start();
            Stopwatch watch = new Stopwatch();
            // Infinite loop.
            while (true)
            {
                watch.Start();
                // Guarantee World updates aren't made or sent any faster than the limit described in settings.
                while (watch.ElapsedMilliseconds < (int)this._settings["MS_PER_FRAME"]) { /* Wait */ }
                watch.Reset();

                // Update the World then send it to the clients.
                this.UpdateWorld();
                this.SendUpdates();

                _fps.Frame++;
                if (_fps.Report(out float fps))
                    Console.WriteLine(fps + " fps.");
            }
        }


        /// <summary>
        /// Start accepting sockets from clients.
        /// </summary>
        private void StartServer()
        {
            // Start a new "event loop."
            _tcp = Networking.StartServer(NewClientConnected, (int)_settings["GAME_PORT"]);
            Console.WriteLine("Server is running. Accepting clients. (Press any Key to Stop Server)");
        }

        /// <summary>
        /// Close Game Server
        /// </summary>
        private void StopServer()
        {
            Console.WriteLine("Shutting down Game Server.");
            lock (_clients)
            {
                foreach (SocketState client in _clients.Values)
                {
                    Networking.SendAndClose(client.TheSocket, "Server Disconnected\n");
                }
                Networking.StopServer(_tcp);
            }
        }


        /// <summary>
        /// Callback method following StartServer.
        /// Log the player name, generate a tank for them, then send the client the player/tank ID
        /// and World size followed by the list of Walls.
        /// </summary>
        /// <param name="obj">The SocketState for this client-server connection.</param>
        private void NewClientConnected(SocketState state)
        {
            if (state.ErrorOccured)
                return;

            // Attempt to split the SocketState's current data into two substrings.
            string[] data = state.GetData().Split(new string[] { Constants.DELIMITER }, 2, StringSplitOptions.None);

            // If we haven't seen a delimiter yet, wait for more data.
            if (data.Length < 2)
            {
                Networking.GetData(state);
                return;
            }
            state.ClearData();

            // The first (and only) substring should be the player's name.
            string name = data[0];
            state.ClearData();

            // Create a new Tank for the player.
            Tank newPlayer;
            lock (_world)
            {
                newPlayer = _world.GetNewTank((int)state.ID, name, (int)_settings["HP"]);
                // Initialize new player's framecount to be at the frames per shot as to allow them to immediately shoot.
                newPlayer.FrameCount = (int)_settings["FRAMES_PER_SHOT"];
            }

            // Add the player's tank to the joined queue.
            lock (_joinedQueue)
            {
                _joinedQueue.Enqueue(newPlayer);
            }

            // Send client the player/tank ID.
            string id = state.ID + Constants.DELIMITER;
            Networking.Send(state.TheSocket, id);

            // Send client the map size.
            Networking.Send(state.TheSocket, _world.Size + Constants.DELIMITER);

            // Send client all the Walls.
            SendWalls(state);

            // Add this client to the list of clients.
            lock (_clients)
            {
                _clients[state.ID] = state;
            }

            // Update the callback.
            state.OnNetworkAction = ReceiveCommand;
            Networking.GetData(state);
        }


        /// <summary>
        /// Sends all of the World's Walls to the client.
        /// </summary>
        /// <param name="state">The SocketState for this client-server connection.</param>
        private void SendWalls(SocketState state)
        {
            StringBuilder wallStr = new StringBuilder();

            foreach (Wall wall in _world.Walls.Values)
                wallStr.Append(JsonConvert.SerializeObject(wall) + Constants.DELIMITER);

            Networking.Send(state.TheSocket, wallStr.ToString());
        }


        /// <summary>
        /// Callback method for Networking.GetData.
        /// "Loop" over this as commands come in from the clients.
        /// Push the commands to the commandQueue for processing.
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveCommand(SocketState state)
        {
            if (state.ErrorOccured)
            {
                lock (_clients)
                {
                    RemoveClient(state.ID);
                    return;
                }
            }

            ArraySegment<string> data = DataParser(state);

            // Queue up received commands to be processed by the Frame loop.
            foreach (string commandStr in data)
            {
                Command command = JsonConvert.DeserializeObject<Command>(commandStr);
                command.OwnerID = (int)state.ID;
                lock (_commandQueue)
                {
                    _commandQueue.Enqueue(command);
                }
            }
            Networking.GetData(state);
        }


        /// <summary>
        /// Helper method that pulls data from the SocketState, splits it into strings around the defined delimiter,
        /// removes any of the complete data from the SocketState, then returns the complete & separated data.
        /// </summary>
        /// <param name="state">The SocketState with data to parse.</param>
        /// <returns>A set of strings that represent the completed data in the SocketState.</returns>
        private ArraySegment<string> DataParser(SocketState state)
        {
            string dataStr = state.GetData();
            string[] data = dataStr.Split(new string[] { Constants.DELIMITER }, StringSplitOptions.None);
            int count = data.Length - 1;
            int dataSize = 0;
            for (int idx = 0; idx < count; idx++)
            {
                dataSize += data[idx].Length + Constants.DELIMITER.Length;
            }
            state.RemoveData(0, dataSize);
            return new ArraySegment<string>(data, 0, count);
        }


        /// <summary>
        /// Updates the World with all queued commands since last frame.
        /// </summary>
        private void UpdateWorld()
        {
            HashSet<int> clientSet = new HashSet<int>();

            lock (_commandQueue)
            {   // Attempt to process each Command in the queue.
                while (_commandQueue.Count > 0)
                {
                    Command command = _commandQueue.Dequeue();
                    // Clients are only allowed 1 Command per frame.
                    if (clientSet.Contains(command.OwnerID))
                        continue;
                    clientSet.Add(command.OwnerID);
                    lock (_world)
                    {
                        ProcessCommand(command);
                    }
                }
            }

            lock (_world)
            {   // Compare each Beam in the World with Tank positions.
                foreach (Beam beam in _world.Beams.Values)
                {
                    foreach (Tank tank in _world.Tanks.Values)
                    {   // Can't shoot yourself.
                        if (beam.Shooter == tank.ID)
                            continue;
                        // Check if the Beam intersects this Tank.
                        if (Intersects(beam.Origin, beam.Direction, tank.Location, (int)_settings["TANK_SIZE"] / 2))
                            BeamHitUpdate(_world.Tanks[beam.Shooter], tank);
                    }
                }
            }

            lock (_world)
            {   // Update each Projectile in the World and check for collisions.
                foreach (Projectile proj in _world.Projectiles.Values)
                {
                    Vector2D prev = proj.Location;
                    Vector2D next = prev + proj.Velocity;
                    proj.Location = next;
                    GameObject gameObj;
                    bool collision = _world.Collision(proj, out gameObj);
                    if (collision)
                    {
                        proj.Dead = true;
                        _removeQueue.Enqueue(proj);
                        if (gameObj is Tank)
                            if (_world.Tanks.ContainsKey(proj.Shooter))
                                ProjHitUpdate(_world.Tanks[proj.Shooter], gameObj as Tank);
                    }
                }
            }

            _powerupCountDown--;
            lock (_world)
            {
                if (_powerupCountDown == 0)
                {
                    _world.GetNewPowerup();
                    if (_world.Powerups.Count < (int)_settings["POWR_MAX"])
                    {
                        Random rand = new Random();
                        _powerupCountDown = rand.Next((int)_settings["POWR_SPAWN_MIN"], (int)_settings["POWR_SPAWN_MAX"]);
                    }
                }
            }

            IncrementFrameCounts();
            RespawnTanks();
        }


        /// <summary>
        /// Increments frame counters on all Tanks.
        /// </summary>
        private void IncrementFrameCounts()
        {
            lock (_world)
            {
                foreach (Tank tank in _world.Tanks.Values)
                    tank.FrameCount++;
            }
        }


        /// <summary>
        /// Respawns all tanks that have been dead long enough to respawn.
        /// </summary>
        private void RespawnTanks()
        {
            lock (_respawnQueue)
            {
                if (_respawnQueue.Count == 0)
                    return;

                while (_respawnQueue.Peek().FrameCount >= (int)_settings["TANK_SPAWN"]
                                                                            || _respawnQueue.Peek().Disconnected)
                {   // As long as the Tank at the front of the queue is ready for respawn, keep respawning Tanks.
                    Tank tank = _respawnQueue.Dequeue();
                    if (!tank.Disconnected)
                    {
                        lock (_world)
                        {
                            tank.HP = (int)_settings["HP"];
                            tank.FrameCount = 0;
                            tank.Location = _world.SpawnPoint('T');
                        }
                    }
                    if (_respawnQueue.Count == 0)
                        break;
                }
            }
        }


        /// <summary>
        /// Helper method that attempts to update the World with player commands.
        /// </summary>
        /// <param name="cmd">The Command to process.</param>
        private void ProcessCommand(Command cmd)
        {   // If tank doesn't exist anymore, don't process its command.
            if (!_world.Tanks.ContainsKey(cmd.OwnerID))
                return;

            Tank tank = _world.Tanks[cmd.OwnerID];

            // Don't allow dead tanks to send commands
            if (_respawnQueue.Contains(tank))
                return;

            tank.Aiming = cmd.TurretDirection;

            if (cmd.Moving != "none")
            {   // If a movement key was set, adjust Tank orientation.
                switch (cmd.Moving)
                {
                    case ("up"):
                        tank.Orientation = new Vector2D(0, -1);
                        break;
                    case ("down"):
                        tank.Orientation = new Vector2D(0, 1);
                        break;
                    case ("left"):
                        tank.Orientation = new Vector2D(-1, 0);
                        break;
                    case ("right"):
                        tank.Orientation = new Vector2D(1, 0);
                        break;
                }
                tank.Velocity = tank.Orientation * (float)_settings["TANK_SPEED"];
            }
            else
                tank.Velocity = new Vector2D(0, 0);

            if (cmd.Moving != "none")
            {
                Vector2D prev = tank.Location;
                // New tank location determined by the orientation * by speed
                tank.Location = prev + tank.Velocity;

                GameObject gameObj;
                // Check for a collision at the Tank's new location.
                bool collision = _world.Collision(tank, out gameObj);
                // If it collided with a wall, revert Tank to previous location.
                if (collision && gameObj is Wall)
                    tank.Location = prev;
                // If it collided with a Powerup, update tank and Powerup.
                else if (collision && gameObj is Powerup)
                {
                    tank.BeamCount++;
                    ((Powerup)gameObj).PickedUp = true;
                    _removeQueue.Enqueue(gameObj);
                }
                // If there wasn't a collision, check for edge of map and wraparound.
                else if (!collision)
                {
                    double x = tank.Location.GetX();
                    double y = tank.Location.GetY();
                    int topL = -_world.Size / 2;
                    int botR = -topL;

                    Vector2D oppositeSide = null;
                    if (x < topL || x > botR)
                        oppositeSide = new Vector2D(-x, y);
                    if (y < topL || y > botR)
                        oppositeSide = new Vector2D(x, -y);

                    if (oppositeSide != null)
                    {
                        tank.Location = oppositeSide;
                        collision = _world.Collision(tank, out gameObj);
                        if (collision && gameObj is Wall)
                            tank.Location = prev;
                        else if (collision && gameObj is Powerup)
                        {
                            tank.BeamCount++;
                            ((Powerup)gameObj).PickedUp = true;
                            _removeQueue.Enqueue(gameObj);
                        }
                    }
                }
            }

            if (cmd.Fire == "main")
            {
                // If enough frames have passed since shooting, allow another shot
                if (tank.FrameCount >= (int)_settings["FRAMES_PER_SHOT"])
                {
                    _world.NewProjectile(tank.Location, tank.Aiming, tank.ID, (float)_settings["PROJ_SPEED"]);
                    tank.Shots++;
                    tank.FrameCount = 0;
                }
            }
            else if (cmd.Fire == "alt")
            {
                if (tank.BeamCount > 0)
                {
                    _world.NewBeam(tank.Location, tank.Aiming, tank.ID);
                    tank.BeamCount--;
                }
            }
        }


        /// <summary>
        /// Helper method that updates Tanks when one strikes another with a Beam.
        /// </summary>
        /// <param name="shooter">The Tank that fired the Beam.</param>
        /// <param name="target">The Tank that was struck by the Beam</param>
        private void BeamHitUpdate(Tank shooter, Tank target)
        {
            shooter.Hits++;
            shooter.Score++;
            target.HP = 0;
            target.Died = true;
            target.FrameCount = 0;
            lock (_respawnQueue)
            {
                _respawnQueue.Enqueue(target);
            }
        }


        /// <summary>
        /// Helper method that updates Tanks when one strikes another with a Projectile.
        /// </summary>
        /// <param name="shooter">The Tank that fired the Projectile.</param>
        /// <param name="target">The Tank that was struck by the Projectile.</param>
        private void ProjHitUpdate(Tank shooter, Tank target)
        {
            if (_world.Tanks.ContainsKey(shooter.ID))
            {
                shooter.Hits++;
                target.HP--;
                if (target.HP < 1)
                {
                    shooter.Score++;
                    target.Died = true;
                    target.FrameCount = 0;
                    lock (_respawnQueue)
                    {
                        _respawnQueue.Enqueue(target);
                    }
                }
            }
        }


        /// <summary>
        /// Sends world updates to all clients.
        /// </summary>
        private void SendUpdates()
        {
            StringBuilder worldUpdate = new StringBuilder();

            lock (_world)
            {   // Loop over each GameObject type and append them to the StringBuilder.
                foreach (Tank tank in _world.Tanks.Values)
                {
                    string tankStr = JsonConvert.SerializeObject(tank) + Constants.DELIMITER;
                    worldUpdate.Append(tankStr);
                    // Reset "died" flag incase this Tank had died this frame.
                    tank.Died = false;
                }
                foreach (Projectile proj in _world.Projectiles.Values)
                {
                    string projStr = JsonConvert.SerializeObject(proj) + Constants.DELIMITER;
                    worldUpdate.Append(projStr);
                }
                foreach (Powerup pow in _world.Powerups.Values)
                {
                    string powStr = JsonConvert.SerializeObject(pow) + Constants.DELIMITER;
                    worldUpdate.Append(powStr);
                }
                foreach (Beam beam in _world.Beams.Values)
                {
                    string beamStr = JsonConvert.SerializeObject(beam) + Constants.DELIMITER;
                    worldUpdate.Append(beamStr);
                }
                // Beams are only sent for one frame.
                _world.Beams.Clear();
            }

            lock (_clients)
            {   // Send the final world data to each client.
                foreach (SocketState state in _clients.Values)
                {
                    Networking.Send(state.TheSocket, worldUpdate.ToString());
                }
            }

            // Clear the joined queue.
            ClearJoinedQueue();
            // Remove any GameObjects from the World that were shown for the last frame.
            RemoveGameObjects();
        }


        /// <summary>
        /// Removes a given client by ID.
        /// </summary>
        /// <param name="id">The Client/Player ID to remove.</param>
        private void RemoveClient(long id)
        {
            Console.WriteLine("Client " + id + " disconnected.");
            lock (_clients)
            {
                _clients.Remove(id);
            }
            lock (_world)
            {
                // In the event the tank is missing (only happens when force closing clients), don't do anything.
                if (_world.Tanks.ContainsKey((int)id))
                {
                    Tank tank = _world.Tanks[(int)id];
                    tank.Died = true;
                    tank.HP = 0;
                    tank.Disconnected = true;
                    tank.Joined = false;
                    _world.DisconnectedTanks.Add(tank.ID, tank);
                    _world.TankUpdate(tank);
                    lock (_removeQueue)
                    {
                        _removeQueue.Enqueue(tank);
                    }
                }
            }
        }


        /// <summary>
        /// Updates any new Tank's joined flag to false following the initial broadcast of that Tank.
        /// </summary>
        private void ClearJoinedQueue()
        {
            lock (_joinedQueue)
            {
                foreach (Tank tank in _joinedQueue)
                {
                    lock (_world)
                    {
                        tank.Joined = false;
                    }
                }
                _joinedQueue.Clear();
            }
        }


        /// <summary>
        /// Removes "dead" Powerups & Projectiles and disconnected Tanks from the World.
        /// </summary>
        private void RemoveGameObjects()
        {
            lock (_removeQueue)
            {
                lock (_world)
                {
                    foreach (GameObject gameObj in _removeQueue)
                    {
                        if (gameObj is Tank)
                        {
                            _world.Tanks.Remove(gameObj.ID);
                            continue;
                        }
                        if (gameObj is Projectile)
                        {
                            _world.Projectiles.Remove(gameObj.ID);
                            continue;
                        }
                        if (gameObj is Powerup)
                            _world.Powerups.Remove(gameObj.ID);
                    }
                }
                _removeQueue.Clear();
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
        private static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
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


        /// <summary>
        /// Reads the settings file and returns a dictionary representing the contents.
        /// If no setting is defined for a needed item, the default is pulled from Constants.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, Object> ReadSettings()
        {
            XElement gameSettings = XElement.Load(Constants.SETTINGS_PATH);
            //IEnumerable<XElement> allSettings = settingsfile.Elements("GameSettings");
            // if (allSettings.Count() == 0)
            //     throw new Exception("No Game Settings Found");
            //XElement gameSettings = allSettings.Last(); // Get last GameSettings in the event there are multiple
            Dictionary<string, Object> settings = new Dictionary<string, object>();

            // Wall parsing
            List<Dictionary<string, Vector2D>> walls = new List<Dictionary<string, Vector2D>>();
            IEnumerable<XElement> wallConfigs = gameSettings.Elements("Wall");
            foreach (var wallConfig in wallConfigs) // Iterate over each defined wall in the xml
            {
                XElement p1 = wallConfig.Element("p1");
                XElement p2 = wallConfig.Element("p2");
                if (p1 == null || p2 == null) // If missing p1 or p2, skip this wall
                    continue;
                XElement p1X = p1.Element("x");
                XElement p1Y = p1.Element("y");
                XElement p2X = p2.Element("x");
                XElement p2Y = p2.Element("y");
                if (p1X == null || p1Y == null || p2X == null || p2Y == null) // If p1 or p2 do not have x or y defined, skip
                    continue;
                bool p1XConverted = int.TryParse(p1X.Value, out int p1XInt);
                bool p1YConverted = int.TryParse(p1Y.Value, out int p1YInt);
                bool p2XConverted = int.TryParse(p2X.Value, out int p2XInt);
                bool p2YConverted = int.TryParse(p2Y.Value, out int p2YInt);
                if (!p1XConverted || !p2XConverted || !p1YConverted || !p2YConverted) // If p1 or p2 are not valid integers, skip this wall
                    continue;
                walls.Add(new Dictionary<string, Vector2D>() { { "p1", new Vector2D(p1XInt, p1YInt) }, { "p2", new Vector2D(p2XInt, p2YInt) } });
            }
            settings["WALLS"] = walls;

            // Other options
            settings["WORLD_SIZE"] = GetSetting(gameSettings, "UniverseSize", Constants.DEFAULT_WORLD_SIZE, "Int32");
            settings["MS_PER_FRAME"] = GetSetting(gameSettings, "MSPerFrame", Constants.DEFAULT_MS_PER_FRAME, "Int32");
            settings["FRAMES_PER_SHOT"] = GetSetting(gameSettings, "FramesPerShot", Constants.DEFAULT_FRAMES_PER_SHOT, "Int32");
            settings["TANK_SPAWN"] = GetSetting(gameSettings, "RespawnRate", Constants.TANK_SPAWN, "Int32");
            settings["GAME_PORT"] = GetSetting(gameSettings, "GameServerPort", Constants.PORT, "Int32");
            settings["WEB_PORT"] = GetSetting(gameSettings, "WebServerPort", Constants.WEB_PORT, "Int32");
            settings["HP"] = GetSetting(gameSettings, "StartingTankHP", Constants.HP, "Int32");
            settings["PROJ_SPEED"] = GetSetting(gameSettings, "ProjectileSpeed", Constants.PROJ_SPEED,"float");
            settings["TANK_SPEED"] = GetSetting(gameSettings, "EngineStrength", Constants.TANK_SPEED,"float");
            settings["WALL_SIZE"] = GetSetting(gameSettings, "WallSize", Constants.WALL_SIZE, "Int32");
            settings["TANK_SIZE"] = GetSetting(gameSettings, "TankSize", Constants.TANK_SIZE, "Int32");
            settings["POWR_MAX"] = GetSetting(gameSettings, "MaxPowerups", Constants.POWR_MAX, "Int32");
            settings["POWR_SPAWN_MIN"] = GetSetting(gameSettings, "MinPowerupSpawnTime", Constants.POWR_SPAWN_MIN, "Int32");
            settings["POWR_SPAWN_MAX"] = GetSetting(gameSettings, "MaxPowerupSpawnTime", Constants.POWR_SPAWN_MAX, "Int32");

            return settings;
        }

        /// <summary>
        /// Retrieves a setting from either the provided gameSettings, or default to the value provided.
        /// In the event two instances of the setting exist in the settings file, the last one is used.
        /// </summary>
        /// <param name="gameSettings">GameSettings to pull from</param>
        /// <param name="defaultVal">Default value to use if none exists</param>
        /// <param name="settingName">Name to look up in file</param>
        /// <returns></returns>
        private static dynamic GetSetting(XElement gameSettings, string settingName, dynamic defaultVal, string type)
        {
            IEnumerable<XElement> elements = gameSettings.Elements(settingName);
            var settingVal = defaultVal;
            int tempInt;
            float tempFloat;

            if ((elements.Count() > 0)) {
                if (type == "Int32")
                {
                    Int32.TryParse(elements.Last().Value, out tempInt);
                    settingVal = tempInt;
                    
                }
                else if (type == "float")
                {
                    float.TryParse(elements.Last().Value, out tempFloat);
                    settingVal = tempFloat;
                }
                else
                {
                    settingVal = Convert.ChangeType(elements.Last().Value, Type.GetType(type));
                }
                
            }
                
            return settingVal;
        }
    }
}
