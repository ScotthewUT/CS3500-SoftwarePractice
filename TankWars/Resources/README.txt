All Your Tanks Are Belong To Us!

# PS8 TankWars Client
Updated 11/24/19

This implementation of TankWars is based on the Fall 2019 CS3500 TankWars specification needed for the TankWars client, written by Scott Crowley and David Gillespie.
Having lost the source code to a previously constructed client, this fresh implementation is broken up into 8 Separate projects, each with their own focus and concern:
- ClientController houses all control oriented tasks for the client. This file handles connecting to the server, parsing JSON payloads, and sending updates back to the
	server interacts with the game. Each stage of the connection handshake process is broken out into separate methods to help make maintainablility high in the future 
	(as we know this is an important item for the client due to past experiences with developers).
- ClientViewer houses everything needed to interact with the windows Form that the user will interact with. This file handles registering input events sending the pertinent
	information to the controller for interpetation and computation. The Viewer has access to the Model representing the state of the application, but it does not write to it.
- Constants houses all static configuration items needed for gameplay. This includes sizes and resources for various items displayed in game.
- DrawingPanel functions very similarly to ClientViewer, and together they consist of the overall 'view' of the application. This panel handles the drawing of all in-game items,
	sprites, and backgrounds. Read locks are implemented here to make sure that no resource is modified while the view is still accessing it.
- GameObjects functions as the base to the application's 'model'. All objects found in game are defined here. This model is updated by the controller and read by the view of
	the application.
- Resources functions as a folder to hold all external resources needed for the application to function. Assets can be found in their respective folders: Sprites for the in-game
	visual representation of objects, and Libraries for external dlls required by the application.
- Vector2D is exactly as set up by the client's previous developer. This helper class is used to represent points in space and vectors used in game.
- WorldModel, along with the aforementioned GameObjects represent the application's 'model'. Using the Objects defined in GameObjects, the World represents the state of the
	application at any given point in time. The World also houses the proper getters and setters to interact with said application state, such as adding/removing Tanks,
	projectiles/beams/etc. These are exposed for the Controller and View to reference. (And to change in the case of the Controller.)

Considerations taken during development:
- Due to the simplicity of this client, and the request that it be passive, frames are displayed at the rate they are received from the server. All necessary changes are built up
	in a message cache of sorts that is sent to the server as soon as the client is done rendering the latest frame.
- Because Beams are only considered on one Frame serverside, they are implemented slightly differently than the other objects. Once a frame containing a beam is found,
	the client will load it into memory for a specified amount of time (defined in Constants), before removing it from the state of the application.
- Wall Segments: In hopes the client will want to add changes to walls (such as breakable walls, or movable walls), the computation of the walls are broken down into segment by segment
	chunks. They currently all load when the walls are initially sent by the server, but the code is open such that it will be easily changed in the future.


# PS9 TankWars Server
Updated 12/6/19

