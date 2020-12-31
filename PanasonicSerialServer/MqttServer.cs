using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json;
using PanasonicSerialCommon;
using PanasonicSerialServer.Interfaces;
using Serilog;
using System;
using System.Text;
using System.Threading;
using PanasonicSerialServer.Queue;

namespace PanasonicSerialServer
{
    public class MqttServer : IDisposable
    {
        private readonly ServerConfig config;
        private readonly JobQueue jobQueue;
        private IMqttServer mqttServer;

        public MqttServer(ServerConfig config)
        {
            this.config = config;
            this.jobQueue = new JobQueue(config);
        }



        public void CreateServer()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                            .WithDefaultEndpoint()
                            .WithDefaultEndpointPort(this.config.ServerPort)
                            .WithoutEncryptedEndpoint()
                            .WithSubscriptionInterceptor(
                                c =>
                                {
                                    c.AcceptSubscription = true;
                                    LogMessage(c, true);
                                }).WithApplicationMessageInterceptor(
                                c =>
                                {
                                    c.AcceptPublish = true;
                                    HandleMessage(c);
                                });

            Log.Debug("Creating MQTT server");
            this.mqttServer = new MqttFactory().CreateMqttServer();
            Log.Debug("MQTT server created");


            Log.Debug("Going async");
            this.mqttServer.StartAsync(optionsBuilder.Build());
            Log.Debug("Gone async");
        }


        /// <summary> 
        ///     Logs the message from the MQTT subscription interceptor context. 
        /// </summary> 
        /// <param name="context">The MQTT subscription interceptor context.</param> 
        /// <param name="successful">A <see cref="bool"/> value indicating whether the subscription was successful or not.</param> 
        private static void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
        {
            if (context == null)
            {
                return;
            }

            Log.Information(successful ? $"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}" : $"Subscription failed for clientId = {context.ClientId}, TopicFilter = {context.TopicFilter}");
        }


        /// <summary>
        ///     Logs the message from the MQTT message interceptor context.
        /// </summary>
        /// <param name="context">The MQTT message interceptor context.</param>
        private void HandleMessage(MqttApplicationMessageInterceptorContext context)
        {
            if (context == null)
            {
                return;
            }

            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            Log.Information(
                $"Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage?.Topic},"
                + $" Payload = {payload}, QoS = {context.ApplicationMessage?.QualityOfServiceLevel},"
                + $" Retain-Flag = {context.ApplicationMessage?.Retain}");


            if (context.ApplicationMessage?.Topic == Topics.RequestAction)
            {
                // Get Message object
                //
                Message message = null;
                try
                {
                    message = JsonConvert.DeserializeObject<Message>(payload);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Couldn't deserialize message");
                }


                if (null != message)
                {
                    // Parse message
                    //
                    IPanasonicCommand panasonicCommand = Commands.GetCommand(message, this.config);

                    if (null != panasonicCommand)
                    {
                        // Add command to queue
                        //
                        this.jobQueue.Add(panasonicCommand);
                    }
                }

                var returnMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(Topics.ActionPerformed)
                    .WithPayload("Message received")
                    .WithExactlyOnceQoS()
                    .Build();
                this.mqttServer.PublishAsync(returnMessage, CancellationToken.None);
            }
            else if (context.ApplicationMessage?.Topic == Topics.ActionPerformed)
            {
                // Safely ignore since we sending these.
            }
            else
            {
                Log.Error("Unknown topic {Topic}", context.ApplicationMessage?.Topic);
            }
        }

        public void Dispose()
        {
            this.mqttServer.Dispose();
        }
    }
}
