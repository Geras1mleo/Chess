using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chess.ChessBackEnd
{
    class Server
    {
        private event Action<string> UpdateTable;

        public Server(Action<string> updateTable)
        {
            this.UpdateTable = updateTable;
            //TODO Connection
            
        }

        /// <returns>returns function that makes new lobby on server</returns>
        public Func<string, string> GetCreateLobbyAction() => CreateNewLobby;
        
        /// <summary>
        /// Creates new lobby on server if possible
        /// </summary>
        /// <param name="nickName">Nickname is gonna be passed on serve</param>
        /// <returns>New lobby id</returns>
        private string CreateNewLobby(string nickname)
        {
            try
            {
                //TODO
                return "12345";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        private void Listening()
        {
            Task.Factory.StartNew(() =>
            {
                UpdateTable("");
            });
        }
    }
}
