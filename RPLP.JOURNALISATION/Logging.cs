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
    public class Logging
    {

        public IManipulationLogs ManipulationLog = new ManipulationLogs();
        private static Logging? _instance;

        private static object _lock = new object();

        public void Journal(Log log)
        {
            ManipulationLog.Journal(log);
        }

        public void ClearLogs()
        {
            ManipulationLog.ClearLogs();
        }



        public static Logging Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logging();
                        }
                    }
                }

                return _instance;
            }
        }  
    }
}