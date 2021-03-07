using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.IO;

namespace Chess.ChessBackEnd
{
    class Server
    {
        const short PORT = 8080;
        const string IP = "93.188.166.178";

        private event Action<string, string, string> ConnectToLobbyHandler;
        private event Action<string, string> TableMovesHandler;
        private event Action<string> OpponentJoinedHandler;
        private event Action OpponentLeftHandler;

        private readonly StreamWriter sw;
        private readonly StreamReader sr;

        private readonly TcpClient client;

        public Server(Action<string, string, string> connectToLobbyHandler, 
                    Action<string> opponentJoinedHandler, 
                    Action<string, string> tableMovesHandler, 
                    Action opponentLeftHandler)
        {
            ConnectToLobbyHandler = connectToLobbyHandler;
            OpponentJoinedHandler = opponentJoinedHandler;
            TableMovesHandler = tableMovesHandler;
            OpponentLeftHandler = opponentLeftHandler;

            try
            {
                var ipEP = new IPEndPoint(IPAddress.Parse(IP), PORT);

                client = new TcpClient();
                client.Connect(ipEP);

                sw = new StreamWriter(client.GetStream()) { AutoFlush = true };
                sr = new StreamReader(client.GetStream());

                ListenToServerAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show("Can't connect to server: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Listens to incoming data from the server asyncronously
        /// </summary>
        private void ListenToServerAsync() => new Task(ListenToServer).Start();
        private void ListenToServer()
        {
            while (true)
            {
                try
                {
                    var data = sr?.ReadLine();

                    ProcessCommand(data);
                }
                catch (Exception e)
                {
                    if (!client.Client.Connected)
                        break;
                    MessageBox.Show("Error while listening to server: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        /// <summary>
        /// Creates new lobby on server if possible
        /// </summary>
        /// <param name="nickName">Nickname is gonna be passed on serve</param>
        /// <returns>New lobby id</returns>
        public void CreateNewLobbyAsync(string nickname) => new Task(()=>CreateNewLobby(nickname)).Start();
        private void CreateNewLobby(string nickname)
        {
            try
            {
                sw?.WriteLine($"CreateLobby/{nickname}");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while creating lobby: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ConnectToLobbyAsync(string lobbyID, string nickname) => new Task(() => ConnectToLobby(lobbyID, nickname)).Start();
        private void ConnectToLobby(string lobbyID, string nickname)
        {
            try
            {
                sw.WriteLine($"Connect/{lobbyID}/{nickname}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while connecting lobby: {e.Message}");
            }
        }

        public void SendMoveAsync(string lobbyID, string move, string parameters) => new Task(()=> { SendMove(lobbyID, move, parameters); }).Start();
        private void SendMove(string lobbyID, string move, string parameters)
        {
            try
            {
                sw?.WriteLine($"Move/{lobbyID}/{move}/{parameters}");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while sending move: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        public void LeaveLobbyAsync(string lobbyID) => new Task(() => { LeaveLobby(lobbyID); }).Start();
        private void LeaveLobby(string lobbyID)
        {
            try
            {
                sw?.WriteLine($"LeaveLobby/{lobbyID}");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while leaving lobby: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessCommand(string command)
        {
            string[] parameters = command.Split('/');
            switch (parameters[0])
            {
                case "Connected":
                    ConnectToLobbyHandler(parameters[1], parameters[2], parameters[3]);
                    break;
                case "OpponentJoined":
                    OpponentJoinedHandler(parameters[1]);
                    break;
                case "NewMove":
                    TableMovesHandler(parameters[1], parameters[2]);
                    break;
                case "ConfirmedMove":
                    //MessageBox.Show("Confirmed: " + parameters[1]);
                    break;
                case "OpponentLeft":
                    OpponentLeftHandler();
                    break;
                default:
                    MessageBox.Show("Non-valid data from server received " + command);
                    break;
            }
        }

        public void Disconnect() => client.Close();
    }
}