TankWarsServer and its partner classes implement the server handling of a TankWars game. It allows for
multiple clients (local or remote) to connect to the same game. Connection handling is managed by the 
referenced networking library (see PS7). TankWarsServer represents the controller and does most the
heavy-lifting required such as initializing the connection callback/event loop, processing commands
from clients, updating the world, and sending updates to the connected clients. The model is represented
by the same World class as used in PS8; however, it has been substantially expanded to support many new
functions such as collision detection. (Note: while PS9 and PS8 both use World classes for their models
they don't reference the same World object.)

Stats for all historical games can be found at SERVER_ADDRESS/games. These stats are held by a MySQL database
housed at the University of Utah. Adding the ?player=PLAYERNAME url parameter allows the viewer to see
games for the specific player named by PLAYERNAME.

Settings for the game can be changed in Resources/settings.xml. If multiple instances of the same setting
exist, the last instance will be used. If no instances exist for a particular setting, a default system value
will be used.
Currently the following settings can be edited and changed:
  UniverseSize
  MSPerFrame
  FramesPerShot
  RespawnRate
  WebServerPort
  StartingTankHP
  ProjectileSpeed
  EngineStrength
  WallSize
  TankSize
  MaxPowerups
  MinPowerupSpawnTime
  MaxPowerupSpawnTime

Development notes:
Initially, a WorldGraph class backed by a Dictionary<Vector2D, GameObject> set was created as a member
of the World model. The idea being than each x-y position in the game could be flagged with an object
such as Wall or Tank. This would allow us to do collision detection that didn't require looping through
each GameObject in the game. Instead, when an Tank or Projectile moved to a new location would could
quickly look at all the pixels at that location for a possible collision. Unfortunately, testing showed
that Projectile movement and collision detection ran far too slowly for this method. Eventually, the
collision detection that worked best made use of System.Drawing.Rectangle.IntersectsWith. Most GameObjects
can be quickly converted to a Rectangle and IntersectsWith has O(1) time complexity. This does require
looping through each object in the game, but the performance remains acceptable.

Running on a single local machine, our TankWars server can support between 20-30 active clients without
too noticeable of a performance loss. Much beyond 30 clients causes FPS to drop precipitously and
input lag becomes unacceptable.

The Database Controller implements a helper executeSQL command that will take a sql string and return the results
as a dictionary. (auto-identifying keys from the column names of the results.)
This slightly narrows what can be done with raw SQL as it only will return values that have a 
column name associated with the result, but for our purposes this works much better than having multiple sets of
code with the SQL connection code.

Due to the variability of user input, and the desire to allow the server to be "smart," the code that handles
the loading of settings backs settings up with the defaults set in PS8 in the Constants class. This way,
if the user accidentally deletes a setting, we have a default to fall back on.



# Change Log:

11/26/19
- Setup a project structure and imported skeleton code provided.
- Added more constants to the Constants class, Created a method Vector2D.Scale to scale vectors, updated GameObjects with new properties and constructors.
- Additional updates to World and GameObject classes to support Server operations (e.g. added setters).
- Created a WorldGraph class backed by a Dictionary<Vector2D, GameObject> that represents a map of all the game objects (walls, tanks, etc). This allows us to find collisions without
needing to iterate over all the game's objects.

12/3/19
- Added basic connection handling organization to Server controller.
- Added methods to read and parse XML settings files and added settings.xml to the Resources.
- Updated ReceiveName to get the player name and return the tank ID and world size to client.
- Futher implementation of connection event loop.

12/4/19
- Removed the ID getter from GameObject's subclasses as they inherit it.
- Updated WorldGraph.Add to support adding new Tanks & Powerups.
- Code cleanup and comments added to TankWarsServer.
- Added a "joined queue" so clients can be alerted when a new player joins.
- Improved SendWalls so Networking.Send is not called from a loop.
- Added an Owner ID field to the Command class.
- Began implementing UpdateWorld and ProcessCommand methods.
- Added more properties to Tank class: Shots, Hits, & FrameCount. Renamed Dead to Died.
- Added WorldGraph.MoveProjectile method.
- World.ProjUpdate is now overloaded to support moving Projectiles and tracking collisions.
- Server has a new field, _respawnQueue, for hodling dead Tanks.
- Fixed FrameLoop method to start the Stopwatch.
- Fixed NewClientConnect callback to clear socket data after getting player name.
- TankWarsServer.UpdateWorld:
    - For-each loops added for Beams and Projectiles.
    - Helper methods, BeamHitUpdate & ProjHitUpdate, added to support loops.
    - A Powerup loop may be unnecessary.
- SendUpdates now resets Tanks' died flag.
- Initial implementation for web server.
- Added a class for database controller.
- Implemented generic executeSQL command for use in specific Add and Get methods.
- Implemented GetGames.
- Fixed a bug with GameObjects' getter/setter not inheriting correctly.

12/5/19
- Collision detection overhauled to improve performance. Initial spawns still clipping.
- IncrementFrameCounts method added to allow for respawn and fire timmers.
- Updated ProcessCommand to handle aiming, moving, shooting, and beam shooting.
- Updated getters/setters for ID and Location for all GameObject classes to fix Wall loading bug.
- Removed threading from ProcessCommand in UpdateWorld. Caused performance issues.
- Added methods for reading database and displaying UI on web page.
- Powerups and Beams now supported in UpdateWorld.
- WorldGraph class completely scrapped. Its collision detection wasn't 100% and was far too slow at tracking Projectiles.
- Added FPS class for the server to report to console.
- Fixed a bug in World.Collision where Projectiles could hit dead Tanks.
- Tracked a stack overflow bug to SpawnPoint method that is recursing far too much.
- Replaced the recursive call in SpawnPoint to a while loop. Found that certain maps cause an infinite loop when trying to spawn the first Tank.
- Removed buffer size parameter from SpawnPoint as it can access it directly from Constants.
- Implemented a new collision detection method using System.Drawing.Rectangles.

12/6/19
- Collision detection is mostly working now. There a slight offset issue with Wall collision.
- Finalized database access code and added logging for server.
- Added code to log shots fire for each tank and calculate accuracy.
- Fixed accuracy int overflow error when 0 shots are taken.
- Added conditions to ProjHitUpdate and RespawnTanks to fix a bug where DC'd tanks could collide with Projectiles or block the respawn queue.
- Fixed a bug in RectangleCollision where Rectangles drawn from Walls weren't using the top-left corner.
