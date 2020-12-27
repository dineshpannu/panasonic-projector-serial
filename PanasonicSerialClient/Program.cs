using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using MQTTnet;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using PanasonicSerialCommon;
using Serilog;

namespace PanasonicSerialClient
{
    public class Program
    {

        static void Main(string[] args)
        {
            // Parse command line
            //
            var result = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => RunProgram(options)) // options is an instance of Options type
                .WithNotParsed(errors => DisplayErrorText(errors)); // errors is a sequence of type IEnumerable<Error>
        }


        static void RunProgram(Options options)
        {
            // Start logging
            //
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PanasonicSerialClient.log");

            if (options.Debug)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.Debug()
                    .WriteTo.File(logFilePath)
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.Debug()
                    .WriteTo.File(logFilePath)
                    .CreateLogger();
            }


            try
            {
                // Create config
                //
                Message message = new Message
                {
                    Command = options.Command,
                    Option = options.CommandOption
                };
                ClientConfig clientConfig = new ClientConfig(options.Host, options.Port, message);


                // Connect to MQTT server
                //
                MqttClient mqttClient = new MqttClient(clientConfig);
                mqttClient.Run();

            }
            catch (Exception ex)
            {
                Log.Error(ex, "We've had an issue");
            }

        }


        static void DisplayErrorText(IEnumerable<Error> errors)
        {
            //foreach (Error error in errors)
            //{
            //    Console.WriteLine("Problem parsing command line: " + JsonConvert.SerializeObject(error));
            //}
            // Sweet fuck all we can do for them now.
        }
    }
}
