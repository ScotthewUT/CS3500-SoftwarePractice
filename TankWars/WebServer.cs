// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 4 Dec 2019


using MySql.Data.MySqlClient;
using NetworkUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;

namespace TankWars
{
    public class WebServer
    {
        private TcpListener _tcplistener;

        private int _port;
        private DatabaseController _dbControl;

        public WebServer(int port)
        {
            _port = port;
            _dbControl = new DatabaseController();
        }

        /// <summary>
        /// Start accepting sockets from clients.
        /// </summary>
        public void StartServer()
        {
            _tcplistener = Networking.StartServer(HandleHttpConnection, _port);
            Console.WriteLine("WebServer is running on port "+_port+".");
        }

        /// <summary>
        /// Stops server
        /// </summary>
        public void StopServer()
        {
            Console.WriteLine("Shutting Down Webserver");
            Networking.StopServer(_tcplistener);
        }

        /// <summary>
        /// Handles incoming HTTP requests to the server
        /// </summary>
        /// <param name="state"></param>
        private void HandleHttpConnection(SocketState state)
        {
            state.OnNetworkAction = ServeHttpRequest;
            Networking.GetData(state);
        }

        /// <summary>
        /// Serves HTTP Response based on the URI and/or URL params present in the request URL
        /// </summary>
        /// <param name="state">Active SocketState</param>
        private void ServeHttpRequest(SocketState state)
        {
            string request = state.GetData();
            String resultString = "";
            if (request.Contains("GET"))
            {
                string playerTag = "?player=";
                if (request.Contains(playerTag))
                {
                    int startIndex = request.IndexOf(playerTag) + playerTag.Length;
                    int endIndex = request.IndexOf("HTTP/1.1");
                    if (endIndex - startIndex > 0)
                    {
                        string queryParam = request.Substring(startIndex, endIndex - startIndex);
                        List<SessionModel> results = _dbControl.GetPlayerGames(queryParam);
                        resultString = WebViews.GetPlayerGames(queryParam, results);
                    }
                }
                else if (request.Contains("/games"))
                {
                    Dictionary<uint, GameModel> allGames = _dbControl.GetAllGames();
                    resultString = WebViews.GetAllGames(allGames);
                }
                else
                    resultString = WebViews.GetHomePage();
            }
            else
            {
                resultString = WebViews.Get404();
            }

            Networking.SendAndClose(state.TheSocket, resultString);

        }
    }


}
