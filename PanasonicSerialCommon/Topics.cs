using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanasonicSerialCommon
{
    /// <summary>
    /// Names the topics our clients and server support.
    /// </summary>
    public class Topics
    {
        /// <summary>
        /// ActionPerformed is subscribed to by clients. It is where the server publishes the outcome of commands sent via serial.
        /// </summary>
        public const string ActionPerformed = "ActionPerformed";

        /// <summary>
        /// RequestAction is subscribed to by the server. Clients publish on RequestAction to let the server know what serial command they want run.
        /// </summary>
        public const string RequestAction = "RequestAction";
    }
}
