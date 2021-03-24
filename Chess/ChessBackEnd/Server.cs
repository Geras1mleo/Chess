using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.IO;

namespace Chess.ChessBackEnd
{
    class Server
    {
        const short PORT = 8080;
        const string IP = "93.188.166.178";

        private event Action<string, string, string, string> ConnectToLobbyHandler;
        private event Action<string, string> TableMovesHandler;
        private event Action<string> OpponentJoinedHandler;
        private event Action OpponentLeftHandler;

        private TcpClient client;

        private StreamWriter sw;
        private StreamReader sr;

        public Server(Action<string, string, string, string> connectToLobbyHandler, 
                    Action<string> opponentJoinedHandler, 
                    Action<string, string> tableMovesHandler, 
                    Action opponentLeftHandler)
        {
            ConnectToLobbyHandler = connectToLobbyHandler;
            OpponentJoinedHandler = opponentJoinedHandler;
            TableMovesHandler = tableMovesHandler;
            OpponentLeftHandler = opponentLeftHandler;

            ConnectToServerAsync();
        }

        /// <summary>
        /// Connects and listens to incoming data from the server asyncronously
        /// </summary>
        public void ConnectToServerAsync() => new Task(ConnectToServer).Start();
        public void ConnectToServer()
        {
            if (!(client?.Connected == true))
            {
                try
                {
                    var ipEP = new IPEndPoint(IPAddress.Parse(IP), PORT);

                    client = new TcpClient();
                    client.Connect(ipEP);

                    sw = new StreamWriter(client.GetStream()) { AutoFlush = true };
                    sr = new StreamReader(client.GetStream());

                    ListenToServer();
                }
                catch (Exception e)
                {
                    if(client != null)
                        MessageBox.Show("Cannot connect to server: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    client = null;
                    sw = null;
                    sr = null;
                }
            }
        }
        
        private void ListenToServer()
        {
            try
            {
                while (true)
                {
                    var data = sr?.ReadLine();
                    if (data is null) continue;

                    ProcessCommand(data);
                }
            }
            catch (Exception e)
            {
                if(client is null)
                    throw e;
                else MessageBox.Show("Error while listening to server: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                sw?.WriteLine($"Connect/{lobbyID}/{nickname}");
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

        public void RematchRequestAsync(string lobbyID) => new Task(() => RematchRequest(lobbyID)).Start();
        private void RematchRequest(string lobbyID)
        {
            try
            {
                sw?.WriteLine($"RematchRequest/{lobbyID}");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while sending rematch request: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessCommand(string command)
        {
            string[] parameters = command.Split('/');
            switch (parameters[0])
            {
                case "Connected":
                    ConnectToLobbyHandler(parameters[1], parameters[2], parameters[3], parameters[4]);
                    break;
                case "OpponentJoined":
                    OpponentJoinedHandler(parameters[1]);
                    break;
                case "NewMove":
                    TableMovesHandler(parameters[1], parameters[2]);
                    break;
                case "ConfirmedMove":
                    //TableMovesHandler(parameters[1], parameters[2]);
                    break;
                case "OpponentLeft":
                    OpponentLeftHandler();
                    break;
                case "Error":
                    MessageBox.Show(parameters[1], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        public void Disconnect()
        {
            client?.Close();
            client = null;
        }
    }
}
