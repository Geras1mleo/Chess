using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    class Program
    {
        const short PORT = 5853;

        static TcpListener server;
        static List<Lobby> lobbies = new List<Lobby>();
        static List<string> IDs = new List<string>();

        static void Main(string[] args)
        {
            server = new TcpListener(new IPEndPoint(IPAddress.Any, PORT));
            StartServerAsync();
        }

        /// <summary>
        /// Accepting new clients here
        /// </summary>
        static void StartServerAsync()
        {
            server.Start();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // When new client connects to server we get new variable of TcpClient here
                    Client client = (Client)server.AcceptTcpClient();

                    // For each client new thread that will be listening to incoming data
                    Task.Factory.StartNew(() => ListenToClient(client));
                }
            });
        }

        static void ListenToClient(Client client)
        {
            var sr = new StreamReader(client.GetStream());
            Console.WriteLine("New client added\t Socket connected to: " + client.Client.RemoteEndPoint.ToString());

            while (client.Connected)
            {
                var str = sr.ReadToEnd();

                ProcessCommand(client, str);
            }
            // Player disconnected => notify opponent
            if (!client.Connected)
            {
                foreach (var item in lobbies)
                {
                    if (item.WhiteClient == client || item.BlackClient == client)
                        item.UserLeftNotify(client);
                }
            }
        }
        /// <summary>
        /// Make new lobby:
        /// NewLobby/*nickname*
        /// 
        /// <br/><br/>
        /// 
        /// Make new Move:
        /// Move/*lobbyID*/*move*
        /// 
        /// <br/><br/>
        /// 
        /// Connect to existing lobby:
        /// Connect/*lobbyID*/*nickname*
        /// 
        /// <br/><br/>
        /// 
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="command"></param>
        static void ProcessCommand(Client client, string command)
        {
            var parameters = command.Split('/');
            switch (parameters[0])
            {
                case "NewLobby":
                    AddNewLobby(client, parameters[1]);
                    break;
                case "Move":
                    NewMove(client, parameters[1], parameters[2]);
                    break;
                case "Connect":
                    ConnectToLobby(client, parameters[1], parameters[2]);
                    break;
                default:
                    Console.WriteLine("Invalid message format received: " + command);
                    break;
            }
        }
        static void AddNewLobby(Client client, string nickname)
        {
            var id = new Random().Next(0, 10000).ToString();
            if (!IDs.Contains(id))
            {
                IDs.Add(id);
                lobbies.Add(new Lobby(id, client, nickname));
                Console.WriteLine("New Lobby created: " + id);
            }
            else
            {
                AddNewLobby(client, nickname);
                return;
            }
        }

        static void NewMove(Client client, string lobbyID, string move)
        {
            foreach (var item in lobbies)
            {
                if(item.LobbyID == lobbyID)
                {
                    item.NewMove(client, move);
                }
            }
        }

        static void ConnectToLobby(Client client, string lobbyID, string nickname)
        {
            foreach (var item in lobbies)
            {
                if (item.LobbyID == lobbyID)
                    item.AddClient(client, nickname);
            }
        }
    }
}
