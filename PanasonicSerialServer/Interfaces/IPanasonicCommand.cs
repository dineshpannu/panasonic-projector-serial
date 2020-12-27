using System;
using System.Collections.Generic;
using System.Text;

namespace PanasonicSerialServer.Interfaces
{
    public interface IPanasonicCommand
    {
        string Command { get; } 
        string SerialCommand { get; }
        bool HasOption { get; }
        string Option { get; set; }
        int PauseDuration { get; }
    }
}
