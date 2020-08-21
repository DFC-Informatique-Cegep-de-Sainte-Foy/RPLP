﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RplpAvecBD.Data;
using RplpAvecBD.Model;

namespace RplpAvecBD.Controllers
{
    public class TeacherController : Controller
    {
        private readonly RplpContext _rplpContext;

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

                // Ajouter le professeur dans la session
                HttpContext.Session.SetString("ProfesseurSession", JsonConvert.SerializeObject(NouveauProfesseur));

                // Rediriger vers la page de paramètres pour qu'il puisse ajouter son API Key
                return RedirectToAction("Parametres", "Teacher");
            }

            // Récuperér le professeur dans la BD
            Professeur professeurExistent = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == User.Identity.Name);

            // Ajouter le professeur dans la session
            HttpContext.Session.SetString("ProfesseurSession", JsonConvert.SerializeObject(professeurExistent));

            if (professeurExistent.apiKey == null || professeurExistent.apiKey == "")
            {
                // Rediriger vers la page de paramètres pour qu'il puisse ajouter son API Key
                return RedirectToAction("Parametres", "Teacher");
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + professeurExistent.apiKey);

                // Récupérer  les cours de CodePost
                List<Course> listeCours = CodePostController.ObtenirListeDesCourses(client);

                // Ajouter la liste de cours dans la session
                HttpContext.Session.SetString("ListeCoursSession", JsonConvert.SerializeObject(listeCours));

                // Ajouter la liste cours dans la ViewBag
                ViewBag.listeCours = listeCours;

                // Affecter la valeur 0 à l'id du cours choisi au départ
                ViewBag.@idCoursChoisi = 0;
            }

            return View();
        }

        //[Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult Index(Course p_cours)
        {
            // Ajouter le cours choisi dans la session
            HttpContext.Session.SetString("IdCoursChoisiSession", JsonConvert.SerializeObject(p_cours.idCoursChoisi));

            // Ajouter le cours choisi dans la ViewBag
            ViewBag.idCoursChoisi = p_cours.idCoursChoisi;

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
                // Récuperér le professeur dans la session
                Professeur professeurSession = JsonConvert.DeserializeObject<Professeur>(HttpContext.Session.GetString("ProfesseurSession"));

                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + professeurSession.apiKey);

                // Récuperér la liste de cours dans la session
                List<Course> listeCours = JsonConvert.DeserializeObject<List<Course>>(HttpContext.Session.GetString("ListeCoursSession"));

                // Ajouter le cours choisi dans la ViewBag
                ViewBag.listeCours = listeCours;

                if (p_cours.idCoursChoisi != null)
                {
                    // Récupérer la liste des étudiants du cours choisi
                    List<string> listeEtudiant = CodePostController.ObtenirListeEtudiant((int)p_cours.idCoursChoisi, client);

                    // Ajouter la liste des étudiants dans la session
                    HttpContext.Session.SetString("ListeEtudiantsSession", JsonConvert.SerializeObject(listeEtudiant));

                    // Ajouter la liste des étudiants dans la ViewBag
                    ViewBag.listeEtudiant = listeEtudiant;

                    // Récupérer toutes les infos du cours choisi
                    Course coursChoisi = CodePostController.RecupererInfoDeCours((int)p_cours.idCoursChoisi, client);

                    // Activer les 3 boutons dans les paramètres du cours au besoin
                    CodePostController.ActiverLesParametresDeCours(coursChoisi, client);

                    // Ajouter l'objet du cours choisi dans la session
                    HttpContext.Session.SetString("coursChoisi", JsonConvert.SerializeObject(coursChoisi));

                    // Ajouter l'objet du cours choisi dans la ViewBag
                    ViewBag.coursChoisi = coursChoisi;

                    // Récupérer la liste d'Assignments (travaux) du cours choisi
                    List<Assignment> listeAssignment = CodePostController.ObtenirListeAssignmentsDansUnCours((int)p_cours.idCoursChoisi, client);

                    // Ajouter la liste des d'Assignments dans la session
                    HttpContext.Session.SetString("ListeAssignmentsSession", JsonConvert.SerializeObject(listeAssignment));

                    // Ajouter la liste d'Assignments dans la ViewBag
                    ViewBag.listeAssignment = listeAssignment;

                    //obtenir info necessaire sur Assignment de cours choisi pour aficher dans View
                    Dictionary<int, (string, int, int)> infoSurLesAssignments = CodePostController.ObtenirDictionaryTravauxTotalEtManquantsDansCours((int)p_cours.idCoursChoisi, client);

                    // Ajouter info necessaire sur Assignment de cours choisi dans la session
                    HttpContext.Session.SetString("infoSurLesAssignmentsSession", JsonConvert.SerializeObject(infoSurLesAssignments));

                    // Ajouter info necessaire sur Assignment de cours choisi  dans la ViewBag
                    ViewBag.infoSurLesAssignments = infoSurLesAssignments;

                    return View();
                }

                ViewBag.@idCoursChoisi = 0;
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
        [HttpPost]
        [RequestSizeLimit(2_000_000)]  //ajuste la taille limite du fichier 2mb
        public async Task<IActionResult> VerifierListeEtudiant(IFormFile fichierCSV)
        {
            using (HttpClient client = new HttpClient())
            {
                // Récuperér le professeur dans la session
                Professeur professeurSession = JsonConvert.DeserializeObject<Professeur>(HttpContext.Session.GetString("ProfesseurSession"));

                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + professeurSession.apiKey);

                // Récuperér l'objet du cours choisi dans la session
                Course coursChoisi = JsonConvert.DeserializeObject<Course>(HttpContext.Session.GetString("coursChoisi"));

                // Ajouter l'objet du cours choisi dans la ViewBag
                ViewBag.coursChoisi = coursChoisi;

                if (fichierCSV == null)
                {
                    // Récupérer la liste des étudiants dans la session
                    List<string> listeEtudiants = JsonConvert.DeserializeObject<List<string>>(HttpContext.Session.GetString("ListeEtudiantsSession"));

                    if (listeEtudiants.Count == 0)
                    {
                        ViewBag.message = "Vous devez sélectionner un fichier CSV avec la liste de tous les étudiants inscrits dans le cours !";

                        return View("ErreurFichierCSV", "Teacher");
                    }

                    if (listeEtudiants.Count > 0)
                    {
                        ViewBag.afficherUpLoadFichierZip = false;

                        ViewBag.erreurFichierZIPIntrouvable = false;

                        return View("AjouterTravail", new Assignment());
                    }

                }
                else
                {
                    List<string> nouvelleListeEtudiant = CodePostController.AjouterEtudiantsDansCours(coursChoisi.id, client, fichierCSV, professeurSession);

                    if (nouvelleListeEtudiant.Count <= 0)
                    {
                        // Ajouter l'objet du cours choisi dans la ViewBag
                        ViewBag.coursChoisi = coursChoisi;
                        ViewBag.message = "Le fichier '" + fichierCSV.FileName + "' a été refusé pour des raisons de sécurité !";

                        return View("ErreurFichierCSV", "Teacher");
                    }
                    else
                    {
                        // Ajouter l'objet du cours choisi dans la ViewBag
                        ViewBag.coursChoisi = coursChoisi;

                        ViewBag.afficherUpLoadFichierZip = false;

                        ViewBag.erreurFichierZIPIntrouvable = false;
                    }
                    
                    return View("AjouterTravail", new Assignment());
                }
            }

            return View();
        }


        //[Authorize("estProfesseur")]
        [HttpPost]
        [RequestSizeLimit(105_000_000)]  //ajuste la taille limite du fichier a 100 Mb (requete du client)
        public async Task<IActionResult> VerifierFichierZIP(IFormFile fichierZIP)
        {
            using (HttpClient client = new HttpClient())
            {
                // Récuperér le professeur dans la session
                Professeur professeurSession = JsonConvert.DeserializeObject<Professeur>(HttpContext.Session.GetString("ProfesseurSession"));

                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + professeurSession.apiKey);

                // Récuperér l'objet du cours choisi dans la session
                Course coursChoisi = JsonConvert.DeserializeObject<Course>(HttpContext.Session.GetString("coursChoisi"));

                // Ajouter l'objet du cours choisi dans la ViewBag
                ViewBag.coursChoisi = coursChoisi;

                if (fichierZIP == null)
                {
                    ViewBag.afficherUpLoadFichierZip = true;

                    ViewBag.erreurFichierZIP = "Fichier introuvable";

                    return View("AjouterTravail", new Assignment());
                }
                else
                {
                    // Obtenir le nom du fichier
                    string fileName = Path.GetFileName(fichierZIP.FileName);

                    string path = Directory.GetCurrentDirectory();
                    // Obtenir répertoire temporaire de l'utilisateur
                    string pathUser = Path.GetTempPath();

                    // Si le fichier existe déjà, on efface celui qui était présent
                    if (System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Delete(fileName);
                    }
                    
                    // Création du nouveau fichier local et copie le contenu du fichier dedans
                    using (FileStream localFile = System.IO.File.OpenWrite(fileName))
                    using (Stream uploadedFile = fichierZIP.OpenReadStream())
                    {
                        // Recevoir le fichier
                        uploadedFile.CopyTo(localFile);
                        string extension = Path.GetExtension(fichierZIP.FileName).ToLowerInvariant();
                        FileInfo fichierRecu = new FileInfo(fichierZIP.FileName);
                        localFile.Close();
                        uploadedFile.Close();

                        // Vérifie si l'extension est vide ou si n'est pas un .zip)
                        if (string.IsNullOrEmpty(extension) ||
                            (extension != ".zip") ||
                            (fichierRecu.FullName.Contains(".jsp")) ||
                            (fichierRecu.FullName.Contains(".exe")) ||
                            (fichierRecu.FullName.Contains(".msi")) ||
                            (fichierRecu.FullName.Contains(".bat")) ||
                            (fichierRecu.FullName.Contains(".php")) ||
                            (fichierRecu.FullName.Contains(".pht")) ||
                            (fichierRecu.FullName.Contains(".phtml")) ||
                            (fichierRecu.FullName.Contains(".asa")) ||
                            (fichierRecu.FullName.Contains(".cer")) ||
                            (fichierRecu.FullName.Contains(".asax")) ||
                            (fichierRecu.FullName.Contains(".swf")) ||
                            (fichierRecu.FullName.Contains(".com")) ||
                            (fichierRecu.FullName.Contains(".xap")))
                        {
                            try
                            {
                                fichierRecu.Delete();
                            }
                            catch (IOException)
                            {
                                fichierRecu.Delete();
                            }
                            catch (UnauthorizedAccessException)
                            {
                                fichierRecu.Delete();
                            }
                        }

                        if (fichierRecu.Exists)
                        {
                            FileInfo fichierDansUpload = new FileInfo(Path.Combine(path, "Upload", fichierRecu.Name));

                            if (fichierDansUpload.Exists)
                            {
                                try
                                {
                                    fichierDansUpload.Delete();
                                }
                                catch (IOException)
                                {
                                    fichierDansUpload.Delete();
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    fichierDansUpload.Delete();
                                }
                            }

                            // Déplacer le fichier dans le répetoire Upload
                            fichierRecu.MoveTo(Path.Combine(path, "Upload", fichierRecu.Name));

                            if (fichierDansUpload.Exists)
                            {
                                // Décompresser ...



                                //fichierDansUpload.Delete();
                            }
                        }
                        else
                        {
                            ViewBag.afficherUpLoadFichierZip = true;

                            ViewBag.erreurFichierZIP = "Fichier réfusé";

                            return View("AjouterTravail", new Assignment());
                        }
                    }
                }
            }

            ViewBag.afficherUpLoadFichierZip = true;

            ViewBag.erreurFichierZIP = "";

            return View("ResultatAjoutTravail");
        }

        //[Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult AjouterTravail(Assignment p_assignment)
        {
            using (HttpClient client = new HttpClient())
            {
                // Récuperér le professeur dans la session
                Professeur professeurSession = JsonConvert.DeserializeObject<Professeur>(HttpContext.Session.GetString("ProfesseurSession"));

                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + professeurSession.apiKey);

                // Récuperér l'objet du cours choisi dans la session
                Course coursChoisi = JsonConvert.DeserializeObject<Course>(HttpContext.Session.GetString("coursChoisi"));

                // Ajouter l'objet du cours choisi dans la ViewBag
                ViewBag.coursChoisi = coursChoisi;

                if (ModelState.IsValid)
                {
                    
                    if (CodePostController.AssignmentEstDejaCree(p_assignment.name, coursChoisi.id, client))
                    {
                        ModelState.AddModelError("AssignmentDejaCree", "Le travail '" + p_assignment.name + "' existe déjà sur Codepost !");

                        ViewBag.afficherUpLoadFichierZip = false;

                        ViewBag.erreurFichierZIP = "";

                        return View();
                    }
  
                    ViewBag.afficherUpLoadFichierZip = true;

                    ViewBag.erreurFichierZIP = "";

                    return View();
                }

                ViewBag.afficherUpLoadFichierZip = false;

                ViewBag.erreurFichierZIP = "";
            }
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
            //obtenir repertoire temporaire de lutilisateur
            string pathUser = Path.GetTempPath();
            string pathDestination = Path.Combine(pathUser + User.Identity.Name);

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
            ///temporairement retiré pour faciliter les tests.
            //EffacerFichierRecu(path, nomDuTravail);
        }

        //[HttpPost]
        //[RequestSizeLimit(105_000_000)]  //ajuste la taille limite du fichier a 100 Mb (requete du client)
        //public bool UploadZIP(Microsoft.AspNetCore.Http.IFormFile file)
        //{
        //    // obtenir le nom du fichier
        //    string fileName = Path.GetFileName(file.FileName);

        //    string path = Directory.GetCurrentDirectory();
        //    //obtenir repertoire temporaire de lutilisateur
        //    string pathUser = Path.GetTempPath();

        //    // si le fichier existe deja, on efface celui qui etait present 
        //    if (System.IO.File.Exists(fileName))
        //    {
        //        System.IO.File.Delete(fileName);
        //    }

        //    // Creation du nouveau fichier local et copie le contenu du fichier dedans
        //    using (FileStream localFile = System.IO.File.OpenWrite(fileName))
        //    using (Stream uploadedFile = file.OpenReadStream())
        //    {
        //        //recevoir le fichier
        //        uploadedFile.CopyTo(localFile);
        //        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //        FileInfo fichierRecu = new FileInfo(file.FileName);
        //        localFile.Close();
        //        uploadedFile.Close();

        //        //verifie si l'extension est vide ou si n'est pas un .zip)
        //        if (string.IsNullOrEmpty(extension) ||
        //            (extension != ".zip") ||
        //            (fichierRecu.FullName.Contains(".jsp")) ||
        //            (fichierRecu.FullName.Contains(".exe")) ||
        //            (fichierRecu.FullName.Contains(".msi")) ||
        //            (fichierRecu.FullName.Contains(".bat")) ||
        //            (fichierRecu.FullName.Contains(".php")) ||
        //            (fichierRecu.FullName.Contains(".pht")) ||
        //            (fichierRecu.FullName.Contains(".phtml")) ||
        //            (fichierRecu.FullName.Contains(".asa")) ||
        //            (fichierRecu.FullName.Contains(".cer")) ||
        //            (fichierRecu.FullName.Contains(".asax")) ||
        //            (fichierRecu.FullName.Contains(".swf")) ||
        //            (fichierRecu.FullName.Contains(".com")) ||
        //            (fichierRecu.FullName.Contains(".xap")))
        //        {
        //            try
        //            {
        //                fichierRecu.Delete();
        //            }
        //            catch (IOException)
        //            {
        //                fichierRecu.Delete();
        //            }
        //            catch (UnauthorizedAccessException)
        //            {
        //                fichierRecu.Delete();
        //            }
        //        }
        //        if (fichierRecu.Exists)
        //        {
        //            FileInfo fichierDansUpload = new FileInfo(Path.Combine(path, "Upload", fichierRecu.Name));
        //            if (fichierDansUpload.Exists)
        //            {
        //                try
        //                {
        //                    fichierDansUpload.Delete();
        //                }
        //                catch (IOException)
        //                {
        //                    fichierDansUpload.Delete();
        //                }
        //                catch (UnauthorizedAccessException)
        //                {
        //                    fichierDansUpload.Delete();
        //                }
        //            }
        //            //deplacer le fichier dans le repetoire upload
        //            fichierRecu.MoveTo(Path.Combine(path, "Upload", fichierRecu.Name));
        //            ViewBag.Message = "Téléchargement effectué avec succès";
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}


        ///// <summary>
        ///// fonction qui efface le fichier recu en parametre 
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="nomDuTravail"></param>
        //public void EffacerFichierRecu(string path, string nomDuTravail)
        //{
        //    string pathEtNomFichierAEffacer = Path.Combine(path, nomDuTravail).ToString();
        //    FileInfo fichierAEffacer = new FileInfo(pathEtNomFichierAEffacer);
        //    try
        //    {
        //        if (fichierAEffacer.Exists)
        //        {
        //            fichierAEffacer.Delete();
        //        }
        //    }
        //    catch (IOException)
        //    {
        //        fichierAEffacer.Delete();
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        fichierAEffacer.Delete();
        //    }
        //}

        //[HttpPost]
        //[RequestSizeLimit(2_000_000)]  //ajuste la taille limite du fichier 2mb 
        //public IActionResult UploadCSV(Microsoft.AspNetCore.Http.IFormFile file)
        //{
        //    // obtenir le nom du fichier
        //    string fileName = Path.GetFileName(file.FileName);

        //    string path = Directory.GetCurrentDirectory();

        //    //obtenir repertoire temporaire de lutilisateur
        //    string pathUser = Path.GetTempPath();

        //    // si le fichier existe deja, on efface celui qui etait present 
        //    if (System.IO.File.Exists(fileName))
        //    {
        //        System.IO.File.Delete(fileName);
        //    }

        //    // Creation du nouveau fichier local et copie le contenu du fichier dedans
        //    using (FileStream localFile = System.IO.File.OpenWrite(fileName))
        //    using (Stream uploadedFile = file.OpenReadStream())
        //    {
        //        //recevoir le fichier
        //        uploadedFile.CopyTo(localFile);
        //        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //        FileInfo fichierRecu = new FileInfo(file.FileName);
        //        localFile.Close();
        //        uploadedFile.Close();

        //        //verifie si l'extension est vide ou si n'est pas un .zip)
        //        if (string.IsNullOrEmpty(extension) ||
        //            (extension != ".csv") ||
        //            (fichierRecu.FullName.Contains(".jsp")) ||
        //            (fichierRecu.FullName.Contains(".exe")) ||
        //            (fichierRecu.FullName.Contains(".msi")) ||
        //            (fichierRecu.FullName.Contains(".bat")) ||
        //            (fichierRecu.FullName.Contains(".php")) ||
        //            (fichierRecu.FullName.Contains(".pht")) ||
        //            (fichierRecu.FullName.Contains(".phtml")) ||
        //            (fichierRecu.FullName.Contains(".asa")) ||
        //            (fichierRecu.FullName.Contains(".cer")) ||
        //            (fichierRecu.FullName.Contains(".asax")) ||
        //            (fichierRecu.FullName.Contains(".swf")) ||
        //            (fichierRecu.FullName.Contains(".com")) ||
        //            (fichierRecu.FullName.Contains(".xap")))
        //        {
        //            try
        //            {
        //                fichierRecu.Delete();
        //            }
        //            catch (IOException)
        //            {
        //                fichierRecu.Delete();
        //            }
        //            catch (UnauthorizedAccessException)
        //            {
        //                fichierRecu.Delete();
        //            }
        //        }
        //        if (fichierRecu.Exists)
        //        {
        //            FileInfo fichierDansUpload = new FileInfo(Path.Combine(path, "Upload", fichierRecu.Name));
        //            if (fichierDansUpload.Exists)
        //            {
        //                try
        //                {
        //                    fichierDansUpload.Delete();
        //                }
        //                catch (IOException)
        //                {
        //                    fichierDansUpload.Delete();
        //                }
        //                catch (UnauthorizedAccessException)
        //                {
        //                    fichierDansUpload.Delete();
        //                }
        //            }
        //            //deplacer le fichier dans le repetoire upload
        //            fichierRecu.MoveTo(Path.Combine(path, "Upload", fichierRecu.Name));
        //            ViewBag.Message = "Téléchargement effectué avec succès";
        //        }
        //        else
        //        {
        //            ViewBag.Message = "fichier refusé";
        //        }
        //    }
        //    return View();
        //}

        public static List<string> CreerListeEtudiantsAPartirDuCSV(string fichierCsv)
        {
            List<string> listeEtudiant = new List<string> { };
            string line = null;
            string suffixe = "@csfoy.ca";
            int compteur = 0;

            StreamReader file = new System.IO.StreamReader(fichierCsv);
            while ((line = file.ReadLine()) != null)
            {
                if (compteur != 0) //skip la ligne d'entete
                {
                    string[] splitText = line.Split('"');
                    listeEtudiant.Add(splitText[1] + suffixe);
                }
                compteur++;
            }            
            file.Close();
            return listeEtudiant;
        }
    }
}
