﻿
using System;
using System.Collections.Generic;
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
using RplpAvecBD.Controllers;

namespace RplpAvecBD.Controllers
{
    public class TeacherController : Controller
    {
        private readonly RplpContext _rplpContext;

        private Professeur _professeurSession;

        //variable pour le repertoire temp de travail ou on nettoye et efface les fichiers indesirables
        DirectoryInfo destination;


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
            this._professeurSession = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == User.Identity.Name);

            if (this._professeurSession.apiKey == null || this._professeurSession.apiKey == "")
            {
                // Rediriger vers la page de paramètres pour qu'il puisse ajouter son API Key
                return RedirectToAction("Parametres", "Teacher");
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + this._professeurSession.apiKey);
                
                //recuperer  les Cours dans la CodePost
                List<Course> listeCours = CodePostController.ObtenirListeDesCourses(client);
                ViewBag.listeCours = listeCours;
            }

            return View();
        }

        //[Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult Index(Course p_cours)
        {
            if (p_cours.idCoursChoisi == 0 || string.IsNullOrEmpty(p_cours.idCoursChoisi.ToString()))
            {
                ModelState.AddModelError("idCoursChoisi", "Vous devez sélectionner un Cours !");
            }

            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("ResultatUnclaim", new { idProfesseur = p_unclaim.idProfesseur, codeEtudiant = p_unclaim.codeEtudiant });
            //}

            using (HttpClient client = new HttpClient())
            {
                // Récuperér le professeur dans la BD
                this._professeurSession = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == User.Identity.Name);

                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + this._professeurSession.apiKey);

                //recuperer  les Cours dans la CodePost
                List<Course> listeCours = CodePostController.ObtenirListeDesCourses(client);
                ViewBag.listeCours = listeCours;


                List<string> listeEtudiant = CodePostController.ObtenirListeEtudiant(p_cours.idCoursChoisi, client);
                ViewBag.listeEtudiant = listeEtudiant;
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

        //[Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult AjouterTravail(Microsoft.AspNetCore.Http.IFormFile file)
        {
            //// obtenir le nom du fichier
            //string fileName = System.IO.Path.GetFileName(file.FileName);

            //// si le fichier existe deja, on efface celui qui etait present 
            //if (System.IO.File.Exists(fileName))
            //{
            //    System.IO.File.Delete(fileName);
            //}

            //// Creation du nouveau fichier local et copie le contenu du fichier dedans
            //using (FileStream localFile = System.IO.File.OpenWrite(fileName))
            //using (Stream uploadedFile = file.OpenReadStream())
            //{
            //    uploadedFile.CopyTo(localFile);
            //}
            ////confirmation de succes
            //ViewBag.Message = "Téléchargement effectué avec succès";


            ////decompresser le fichier recu (fichier source, destination)
            //Decompresser(fileName, ".\\Fichiers");

            return View();
        }


        //[Authorize("estProfesseur")]
        public IActionResult ResultatAjoutTravail()
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

        public void Decompresser_faireMenage_CodePost(String nomDuTravail)
        {
            //TEMPORAIRE 
            // pour pas avoir a effacer a chaque fois durant les tests
            if (Directory.Exists(@"C:\Users\the_e\AppData\Local\Temp\1992178@csfoy.ca"))
            {
                Directory.Delete(@"C:\Users\the_e\AppData\Local\Temp\1992178@csfoy.ca", true);
            }
            //obtenir repertoire courrant
            string path = Directory.GetCurrentDirectory();
            Console.WriteLine("current directory : " + path);
            //obtenir repertoire temporaire de lutilisateur
            string pathUser = Path.GetTempPath();
            Console.WriteLine("temp path : " + pathUser);

            string pathDestination = Path.Combine(pathUser + User.Identity.Name);
            Console.WriteLine("path destination ..pathuser / matricule : " + pathDestination);

            //creer un repertoire avec le matricule de l'utilisateur dans le repertoire temporaire
            Directory.CreateDirectory(Path.Combine(pathUser, User.Identity.Name));

            //decompresser
            Decompresser(Path.Combine(path, nomDuTravail), pathDestination);
            //conversion de type string -> DirectoryInfo 
            destination = new DirectoryInfo(pathDestination);

            //parcourir les repertoires qui correspondent au regex
            Regex regexValidationFolder = new Regex("[A-Za-z]*_(?<numeroMatricule>(\\d{7,}))_[A-Za-z]*");
            foreach (DirectoryInfo dir in destination.GetDirectories())
            {
                Match resultat = regexValidationFolder.Match(dir.Name);
                if (resultat.Success)
                {
                    //effacer les fichiers indésirables
                    string[] TypeDeFichierAEffacer = new string[] { "*.suo", "*.user", "*.userosscache", "*.sln.docstates", "*.project", "*.mdj", "*.svg" };
                    foreach (string type in TypeDeFichierAEffacer)
                    {
                        {
                            FileInfo[] fichier = destination.GetFiles(type, SearchOption.AllDirectories);
                            foreach (FileInfo fi in fichier)
                            {
                                try
                                {
                                    if (fi.Extension == type) ;
                                    fi.Delete();
                                }
                                catch (IOException)
                                {
                                    fi.Delete();
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    fi.Delete();
                                }
                            }
                        }
                    }
                    //renommer le repertoire en utilisant la commande move. 
                    string matricule = resultat.Groups["numeroMatricule"].Value;
                    try
                    {
                        Directory.Move(Path.Combine(destination.ToString(), dir.Name), Path.Combine(destination.ToString(), matricule + "@csfoy.ca"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                //effacer les repertoires indésirables
                string[] RepetoireAEffacer = new string[] { ".vs", "bin", "obj", "build" };
                foreach (string NomDirectoryAEffacer in RepetoireAEffacer)
                {
                    DirectoryInfo[] repertoire = destination.GetDirectories(NomDirectoryAEffacer, SearchOption.AllDirectories); ;

                    foreach (DirectoryInfo di in repertoire)
                    {
                        try
                        {
                            di.Delete(true);
                        }
                        catch (IOException)
                        {
                            di.Delete(true);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            di.Delete(true);
                        }
                    }
                }
            }
            ///temporairement retire pour faciliter les tests.
            //EffacerFichierRecu(path, nomDuTravail);
        }

        /// <summary>
        /// fonction qui efface le fichier recu en parametre 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nomDuTravail"></param>
        public void EffacerFichierRecu(string path, string nomDuTravail)
        {
            string pathEtNomFichierAEffacer = Path.Combine(path, nomDuTravail).ToString();
            FileInfo fichierAEffacer = new FileInfo(pathEtNomFichierAEffacer);
            try
            {
                if (fichierAEffacer.Exists)
                {
                    fichierAEffacer.Delete();
                }
            }
            catch (IOException)
            {
                fichierAEffacer.Delete();
            }
            catch (UnauthorizedAccessException)
            {
                fichierAEffacer.Delete();
            }
        }

        //temporaire
        public IActionResult Script()
        {
            Decompresser_faireMenage_CodePost("Travail_Demo_1_420429SF_467_OK.zip");
            return View();
        }
    }
}
