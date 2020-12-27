using System;
using System.Collections.Generic;
using System.Text;
using PanasonicSerialServer.Interfaces;

namespace PanasonicSerialServer.Queue
{
    public class JobInfo
    {

        public IPanasonicCommand PanasonicCommand { get; set; } // Command to run.
        public string Output { get; set; } // Command output as string.
        public int ThreadID { get; set; }  // Debugging.

        public override string ToString()
        {
            return $"Command:{PanasonicCommand.Command} Output:{Output} ThreadID:{ThreadID}";
        }
    }
}
