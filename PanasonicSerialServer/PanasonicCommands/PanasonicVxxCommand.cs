using PanasonicSerialServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanasonicSerialServer.PanasonicCommands
{
    public class PanasonicVxxCommand : PanasonicCommand
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="pauseDuration"></param>
        public PanasonicVxxCommand(string command, int pauseDuration = 0) : base(command, "VXX", true, pauseDuration)
        {
        }
    }
}
