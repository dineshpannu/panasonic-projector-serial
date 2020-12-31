using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PanasonicSerialCommon;
using Serilog;

namespace PanasonicSerialServer
{
    public class ServerConfig
    {
        private const string ComPortKey = "ComPort";
        private const string DefaultComPort = "COM1";

        /// <summary>
        /// TCP port to run MQTT server on
        /// </summary>
        public int ServerPort { get; set; } = CommonConfig.DefaultServerPort;
        public string ComPort { get; set; }
        public Dictionary<LensEnum, double> LensAspectRatios { get; set; } = new Dictionary<LensEnum, double>();


        public static ServerConfig Load()
        {
            ServerConfig config = new ServerConfig();

            if (int.TryParse(ConfigurationManager.AppSettings[CommonConfig.ServerPortKey], out int serverPort))
            {
                config.ServerPort = serverPort;
            }

            config.LensAspectRatios[LensEnum.LensMemory1] = AspectRatio.Parse(ConfigurationManager.AppSettings[LensEnum.LensMemory1.ToString()]);
            config.LensAspectRatios[LensEnum.LensMemory2] = AspectRatio.Parse(ConfigurationManager.AppSettings[LensEnum.LensMemory2.ToString()]);
            config.LensAspectRatios[LensEnum.LensMemory3] = AspectRatio.Parse(ConfigurationManager.AppSettings[LensEnum.LensMemory3.ToString()]);
            config.LensAspectRatios[LensEnum.LensMemory4] = AspectRatio.Parse(ConfigurationManager.AppSettings[LensEnum.LensMemory4.ToString()]);
            config.LensAspectRatios[LensEnum.LensMemory5] = AspectRatio.Parse(ConfigurationManager.AppSettings[LensEnum.LensMemory5.ToString()]);
            config.LensAspectRatios[LensEnum.LensMemory6] = AspectRatio.Parse(ConfigurationManager.AppSettings[LensEnum.LensMemory6.ToString()]);

            config.ComPort = FindComPort(ConfigurationManager.AppSettings[ComPortKey]);

            return config;
        }



        private static string FindComPort(string configuredPort)
        {
            string comPort = DefaultComPort;


            if (string.IsNullOrEmpty(configuredPort))
            {
                string[] ports = SerialPort.GetPortNames();
                Log.Information("Found ports {Ports}", ports);

                switch (ports.Length)
                {
                    case 0: 
                    {
                        Log.Information("No COM port found. Using default {DefaultPort}.", DefaultComPort);
                        break;
                    }

                    case 1: 
                    {
                        comPort = ports[0];
                        break;
                    }

                    default:
                    {
                        Log.Information("More than one COM port found. Specify which is connected to the projector in config file.");
                        break;
                    }
                }
            }
            else
            {
                comPort = configuredPort;
            }


            return comPort;
        }
    }
}
