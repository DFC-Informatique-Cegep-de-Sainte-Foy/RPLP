using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPLP.DAL.DTO.Json;
using RPLP.ENTITES;
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
        private Allocations_JSONDTO allocations;

        private string[] subject = { "rplp.assignations.students" };
        public Consummer(ScriptGithubRPLP script)
        {
            this.Script = script;
            this.allocations = new Allocations_JSONDTO();
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

                        this.allocations = message.Allocations;

                        foreach (Allocation allocation in this.allocations.Pairs)
                        {
                            string p_organisationName = message.Allocations._classroom.OrganisationName;

                            string p_repositoryName = this.Script.getNameOfRepository(allocation.RepositoryId);

                            string p_reviewerName = message.Allocations._classroom.Students.FirstOrDefault(reviewer => reviewer.Id == allocation.StudentId).Username;

                            if (allocation.Status == 2)
                            {
                                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"Assignation effectuée au 10 secondes :" +
                                                $" organisationName : {p_organisationName} - repositoryName : {p_repositoryName} - NomDuReviewer : {p_reviewerName}"));
                                Thread.Sleep(10000);
                                this.Script.createPullRequestAndAssignUser(p_organisationName, p_repositoryName, p_reviewerName);
                                this.Script.SetAllocationAfterAssignation(allocation);
                            }
                        }

                        this.allocations.Pairs = this.Script.GetAllocationBySelectedAllocationID(message.Allocations.Pairs);

                        if (this.allocations.Status == 3)
                        {
                            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"Assignations complétées avec succès - Message ID : {message.MessageID}"));

                            canalDeCommunication.BasicAck(argumentEvenement.DeliveryTag, false);
                        }
                    };

                    canalDeCommunication.BasicConsume(queue: "rplp-message-thread-assignation-students",
                                        autoAck: false,
                                        consumerTag: "rplp-message-thread-assignation-students",
                                        consumer: consommateurServeur);

                    faireAttendreProgrammePrincipal.WaitOne();
                }
            }
        }
    }
}
