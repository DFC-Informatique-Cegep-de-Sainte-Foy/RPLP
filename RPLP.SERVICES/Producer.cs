using Newtonsoft.Json;
using RabbitMQ.Client;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.Github;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES
{
    public static class Producer
    {
        private static ConnectionFactory ConnexionFactory = new ConnectionFactory() { HostName = "rplp.rabbitmq" };

        public static void CallGitHubAPI(Allocations allocations, string p_organisationName, string p_repositoryName, string p_sha, string p_username)
        {
            RPLP.JOURNALISATION.Logging.Journal(new Log("Producer 1"));

            using (IConnection connexion = ConnexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log("Producer 2"));

                    canalDeCommunication.ExchangeDeclare(
                        exchange: "rplp-message-thread",
                        type: "topic",
                        durable: true,
                        autoDelete: false
                    );

                    RPLP.JOURNALISATION.Logging.Journal(new Log("Producer 3"));

                    string sujet = $"rplp.assignations.students";

                    MessageGitHubAPI message = new MessageGitHubAPI(Guid.NewGuid(), allocations, p_organisationName, p_repositoryName, p_sha, p_username);

                    JsonSerializerSettings parametres = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };

                    string messageConvertit = JsonConvert.SerializeObject(message, parametres);

                    byte[] body = Encoding.UTF8.GetBytes(messageConvertit);

                    canalDeCommunication.BasicPublish(
                        exchange: "rplp-message-thread",
                        routingKey: sujet,
                        basicProperties: null,
                        body: body);

                    RPLP.JOURNALISATION.Logging.Journal(new Log("Producer 4"));
                }
            }
        }
    }
}
