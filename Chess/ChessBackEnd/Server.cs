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

        private event Action<string> UpdateTable;
        private event Action<string> ConnectToLobby;

        private StreamWriter sw;
        private StreamReader sr;

        private TcpClient client;

        public Server(Action<string> updateTable, Action<string> connectToLobby)
        {
            UpdateTable = updateTable;
            ConnectToLobby = connectToLobby;
            try
            {
                var ipEP = new IPEndPoint(IPAddress.Parse(IP), PORT);
                client = new TcpClient();
                client.Connect(ipEP);

                sw = new StreamWriter(client.GetStream());
                sw.AutoFlush = true;
                sr = new StreamReader(client.GetStream());

                Listening();
            }
            catch (Exception e)
            {
                MessageBox.Show("Can't connect to server: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Creates new lobby on server if possible
        /// </summary>
        /// <param name="nickName">Nickname is gonna be passed on serve</param>
        /// <returns>New lobby id</returns>
        public void CreateNewLobby(string nickname)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    sw.Write($"NewLobby/{nickname}");

                    var data = sr.ReadToEnd();

                    if (data.Contains("NewLobby/"))
                    {
                        var lobbyID = data.Replace("NewLobby/", "");
                        ConnectToLobby(lobbyID);
                    }
                    else MessageBox.Show("Error: Invalid data received from server");
                }
                catch (Exception e)
                {
                    MessageBox.Show( $"Error while creating lobby: {e.Message}");
                }
            });
        }

        private void Listening()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var data = sr.ReadToEnd();

                        UpdateTable(data);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error while listening to server\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (!client.Client.Connected)
                            break;
                        
                    }
                }
            });
        }

        public void SendMove(string lobbyID, string move)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    sw.Write($"Move/{lobbyID}/{move}");

                    var data = sr.ReadToEnd();

                    if (data.Contains("ConfirmMove/"))
                    {
                        MessageBox.Show("Data send succesfully: " + data);
                    }
                    else
                    {
                        MessageBox.Show("Invalid data received when sending move to server: " + data);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e.Message);
                }
            });
        }
    }
}
