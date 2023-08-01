using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.Helpers
{
    public static class LoggingEvents
    {
        public static readonly EventId INIT_DATABASE = new EventId(101, "Error while creating and seeding database");
        public static readonly EventId SEND_EMAIL = new EventId(201, "Error while sending email");
        public static readonly EventId APPLICATION_STOPPING = new EventId(301, "Application is stopping");
        public static readonly EventId APPLICATION_STOPPED = new EventId(302, "Application has stopped");
        public static readonly EventId INIT_USER_CONNECTION = new EventId(401, "Initialising all user connections");
        public static readonly EventId INIT_DEVICE_CONNECTION = new EventId(601, "Initialising all device connections");
        public static readonly EventId DEVICE_REFRESH = new EventId(701, "Refreshing the device");
        public static readonly EventId DEVICE_REBOOT = new EventId(701, "Rebooting the device");
        public static readonly EventId DEVICE_REBOOT_ERROR = new EventId(701, "Error while rebooting the device");
        public static readonly EventId SIGNALR_STATUS = new EventId(801, "Signalr Status");
        public static readonly EventId CONNECTION_STATUS = new EventId(901, "Connection Status");
    }

}
