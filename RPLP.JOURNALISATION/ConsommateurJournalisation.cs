using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public class ConsommateurJournalisation
    {
        private ManualResetEvent faireAttendreProgrammePrincipal = new ManualResetEvent(false);
        private ConnectionFactory fabriqueDeConnexion = new ConnectionFactory() { HostName = "rplp.rabbitmq" };
        private static string CheminDossierLogs = @"/var/log/";
        private static string CheminFichierDeLogs = CheminDossierLogs + "Log_Revue_Par_Les_Paires.csv";

        public ConsommateurJournalisation()
        {
            DeclarerLaQueue();
        }

        public void DeclarerLaQueue()
        {
            using (IConnection connexion = fabriqueDeConnexion.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    canalDeCommunication.QueueDeclare(queue: "fdm_rplp_journalisation",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                }
            }
        }

        public void Ecouter()
        {
            using (IConnection connexion = fabriqueDeConnexion.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    EventingBasicConsumer consommateurServeur = new EventingBasicConsumer(canalDeCommunication);

                    consommateurServeur.Received += (model, argumentEvenement) =>
                    {
                        byte[] donnees = argumentEvenement.Body.ToArray();
                        string logNonConvertit = Encoding.UTF8.GetString(donnees);

                        File.AppendAllLines(CheminFichierDeLogs, new List<string> { logNonConvertit });

                        canalDeCommunication.BasicAck(argumentEvenement.DeliveryTag, false);
                    };

                    canalDeCommunication.BasicConsume(queue: "fdm_rplp_journalisation",
                    autoAck: false,
                    consumer: consommateurServeur
                    );

                    faireAttendreProgrammePrincipal.WaitOne();
                }
            }
        }

    }
}
