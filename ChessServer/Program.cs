using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChessServer
{
    class Program
    {
        const short PORT = 8080;

        static readonly TcpListener server = new TcpListener(new IPEndPoint(IPAddress.Any, PORT));

        static readonly List<Lobby> lobbies = new List<Lobby>();
        static readonly List<string> IDs = new List<string>();

        static void Main()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var str = Console.ReadLine();

                    switch (str)
                    {
                        case "info":
                            Console.WriteLine("Amount lobbies: " + lobbies.Count);
                            foreach (var item in IDs)
                                Console.Write(item + "\t");
                            continue;
                        case "clear":
                            lobbies.Clear();
                            IDs.Clear();
                            Console.WriteLine("Lobbies has been deleted succesfully!");
                            continue;
                        case "stop":
                            server.Stop();
                            break;
                        case "moves":
                            Console.Write("Enter lobby id: ");
                            string id = Console.ReadLine();
                            foreach (var item in lobbies)
                            {
                                if (item.LobbyID == id)
                                {
                                    foreach (var move in item.Moves)
                                        Console.WriteLine(move);
                                    break;
                                }
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown command");
                            continue;
                    }
                }
            });
            StartServer();
        }

        /// <summary>
        /// Accepting new clients here
        /// IN MAIN THREAD
        /// </summary>
        static void StartServer()
        {
            Console.WriteLine("Starting Server");
            server.Start();
            
            Console.WriteLine("Waiting for clients to join...");
            try
            {
                while (true)
                {
                    if (!server.Pending())
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    // When new client connects to server we get new variable of TcpClient here
                    var client = server.AcceptTcpClient();

                    // For each client new thread that will be listening to incoming data
                    if(client.Connected)
                        new Thread(() => ListenToClient(client)).Start();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Server stopped");
                Environment.Exit(0);
            }
        }

        static void ListenToClient(TcpClient client)
        {
            try
            {
                Console.WriteLine("New client added \nSocket connected to: " + client.Client.RemoteEndPoint.ToString());
                var sr = new StreamReader(client.GetStream());

                while (client.Connected)
                {
                    var data = sr.ReadLine();
                    if (data is null)
                    {
                        client.Close();
                        Console.WriteLine("Disconnected client");
                        break;
                    }

                    ProcessCommand(client, data);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Disconnected client");
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
        static void ProcessCommand(TcpClient client, string command)
        {
            var parameters = command.Split('/');
            switch (parameters[0])
            {
                case "CreateLobby":
                    AddNewLobby(client, parameters[1]);
                    break;
                case "Connect":
                    ConnectToLobby(client, parameters[1], parameters[2]);
                    break;
                case "Move":
                    NewMove(client, parameters[1], parameters[2], parameters[3]);
                    break;
                case "LeaveLobby":
                    LeaveLobby(client, parameters[1]);
                    break;
                case "RematchRequest":
                    RematchRequest(client, parameters[1]);
                    break;
                default:
                    Console.WriteLine("Invalid message format received: " + command + "\nFrom: " + client.Client.RemoteEndPoint.ToString());
                    client.Close();
                    Console.WriteLine("Connection has been forced closed");
                    break;
            }
        }

        static void AddNewLobby(TcpClient client, string nickname)
        {
            // Generating new id for lobby
            var id = new Random().Next(0, 10000).ToString();

            if (!IDs.Contains(id))
            {
                IDs.Add(id);
                Console.WriteLine("New Lobby created: " + id);

                lobbies.Add(new Lobby(id, client, nickname));
            }
            else
            {
                AddNewLobby(client, nickname);
                return;
            }
        }

        static void ConnectToLobby(TcpClient client, string lobbyID, string nickname)
        {
            foreach (var item in lobbies)
            {
                if (item.LobbyID == lobbyID)
                {
                    item.AddClient(client, nickname);
                    return;
                }
            }
            ReportError(client, $"Lobby {lobbyID} does not exist, please try again");
        }

        static void NewMove(TcpClient client, string lobbyID, string move, string parameters)
        {
            foreach (var item in lobbies)
            {
                if(item.LobbyID == lobbyID)
                {
                    item.NewMove(client, move, parameters);
                    return;
                }
            }
        }

        static void LeaveLobby(TcpClient client, string lobbyID)
        {
            var id = IDs.IndexOf(lobbyID);

            if(id > -1)
            {
                lobbies[id].UserLeft(client);

                if (lobbies[id].Black is null && lobbies[id].White is null)
                {
                    lobbies.Remove(lobbies[id]);
                    IDs.Remove(lobbyID);
                    Console.WriteLine("Lobby: " + lobbyID + " has been deleted");
                }
            }
        }

        static void RematchRequest(TcpClient client, string lobbyID)
        {
            var id = IDs.IndexOf(lobbyID);

            if (id > -1)
            {
                lobbies[id].RematchRequest(client);
            }
        }

        public static void ReportError(TcpClient client, string message)
        {
            try
            {
                var sw = new StreamWriter(client.GetStream()) { AutoFlush = true };
                sw.WriteLine("Error/" + message);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error while reporting error\n" + e.Message);
            }
        }
    }
}
