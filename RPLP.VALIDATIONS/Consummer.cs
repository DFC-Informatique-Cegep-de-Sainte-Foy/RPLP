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
        private ManualResetEvent _faireAttendreProgrammePrincipal = new ManualResetEvent(false);
        private ConnectionFactory _connexionFactory = new ConnectionFactory() { HostName = "rplp.rabbitmq" };
        private ScriptGithubRPLP _script;
        private Allocations_JSONDTO _allocations;

        private string[] subject = { "rplp.assignations.students", "rplp.assignations.professor" };
        public Consummer(ScriptGithubRPLP script)
        {
            this._script = script;
            this._allocations = new Allocations_JSONDTO();
        }

        public void DeclareExchange()
        {
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

                    canalDeCommunication.QueueDeclare(
                    "rplp-message-thread-assignation",
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
            using (IConnection connexion = _connexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    foreach (string sujet in subject)
                    {
                        canalDeCommunication.QueueBind(
                            queue: "rplp-message-thread-assignation",
                            exchange: "rplp-message-thread",
                            routingKey: sujet);
                    }

                    EventingBasicConsumer consommateurServeur = new EventingBasicConsumer(canalDeCommunication);

                    consommateurServeur.Received += (model, argumentEvenement) =>
                    {
                        byte[] donnees = argumentEvenement.Body.ToArray();
                        string messageNonConvertit = Encoding.UTF8.GetString(donnees);
                        string sujet = argumentEvenement.RoutingKey;

                        if(sujet == "rplp.assignations.students")
                        {
                            MessageGitHubAPI message = JsonConvert.DeserializeObject<MessageGitHubAPI>(messageNonConvertit);

                            ManageAssignmentStudent(message, canalDeCommunication, argumentEvenement);
                        }
                    };

                    canalDeCommunication.BasicConsume(queue: "rplp-message-thread-assignation",
                                        autoAck: false,
                                        consumerTag: "rplp-message-thread-assignation",
                                        consumer: consommateurServeur);

                    _faireAttendreProgrammePrincipal.WaitOne();
                }
            }
        }

        public void ManageAssignmentStudent(MessageGitHubAPI message, IModel canalDeCommunication, BasicDeliverEventArgs argumentEvenement)
        {
            this._allocations = message.Allocations;

            string status = "Stand";

            foreach (Allocation allocation in this._allocations.Pairs)
            {
                string p_organisationName = message.Allocations._classroom.OrganisationName;

                string p_repositoryName = this._script.GetNameOfRepository(allocation.RepositoryId);

                string p_reviewerName = message.Allocations._classroom.Students.FirstOrDefault(reviewer => reviewer.Id == allocation.StudentId).Username;

                if (allocation.Status == 42)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"Assignation effectuée au 10 secondes :" +
                                    $" organisationName : {p_organisationName} - repositoryName : {p_repositoryName} - NomDuReviewer : {p_reviewerName}"));
                    
                    Thread.Sleep(10000);

                    status = this._script.CreatePullRequestAndAssignUser(p_organisationName, p_repositoryName, p_reviewerName);

                    if (status == "Forbidden")
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"Assignation arrêté - status Forbidden reçu"));

                        Thread.Sleep(60000);
                    }
                    else
                    {
                        if (status == "Created")
                        {
                            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"Allocation : {allocation.Id} est affecté au status 53"));

                            this._script.SetAllocationAfterAssignation(allocation);
                        }
                    }
                }
            }

            this._allocations.Pairs = this._script.GetAllocationBySelectedAllocationID(message.Allocations.Pairs);

            if (this._allocations.Status == 53)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"Assignations complétées avec succès - Message ID : {message.MessageID}"));

                canalDeCommunication.BasicAck(argumentEvenement.DeliveryTag, false);
            }
        }
    }
}
