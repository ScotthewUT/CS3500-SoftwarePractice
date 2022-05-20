// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 25 November 2019

using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TankWars
{
    /// <summary>
    /// Represents the game controller in MVC design. Handles the "heavy lifting" of the TankWars
    /// client. Responsible for networking protocol, processing commands, parsing data, etc.
    /// </summary>
    public class ClientController
    {
        private World _world;                               // Instance of the game World.
        private bool _connected = false;                    // True if connected to the server.
        private SocketState _socketState;                   // The SocketState for this connection.
        private Dictionary<string, Object> _commandSet;     // A set of commands to be sent next frame.
        private List<Keys> _movement = new List<Keys>();    // Holds 2 currently pressed movement keys.


        /// <summary>
        /// Default constructor. ClientViewer needs an instance of the controller.
        /// </summary>
        public ClientController()
        {
            _world = new World();

            _commandSet = new Dictionary<string, Object>() {
                {"moving", "none"},
                {"fire", "none" },
                {"tdir", new Vector2D(0, 1)} };
        }


        /// <summary>
        /// Returns connection status
        /// </summary>
        public bool Connected
        {
            get => _connected;
        }


        /// <summary>
        /// Acknowledges the Connect button was pressed and begins the connection process.
        /// </summary>
        /// <param name="server">The host name or IP address.</param>
        /// <param name="playerName">The player's name.</param>
        public void ConnectButton(string server, string playerName)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 _-]");
            string name = rgx.Replace(playerName, "");
            if(name.Length > 16)
                name = name.Substring(0, Constants.MAX_NAME_SIZE);

            _world = new World(name);
            Networking.ConnectToServer(FirstContact, server, Constants.PORT);
        }

        
        /// <summary>
        /// Handles a movement key being pressed down.
        /// </summary>
        /// <param name="key">The key that was depressed.</param>
        public void MoveKeyDepress(Keys key)
        {
            lock (_movement)
            {
                // Ignore movement keys beyond the two held down.
                if (_movement.Count > 1)
                    return;
                // If no movement key is currently held down, add this key to the list.
                if (_movement.Count == 0)
                    _movement.Add(key);
                // Otherwise, move the held key to the 2nd position and add this key to the head of the list.
                else if (_movement.Count < 2)
                {
                    if (!_movement.Contains(key))
                    {
                        _movement.Add(_movement[0]);
                        _movement[0] = key;
                    }
                }
            }
        }


        /// <summary>
        /// Handles a movement key being released.
        /// </summary>
        /// <param name="key">The key that was released.</param>
        public void MoveKeyRelease(Keys key)
        {   
            lock (_movement)
            {
                _movement.Remove(key);
            }
        }


        /// <summary>
        /// Updates the command set to acknowledge the main fire key was pressed.
        /// </summary>
        public void FireKeyDepress()
        {
            lock (_commandSet)
            {
                _commandSet["fire"] = "main";
            }
        }


        /// <summary>
        /// Updates the command set to acknowledge the main fire key was released.
        /// </summary>
        public void FireKeyRelease()
        {
            lock (_commandSet)
            {
                _commandSet["fire"] = "none";
            }
        }


        /// <summary>
        /// Updates the command set to acknowledge a left click.
        /// </summary>
        public void LeftMouseClick()
        {
            lock (_commandSet)
            {
                _commandSet["fire"] = "main";
            }
        }


        /// <summary>
        /// Updates the command set to acknowledge a right click.
        /// </summary>
        public void RightMouseClick()
        {
            lock (_commandSet)
            {
                _commandSet["fire"] = "alt";
            }
        }


        /// <summary>
        /// Updates the command set with the current turret direction.
        /// </summary>
        /// <param name="x">The x-position of the mouse relative to center point.</param>
        /// <param name="y">The y-position of the mouse relative to center point.</param>
        public void TurretDirection(int x, int y)
        {
            if (x == 0 && y == 0)
                return;
            Vector2D turretDir = new Vector2D(x, y);
            turretDir.Normalize();
            lock (_commandSet)
            {
                _commandSet["tdir"] = turretDir;
            }
        }


        /// <summary>
        /// Returns the game world.
        /// </summary>
        /// <returns></returns>
        public World GetWorld()
        {
            return _world;
        }


        public delegate void ServerUpdateHandler();
        private event ServerUpdateHandler UpdateArrived;
        /// <summary>
        /// Registers the handler (ClientViewer.OnFrame) with UpdateArrived event.
        /// </summary>
        public void RegisterServerUpdateHandler(ServerUpdateHandler handler)
        {
            UpdateArrived += handler;
        }


        /// <summary>
        /// Callback method following the first handshake with server. Saves the SocketState, updates the
        /// callback, and sends the player name to the server.
        /// </summary>
        /// <param name="state">The SocketState for this client-server connection.</param>
        private void FirstContact(SocketState state)
        {
            _socketState = state;
            _connected = true;
            string playerName = _world.PlayerName;
            playerName += Constants.DELIMITER;
            state.OnNetworkAction = ReceiveStartup;
            Networking.Send(state.TheSocket, playerName);
            Networking.GetData(state);
        }


        /// <summary>
        /// Callback method for receiving the player ID and map size from the server.
        /// </summary>
        /// <param name="state">The SocketState for this client-server connection.</param>
        private void ReceiveStartup(SocketState state)
        {
            // Attempt to split the SocketState's current data into 3 substrings.
            string[] data = state.GetData().Split(new string[] {Constants.DELIMITER}, 3, StringSplitOptions.None);

            // If we haven't seen 2 escape characters, wait for more data.
            if(data.Length < 3)
            {
                Networking.GetData(state);
                return;
            }

            // The 1st 2 substrings contain player ID & world size; the 3rd will be partial Walls.
            int id = int.Parse(data[0]);
            int size = int.Parse(data[1]);

            // Recreate the World with the proper ID & size.
            string playerName = _world.PlayerName;
            _world = new World(playerName, id, size);

            // Trim the player ID & world size off the SocketState's data.
            state.RemoveData(0, data[0].Length + data[1].Length + (2 * Constants.DELIMITER.Length));

            // Update SocketState's OnNetworkAction.
            state.OnNetworkAction = ReceiveWalls;
            Networking.GetData(state);
        }


        /// <summary>
        /// Callback method for receiving Walls from the server.
        /// </summary>
        /// <param name="state">The SocketState for this client-server connection.</param>
        private void ReceiveWalls(SocketState state)
        {
            ArraySegment<String> data = DataParser(state);
            lock (_world)
            {
                foreach (string next in data)
                {
                    JObject jsonObj = JObject.Parse(next);
                    JToken token = jsonObj["wall"];
                    if (token is null)
                    {
                        state.OnNetworkAction = ReceiveWorld;
                        break;
                    }
                    Wall newWall = JsonConvert.DeserializeObject<Wall>(next);
                    _world.AddWall(newWall);
                }
            }
            Networking.GetData(state);
        }


        /// <summary>
        /// Callback method for receiving game data from the server.
        /// Continues the event loop.
        /// </summary>
        /// <param name="state">The SocketState for this client-server connection.</param>
        private void ReceiveWorld(SocketState state)
        {
            ArraySegment<string> data = DataParser(state);

            lock (_world)
            {
                foreach (string next in data)
                {
                    Tuple<string, Object> gameObject = DeserializeGameObject(next);
                    switch (gameObject.Item1)
                    {
                        case "Tank":
                            _world.TankUpdate((Tank)gameObject.Item2);
                            break;

                        case "Powerup":
                            _world.PowerUpdate((Powerup)gameObject.Item2);
                            break;

                        case "Projectile":
                            _world.ProjUpdate((Projectile)gameObject.Item2);
                            break;

                        case "Beam":
                            _world.NewBeam((Beam)gameObject.Item2);
                            break;

                        case "":
                        default:
                            break;
                    }
                }
            }
            Networking.GetData(state);

            // Notify any listeners (the view) that a new game world has arrived from the server
            if (UpdateArrived != null)
                UpdateArrived();

            SendCommand();
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
            for(int idx = 0; idx < count; idx++)
            {
                dataSize += data[idx].Length + Constants.DELIMITER.Length;
            }
            state.RemoveData(0, dataSize);
            return new ArraySegment<string>(data, 0, count);
        }


        /// <summary>
        /// Helper method that translates JSON strings into GameObjects.
        /// </summary>
        /// <param name="gameObjStr">A JSON string to deserialize.</param>
        /// <returns>A tuple with the name of the GameObject type and the GameObject itself.</returns>
        private Tuple<string, Object> DeserializeGameObject(string gameObjStr)
        {
            string gameObjType = "";
            Object gameObject = new Object();

            JObject jsonObj = JObject.Parse(gameObjStr);

            foreach (string next in Constants.WORLD_DRAW_LIST)
            {
                JToken token = jsonObj[next];
                if (token is null)
                    continue;

                switch (next)
                {
                    case "tank":
                        gameObjType = "Tank";
                        gameObject = JsonConvert.DeserializeObject<Tank>(gameObjStr);
                        break;

                    case "power":
                        gameObjType = "Powerup";
                        gameObject = JsonConvert.DeserializeObject<Powerup>(gameObjStr);
                        break;

                    case "proj":
                        gameObjType = "Projectile";
                        gameObject = JsonConvert.DeserializeObject<Projectile>(gameObjStr);
                        break;

                    case "beam":
                        gameObjType = "Beam";
                        gameObject = JsonConvert.DeserializeObject<Beam>(gameObjStr);
                        break;

                    default:
                        break;
                }
            }
            return new Tuple<string, Object>(gameObjType, gameObject);
        }


        /// <summary>
        /// Sends command phrase to server.
        /// </summary>
        /// <returns>False, if anything went wrong with Networking.Send; otherwise true.</returns>
        private bool SendCommand()
        {
            Keys move;
            string commandStr;

            lock (_commandSet)
            {
                lock (_movement)
                {
                    if (_movement.Count > 0)
                    {
                        move = _movement[0];
                        switch (move)
                        {
                            case Keys.W:
                                _commandSet["moving"] = "up";
                                break;
                            case Keys.A:
                                _commandSet["moving"] = "left";
                                break;
                            case Keys.S:
                                _commandSet["moving"] = "down";
                                break;
                            case Keys.D:
                                _commandSet["moving"] = "right";
                                break;
                            case Keys.Up:
                                _commandSet["moving"] = "up";
                                break;
                            case Keys.Left:
                                _commandSet["moving"] = "left";
                                break;
                            case Keys.Down:
                                _commandSet["moving"] = "down";
                                break;
                            case Keys.Right:
                                _commandSet["moving"] = "right";
                                break;
                        }
                    }
                    else
                        _commandSet["moving"] = "none";
                }

                commandStr = JsonConvert.SerializeObject(_commandSet);
                commandStr += "\n";

                _commandSet["moving"] = "none";

                if (_commandSet["fire"].Equals("alt"))
                    _commandSet["fire"] = "none";

                return Networking.Send(_socketState.TheSocket, commandStr);
            }
        }
    }
}
