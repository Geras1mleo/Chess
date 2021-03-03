using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ChessServer
{
    class Lobby
    {
        public readonly string LobbyID;

        public Client WhiteClient { get; set; }
        public Client BlackClient { get; set; }

        public List<string> Moves { get; set; }

        public Lobby(string lobbyID, Client client, string nickname)
        {
            LobbyID = lobbyID;
            //User that has made the lobby plays white
            AddClient(client, nickname);
        }

        public void AddClient(Client client, string nickname)
        {
            if (WhiteClient is null)
            {
                WhiteClient = client;
                WhiteClient.Nickname = nickname;
                Console.WriteLine("White player connected to lobby: " + LobbyID);
            }

            else if (BlackClient is null)
            {
                BlackClient = client;
                BlackClient.Nickname = nickname;
                Console.WriteLine("Black player connected to lobby: " + LobbyID);
            }
        }

        public void UserLeftNotify(TcpClient client)
        {
            try
            {
                if(client == WhiteClient)
                {
                    WhiteClient.Close();
                    WhiteClient = null;

                    var sw = new StreamWriter(BlackClient.GetStream());
                    sw.AutoFlush = true;

                    //TODO
                    sw.Write("");
                }
                else if (client == BlackClient)
                {
                    BlackClient.Close();
                    BlackClient = null;

                    var sw = new StreamWriter(WhiteClient.GetStream());
                    sw.AutoFlush = true;

                    //TODO
                    sw.Write("");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while notifying opponent that user left lobby: " + LobbyID + "\t Error message: " + e.Message);
            }
        }

        public void NewMove(Client client, string move)
        {
            // If someone else is trying to make move in this lobby...
            if (!(client == WhiteClient || client == BlackClient)) return;

            // If one of players is not connected...
            if (WhiteClient is null || BlackClient is null) return;

            try
            {
                var sww = new StreamWriter(WhiteClient.GetStream());
                sww.AutoFlush = true;

                var swb = new StreamWriter(BlackClient.GetStream());
                swb.AutoFlush = true;

                // Confirm move to the same client
                if (client == WhiteClient)
                {
                    swb.Write($"Move/{move}");
                    sww.Write($"ConfirmMove/{move}");
                }
                else if (client == BlackClient)
                {
                    sww.Write($"Move/{move}");
                    swb.Write($"ConfirmMove/{move}");
                }
                Moves.Add(move);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while writing new move in lobby: " + LobbyID + "\t Error message: " + e.Message);
            }
        }
    }
}
