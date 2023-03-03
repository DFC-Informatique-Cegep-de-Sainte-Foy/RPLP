using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public static class Journalisation
    {
        //A changer pour la bd
        private static string CheminDossierLogs = @"/var/log/";
        private static string CheminFichierDeLogs = CheminDossierLogs + "Log_Revue_Par_Les_Paires.csv";

        public static void Journaliser(Log log)
        {
            //CreerDossierSiNonExistant();
            CreerNomDuFichierLog();
            AjouterJournalisation(log);
        }

        private static void CreerDossierSiNonExistant()
        {
            if (!Directory.Exists(CheminDossierLogs))
            {
                Directory.CreateDirectory(CheminDossierLogs);
            }
        
            CreerNomDuFichierLog();
        }

        private static void CreerNomDuFichierLog()
        {
            if (!File.Exists(CheminFichierDeLogs))
            {
                var fichierLog = File.Create(CheminFichierDeLogs);
                fichierLog.Close();
            }
        }

        private static void AjouterJournalisation(Log log)
        {
            string path = CheminFichierDeLogs;
            Console.WriteLine(log.ToString());
            Console.WriteLine(path);
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(log.ToString());
            }
            //File.AppendAllLines(@"logs/Log_Revue_Par_Les_Paires.csv", new List<string> { log.ToString() });
        }
    }
}