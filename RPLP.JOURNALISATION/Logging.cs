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
        private static ConnectionFactory ConnexionFactory = new ConnectionFactory() { HostName = "rplp.rabbitmq" };

        public static void Journal(Log log)
        {
            AddLog(log);
        }

        public static void ClearLogs()
        {
            ClearAllLogs();
        }

        private static void AddLog(Log log)
        {
            using (IConnection connexion = ConnexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    canalDeCommunication.QueueDeclare(queue: "fdm_rplp_journalisation",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                    Message message = new Message(Guid.NewGuid(), "Journalisation", log.ToString());

                    JsonSerializerSettings parametres = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };

                    string messageConvertit = JsonConvert.SerializeObject(message, parametres);

                    byte[] body = Encoding.UTF8.GetBytes(messageConvertit);
                    canalDeCommunication.BasicPublish(exchange: "", routingKey: "fdm_rplp_journalisation", body: body);
                }
            }
        }

        private static void ClearAllLogs()
        {
            using (IConnection connexion = ConnexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    canalDeCommunication.QueueDeclare(queue: "fdm_rplp_journalisation",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                    Message message = new Message(Guid.NewGuid(), "ClearLogs", "");

                    JsonSerializerSettings parametres = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };

                    string messageConvertit = JsonConvert.SerializeObject(message, parametres);

                    byte[] body = Encoding.UTF8.GetBytes(messageConvertit);
                    canalDeCommunication.BasicPublish(exchange: "", routingKey: "fdm_rplp_journalisation", body: body);
                }
            }
        }
    }
}