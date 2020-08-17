
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RplpAvecBD.Data;
using RplpAvecBD.Model;

namespace RplpAvecBD.Controllers
{
    public class TeacherController : Controller
    {
        private readonly RplpContext _rplpContext;

        public TeacherController(RplpContext p_context)
        {
            _rplpContext = p_context;
        }

        public bool estProfesseurExistant(string p_courriel)
        {
            // Vérifier si ce professeur existe déjà dans la base de données
            Professeur professeur = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == p_courriel);

            return (professeur != null);
        }

        public Professeur ajouterProfesseur(string p_courriel)
        {
            Professeur nouveauProfesseur = new Professeur();
            nouveauProfesseur.courriel = p_courriel;

            // Ajouter le professeur dans la base de données
            _rplpContext.Professeurs.Add(nouveauProfesseur);
            _rplpContext.SaveChanges();

            return nouveauProfesseur;
        }

        //[Authorize("estProfesseur")]
        public IActionResult Index()
        {
            // Si ce professeur n'existe pas dans la base de données
            if (!estProfesseurExistant(User.Identity.Name))
            {
                // Ajouter le professeur dans la base de données
                Professeur NouveauProfesseur = ajouterProfesseur(User.Identity.Name);

                // Rediriger vers la page de paramètres pour qu'il puisse ajouter son API Key
                return RedirectToAction("Parametres", "Teacher");
            }

            // Récuperér le professeur dans la BD
            Professeur professeurExistente = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == User.Identity.Name);

            if (professeurExistente.apiKey == null || professeurExistente.apiKey == "")
            {
                // Rediriger vers la page de paramètres pour qu'il puisse ajouter son API Key
                return RedirectToAction("Parametres", "Teacher");
            }

            return View();
        }

        //[Authorize("estProfesseur")]
        public IActionResult Parametres()
        {
            // Récuperér le professeur dans la BD
            Professeur professeurExistente = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == User.Identity.Name);

            // L'ajouter dans une ViewBag pour récuperer ses informations dans la page Parametres.cshtml
            ViewBag.InfosProfesseur = professeurExistente;

            return View();
        }

        //[Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult Parametres(Professeur p_professeurModel)
        {
            // Récuperér le professeur dans la BD
            Professeur professeurExistente = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == User.Identity.Name);

            if (string.IsNullOrEmpty(p_professeurModel.apiKey))
            {
                ModelState.AddModelError("apiKey", "Le champs API Key ne doit pas être vide !");
            }


            if (string.IsNullOrEmpty(p_professeurModel.nom))
            {
                ModelState.AddModelError("nom", "Le champs Nom ne doit pas être vide !");
            }

            ModelState.Remove("courriel");

            // L'ajouter dans une ViewBag pour récuperer ses informations dans la page Parametres.cshtml
            ViewBag.InfosProfesseur = professeurExistente;

            if (ModelState.IsValid)
            {
                return RedirectToAction("ResultatMiseAJourParametres", new { p_nom = p_professeurModel.nom, p_courriel = professeurExistente.courriel, p_apiKey = p_professeurModel.apiKey });
            }

            return View();
        }

        //[Authorize("estProfesseur")]
        public IActionResult ResultatMiseAJourParametres(string p_nom, string p_courriel, string p_apiKey)
        {
            mettreAJourParametres(p_nom, p_courriel, p_apiKey);

            return View();
        }


        //[Authorize("estProfesseur")]
        public IActionResult AideSelectionnerCours()
        {
            return View();
        }

        //[Authorize("estProfesseur")]
        public IActionResult AjouterTravail()
        {
            return View();
        }


        public void mettreAJourParametres(string p_nom, string p_courriel, string p_apiKey)
        {
            Professeur professeur = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == p_courriel);
            professeur.nom = p_nom;
            professeur.apiKey = p_apiKey;

            // Mettre à jour les informations du professeur dans la base de données
            _rplpContext.Professeurs.Update(professeur);
            _rplpContext.SaveChanges();
        }

        /// <summary>
        /// fonction pour decompresser fichier zip recoit en parametre le nom du fichier et le path de destination
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="destinationFolder"></param>
        private void Decompresser(string fileName, string destinationFolder)
        {
            if (fileName != null)
            {
                ZipFile.ExtractToDirectory(fileName, destinationFolder);
            }
        }

        private string ObtenirPathDestinationFichier()
        {
            string nomRepertoire = User.Identity.Name;
            string pathDestination = Path.GetTempPath() + nomRepertoire;
            return pathDestination;
        }

        public void Decompresser_CodePost()
        {
            //TEMPORAIRE
            string nomDuTravail = "patate";
            if (Directory.Exists("C:\\Users\\the_e\\AppData\\Local\\Temp\\1992178@csfoy.ca"))
            {
                Directory.Delete("C:\\Users\\the_e\\AppData\\Local\\Temp\\1992178@csfoy.ca", true);
            }


            string path = Directory.GetCurrentDirectory() + "\\";

            //obtenir repertoire temporaire de lutilisateur
            Console.WriteLine("current path" + path);
            string pathUser = Path.GetTempPath();
            string pathDestination = pathUser + User.Identity.Name;
            //creer un repertoire avec le matricule de l'utilisateur dans le repertoire temporaire
            Directory.CreateDirectory(pathUser + User.Identity.Name);
            Console.WriteLine("user temp path" + pathUser);
            Decompresser(path + "Travail_Demo_1_420429SF_467_OK.zip", pathDestination);
            DirectoryInfo destination = new DirectoryInfo(pathDestination);

            //parcourir les repertoires
            Regex regexValidationFolder = new Regex("[A-Za-z]*_(?<numeroMatricule>(\\d{7,}))_[A-Za-z]*");
            foreach (DirectoryInfo dir in destination.GetDirectories())
            {
                //if (dir.Name == "obj" || dir.Name == "bin" || dir.Name == "vs")
                //{
                //    Console.WriteLine("deleted");
                //    dir.Delete(true);
                //}
                Match resultat = regexValidationFolder.Match(dir.Name);
                if (resultat.Success)
                {
                    //nettoyer (effacer tout les) "*.suo", "*.user", "*.userosscache", "*.sln.docstates", ".vs", "bin", "obj", "build", "*.class", ".settings", ".classpath", ".project", "*.mdj", "*.svg"
                    foreach (FileInfo file in dir.GetFiles())
                    {

                        string extension = file.Extension.ToLower();
                        if (
                            extension.Equals(".suo") ||
                            extension.Equals(".user") ||
                            extension.Equals(".userosscache") ||
                            extension.Equals(".sln.docstates") ||
                            extension.Equals(".project") ||
                            extension.Equals(".mdj") ||
                            extension.Equals(".svg") ||

                            // a verifier avec thiago si repertoires ou extensions de fichiers ?????
                            extension.Equals(".vs") ||
                            extension.Equals("bin") ||
                            extension.Equals("obj") ||
                            extension.Equals("build")
                            )
                        {
                            file.Delete();
                        }
                    }
                    //renommer le repertoire en utilisant la commande move. 
                    string matricule = resultat.Groups["numeroMatricule"].Value;
                    try
                    {
                        Directory.Move(destination + "//" + dir.Name, destination + "//" + matricule + "@csfoy.ca");

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("exception! path : " + path + "    destination : " + destination);
                        Console.WriteLine(e.Message);
                    }


                }


            }

        }

        //temporaire
        public IActionResult Script()
        {
            Decompresser_CodePost();
            return View();
        }
    }
}
