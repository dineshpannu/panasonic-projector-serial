using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanasonicSerialCommon;
using Serilog;

namespace PanasonicSerialClient
{
    public class ClientConfig
    {
        private const string HostKey = "Server";
        private const string DefaultHost = "localhost";

        public string Host { get; private set; } = DefaultHost;
        public int Port { get; private set; } = CommonConfig.DefaultServerPort;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        public ClientConfig(string host, int port)
        {
            Log.Debug("host: {Host}, port: {Port}", host, port);

            this.LoadFromSettings();

            if (!string.IsNullOrEmpty(host))
            {
                this.Host = host;
            }

            if (0 != port)
            {
                this.Port = port;
            }
        }


        /// <summary>
        /// Parse appSettings in application config
        /// </summary>
        private void LoadFromSettings()
        {
            if (int.TryParse(ConfigurationManager.AppSettings[CommonConfig.ServerPortKey], out int serverPort))
            {
                Log.Debug("Port read from appSettings: {Port}", serverPort);
                this.Port = serverPort;
            }

            string host = ConfigurationManager.AppSettings[HostKey];
            Log.Debug("Host read from appSettings: {Host}", host);
            if (!string.IsNullOrEmpty(host))
            {
                this.Host = host;
            }
        }
    }
}
