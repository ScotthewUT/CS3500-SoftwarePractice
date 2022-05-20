// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 6 December 2019

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace TankWars
{
    public class DatabaseController
    {
        // Connection String for SQL Server. NOTE: Never do this in production
        // unless your professor tells you to for an assignment. ;) 
        private readonly string connectionString = "server=atr.eng.utah.edu;" +
         "database=cs3500_u0720569;" +
         "uid=cs3500_u0720569;" +
         "password=imchanged";

        /// <summary>
        /// Executes a given SQL statement, and returns the results as a list of dictionaries
        /// </summary>
        /// <param name="sql">SQL String to Execute</param>
        /// <returns></returns>
        public List<Dictionary<string, Object>> executeSQL(string sql)
        {
            List<Dictionary<string, Object>> results = new List<Dictionary<string, object>>();
            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = sql;

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Get schema of results to identify the column names with
                        DataTable resultSchema = reader.GetSchemaTable();
                        // Loop over all rows returned
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            foreach (DataRow column in resultSchema.Rows)
                            {
                                string columnName = column["ColumnName"].ToString();
                                row.Add(columnName, reader[columnName]);
                            }
                            results.Add(row);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return results;
        }

        /// <summary>
        /// Adds records representing the provided game to the database
        /// </summary>
        /// <param name="gameStats">GameModel to Save to DB</param>
        public void SaveGameStats(GameModel gameStats)
        {
            uint gameId = gameStats.ID;
            List<PlayerModel> players = gameStats.GetPlayers();
            foreach (PlayerModel player in players)
            {
                uint ID = GetOrCreatePlayerID(player.Name);
                uint Score = player.Score;
                uint Accuracy = player.Accuracy;
                WriteGamePlayedToDB(gameId, ID, Score, Accuracy);
            }
        }

        /// <summary>
        /// Writes given stats to the database
        /// </summary>
        /// <param name="gameId">ID of the game</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="Score">Score of the player</param>
        /// <param name="Accuracy">Accuracy of the player</param>
        private void WriteGamePlayedToDB(uint gameId, uint playerId, uint Score, uint Accuracy)
        {
            string sql_statement = "INSERT INTO GamesPlayed (gID, pID, Score, Accuracy) VALUES({0}, {1}, {2}, {3})";
            sql_statement = String.Format(sql_statement, gameId, playerId, Score, Accuracy);
            this.executeSQL(sql_statement);
        }

        /// <summary>
        /// Creates a gameModel of given duration
        /// </summary>
        /// <param name="duration">Time in seconds the game was running</param>
        /// <returns></returns>
        public GameModel CreateGame(uint duration)
        {
            string sql_statement = "INSERT INTO Games (duration) VALUES(" + duration + ")";
            this.executeSQL(sql_statement);
            string newGameQ = "SELECT id FROM Games WHERE id = (SELECT MAX(ID) FROM Games)";
            List<Dictionary<string, Object>> results = executeSQL(newGameQ);
            uint gameId = (uint)(int)results[0]["id"];
            GameModel game = new GameModel(gameId, duration);
            return game;
        }

        /// <summary>
        /// Gets or creates a new player, and returns its ID
        /// </summary>
        /// <param name="name">Name of the player</param>
        /// <returns></returns>
        private uint GetOrCreatePlayerID(string name)
        {
            // Insert if the player doesn't already exist, IGNORE otherwise
            string sql_statement = "INSERT IGNORE INTO Players (Name) VALUES(\"" + name + "\")";
            this.executeSQL(sql_statement);
            // Get player ID by name.
            string playerInfo = "SELECT id FROM Players WHERE Name=\"" + name + "\"";
            List<Dictionary<string, Object>> Player = this.executeSQL(playerInfo);
            return (uint)(int)Player[0]["id"];
        }

        public Dictionary<uint, GameModel> GetAllGames()
        {
            // Get Game Stats from DB
            string sql_statement = "SELECT Games.id, Games.duration, GamesPlayed.Score, GamesPlayed.Accuracy, Players.Name " +
                                    "FROM GamesPlayed " +
                                    "INNER JOIN Games ON GamesPlayed.gID=Games.id " +
                                    "INNER JOIN Players ON GamesPlayed.pID=Players.id;";
            List<Dictionary<string, Object>> Games = this.executeSQL(sql_statement);
            List<PlayerModel> players = new List<PlayerModel>();
            Dictionary<uint, GameModel> gameDict = new Dictionary<uint, GameModel>();

            // For every game, build a GameModel representing the DB state.
            foreach (Dictionary<string, Object> dict in Games)
            {
                GameModel game;
                if (gameDict.ContainsKey(Convert.ToUInt32(dict["id"])))
                {
                    game = gameDict[Convert.ToUInt32(dict["id"])];
                }
                else
                {
                    game = new GameModel(Convert.ToUInt32(dict["id"]), Convert.ToUInt32(dict["duration"]));
                    gameDict[(uint)(int)dict["id"]] = game;
                }
                game.AddPlayer((string)dict["Name"],
                               Convert.ToUInt32(dict["Score"]),
                               Convert.ToUInt32(dict["Accuracy"]));
            }
            return gameDict;
        }

        public List<SessionModel> GetPlayerGames(string playerName)
        {
            string sql_statement = "SELECT Games.id, Games.duration, GamesPlayed.Score, GamesPlayed.Accuracy, Players.Name " +
                                    "FROM GamesPlayed " +
                                    "INNER JOIN Games ON GamesPlayed.gID=Games.id " +
                                    "INNER JOIN Players ON GamesPlayed.pID=Players.id " +
                                    "WHERE Players.Name = \"" + playerName + "\"";
            List<Dictionary<string, Object>> Games = this.executeSQL(sql_statement);
            List<SessionModel> sessions = new List<SessionModel>();

            // For every game, build a SessionModel representing the DB state.
            foreach (Dictionary<string, Object> dict in Games)
            {
                SessionModel session = new SessionModel(Convert.ToUInt32(dict["id"]),
                                                        Convert.ToUInt32(dict["duration"]),
                                                        Convert.ToUInt32(dict["Score"]),
                                                        Convert.ToUInt32(dict["Accuracy"]));
                sessions.Add(session);
            }
            return sessions;
        }

    }
}
