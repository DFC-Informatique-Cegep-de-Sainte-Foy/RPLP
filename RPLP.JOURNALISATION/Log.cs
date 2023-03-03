using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public class Log
    {
        public Guid Identifiant { get; private set; }
        public string TypeDeLog { get; private set; }
        public DateTime DateDuLog { get; private set; }


        //Erreurs
        public Exception ExceptionLevee { get; private set; }
        public StackTrace StackTrace { get; private set; }

        //Api
        public string RouteApi { get; private set; }
        public int CodeStatusRequete { get; set; }


        //Base de données 
        public string TableAffectee { get; private set; }

        //Connexion
        public string UtilisateurConnecte { get; private set; }
        public string Role { get; set; }


        public string MessageSupplementaire { get; private set; }

        //Complet
        public Log(string p_TypeDeLog, Exception p_exception, StackTrace p_stacktrace, string p_RouteApi,
            int p_CodeStatusRequete, string p_tableAffectee, string p_utilisateurConnectee, string p_role, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = p_TypeDeLog;
            this.DateDuLog = DateTime.Now;
            this.ExceptionLevee = p_exception;
            this.StackTrace = p_stacktrace;
            this.RouteApi = p_RouteApi;
            this.CodeStatusRequete = p_CodeStatusRequete;
            this.TableAffectee = p_tableAffectee;
            this.UtilisateurConnecte = p_utilisateurConnectee;
            this.Role = p_role;
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des Connexions BD
        public Log(string p_tableAffectee, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = "RequeteBaseDeDonnees";
            this.DateDuLog = DateTime.Now;
            this.UtilisateurConnecte = "";
            this.ExceptionLevee = null;
            this.StackTrace = null;
            this.RouteApi = "";
            this.CodeStatusRequete = 0;
            this.TableAffectee = p_tableAffectee;
            this.UtilisateurConnecte = "";
            this.Role = "";
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des erreurs
        public Log(Exception p_exception, StackTrace p_stacktrace, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = "Erreur";
            this.DateDuLog = DateTime.Now;
            this.UtilisateurConnecte = "";
            this.ExceptionLevee = p_exception;
            this.StackTrace = p_stacktrace;
            this.RouteApi = "";
            this.CodeStatusRequete = 0;
            this.TableAffectee = "";
            this.UtilisateurConnecte = "";
            this.Role = "";
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des appels API
        public Log(string p_RouteApi, int p_CodeStatusRequete, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = "AppelAPI";
            this.DateDuLog = DateTime.Now;
            this.UtilisateurConnecte = "";
            this.ExceptionLevee = null;
            this.StackTrace = null;
            this.RouteApi = p_RouteApi;
            this.CodeStatusRequete = p_CodeStatusRequete;
            this.TableAffectee = "";
            this.UtilisateurConnecte = "";
            this.Role = "";
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation de la connexion 
        public Log(string p_utilisateurConnectee, string p_role, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = "AppelAPI";
            this.DateDuLog = DateTime.Now;
            this.UtilisateurConnecte = "";
            this.ExceptionLevee = null;
            this.StackTrace = null;
            this.RouteApi = "";
            this.CodeStatusRequete = 0;
            this.TableAffectee = "";
            this.UtilisateurConnecte = p_utilisateurConnectee;
            this.Role = p_role;
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        public override string ToString()
        {
            return ConstruireChaineDeCaracteresPourFichierCSV();
        }

        private string ConstruireChaineDeCaracteresPourFichierCSV()
        {
            string ligneFichierCSV = "";

            foreach (PropertyInfo propriete in this.GetType().GetProperties())
            {
                ligneFichierCSV += propriete.GetValue(this).ToString();
                ligneFichierCSV += "~";
            }

            return ligneFichierCSV;
        }
    }
}
