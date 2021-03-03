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
        const short PORT = 5853;
        const string IP = "";
        private event Action<string> UpdateTable;
        private event Action<string> ConnectToLobby;

        private TcpClient client;

        public Server(Action<string> updateTable, Action<string> connectToLobby)
        {
            UpdateTable = updateTable;
            ConnectToLobby = connectToLobby;
            try
            {
                var ipEP = new IPEndPoint(IPAddress.Parse(IP), PORT);
                client = new TcpClient(ipEP);
                Listening();
            }
            catch (Exception)
            {
                MessageBox.Show("Can't connect to server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// Creates new lobby on server if possible
        /// </summary>
        /// <param name="nickName">Nickname is gonna be passed on serve</param>
        /// <returns>New lobby id</returns>
        public string CreateNewLobby(string nickname)
        {
            try
            {
                var stream = client.GetStream();
                stream.ReadTimeout = 1500;

                var sw = new StreamWriter(stream);
                sw.AutoFlush = true;

                sw.Write($"NewLobby/{nickname}");

                var sr = new StreamReader(stream);
                
                var data = sr.ReadToEnd();

                if (data.Contains("NewLobby/"))
                {
                    var lobbyID = data.Replace("NewLobby/", "");
                    ConnectToLobby(lobbyID);

                    return lobbyID;
                }
                
                else return "Error: Invalid data received from server";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        private void Listening()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var sr = new StreamReader(client.GetStream());
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
            try
            {
                var stream = client.GetStream();
                stream.ReadTimeout = 1500;

                var sw = new StreamWriter(stream);
                sw.AutoFlush = true;

                sw.Write($"Move/{lobbyID}/{move}");

                var sr = new StreamReader(stream);
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
        }
    }
}
