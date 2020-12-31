using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanasonicSerialServer;
using Serilog;

namespace PanasonicSerialServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .WriteTo.Debug()
                        .CreateLogger();


            Log.Debug("Hello");


            ServerConfig config = ServerConfig.Load();


            MqttServer mqttServer = new MqttServer(config);
            mqttServer.CreateServer();

            Log.Information("MQTT server started");


            Console.ReadLine();
        }
    }
}
