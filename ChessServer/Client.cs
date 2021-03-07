using System.IO;
using System.Net.Sockets;

namespace ChessServer
{
    class Client
    {
        public TcpClient ClientCon { get; set; }
        public StreamWriter SW { get;}
        public StreamReader SR { get; }

        public string Nickname { get; set; }

        public Client(TcpClient tcpCient, string nickname)
        {
            ClientCon = tcpCient;
            Nickname = nickname;
            SW = new StreamWriter(ClientCon.GetStream()) { AutoFlush = true };
            SR = new StreamReader(ClientCon.GetStream());
        }
    }
}
