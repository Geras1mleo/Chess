using System.Net.Sockets;

namespace ChessServer
{
    class Client
    {
        public TcpClient ClientCon { get; set; }
        public string Nickname { get; set; }

        public Client(TcpClient tcpCient, string nickname)
        {
            ClientCon = tcpCient;
            Nickname = nickname;
        }
    }
}
