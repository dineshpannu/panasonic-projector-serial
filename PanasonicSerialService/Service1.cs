using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PanasonicSerialService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            
            // Setup logging
        }

        protected override void OnStart(string[] args)
        {

            // Create MQTT server
        }

        protected override void OnStop()
        {
        }
    }
}
