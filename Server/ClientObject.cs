using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientObject
    {
        protected internal string Id { get; }
        protected internal NetworkStream Stream { get; private set; }
        private TcpClient client;
        private string userName;
        private ServerObject server;

        public ClientObject(TcpClient client, ServerObject server)
        {
            this.client = client;
            this.server = server;
            Id = Guid.NewGuid().ToString();
            server.AddConnection(this);
        }
    }
}