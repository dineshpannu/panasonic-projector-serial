using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using PanasonicSerialCommon;

namespace PanasonicSerialClient
{
    public class Options
    {
        [Option('d', "debug", Required = false, HelpText = "Enable debug output.")]
        public bool Debug { get; set; }

        [Option('h', "host", Required = false, HelpText = "IP address of PanasonicSerialServer.")]
        public string Host { get; set; }
        
        [Option('p', "port", Required = false, HelpText = "Port for PanasonicSerialServer.")]
        public int Port { get; set; }

        [Value(0, MetaName = "Command", Required = true, HelpText = "Serial command to send.")]
        public string Command { get; set; }

        [Value(1, MetaName = "Option", Required = false, HelpText = "Any options the command may have.")]
        public string CommandOption { get; set; }
    }
}
