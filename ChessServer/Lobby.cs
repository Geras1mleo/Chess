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

        private bool whiteRematch, blackRematch;

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

                White.SW.WriteLine($"Connected/{LobbyID}/White/{nickname}/{Black?.Nickname}");
                Black?.SW.WriteLine($"OpponentJoined/{White.Nickname}");

                Console.WriteLine($"White player: {nickname} has connected to lobby: " + LobbyID);
            }
            else if (Black is null)
            {
                Black = new Client(client, nickname);

                Black.SW.WriteLine($"Connected/{LobbyID}/Black/{nickname}/{White?.Nickname}");
                White?.SW.WriteLine($"OpponentJoined/{Black.Nickname}");

                Console.WriteLine($"Black player: {nickname} has connected to lobby: " + LobbyID);
            }
            else
            {
                Program.ReportError(client, "This lobby is not aviable");
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
            try
            {
                // Confirm move to the same client
                if (client == White?.ClientCon)
                {
                    Black?.SW.WriteLine($"NewMove/{move}/{parameters}");
                    White?.SW.WriteLine($"ConfirmedMove/{move}/{parameters}");
                    Moves.Add(move);
                }
                else if (client == Black?.ClientCon)
                {
                    White?.SW.WriteLine($"NewMove/{move}/{parameters}");
                    Black?.SW.WriteLine($"ConfirmedMove/{move}/{parameters}");
                    Moves.Add(move);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while writing new move in lobby: " + LobbyID + "\nError message: " + e.Message);
            }
        }

        public void RematchRequest(TcpClient client)
        {
            if(White is null || Black is null)
            {
                Program.ReportError(client, "Your opponent has already left the lobby");
                blackRematch = false; whiteRematch = false;
                return;
            }

            try
            {
                if (client == White.ClientCon)
                {
                    whiteRematch = true;

                    if(!blackRematch)
                        Black.SW.WriteLine("RematchRequest");
                }
                else if (client == Black.ClientCon)
                {
                    blackRematch = true;

                    if(!whiteRematch)
                        White.SW.WriteLine("RematchRequest");
                }
            }
            catch (Exception) { }

            if(blackRematch && whiteRematch)    
                SendRematch();
        }

        public void SendRematch()
        {
            try
            {
                White.SW.WriteLine("RematchConfirmed");
                Black.SW.WriteLine("RematchConfirmed");
                Moves.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while sending rematch confirmation\nError message: " + e.Message);
            }
        }
    }
}
