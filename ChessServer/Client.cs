using System.Net.Sockets;

namespace ChessServer
{
    class Client : TcpClient
    {
        public string Nickname { get; set; }
    }
}
