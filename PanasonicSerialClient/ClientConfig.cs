using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanasonicSerialCommon;

namespace PanasonicSerialClient
{
    public class ClientConfig
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public Message Message { get; private set; }


        public ClientConfig(string host, int port, Message message)
        {
            this.Host = host;
            this.Port = port;
            this.Message = message;
        }
    }
}
