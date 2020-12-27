using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace PanasonicSerialCommon
{
    public class Config
    {
        public const int DefaultServerPort = 8889;
        private const string ServerPortKey = "ServerPort";
        private const string ComPortKey = "ComPort";
        private const string DefaultComPort = "COM1";

        /// <summary>
        /// TCP port to run MQTT server on
        /// </summary>
        public int ServerPort { get; set; } = DefaultServerPort;
        public string ComPort { get; set; }
        public Dictionary<LensEnum, double> LensAspectRatios { get; set; } = new Dictionary<LensEnum, double>();


        public static Config Load()
        {
            Config config = new Config();

            string assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(assemblyPath);

            if (int.TryParse(cfg.AppSettings.Settings[ServerPortKey].Value, out int serverPort))
            {
                config.ServerPort = serverPort;
            }

            config.LensAspectRatios[LensEnum.LensMemory1] = AspectRatio.Parse(cfg.AppSettings.Settings[LensEnum.LensMemory1.ToString()]?.Value);
            config.LensAspectRatios[LensEnum.LensMemory2] = AspectRatio.Parse(cfg.AppSettings.Settings[LensEnum.LensMemory2.ToString()]?.Value);
            config.LensAspectRatios[LensEnum.LensMemory3] = AspectRatio.Parse(cfg.AppSettings.Settings[LensEnum.LensMemory3.ToString()]?.Value);
            config.LensAspectRatios[LensEnum.LensMemory4] = AspectRatio.Parse(cfg.AppSettings.Settings[LensEnum.LensMemory4.ToString()]?.Value);
            config.LensAspectRatios[LensEnum.LensMemory5] = AspectRatio.Parse(cfg.AppSettings.Settings[LensEnum.LensMemory5.ToString()]?.Value);
            config.LensAspectRatios[LensEnum.LensMemory6] = AspectRatio.Parse(cfg.AppSettings.Settings[LensEnum.LensMemory6.ToString()]?.Value);

            config.ComPort = FindComPort(cfg.AppSettings.Settings[ComPortKey].Value);

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
