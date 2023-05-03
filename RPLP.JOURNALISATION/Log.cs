using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public class Log
    {
        public Guid LogID { get; private set; }
        public string TypeOfLog { get; private set; }
        public DateTime DateOfLog { get; private set; }


        //Erreurs
        public string? RaiseException { get; private set; }
        public string? StackTrace { get; private set; }

        //Api
        public string RouteApi { get; private set; }
        public int StatusCodeQuery { get; set; }
        public int RemainingQuery{ get; private set; }


        //Base de données 
        public string AffectedTable { get; private set; }

        //Connexion
        public string ConnectedUser { get; private set; }
        public string Role { get; set; }


        public string Message { get; private set; }

        //Complet
        public Log(string p_TypeDeLog, string p_exception, string p_stacktrace, string p_RouteApi,
            int p_CodeStatusRequete, int p_requetesRestantes, string p_tableAffectee, string p_utilisateurConnectee, string p_role, string p_messageSupplementaire)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = p_TypeDeLog;
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = p_exception;
            this.StackTrace = p_stacktrace;
            this.RouteApi = p_RouteApi;
            this.StatusCodeQuery = p_CodeStatusRequete;
            this.RemainingQuery = p_requetesRestantes;
            this.AffectedTable = p_tableAffectee;
            this.ConnectedUser = p_utilisateurConnectee;
            this.Role = p_role;
            this.Message = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des Connexions BD
        public Log(string p_tableAffectee, string p_messageSupplementaire)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = "RequeteBaseDeDonnees";
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = null;
            this.StackTrace = null;
            this.RouteApi = "";
            this.StatusCodeQuery = 0;
            this.RemainingQuery = 0;
            this.AffectedTable = p_tableAffectee;
            this.ConnectedUser = "";
            this.Role = "";
            this.Message = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des Connexions BD
        public Log(string p_tableAffectee, string p_messageSupplementaire, int defaut)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = "EchecRequeteBaseDeDonnees";
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = null;
            this.StackTrace = null;
            this.RouteApi = "";
            this.StatusCodeQuery = 0;
            this.RemainingQuery = 0;
            this.AffectedTable = p_tableAffectee;
            this.ConnectedUser = "";
            this.Role = "";
            this.Message = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des erreurs
        public Log(string p_exception, string p_stacktrace, string p_messageSupplementaire, int defaut)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = "Erreur";
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = p_exception;
            this.StackTrace = p_stacktrace;
            this.RouteApi = "";
            this.StatusCodeQuery = 0;
            this.RemainingQuery = 0;
            this.AffectedTable = "";
            this.ConnectedUser = "";
            this.Role = "";
            this.Message = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des appels API RPLP
        public Log(string p_RouteApi, int p_CodeStatusRequete, string p_messageSupplementaire)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = "AppelAPI_RPLP";
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = null;
            this.StackTrace = null;
            this.RouteApi = p_RouteApi;
            this.StatusCodeQuery = p_CodeStatusRequete;
            this.RemainingQuery = 0;
            this.AffectedTable = "";
            this.ConnectedUser = "";
            this.Role = "";
            this.Message = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des appels API GitHub
        public Log(string p_RouteApi, int p_CodeStatusRequete, int p_requetesRestantes, string p_messageSupplementaire)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = "AppelAPI_GitHub";
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = null;
            this.StackTrace = null;
            this.RouteApi = p_RouteApi;
            this.StatusCodeQuery = p_CodeStatusRequete;
            this.RemainingQuery = p_requetesRestantes;
            this.AffectedTable = "";
            this.ConnectedUser = "";
            this.Role = "";
            this.Message = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation de la connexion 
        public Log(string p_utilisateurConnectee, string p_role, string p_messageSupplementaire)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = "ConnexionAuth0";
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = null;
            this.StackTrace = null;
            this.RouteApi = "";
            this.StatusCodeQuery = 0;
            this.RemainingQuery = 0;
            this.AffectedTable = "";
            this.ConnectedUser = p_utilisateurConnectee;
            this.Role = p_role;
            this.Message = p_messageSupplementaire;
        }

        //Constructeur Debug
        public Log(string p_messageSupplementaire)
        {
            this.LogID = Guid.NewGuid();
            this.TypeOfLog = "Debug";
            this.DateOfLog = DateTime.UtcNow;
            this.RaiseException = null;
            this.StackTrace = null;
            this.RouteApi = "";
            this.StatusCodeQuery = 0;
            this.RemainingQuery = 0;
            this.AffectedTable = "";
            this.ConnectedUser = "";
            this.Role = "";
            this.Message = p_messageSupplementaire;
        }

        public override string ToString()
        {
            return ConstruireChaineDeCaracteresPourFichierCSV();
        }

        private string ConstruireChaineDeCaracteresPourFichierCSV()
        {
            string ligneFichierCSV = "";

            int compteur = 0;

            foreach (PropertyInfo propriete in this.GetType().GetProperties())
            {
                compteur++;

                if (propriete != null)
                {
                    if (propriete.GetValue(this) != null && propriete.GetValue(this) != "")
                    {
                        ligneFichierCSV += propriete.GetValue(this).ToString();
                    }
                    else
                    {
                        ligneFichierCSV += "";
                    }
                }
                else
                {
                    ligneFichierCSV += "";
                }

                if(compteur < this.GetType().GetProperties().Count())
                {
                    ligneFichierCSV += "~";
                }
            }

            return ligneFichierCSV;
        }
    }
}

