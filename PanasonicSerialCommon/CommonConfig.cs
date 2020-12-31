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
    public class CommonConfig
    {
        public const int DefaultServerPort = 8889;
        public const string ServerPortKey = "ServerPort";
    }
}
