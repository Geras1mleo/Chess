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

        private event Action<string> TableMovesHandler;
        private event Action<string> ConnectToLobbyHandler;

        private StreamWriter sw;
        private StreamReader sr;

        private TcpClient client;

        public Server(Action<string> updateTable, Action<string> connectToLobby)
        {
            TableMovesHandler = updateTable;
            ConnectToLobbyHandler = connectToLobby;
            try
            {
                var ipEP = new IPEndPoint(IPAddress.Parse(IP), PORT);

                client = new TcpClient();
                client.Connect(ipEP);

                sw = new StreamWriter(client.GetStream());
                sw.AutoFlush = true;
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
                    var data = sr.ReadLine();
                    MessageBox.Show(data);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error while listening to server\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (!client.Client.Connected)
                        break;

                }
            }
        }

        /// <summary>
        /// Creates new lobby on server if possible
        /// </summary>
        /// <param name="nickName">Nickname is gonna be passed on serve</param>
        /// <returns>New lobby id</returns>
        public void CreateNewLobbyAsync(string nickname) => new Task(()=>CreateNewLobby(nickname)).Start();
        public void CreateNewLobby(string nickname)
        {
            try
            {
                sw.WriteLine($"NewLobby/{nickname}");

                MessageBox.Show("Request sent");

                var data = sr.ReadLine();

                if (data.Contains("NewLobby/"))
                {
                    var lobbyID = data.Replace("NewLobby/", "");
                    ConnectToLobbyHandler(lobbyID);
                }
                else MessageBox.Show("Error: Invalid data received from server: " + data);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while creating lobby: {e.Message}");
            }
        }

        public void SendMoveAsync(string lobbyID, string move) => new Task(()=> { SendMove(lobbyID, move); }).Start();
        private void SendMove(string lobbyID, string move)
        {
            try
            {
                sw.WriteLine($"Move/{lobbyID}/{move}");
                var data = sr.ReadLine();
                MessageBox.Show(data);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
            
        }
    }
}
