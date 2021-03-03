using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessServer
{
    class Program
    {
        const short PORT = 8080;

        static TcpListener server;
        static bool listen = true;

        static List<Lobby> lobbies = new List<Lobby>();
        static List<string> IDs = new List<string>();

        static void Main(string[] args)
        {
            server = new TcpListener(new IPEndPoint(IPAddress.Any, PORT));
            StartServerAsync();
            while (true)
            {
                var str = Console.ReadLine();
                if(str == "stop")
                {
                    listen = false;
                    server.Stop();
                    Console.WriteLine("Server Stopped");
                }
                else if(str == "start" && !listen)
                {
                    listen = true;
                    StartServerAsync();
                }
                else if(str == "info")
                {
                    Console.WriteLine("Amount lobbies: " + lobbies.Count);
                }
            }
        }

        /// <summary>
        /// Accepting new clients here
        /// </summary>
        static void StartServerAsync()
        {
            Console.WriteLine("Starting Server");
            server.Start();

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Waiting for clients to join...");
                while (listen)
                {
                    if (!server.Pending())
                    {
                        Thread.Sleep(50); 
                        continue;
                    }
                    // When new client connects to server we get new variable of TcpClient here
                    var client = (Client)server.AcceptTcpClient();

                    Console.WriteLine("New client");
                    // For each client new thread that will be listening to incoming data
                    //if(client.Connected)
                        new Thread(() => ListenToClient(client)) { IsBackground = true}.Start();
                }
            });
        }

        static void ListenToClient(Client client)
        {
            Console.WriteLine("New client added\t Socket connected to: " + client.Client.RemoteEndPoint.ToString());
            var sr = new StreamReader(client.GetStream());

            while (client.Connected)
            {
                var data = sr.ReadToEnd();

                ProcessCommand(client, data);
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
                    Console.WriteLine("Invalid message format received: " + command + "\tFrom: " + client.Client.RemoteEndPoint.ToString());
                    break;
            }
        }

        static void AddNewLobby(Client client, string nickname)
        {
            // Generating new id for lobby
            var id = new Random().Next(0, 10000).ToString();

            if (!IDs.Contains(id))
            {
                IDs.Add(id);
                lobbies.Add(new Lobby(id, client, nickname));

                var sw = new StreamWriter(client.GetStream());
                sw.AutoFlush = true;

                sw.Write($"NewLobby/{id}");

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
                    item.NewMove(client, move);
                
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
