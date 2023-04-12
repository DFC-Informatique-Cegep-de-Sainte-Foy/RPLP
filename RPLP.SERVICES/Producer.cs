using Newtonsoft.Json;
using RabbitMQ.Client;
using RPLP.DAL.DTO.Json;
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
        private static ConnectionFactory _connexionFactory = new ConnectionFactory() { HostName = "rplp.rabbitmq" };

        public static void CallGitHubAPI(Allocations allocations)
        {
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Le Producteur à été appelé"));

            using (IConnection connexion = _connexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    canalDeCommunication.ExchangeDeclare(
                        exchange: "rplp-message-thread",
                        type: "topic",
                        durable: true,
                        autoDelete: false
                    );

                    string sujet = $"rplp.assignations.students";

                    MessageGitHubAPI message = new MessageGitHubAPI(Guid.NewGuid(), new Allocations_JSONDTO(allocations));

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"Le Producteur à envoyer le message {message.MessageID}"));

                    JsonSerializerSettings parametres = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };

                    string messageConvertit = JsonConvert.SerializeObject(message, parametres);

                    byte[] body = Encoding.UTF8.GetBytes(messageConvertit);

                    canalDeCommunication.BasicPublish(
                        exchange: "rplp-message-thread",
                        routingKey: sujet,
                        basicProperties: null,
                        body: body);
                }
            }
        }
    }
}
