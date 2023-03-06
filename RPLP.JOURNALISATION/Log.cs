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
        public Guid Identifiant { get; private set; }
        public string TypeDeLog { get; private set; }
        public DateTime DateDuLog { get; private set; }


        //Erreurs
        public string? ExceptionLevee { get; private set; }
        public string? StackTrace { get; private set; }

        //Api
        public string RouteApi { get; private set; }
        public int CodeStatusRequete { get; set; }
        public int RequetesRestantes { get; private set; }


        //Base de données 
        public string TableAffectee { get; private set; }

        //Connexion
        public string UtilisateurConnecte { get; private set; }
        public string Role { get; set; }


        public string MessageSupplementaire { get; private set; }

        //Complet
        public Log(string p_TypeDeLog, string p_exception, string p_stacktrace, string p_RouteApi,
            int p_CodeStatusRequete, int p_requetesRestantes, string p_tableAffectee, string p_utilisateurConnectee, string p_role, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = p_TypeDeLog;
            this.DateDuLog = DateTime.Now;
            this.ExceptionLevee = p_exception;
            this.StackTrace = p_stacktrace;
            this.RouteApi = p_RouteApi;
            this.CodeStatusRequete = p_CodeStatusRequete;
            this.RequetesRestantes = p_requetesRestantes;
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
            this.RequetesRestantes = 0;
            this.TableAffectee = p_tableAffectee;
            this.UtilisateurConnecte = "";
            this.Role = "";
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des erreurs
        public Log(string p_exception, string p_stacktrace, string p_messageSupplementaire, int defaut)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = "Erreur";
            this.DateDuLog = DateTime.Now;
            this.UtilisateurConnecte = "";
            this.ExceptionLevee = p_exception;
            this.StackTrace = p_stacktrace;
            this.RouteApi = "";
            this.CodeStatusRequete = 0;
            this.RequetesRestantes = 0;
            this.TableAffectee = "";
            this.UtilisateurConnecte = "";
            this.Role = "";
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des appels API RPLP
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
            this.RequetesRestantes = 0;
            this.TableAffectee = "";
            this.UtilisateurConnecte = "";
            this.Role = "";
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation des appels API GitHub
        public Log(string p_RouteApi, int p_CodeStatusRequete, int p_requetesRestantes, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = "AppelAPI";
            this.DateDuLog = DateTime.Now;
            this.UtilisateurConnecte = "";
            this.ExceptionLevee = null;
            this.StackTrace = null;
            this.RouteApi = p_RouteApi;
            this.CodeStatusRequete = p_CodeStatusRequete;
            this.RequetesRestantes = p_requetesRestantes;
            this.TableAffectee = "";
            this.UtilisateurConnecte = "";
            this.Role = "";
            this.MessageSupplementaire = p_messageSupplementaire;
        }

        //Constructeur pour la journalisation de la connexion 
        public Log(string p_utilisateurConnectee, string p_role, string p_messageSupplementaire)
        {
            this.Identifiant = Guid.NewGuid();
            this.TypeDeLog = "ConnexionAuth0";
            this.DateDuLog = DateTime.Now;
            this.UtilisateurConnecte = "";
            this.ExceptionLevee = null;
            this.StackTrace = null;
            this.RouteApi = "";
            this.CodeStatusRequete = 0;
            this.RequetesRestantes = 0;
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

