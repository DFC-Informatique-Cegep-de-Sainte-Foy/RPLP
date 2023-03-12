using Aspose.Zip.Saving;
using Aspose.Zip.SevenZip;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public class ConsummerLogs
    {
        private ManualResetEvent faireAttendreProgrammePrincipal = new ManualResetEvent(false);
        private ConnectionFactory ConnexionFactory = new ConnectionFactory() { HostName = "rplp.rabbitmq" };
        private static string CheminDossierLogs = @"/var/log/rplp/";
        private static string CheminDossierZipLogs = @"/var/log/zip_rplp/";
        private static string CheminFichierDeLogs = CheminDossierLogs + "Log_Revue_Par_Les_Paires.csv";
        private static string Header = "Identifiant~TypeDeLog~DateDuLog~ExceptionLevee~StackTrace~RouteApi~CodeStatusRequete~RequetesRestantes~TableAffectee~UtilisateurConnecte~Role~MessageSupplementaire";

        public ConsummerLogs()
        {
            if (!Directory.Exists(CheminDossierLogs))
            {
                Directory.CreateDirectory(CheminDossierLogs);
            }

            if (!Directory.Exists(CheminDossierZipLogs))
            {
                Directory.CreateDirectory(CheminDossierZipLogs);
            }

            if (!File.Exists(CheminFichierDeLogs))
            {
                File.AppendAllLines(CheminFichierDeLogs, new List<string> { Header });
            }
        }

        public void DeclareQueue()
        {
            using (IConnection connexion = ConnexionFactory.CreateConnection())
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

        public void Listen()
        {
            using (IConnection connexion = ConnexionFactory.CreateConnection())
            {
                using (IModel canalDeCommunication = connexion.CreateModel())
                {
                    EventingBasicConsumer consommateurServeur = new EventingBasicConsumer(canalDeCommunication);

                    consommateurServeur.Received += (model, argumentEvenement) =>
                    {
                        byte[] donnees = argumentEvenement.Body.ToArray();
                        string messageNonConvertit = Encoding.UTF8.GetString(donnees);

                        Message message = JsonConvert.DeserializeObject<Message>(messageNonConvertit);

                        HandleSizeOfTheFile();
                        HandleTheMessage(message);

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
        
        private static double GetSizeOfFile()
        {
            FileInfo informations = new FileInfo(CheminFichierDeLogs);
         
            return (informations.Length / 8e+6);
        }


        private static void HandleSizeOfTheFile()
        {
            //À titre de démonstation
            //if (GetSizeOfFile() >= 20)
            if (GetSizeOfFile() >= 0.00050)
            {
                string nameOfZipLogsFile = $"{Guid.NewGuid()}_zip_logs";

                //https://docs.aspose.com/zip/net/how-to/
                using (var archive = new SevenZipArchive(new SevenZipEntrySettings(new SevenZipLZMACompressionSettings())))
                {
                    //https://github.com/JanKallman/EPPlus/issues/31
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    archive.CreateEntry(nameOfZipLogsFile, CheminFichierDeLogs);
                    archive.Save(CheminDossierZipLogs + nameOfZipLogsFile + ".7z");
                }

                ClearAllLogsFromFile();
            }
        }

        private static void HandleTheMessage(Message p_message)
        {
            if (p_message != null)
            {
                if (p_message.TypeOfMessage == "Journalisation")
                {
                    File.AppendAllLines(CheminFichierDeLogs, new List<string> { p_message.Log });

                }
                else if (p_message.TypeOfMessage == "ClearLogs")
                {
                    ClearAllLogsFromFile();
                }
            }
        }

        private static void ClearAllLogsFromFile()
        {
            File.Create(CheminFichierDeLogs).Close();
            File.AppendAllLines(CheminFichierDeLogs, new List<string> { Header });
        }
    }
}
