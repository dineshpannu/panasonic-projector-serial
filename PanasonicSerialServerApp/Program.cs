using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanasonicSerialCommon;
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


            Config config = Config.Load();


            MqttServer mqttSErver = new MqttServer(config);
            mqttSErver.CreateServer();

            Log.Information("MQTT server started");


            Console.ReadLine();
        }
    }
}
