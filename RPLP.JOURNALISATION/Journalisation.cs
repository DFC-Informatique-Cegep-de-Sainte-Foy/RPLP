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
        //private static string CheminDossierLogs = @"/app/logs";
        //private static string CheminFichierDeLogs = CheminDossierLogs + "/Log_Revue_Par_Les_Paires.csv";

        public static void Journaliser(Log log)
        {
            //CreerDossierSiNonExistant();
            AjouterJournalisation(log);
        }

        // private static void CreerDossierSiNonExistant()
        // {
        //     if (!Directory.Exists(CheminDossierLogs))
        //     {
        //         Directory.CreateDirectory(CheminDossierLogs);
        //     }
        //
        //     CreerNomDuFichierLog();
        // }

        // private static void CreerNomDuFichierLog()
        // {
        //     if (!File.Exists(CheminFichierDeLogs))
        //     {
        //         var fichierLog = File.Create(CheminFichierDeLogs);
        //         fichierLog.Close();
        //     }
        // }

        private static void AjouterJournalisation(Log log)
        {
            File.AppendAllLines(@"logs/Log_Revue_Par_Les_Paires.csv", new List<string> { log.ToString() });
        }
    }
}