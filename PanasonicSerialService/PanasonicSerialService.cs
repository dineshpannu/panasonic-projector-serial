using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using PanasonicSerialServer;
using Serilog;

namespace PanasonicSerialService
{
    public partial class PanasonicSerialService : ServiceBase
    {
        private MqttServer mqttServer = null;

        public PanasonicSerialService()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // Setup logging
            //
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.EventLog("Panasonic Serial Service", manageEventSource:true)
                .CreateLogger();
            Log.Information("Starting PanasonicSerialService");


            // Read config
            //
            ServerConfig config = ServerConfig.Load();


            // Create MQTT server
            //
            this.mqttServer = new MqttServer(config);
            this.mqttServer.CreateServer();

        }


        /// <summary>
        /// Stop the service by disposing server.
        /// </summary>
        protected override void OnStop()
        {
            Log.Information("Stopping PanasonicSerialService");
            this.mqttServer.Dispose();
            this.mqttServer = null;
        }
    }
}
