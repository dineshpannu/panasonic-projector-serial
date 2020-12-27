using PanasonicSerialServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanasonicSerialServer.PanasonicCommands
{
    public class PanasonicCommand : IPanasonicCommand
    {
        public string Command { get; private set; }
        public string SerialCommand { get; private set; }

        public bool HasOption { get; private set; }

        public virtual string Option { get; set; }
        public int PauseDuration { get; } = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="serialCommand"></param>
        /// <param name="hasOption"></param>
        public PanasonicCommand(string command, string serialCommand, bool hasOption, int pauseDuration = 0)
        {
            this.Command = command;
            this.SerialCommand = serialCommand;
            this.HasOption = hasOption;
            this.PauseDuration = pauseDuration;
        }
    }
}
