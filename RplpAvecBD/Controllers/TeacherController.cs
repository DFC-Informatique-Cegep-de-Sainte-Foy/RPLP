using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
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

        public TeacherController(RplpContext p_context)
        {
            _rplpContext = p_context;
        }

        // -------------------------------------------------------- 
        //
        // Méthodes HTTP Get / HTTP Post
        // 
        // -------------------------------------------------------- 

        [Authorize("estProfesseur")]
        public IActionResult Index(int p_idCoursChoisi)
        {
            // Si ce professeur n'existe pas dans la base de données
            if (!estProfesseurExistantBD(User.Identity.Name))
            {
                // Ajouter le professeur dans la base de données
                Professeur NouveauProfesseur = ajouterProfesseurBD(User.Identity.Name);

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
                ViewBag.@idCoursChoisi = p_idCoursChoisi;

                ViewBag.suppressionAssignment = false;
            }

            return View();
        }

        [Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult Index(Course p_cours)
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }
            
            // Ajouter le cours choisi dans la session
            HttpContext.Session.SetString("IdCoursChoisiSession", JsonConvert.SerializeObject(p_cours.idCoursChoisi));

            // Ajouter le cours choisi dans la ViewBag
            ViewBag.idCoursChoisi = p_cours.idCoursChoisi;

            if (p_cours.idCoursChoisi == 0 || string.IsNullOrEmpty(p_cours.idCoursChoisi.ToString()))
            {
                ModelState.AddModelError("idCoursChoisi", "Vous devez sélectionner un Cours !");
            }

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

                    ViewBag.suppressionAssignment = false;

                    return View();
                }

                ViewBag.@idCoursChoisi = 0;

                ViewBag.suppressionAssignment = false;
            }

            return View();
        }

        [Authorize("estProfesseur")]
        public IActionResult Parametres()
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

            // Récuperér le professeur dans la BD
            Professeur professeurExistente = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == User.Identity.Name);

            // L'ajouter dans une ViewBag pour récuperer ses informations dans la page Parametres.cshtml
            ViewBag.InfosProfesseur = professeurExistente;

            return View();
        }

        [Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult Parametres(Professeur p_professeurModel)
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

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

        [Authorize("estProfesseur")]
        public IActionResult ResultatMiseAJourParametres(string p_nom, string p_courriel, string p_apiKey)
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

            mettreAJourParametresBD(p_nom, p_courriel, p_apiKey);

            return View();
        }

        [Authorize("estProfesseur")]
        [HttpPost]
        [RequestSizeLimit(2_000_000)]  //ajuste la taille limite du fichier 2mb
        public IActionResult VerifierListeEtudiant(IFormFile fichierCSV)
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

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

                    if (nouvelleListeEtudiant == null)
                    {
                        // Ajouter l'objet du cours choisi dans la ViewBag
                        ViewBag.coursChoisi = coursChoisi;
                        ViewBag.message = "Le fichier '" + fichierCSV.FileName + "' a été refusé pour des raisons de sécurité !";

                        return View("ErreurFichierCSV", "Teacher");
                    }

                    // Ajouter l'objet du cours choisi dans la ViewBag
                    ViewBag.coursChoisi = coursChoisi;

                    ViewBag.afficherUpLoadFichierZip = false;

                    ViewBag.erreurFichierZIPIntrouvable = false;
                    
                    return View("AjouterTravail", new Assignment());
                }
            }

            return View();
        }

        [Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult AjouterTravail(Assignment p_assignment)
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

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

                    // Ajouter le nom du travail dans la session
                    HttpContext.Session.SetString("nomTravailSession", JsonConvert.SerializeObject(p_assignment.name));

                    // Ajouter les points du travail dans la session
                    HttpContext.Session.SetString("pointsTravailSession", JsonConvert.SerializeObject(p_assignment.points));

                    return View();
                }

                ViewBag.afficherUpLoadFichierZip = false;

                ViewBag.erreurFichierZIP = "";

                // Ajouter le professeur dans la session
                HttpContext.Session.SetString("nomTravailSession", JsonConvert.SerializeObject(p_assignment.name));

                // Ajouter les points du travail dans la session
                HttpContext.Session.SetString("pointsTravailSession", JsonConvert.SerializeObject(p_assignment.points));
            }

            return View();
        }

        [Authorize("estProfesseur")]
        [HttpPost]
        [RequestSizeLimit(105_000_000)]  //ajuste la taille limite du fichier a 100 Mb (requete du client)
        public IActionResult VerifierFichierZIP(IFormFile fichierZIP)
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

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

                    // Obtenir répertoire temporaire courant
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

                            if (!Directory.Exists(Path.Combine(path, "Upload")))
                            {
                                Directory.CreateDirectory(Path.Combine(path, "Upload"));
                            }

                            // Déplacer le fichier dans le répetoire Upload
                            fichierRecu.MoveTo(Path.Combine(path, "Upload", fichierRecu.Name));

                            string pathFichierZip = Path.Combine(path, "Upload", fichierRecu.Name);
                            
                            // Récuperér le nom du travail dans la session
                            string nomTravail = JsonConvert.DeserializeObject<string>(HttpContext.Session.GetString("nomTravailSession"));

                            // Récuperér les points du travail dans la session
                            int pointsTravail = JsonConvert.DeserializeObject<int>(HttpContext.Session.GetString("pointsTravailSession"));

                            // Décompresser et faire le menage
                            DirectoryInfo pathTempDestination = DecompresserFaireMenageCodePost(nomTravail, pathFichierZip);

                            // -------------------------
                            // Envoyer à Codepost
                            // -------------------------

                            CodePostController.CreerAssignment(nomTravail, pointsTravail, coursChoisi.id, client);

                            int idAssignment = CodePostController.ObtenirIdAssignment(coursChoisi.id, nomTravail, client);

                            foreach (DirectoryInfo dir in pathTempDestination.GetDirectories())
                            {
                                string courrielEtudiant = dir.Name;
                                CodePostController.CreerSubmission(idAssignment, courrielEtudiant, client);
                            }

                            CodePostController.UploadTravauxTousEtudiants(idAssignment, pathTempDestination.ToString(), client);

                            (int, int) nbSoumissionsCrees = CodePostController.SubmissionsTotalEtManquantsDansAssignment(idAssignment, client);

                            if (nbSoumissionsCrees.Item1 == 0)
                            {
                                // Effacer fichier Zip dans le répertoire Upload
                                fichierDansUpload.Delete();

                                // Effacer le répertoire temporaire dans le OS du client 
                                System.IO.Directory.Delete(pathTempDestination.ToString(), true);

                                CodePostController.SupprimerAssignment(idAssignment, client);

                                ViewBag.afficherUpLoadFichierZip = true;

                                ViewBag.erreurFichierZIP = "Problème fichier ZIP";

                                return View("AjouterTravail", new Assignment());
                            }

                            // Effacer fichier Zip dans le répertoire Upload
                            fichierDansUpload.Delete();

                            // Effacer le répertoire temporaire dans le OS du client 
                            System.IO.Directory.Delete(pathTempDestination.ToString(), true);
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

        [Authorize("estProfesseur")]
        public IActionResult ResultatAjoutTravail()
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

            return View();
        }

        [Authorize("estProfesseur")]
        public IActionResult AideSelectionnerCours()
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

            return View();
        }

        [Authorize("estProfesseur")]
        public IActionResult GuideCodePostProfesseur()
        {
            // Vérifier si la session a été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") == null)
            {
                return RedirectToAction("ErreurSession", "Teacher");
            }

            return View();
        }

        [Authorize("estProfesseur")]
        public IActionResult ErreurSession()
        {
            return View();
        }


        // -------------------------------------------------------- 
        //
        // Méthodes locales
        // 
        // -------------------------------------------------------- 

        public bool estProfesseurExistantBD(string p_courriel)
        {
            // Vérifier si ce professeur existe déjà dans la base de données
            Professeur professeur = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == p_courriel);

            return (professeur != null);
        }

        public Professeur ajouterProfesseurBD(string p_courriel)
        {
            Professeur nouveauProfesseur = new Professeur();
            nouveauProfesseur.courriel = p_courriel;

            // Ajouter le professeur dans la base de données
            _rplpContext.Professeurs.Add(nouveauProfesseur);
            _rplpContext.SaveChanges();

            return nouveauProfesseur;
        }

        public void mettreAJourParametresBD(string p_nom, string p_courriel, string p_apiKey)
        {
            Professeur professeur = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == p_courriel);
            professeur.nom = p_nom;
            professeur.apiKey = p_apiKey;

            // Mettre à jour les informations du professeur dans la base de données
            _rplpContext.Professeurs.Update(professeur);
            _rplpContext.SaveChanges();
        }

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

        public DirectoryInfo DecompresserFaireMenageCodePost(string p_nomDuTravail, string p_pathFichierZip)
        {
            // Obtenir répertoire temporaire de l'utilisateur
            string pathTempUser = Path.GetTempPath();
            string pathTempDestination = Path.Combine(pathTempUser + User.Identity.Name);

            if (System.IO.Directory.Exists(pathTempDestination))
            {
                System.IO.Directory.Delete(pathTempDestination, true);
            }

            // Créer un répertoire avec le matricule de l'utilisateur dans le répertoire temporaire
            Directory.CreateDirectory(Path.Combine(pathTempUser, User.Identity.Name));

            // Décompresser le fichier Zip
            Decompresser(p_pathFichierZip, pathTempDestination);
            
            // Conversion de type string -> DirectoryInfo 
            DirectoryInfo destination = new DirectoryInfo(pathTempDestination);

            // Parcourir les répertoires qui correspondent au regex
            Regex regexValidationFolder = new Regex("[A-Za-z]*_(?<numeroMatricule>(\\d{7,}))_[A-Za-z]*");
            
            foreach (DirectoryInfo dir in destination.GetDirectories())
            {
                Match resultat = regexValidationFolder.Match(dir.Name);
                if (resultat.Success)
                {
                    // Effacer les fichiers indésirables
                    string[] TypeDeFichierAEffacer = new string[] { "*.suo", "*.user", "*.userosscache", "*.sln.docstates", ".vs", "bin", "obj", "build", "*.class", ".settings", ".classpath", ".project", "*.mdj", "*.svg"};
                    
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

                // Effacer les répertoires indésirables
                string[] RepetoireAEffacer = new string[] { ".vs", "bin", "obj", "build", ".settings" };
                
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

            return destination;
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

        public void SuppressionAssignment(string id)
        {
            // Remarque : id = nomTravail

            // Si la session n'a pas été expiré
            if (HttpContext.Session.GetString("ProfesseurSession") != null)
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

                    // Ajouter l'iddu cours choisi dans la ViewBag
                    ViewBag.idCoursChoisi = coursChoisi.id;

                    int idAssignment = CodePostController.ObtenirIdAssignment(coursChoisi.id, id, client);

                    CodePostController.SupprimerAssignment(idAssignment, client);
                }

                // Récuperér la liste des étudiants dans la session
                List<string> listeEtudiant = JsonConvert.DeserializeObject<List<string>>(HttpContext.Session.GetString("ListeEtudiantsSession"));

                // Ajouter la liste des étudiants dans la ViewBag
                ViewBag.listeEtudiant = listeEtudiant;

                /// Récuperér la liste d'assignments dans la session
                List<Assignment> listeAssignment = JsonConvert.DeserializeObject<List<Assignment>>(HttpContext.Session.GetString("ListeAssignmentsSession"));

                // Ajouter la liste d'Assignments dans la ViewBag
                ViewBag.listeAssignment = listeAssignment;

                // Récuperér les infos necessaires sur Assignment du cours choisi dans la session
                Dictionary<int, (string, int, int)> infoSurLesAssignments = JsonConvert.DeserializeObject<Dictionary<int, (string, int, int)>>(HttpContext.Session.GetString("infoSurLesAssignmentsSession"));

                // Ajouter info necessaire sur Assignment de cours choisi  dans la ViewBag
                ViewBag.infoSurLesAssignments = infoSurLesAssignments;
            }
        }
    }
}
