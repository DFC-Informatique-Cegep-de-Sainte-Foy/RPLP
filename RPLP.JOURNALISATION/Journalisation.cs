using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public static class Journalisation
    {
        private static ConnectionFactory fabriqueDeConnexion = new ConnectionFactory() { HostName = "rplp.rabbitmq" };

        public static void Journaliser(Log log)
        {
            AjouterJournalisation(log);
        }

        private static void AjouterJournalisation(Log log)
        {
            using (IConnection connexion = fabriqueDeConnexion.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    canalDeCommunication.QueueDeclare(queue: "fdm_rplp_journalisation",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                    string logConvertit = log.ToString();

                    byte[] body = Encoding.UTF8.GetBytes(logConvertit);
                    canalDeCommunication.BasicPublish(exchange: "", routingKey: "fdm_rplp_journalisation", body: body);
                }
            }
        }
    }
}