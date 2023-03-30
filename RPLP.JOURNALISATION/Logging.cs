using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public static class Logging
    {
        public static IManipulationLogs ManipulationLog = new ManipulationLogs();
        public static void Journal(Log log)
        {
           ManipulationLog.Journal(log);
        }
        public static void ClearLogs()
        {
            ManipulationLog.ClearLogs();
        }
    }
}