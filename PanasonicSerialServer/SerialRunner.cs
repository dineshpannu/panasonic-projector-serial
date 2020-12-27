using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using PanasonicSerialCommon;
using PanasonicSerialServer.Interfaces;
using PanasonicSerialServer.PanasonicCommands;
using Serilog;

namespace PanasonicSerialServer
{
    public class SerialRunner
    {
        private const int BaudRate = 9600;
        private const Parity ComParity = Parity.None;
        private const int DataBits = 8;
        private const StopBits ComStopBits = StopBits.One;

        private readonly byte[] STX = new byte[] { 0x02 };
        private readonly byte[] ETX = new byte[] { 0x03 };

        private static LensEnum? lensMemoryState = null;

        private readonly Config config;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public SerialRunner(Config config)
        {
            this.config = config;
        }


        public void Execute(IPanasonicCommand command)
        {
            if (this.ShouldExecute(command))
            {
                string serialCommand = this.CreateSerialCommand(command);
                Log.Information("SerialRunner sending command: {Command}", serialCommand);

#if !DEBUG
            SerialPort serialPort = new SerialPort(config.ComPort, BaudRate, ComParity, DataBits, ComStopBits);
            serialPort.Open();
            serialPort.WriteLine($"{STX}{serialCommand}{ETX}");
                
            string reply = serialPort.ReadLine();

            Log.Debug("Got reply: [{Reply}]", reply);
#endif

                this.SetState(command);

                if (command.PauseDuration > 0)
                {
                    // Pause before exiting to give projector time to focus - this mechanism is used to help us queue lens commands
                    Log.Information("Pausing for {ShiftDuration} seconds", command.PauseDuration);
                    Thread.Sleep(command.PauseDuration * 1000);
                    Log.Debug("Finished pause.");
                }
            }
            else
            {
                Log.Information("Not performing command as current state is same as new state. Lens: {LensMemoryState} New Command: {Command} Option: {Option}", lensMemoryState, command.Command, command.Option);
            }
        }


        private bool ShouldExecute(IPanasonicCommand command)
        {
            bool shouldExecute = true;

            if (Commands.LensMemoryLoad == command.Command)
            {
                if (command is PanasonicVxxLensCommand lensCommand)
                {
                    shouldExecute = lensCommand.LensMemory != lensMemoryState;
                }
            }

            return shouldExecute;
        }


        private string CreateSerialCommand(IPanasonicCommand command)
        {
            string serialCommand = command.SerialCommand;
            if (command.HasOption)
            {
                serialCommand += command.Option;
            }

            return serialCommand;
        }

        private void SetState(IPanasonicCommand command)
        {
            if (Commands.LensMemoryLoad == command.Command)
            {
                if (command is PanasonicVxxLensCommand lensCommand)
                {
                    lensMemoryState = lensCommand.LensMemory;
                }
            }
        }
    }
}
