using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPLP.JOURNALISATION;
using RPLP.SERVICES;
using RPLP.SERVICES.Github;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.VALIDATIONS
{
    public class Consummer
    {
        private ManualResetEvent faireAttendreProgrammePrincipal = new ManualResetEvent(false);
        private ConnectionFactory ConnexionFactory = new ConnectionFactory() { HostName = "rplp.rabbitmq" };
        private ScriptGithubRPLP Script;

        private string[] subject = { "rplp.assignations.students" };
        public Consummer(ScriptGithubRPLP script)
        {
            this.Script = script;
        }

        public void DeclareExchange()
        {
            using (IConnection connexion = ConnexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    canalDeCommunication.ExchangeDeclare(
                     exchange: "rplp-message-thread",
                     type: "topic",
                     durable: true,
                     autoDelete: false
                     );

                    canalDeCommunication.QueueDeclare(
                    "rplp-message-thread-assignation-students",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
                }
            }
        }

        public void Listen()
        {
            using (IConnection connexion = ConnexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {

                    foreach (string sujet in subject)
                    {
                        canalDeCommunication.QueueBind(
                            queue: "rplp-message-thread-assignation-students",
                            exchange: "rplp-message-thread",
                            routingKey: sujet);
                    }


                    EventingBasicConsumer consommateurServeur = new EventingBasicConsumer(canalDeCommunication);

                    consommateurServeur.Received += (model, argumentEvenement) =>
                    {
                        byte[] donnees = argumentEvenement.Body.ToArray();
                        string messageNonConvertit = Encoding.UTF8.GetString(donnees);
                        string sujet = argumentEvenement.RoutingKey;

                        MessageGitHubAPI message = JsonConvert.DeserializeObject<MessageGitHubAPI>(messageNonConvertit);

                        if (message.Allocations.Status == 1)
                        {
                            this.Script.createPullRequestAndAssignUser(message.OrganisationName, message.RepositoryName, message.SHA, message.Username);
                        }

                        //if (message.Allocations.Status == 2)
                        //{
                        //   this.Script.createPullRequestAndAssignUser(message.OrganisationName, message.RepositoryName, message.SHA, message.Username);
                        //}

                    };

                    canalDeCommunication.BasicConsume(queue: "rplp-message-thread-assignation-students",
                        autoAck: true,
                        consumerTag: "rplp-message-thread-assignation-students",
                        consumer: consommateurServeur);

                    //Ne pas ajouter sinon ça bloque le programme
                    //faireAttendreProgrammePrincipal.WaitOne();
                }
            }
        }
    }
}
