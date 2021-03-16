using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ChessServer
{
    class Lobby
    {
        public readonly string LobbyID;

        public Client White { get; set; }
        public Client Black { get; set; }

        public List<string> Moves { get; set; } = new List<string>();

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

                White.SW.WriteLine($"Connected/{LobbyID}/White/{Black?.Nickname}");
                Black?.SW.WriteLine($"OpponentJoined/{White.Nickname}");

                Console.WriteLine($"White player: {nickname} has connected to lobby: " + LobbyID);
            }
            else if (Black is null)
            {
                Black = new Client(client, nickname);

                Black.SW.WriteLine($"Connected/{LobbyID}/Black/{White?.Nickname}");
                White?.SW.WriteLine($"OpponentJoined/{Black.Nickname}");

                Console.WriteLine($"Black player: {nickname} has connected to lobby: " + LobbyID);
            }
            else
            {
                Program.RepportError(client, "This lobby is not aviable");
            }
        }

        public void UserLeft(TcpClient client)
        {
            try
            {
                if(client == White?.ClientCon)
                {
                    White = null;

                    Black?.SW.WriteLine("OpponentLeft");
                    Moves.Clear();
                    Console.WriteLine("White player left lobby: " + LobbyID);
                }
                else if (client == Black?.ClientCon)
                {
                    Black = null;

                    White?.SW.WriteLine("OpponentLeft");
                    Moves.Clear();
                    Console.WriteLine("Black player left lobby: " + LobbyID);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while notifying opponent that user left lobby: " + LobbyID + "\nError message: " + e.Message);
            }
        }

        public void NewMove(TcpClient client, string move, string parameters)
        {
            // If someone else is trying to make move in this lobby...
            if (!(client == White.ClientCon || client == Black.ClientCon)) return;

            // If one of players is not connected...
            if (White is null || Black is null) return;

            try
            {
                // Confirm move to the same client
                if (client == White.ClientCon)
                {
                    Black.SW.WriteLine($"NewMove/{move}/{parameters}");
                    White.SW.WriteLine($"ConfirmedMove/{move}/{parameters}");
                }
                else if (client == Black.ClientCon)
                {
                    White.SW.WriteLine($"NewMove/{move}/{parameters}");
                    Black.SW.WriteLine($"ConfirmedMove/{move}/{parameters}");
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
