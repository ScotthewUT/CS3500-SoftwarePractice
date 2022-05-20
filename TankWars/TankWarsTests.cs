// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 22 November 2019

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TankWars
{
    [TestClass]
    public class TankWarsTests
    {
        [TestMethod]
        public void TestTankJSONConstructor()
        {
            string jsonTank = "{'tank':0,'loc':{'x':220.995264,'y':-235.63331367},'bdir':{'x':1.0,'y':0.0},'tdir':{'x':-0.795849908004867,'y':0.60549395036502607},'name':'Danny','hp':3,'score':0,'died':false,'dc':false,'join':false}";

            Tank tank = JsonConvert.DeserializeObject<Tank>(jsonTank);

            Assert.AreEqual(0, tank.ID);
            Assert.AreEqual("Danny", tank.Name);
        }

        [TestMethod]
        public void DemoStringSplitMethod()
        {
            string testStr = "FirstSetOfData\\n2ndSetOfData\\n3rdSetOfData\\nDon'tNeedThisData";
            string[] data = testStr.Split(new string[] {"\\n"}, 3, StringSplitOptions.None);

            Assert.AreEqual(3, data.Length);

            testStr = "FirstSetOfData\\n2ndSetOfData\\n";
            data = testStr.Split(new string[] { "\\n" }, 3, StringSplitOptions.None);

            Assert.AreEqual(3, data.Length);
            Assert.AreEqual("", data[2]);
        }

        [TestMethod]
        public void TestSerializeDictionaryToCommandString()
        {
            Dictionary<string, Object> dict = new Dictionary<string, Object>();

            dict.Add("moving", "up");
            dict.Add("fire", "alt");
            dict.Add("tdir", new Vector2D(1.0, 0.0));

            string commandStr = JsonConvert.SerializeObject(dict);

            Assert.AreEqual("{\"moving\":\"up\",\"fire\":\"alt\",\"tdir\":{\"x\":1.0,\"y\":0.0}}", commandStr);
        }

        /*        [TestMethod] // To get this working, you'll need to make both the class and readsettings public
                public void TestFileReading()
                {
                    Dictionary<string, Object> test = TankWarsServer.readSettings();
                    string done = "done";
                }*/

        [TestMethod]
        public void TestSQLReading()
        {
            DatabaseController cont = new DatabaseController();
            Dictionary<uint,GameModel>test = cont.GetAllGames();
            List<SessionModel> playerTest = cont.GetPlayerGames("David");
        }

        [TestMethod]
        public void TestSQLWriting()
        {
            DatabaseController cont = new DatabaseController();
            string sql_statement = "INSERT IGNORE INTO Players (Name) VALUES(\"" + "Player" + "\"";
            cont.executeSQL(sql_statement);
            string playerInfo = "SELECT id from Players WHERE Name=\"" + "Player" + "\"";
            List<Dictionary<string, Object>> Player = cont.executeSQL(playerInfo);

        }

        [TestMethod]
        public void TestGameSave()
        {
            DatabaseController cont = new DatabaseController();
            GameModel game = cont.CreateGame(5000);
            game.AddPlayer("Player2", 15, 3);
            game.AddPlayer("Player3", 1, 0);
            game.AddPlayer("Test", 100, 15);
            cont.SaveGameStats(game);
            string sql_statement = "SELECT * FROM GamesPlayed WHERE gID = " + game.ID;
            Dictionary<string, Object> savedResults = cont.executeSQL(sql_statement)[0];

        }
    }
}
