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
        private static string CheminDossierLogs = @"/app/log";
        private static string CheminFichierDeLogs = CheminDossierLogs + "/" + "Log_Revue_Par_Les_Paires.csv";

        public static void Journaliser(Log log)
        {
            CreerDossierSiNonExistant();
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
                File.Create(CheminFichierDeLogs);
            }
        }

        private static void AjouterJournalisation(Log log)
        {
            using (StreamWriter sw = new StreamWriter(CheminFichierDeLogs))
            {
                sw.WriteLine(log.ToString());
                //File.AppendAllLines(CheminFichierDeLogs, new List<string> { log.ToString() });
            }
        }
    }
}