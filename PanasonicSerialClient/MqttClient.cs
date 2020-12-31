using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using PanasonicSerialCommon;
using Serilog;

namespace PanasonicSerialClient
{
    public class MqttClient
    {
        private readonly ClientConfig clientConfig;

        public MqttClient(ClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
        }

        public void Run()
        { 
            Log.Information("Creating MQTT client");

            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            mqttClient.UseConnectedHandler(async e =>
            {
                Log.Information("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("#").Build());

                Log.Information("### SUBSCRIBED ###");
            });

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

            // Create TCP based options using the builder.
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("PanasonicSerialClient")
                .WithTcpServer(this.clientConfig.Host, this.clientConfig.Port)
                .Build();

            Log.Information("Connecting to server {Server}:{Port}", clientConfig.Host, clientConfig.Port);
            var connectTask = mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None); // Since 3.0.5 with CancellationToken
            connectTask.Wait();


            // Send command
            //

            if (mqttClient.IsConnected)
            {
                Log.Information("Sending message: {Message}", JsonConvert.SerializeObject(this.clientConfig.Message));
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(Topics.RequestAction)
                    .WithPayload(JsonConvert.SerializeObject(this.clientConfig.Message))
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();

                var publishTask = mqttClient.PublishAsync(mqttMessage, CancellationToken.None); // Since 3.0.5 with CancellationToken
                publishTask.Wait();

            }
            else
            {
                Log.Information("Could not connect to {Server}:{Port}", clientConfig.Host, clientConfig.Port);
            }
        }
    }
}
