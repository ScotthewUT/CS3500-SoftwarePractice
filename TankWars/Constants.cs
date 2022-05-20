// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 6 December 2019

using System.Collections.Generic;
using System.Drawing;

namespace TankWars

{
    /// <summary>
    /// Container for common constants needed for TankWars. They're placed here for ease of updating.
    /// </summary>
    public static class Constants
    {
        public const int PORT = 11000;                  // Network port ID.
        public const int WEB_PORT = 80;                 // Port for Webserver.
        public const string DELIMITER = "\n";           // Delimiter used to separating data strings.
        public const int HP = 3;                        // HP of Tanks.
        public const int VIEW_SIZE = 800;               // Height & width of the client view in pixels.
        public const int TANK_SIZE = 60;                // Height & width of Tank draw boxes in pixels.
        public const int TURR_SIZE = 50;                // Height & width of turret draw boxes.
        public const int WALL_SIZE = 50;                // Height & width of Wall draw boxes.
        public const int PROJ_SIZE = 30;                // Height & width of Projectile draw boxes.
        public const int POWR_SIZE = 15;                // Height & width of Powerup draw boxes.
        public const int BEAM_SIZE = 15;                // Width of the Beam draw boxes.
        public const int BEAM_LIFE = 300;               // How long in ms to display Beams.
        public const int MAX_NAME_SIZE = 16;            // Max length of player names.
        public const int POWR_MAX = 5;                  // Maximum number of powerups in the field.
        public const int POWR_SPAWN_MIN = 588;          // Minimum respawn delay of powerups in frames.
        public const int POWR_SPAWN_MAX = 2353;         // Maximum respawn delay of powerups in frames.
        public const int SPAWN_BUFFER = 40;             // How much space to give new spawns.
        public const int TANK_SPAWN = 300;              // Tank respawn delay in frames.
        public const float TANK_SPEED = 2.5f;           // Default speed of tanks in units/frame.
        public const float PROJ_SPEED = 12.0f;          // Default speed of projectiles in units/frame.
        public const int DEFAULT_WORLD_SIZE = 1200;     // Default World size
        public const int DEFAULT_MS_PER_FRAME = 17;     // Default miliseconds per frame
        public const int DEFAULT_FRAMES_PER_SHOT = 80;  // Default frames per shot allowed

        // Settings file location
        public const string SETTINGS_PATH = @"..\..\..\Resources\settings.xml";

        // List of game objects sent by the server each frame.
        public static readonly string[] WORLD_DRAW_LIST = { "tank", "power", "proj", "beam" };

        // Collection of images for drawing the world view.
        public static readonly Dictionary<string, Image> SPRITES = new Dictionary<string, Image>()
        {
            {"background",  Image.FromFile(@"..\..\..\Resources\Sprites\Background.png")},
            {"wall",        Image.FromFile(@"..\..\..\Resources\Sprites\WallSprite.png")},

            {"tank0",       Image.FromFile(@"..\..\..\Resources\Sprites\RedTank.png")},
            {"tank1",       Image.FromFile(@"..\..\..\Resources\Sprites\OrangeTank.png")},
            {"tank2",       Image.FromFile(@"..\..\..\Resources\Sprites\YellowTank.png")},
            {"tank3",       Image.FromFile(@"..\..\..\Resources\Sprites\LightGreenTank.png")},
            {"tank4",       Image.FromFile(@"..\..\..\Resources\Sprites\GreenTank.png")},
            {"tank5",       Image.FromFile(@"..\..\..\Resources\Sprites\BlueTank.png")},
            {"tank6",       Image.FromFile(@"..\..\..\Resources\Sprites\DarkTank.png")},
            {"tank7",       Image.FromFile(@"..\..\..\Resources\Sprites\PurpleTank.png")},

            {"turret0",     Image.FromFile(@"..\..\..\Resources\Sprites\RedTurret.png")},
            {"turret1",     Image.FromFile(@"..\..\..\Resources\Sprites\OrangeTurret.png")},
            {"turret2",     Image.FromFile(@"..\..\..\Resources\Sprites\YellowTurret.png")},
            {"turret3",     Image.FromFile(@"..\..\..\Resources\Sprites\LightGreenTurret.png")},
            {"turret4",     Image.FromFile(@"..\..\..\Resources\Sprites\GreenTurret.png")},
            {"turret5",     Image.FromFile(@"..\..\..\Resources\Sprites\BlueTurret.png")},
            {"turret6",     Image.FromFile(@"..\..\..\Resources\Sprites\DarkTurret.png")},
            {"turret7",     Image.FromFile(@"..\..\..\Resources\Sprites\PurpleTurret.png")},

            {"shot0",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_red.png")},
            {"shot1",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_orange.png")},
            {"shot2",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_yellow.png")},
            {"shot3",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_light_green.png")},
            {"shot4",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_green.png")},
            {"shot5",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_blue.png")},
            {"shot6",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_indigo.png")},
            {"shot7",       Image.FromFile(@"..\..\..\Resources\Sprites\shot_violet.png")},

            {"beam0",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_red.png")},
            {"beam1",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_orange.png")},
            {"beam2",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_yellow.png")},
            {"beam3",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_light_green.png")},
            {"beam4",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_green.png")},
            {"beam5",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_blue.png")},
            {"beam6",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_indigo.png")},
            {"beam7",       Image.FromFile(@"..\..\..\Resources\Sprites\beam_violet.png")},

            {"skull",     Image.FromFile(@"..\..\..\Resources\Sprites\skull.png")}
        };
    }
}
