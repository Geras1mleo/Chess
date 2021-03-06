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

        public Client White { get; set; }
        public Client Black { get; set; }

        public List<string> Moves { get; set; }

        public Lobby(string lobbyID, TcpClient client, string nickname)
        {
            LobbyID = lobbyID;
            //User that has made the lobby plays white
            AddClient(client, nickname);
        }

        public void AddClient(TcpClient client, string nickname)
        {
            if (White is null)
            {
                White = new Client(client, nickname);
                Console.WriteLine("White player connected to lobby: " + LobbyID);
            }

            else if (Black is null)
            {
                Black = new Client(client, nickname);
                Console.WriteLine("Black player connected to lobby: " + LobbyID);
            }
        }

        public void UserLeftNotify(TcpClient client)
        {
            try
            {
                if(client == White.ClientCon)
                {
                    White.ClientCon.Close();
                    White = null;

                    var sw = new StreamWriter(Black.ClientCon.GetStream());
                    sw.AutoFlush = true;

                    //TODO
                    //sw.Write("");
                }
                else if (client == Black.ClientCon)
                {
                    Black.ClientCon.Close();
                    Black = null;

                    var sw = new StreamWriter(White.ClientCon.GetStream());
                    sw.AutoFlush = true;

                    //TODO
                    //sw.Write("");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while notifying opponent that user left lobby: " + LobbyID + "\t Error message: " + e.Message);
            }
        }

        public void NewMove(TcpClient client, string move)
        {
            // If someone else is trying to make move in this lobby...
            if (!(client == White.ClientCon|| client == Black.ClientCon)) return;

            // If one of players is not connected...
            //if (White is null || Black is null) return;

            try
            {
                var sww = new StreamWriter(White.ClientCon.GetStream()) { AutoFlush = true };
                var swb = new StreamWriter(Black.ClientCon.GetStream()) { AutoFlush = true };

                // Confirm move to the same client
                if (client == White.ClientCon)
                {
                    swb.WriteLine($"Move/{move}");
                    sww.WriteLine($"ConfirmMove/{move}");
                }
                else if (client == Black.ClientCon)
                {
                    sww.WriteLine($"Move/{move}");
                    swb.WriteLine($"ConfirmMove/{move}");
                }
                Moves.Add(move);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while writing new move in lobby: " + LobbyID + "\nError message: " + e.Message);
            }
        }
    }
}
