using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RplpAvecBD.Data;
using RplpAvecBD.Model;

namespace RplpAvecBD.Controllers
{
    public class CodePostController : Controller
    {
        
        private readonly RplpContext _rplpContext;
        private Professeur _professeur; 
        HttpClient _client { get; set; }

        //public CodePostController(string p_courriel)
        //{
        //    this._professeur = _rplpContext.Professeurs.SingleOrDefault(p => p.courriel == p_courriel);
        //    this._client = new HttpClient();

        //    _client.DefaultRequestHeaders.Add("authorization", "Token " + _professeur.apiKey);
        //}

        /// <summary>
        /// Procédure pour obtenir la liste de tous les Cours
        /// </summary>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        /// <returns></returns>
        public static List<Course> ObtenirListeDesCourses(HttpClient p_client)
        {
            var task = p_client.GetAsync("https://api.codepost.io/courses/");
            task.Wait();
            var result = task.Result;
            string chaineDesCourses = result.Content.ReadAsStringAsync().Result;
            string[] tableCours = SeparerChaine(chaineDesCourses);

            List<Course> listeCours = new List<Course>();

            for (int i = 0; i < tableCours.Length; i++)
            {
                if (tableCours[i] != null)
                {
                    JObject objet = JObject.Parse(tableCours[i]);

                    int id = (int)objet.SelectToken("id");
                    listeCours.Add(RecupererInfoDeCours(id, p_client));
                }
            }
            return listeCours;
        }

        /// <summary>
        /// Procédure pour séparer la chaine des Cours dans une table des chaines des Cours
        /// </summary>
        /// <param name="p_chaineDesCourses">La chaine des tous les cours, qui a été retourné de CodePost</param>
        /// <returns></returns>
        public static string[] SeparerChaine(string p_chaine)
        {
            if (p_chaine != "")
            {
                string[] separator = new string[] { "[{", "}]", "},{" };
                string[] table = p_chaine.Split(separator, StringSplitOptions.None);

                //****** pour ajouter les "{"..."}" de sorte que la syntaxe est comme dans JSon et suprimer ler chaines vides 
                int j = 0;
                string[] newTable = new string[table.Length];
                for (int i = 0; i < table.Length; i++)
                {
                    if (table[i] != "")
                    {
                        newTable[j] = "{" + table[i] + "}";
                        j++;
                    }
                }
                //***********
                return newTable;
            }
            else
            {
                string[] newTable = new string[1];
                return newTable;
            }
        }

        /// <summary>
        /// Procédure pour recupérer toutes les informations nécessaires sur le Cours
        /// </summary>
        /// <param name="p_id">Id de Cours dans CodePost</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        /// <returns></returns>
        public static Course RecupererInfoDeCours(int p_id, HttpClient p_client)
        {
            var task = p_client.GetAsync("https://api.codepost.io/courses/" + p_id + "/");
            task.Wait();
            var result = task.Result;

            string chaineInfoCours = result.Content.ReadAsStringAsync().Result;
            JObject objet = JObject.Parse(chaineInfoCours);

            int id = (int)objet.SelectToken("id");
            string name = (string)objet.SelectToken("name");
            string period = (string)objet.SelectToken("period");
            bool sendReleasedSubmissionsToBack = (bool)objet.SelectToken("sendReleasedSubmissionsToBack");
            bool emailNewUsers = (bool)objet.SelectToken("emailNewUsers");
            bool anonymousGradingDefault = (bool)objet.SelectToken("anonymousGradingDefault");

            Course cours = new Course(id, name, period, sendReleasedSubmissionsToBack, emailNewUsers, anonymousGradingDefault);

            return cours;
        }

        /// <summary>
        /// Procédure pour activer des paramètres du cours
        /// </summary>
        /// <param name="p_id">Id du Cours dans CodePost</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        public static void ActiverLesParametresDeCours(Course p_cours, HttpClient p_client)
        {
            if (p_cours.anonymousGradingDefault == false)
            {
                p_cours.anonymousGradingDefault = true;
            }
            if (p_cours.emailNewUsers == false)
            {
                p_cours.emailNewUsers = true;
            }
            if (p_cours.sendReleasedSubmissionsToBack == false)
            {
                p_cours.sendReleasedSubmissionsToBack = true;
            }

            var json = JsonConvert.SerializeObject(p_cours);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var task = p_client.PatchAsync("https://api.codepost.io/courses/" + p_cours.id + "/", content);
            task.Wait();
            var result = task.Result;
            //ViewData["result"] = result;
        }

        /// <summary>
        /// Procédure pour ajouter les étudiants dans un Cours et leur donner le rôle de graders
        /// </summary>
        /// <param name="p_idCours">Id de Cours dans CodePost</param>
        /// <param name="p_listeEtudiants">Liste de tous les étudiants qui suivent le Cours</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        public static void AjouterEtudiantsDansCours(int p_idCours, List<string> p_listeEtudiants, HttpClient p_client, IFormFile p_fichierCSV, Professeur p_professeur)
        {
            CourseRoster courseRoster = new CourseRoster(p_idCours, p_listeEtudiants);

            if (p_fichierCSV != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), p_fichierCSV.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    p_fichierCSV.CopyToAsync(stream);
                    stream.Close();
                    stream.Dispose();
                }

                List<string> NouvelleListeEtudiants = TeacherController.CreerListeEtudiantsAPartirDuCsv(filePath);

                courseRoster.students = NouvelleListeEtudiants;
                courseRoster.graders = NouvelleListeEtudiants;
                courseRoster.graders.Add(p_professeur.courriel);
            }

            var json = JsonConvert.SerializeObject(courseRoster);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var task = p_client.PatchAsync("https://api.codepost.io/courses/" + p_idCours + "/roster/", content);
            task.Wait();
            var result = task.Result;
        }

        /// <summary>
        /// Procédure pour créér un assignment (travail) sur CodePost 
        /// </summary>
        /// <param name="p_name">Le nom de travail</param>
        /// <param name="p_points">Le points pour le travail</param>
        /// <param name="p_idCourse">L'id du cours auquel appartient le travail</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        public static void CreerAssignment(string p_name, int p_points, int p_idCourse, HttpClient p_client)
        {
            bool estCree = AssignmentEstDejaCreer(p_name, p_idCourse, p_client);

            if (!estCree)
            {
                Assignment assignment = new Assignment(p_name, p_points, p_idCourse, true, true);
                var json = JsonConvert.SerializeObject(assignment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var task = p_client.PostAsync("https://api.codepost.io/assignments/", content);
                task.Wait();
                var result = task.Result;
                //ViewData["result"] = result;
            }
            else
            {
                throw new ArgumentException("Le travail existe déjà sur Codepost", p_name);
            }

        }

        /// <summary>
        /// Le fonction pour verifier si travail avec nom donné est cree
        /// </summary>
        /// <param name="p_name">le nom de travail pour cree</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        /// <returns>true si le travail est cree ou false si non</returns>
        public static bool AssignmentEstDejaCreer(string p_name, int p_idCours, HttpClient p_client)
        {
            bool estCreer = false;
            int i = 0;
            List<Assignment> listAssignments = ObtenirListeAssignmentsDansUnCours(p_idCours, p_client);

            while (i < listAssignments.Count && !estCreer)
            {
                if (listAssignments[i].name == p_name)
                {
                    estCreer = true;
                    break;
                }
                else
                {
                    i++;
                }
            }
            return estCreer;
        }

        /// <summary>
        /// Le fonction pour obtenir la liste de tous les assignments dans cours donnee
        /// </summary>
        /// <param name="p_idCours">l'id de Cours</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        /// <returns>la liste d'objet de type Assignments</returns>
        public static List<Assignment> ObtenirListeAssignmentsDansUnCours(int p_idCours, HttpClient p_client)
        {
            List<Assignment> listAssignment = new List<Assignment>();

            var task = p_client.GetAsync("https://api.codepost.io/courses/" + p_idCours + "/");
            task.Wait();
            var result = task.Result;

            string shaineInfoSurCours = result.Content.ReadAsStringAsync().Result;
            JObject objet = JObject.Parse(shaineInfoSurCours);
            IEnumerable<JToken> assignments = objet.SelectToken("assignments");

            foreach (JToken travail in assignments)
            {
                listAssignment.Add(RecupererInfoSurAssignment((int)travail, p_client));
            }
            return listAssignment;
        }

        public static List<string> ObtenirListeEtudiant(int p_idCours, HttpClient p_client)
        {
            List<string> listeEtudiant = new List<string>();

            if (p_idCours > 0)
            {
                var task = p_client.GetAsync("https://api.codepost.io/courses/" + p_idCours + "/roster/");
                task.Wait();
                var result = task.Result;

                string chaineInfoSurCoursRoster = result.Content.ReadAsStringAsync().Result;
                JObject objet = JObject.Parse(chaineInfoSurCoursRoster);
                IEnumerable<JToken> students = objet.SelectToken("students");

                foreach (JToken etudiant in students)
                {
                    listeEtudiant.Add((string)etudiant);
                }
            }

            return listeEtudiant;
        }

        /// <summary>
        /// Procédure pour récupérer les informations sur un Assignment
        /// </summary>
        /// <param name="p_id">L'id de l'assignment</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        /// <returns>Objet du type Assignment</returns>
        public static Assignment RecupererInfoSurAssignment(int p_id, HttpClient p_client)
        {
            var task = p_client.GetAsync("https://api.codepost.io/assignments/" + p_id + "/");
            task.Wait();
            var result = task.Result;

            string shaineInfoSurAssignment = result.Content.ReadAsStringAsync().Result;
            JObject objet = JObject.Parse(shaineInfoSurAssignment);

            //int id = (int)objet.SelectToken("id");
            string name = (string)objet.SelectToken("name");
            int points = (int)objet.SelectToken("points");
            int course = (int)objet.SelectToken("course");
            bool isReleased = (bool)objet.SelectToken("isReleased");
            bool isVisible = (bool)objet.SelectToken("isVisible");

            Assignment assignment = new Assignment(name, points, course, isReleased, isVisible);
            return assignment;
        }

        /// <summary>
        /// Procédure pour la création d'une Submission afin d'y envoyer par la suite le travail de l'étudiant
        /// </summary>
        /// <param name="p_assignment">L'id de l'assignment (travail)</param>
        /// <param name="p_emailEtudiant">Email de l'étudiant</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        /// <returns>Id de Submission</returns>
        public static int CreerSubmission(int p_assignment, string p_emailEtudiant, HttpClient p_client)
        {
            int idSubmission = 0;

            List<string> listEtudiants = new List<string>(); //dans requet HTTP il faut envoyer la list d'etudiants
            listEtudiants.Add(p_emailEtudiant);

            Submission submission = new Submission(p_assignment.ToString(), listEtudiants);
            var json = JsonConvert.SerializeObject(submission);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var task = p_client.PostAsync("https://api.codepost.io/submissions/", content);
            task.Wait();
            var result = task.Result;
            //ViewData["result"] = result;

            string resultCreationSubmission = result.Content.ReadAsStringAsync().Result;
            JObject objet = JObject.Parse(resultCreationSubmission);
            idSubmission = (int)objet.SelectToken("id");

            return idSubmission;
        }

        /// <summary>
        /// Procédure pour obtenir le Dictionary avec key=id de la submission et la valeur=objet de la submission
        /// </summary>
        /// <param name="p_assignment">l'id de l'assignment sur CodePost</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client (le prof)</param>
        /// <returns></returns>
        public static Dictionary<int, Submission> ObtenirDictionarySubmissionDansTravail(int p_assignment, HttpClient p_client)
        {

            var task = p_client.GetAsync("https://api.codepost.io/assignments/" + p_assignment + "/submissions/");
            task.Wait();
            var result = task.Result;
            string chainDeSubmissions = result.Content.ReadAsStringAsync().Result;
            string[] tableSubmissions = SeparerChaine(chainDeSubmissions);

            Dictionary<int, Submission> infoARetourner = new Dictionary<int, Submission>();

            for (int i = 0; i < tableSubmissions.Length; i++)
            {
                if (tableSubmissions[i] != null)
                {
                    if (tableSubmissions[i] != "{[]}")
                    {
                        JObject objet = JObject.Parse(tableSubmissions[i]);

                        int id = (int)objet.SelectToken("id");
                        string assignment = (string)objet.SelectToken("assignment");
                        IEnumerable<JToken> students = objet.SelectToken("students");

                        List<string> listEtudiants = new List<string>();
                        foreach (JToken email in students)
                        {
                            listEtudiants.Add((string)email);
                        }
                        infoARetourner.Add(id, new Submission(assignment, listEtudiants));
                    }
                }
            }
            return infoARetourner;
        }

        /// <summary>
        /// Procédure pour determination de tous les fichiers dans un repertoire donne
        /// </summary>
        /// <param name="p_path">le repertoire dans lequel on collecte tous les fichiers</param>
        /// <param name="p_fichiers">liste de tous les fichiers presentes dans le repertoire donne</param>
        public static void RecupererTousLesFichiers(string p_path, List<string> p_fichiers)  // @Olena, aucun return ?
        {

            p_fichiers.AddRange(Directory.GetFiles(p_path));
            string[] tableRepertoires = Directory.GetDirectories(p_path);

            if (tableRepertoires.Length != 0)
            {
                foreach (string repertoire in tableRepertoires)
                {
                    RecupererTousLesFichiers(repertoire, p_fichiers);
                }
            }
        }

        /// <summary>
        /// Procédure pour obtenir tous les chemins vers les répertoires avec le travail de chaque étudiant
        /// </summary>
        /// <param name="p_path">Le path vers le répertoire des travaux de tous les étudiants pour uploader</param>
        /// <returns>La table avec les path vers le répertoire des travaux de tous les étudiants</returns>
        public static string[] ObtenirSousRepertoiresEtudiantsAvecTravaux(string p_path)
        {
            string[] tableRepertoires = Directory.GetDirectories(p_path);

            if (tableRepertoires.Length == 0)
            {
                Console.WriteLine("Erreur. Il n'y a pas de travaux d'etudiants dans le répertoire " + p_path);
            }
            return tableRepertoires;
        }

        /// <summary>
        /// Procédure pour convertir le path complet dans un path relatif
        /// </summary>
        /// <param name="p_path">Le path complet</param>
        /// <param name="p_separator">Le nom de répertoire pour couper le chemin (tout ce qu'il y a après ce repertoire est reflété)</param>
        /// <returns>le path relative</returns>
        public static string ConvertirePathEnRelative(string p_path, string p_separator)
        {
            string[] table = new string[2];
            table = p_path.Split(p_separator, StringSplitOptions.None);

            string pathRelative = table[1];
            return pathRelative;
        }

        /// <summary>
        /// Procédure pour obtenir le courriel de l'étudiant
        /// </summary>
        /// <param name="p_path">Le path vers le répertoire du travail de l'étudiant pour uploader</param>
        /// <returns>Le courriel de l'étudiant comme chaine de caractères</returns>
        public static string ObtenirCourrielEtudiant(string p_path)
        {
            return new DirectoryInfo(p_path).Name;
        }

        /// <summary>
        /// Procédure pour uploader tous les fichiers d'un seul étudiant
        /// </summary>
        /// <param name="p_assignment">L'id de l'assignment (travail) sur CodePost</param>
        /// <param name="p_dictionary">Le dictionary où la key est l'id de la submission et la valeur est l'objet du type submission</param>
        /// <param name="p_path">Le path vers le répertoire du travail d'un seul étudiant pour uploader</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client(le prof)</param>
        public static void UploadTousLesFichiersEtudiant(int p_assignment, Dictionary<int, Submission> p_dictionary, string p_path, HttpClient p_client)
        {
            List<string> listFichiersEtudiant = new List<string>();
            RecupererTousLesFichiers(p_path, listFichiersEtudiant);
            string emailEtudiant = ObtenirCourrielEtudiant(p_path);

            bool estCreeSubmission = false;
            int idSubmission = 0;

            foreach (KeyValuePair<int, Submission> kvp in p_dictionary)
            {
                foreach (string email in kvp.Value.students)
                {
                    if (emailEtudiant == email)
                    {
                        estCreeSubmission = true;
                        idSubmission = kvp.Key;
                    }
                }
            }

            if (!estCreeSubmission)
            {
                idSubmission = CreerSubmission(p_assignment, emailEtudiant, p_client);
            }

            foreach (string fichier in listFichiersEtudiant)
            {
                CreerFichier(fichier, emailEtudiant, idSubmission, p_client);
            }
        }

        /// <summary>
        /// Procédure pour uploader les travaux de tous les étudiants
        /// </summary>
        /// <param name="p_assignment">L'id de l'assignment (travail) sur CodePost</param>
        /// <param name="p_path">Le path vers le répertoire des travaux de tous les étudiants pour uploader</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client(le prof)</param>
        public static void UploadTravauxTousEtudiants(int p_assignment, string p_path, HttpClient p_client)
        {
            Dictionary<int, Submission> dictionarySubmission = ObtenirDictionarySubmissionDansTravail(p_assignment, p_client);
            string[] tableRepertoiresEtudiants = ObtenirSousRepertoiresEtudiantsAvecTravaux(p_path);
            foreach (string repertoire in tableRepertoiresEtudiants)
            {
                UploadTousLesFichiersEtudiant(p_assignment, dictionarySubmission, repertoire, p_client);
            }
        }

        /// <summary>
        /// Procédure pour convertir le chemin des fichiers dans un format qui convient à CodePost
        /// </summary>
        /// <param name="p_path">Le chemin vers le fichier</param>
        /// <returns>Le path dans le format qui convient à CodePost</returns>
        public static string ConvertirePathPourCodePost(string p_path)  //@Olena, il n'y a pas peut-être une sintaxe pour faire ça automatiquement? 
        {                                                        // comment en powershell - Join-Path ?
            string newPath = p_path.Replace(@"\", @"/");
            return newPath;
        }

        /// <summary>
        /// Procédure pour créer le fichier dans CodePost
        /// </summary>
        /// <param name="p_nameAvecPath">Le nom du fichier avec le chemin </param>
        /// <param name="p_submission">L'id de la submission (=le numéro d'etudiant) sur CodePost pour uploader le fichier</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client(le prof)</param>
        public static void CreerFichier(string p_nameAvecPath, string p_emailEtudiant, int p_submission, HttpClient p_client)
        {
            string nameFichier = Path.GetFileName(p_nameAvecPath);
            string extantionFichier = Path.GetExtension(p_nameAvecPath);
            string path = Path.GetDirectoryName(p_nameAvecPath);
            string pathPourCodePost = ConvertirePathPourCodePost(path);
            string codeFichier = ConvertireFichierDansString(p_nameAvecPath);

            Console.WriteLine("name: " + nameFichier);
            Console.WriteLine("extantion:" + extantionFichier);
            Console.WriteLine("path:" + path);

            string separateur = ConvertirePathEnRelative(path, p_emailEtudiant);

            if (separateur == "")
            {
                separateur = "/" + p_emailEtudiant;
            }
            else
            {
                separateur = "/" + p_emailEtudiant + "/";
            }
            CreerFichier(nameFichier, codeFichier, extantionFichier, p_submission, ConvertirePathEnRelative(pathPourCodePost, separateur), p_client);
        }

        /// <summary>
        /// Procédure pour créer le fichier dans l'assignment(Travail) = uploader le fichier de l'étudiant 
        /// </summary>
        /// <param name="p_name">Le nom du fichier</param>
        /// <param name="p_code">Le code du fichier en format string</param>
        /// <param name="p_extansion">L'extention du fichier</param>
        /// <param name="p_submission">L'id de la submission (le numéro de l'étudiant)</param>
        /// <param name="p_path">Le chemin vers le fichier pour créer l'arboresence</param>
        /// <param name="p_client">HttpClient qui a été créé avec l'APIKey du client(le prof)</param>
        public static void CreerFichier(string p_name, string p_code, string p_extansion, int p_submission, string p_path, HttpClient p_client)
        {
            Model.File file = new Model.File(p_name, p_extansion, p_code, p_submission, p_path);

            var json = JsonConvert.SerializeObject(file);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var task = p_client.PostAsync(" https://api.codepost.io/files/", content);
            task.Wait();

            var result = task.Result;
            //ViewData["result"] = result;
        }

        /// <summary>
        /// Procédure pour convertir des fichiers text dans string
        /// </summary>
        /// <param name="p_path">Le chemin vers le fichier avec son nom</param>
        /// <returns>Le contenu du fichier comme une chaine de caractères</returns>
        public static string ConvertirFichierTextDansString(string p_path)
        {
            string textDeFichier = "";

            if (string.IsNullOrEmpty(p_path))
            {
                Console.WriteLine("Erreur.Le chemin vers le fichier ne peut pas être vide ou nul.");
                Console.ReadKey();
            }
            else
            {
                StreamReader contenu = new StreamReader(p_path, Encoding.UTF8);
                textDeFichier = "";
                while (!contenu.EndOfStream)
                {
                    textDeFichier += contenu.ReadToEnd();
                }
                contenu.Close();
            }
            return textDeFichier;
        }

        /// <summary>
        /// Procédure pour convertir des fichiers image dans string
        /// </summary>
        /// <param name="p_path">Le chemin vers le fichier avec son nom</param>
        /// <returns>Le contenu du fichier comme une chaine de caractères</returns>
        public static string ConvertirFichierImageDansString(string p_path)
        {
            string fichier_B64 = "";

            if (string.IsNullOrEmpty(p_path))
            {
                Console.WriteLine("Erreur.Le chemin vers le fichier ne peut pas être vide ou nul.");
                Console.ReadKey();
            }
            else
            {
                fichier_B64 = "data:application/binary;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(p_path));
            }
            return fichier_B64;
        }

        /// <summary>
        /// Procédure pour convertir tous les types des fichiers (text et image) dans string
        /// </summary>
        /// <param name="p_path"></param>
        /// <returns></returns>
        public static string ConvertireFichierDansString(string p_path)
        {
            string stringARetourner = "";

            if (string.IsNullOrEmpty(p_path))
            {
                Console.WriteLine("Erreur.Le chemin vers le fichier ne peut pas être vide ou nul.");
                Console.ReadKey();
            }
            else
            {
                List<string> extansionsFichierImage = new List<string> { ".jpeg", ".jpg", ".pdf", ".gif", ".png" };
                string extansionFichier = Path.GetExtension(p_path);

                if (extansionsFichierImage.Contains(extansionFichier))
                {
                    stringARetourner = ConvertirFichierImageDansString(p_path);
                }
                else stringARetourner = ConvertirFichierTextDansString(p_path);
            }
            return stringARetourner;
        }

        public IActionResult Index() 
        {
            //string keyAPI = "e395e7df7d367ea5cc70cc802dd0f351fdafb695";

            //using (HttpClient client = new HttpClient())
            //{

            //    client.BaseAddress = new Uri("https://api.codepost.io");
            //    client.DefaultRequestHeaders.Add("authorization", "Token " + keyAPI);

            //    //*****************upload tous les traveaux etudiants
            //    string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\Revue - Iteration 1\PartieProf\Travail_Demo_5";
            //    int assignment = 5695;
            //    //ReglerLesParametresDeCours(1220, client);
            //    //CreerAssignment("TP_test_08", 25, 1220, client);
            //    UploadTravauxTousEtudiants(assignment, path, client);
            //    //int idCours = 1220;
            //    //List<string> students = new List<string>(){
            //    //"1992343@csfoy.ca",
            //    //"1992473@csfoy.ca",
            //    //"1992178@csfoy.ca",
            //    //"1895949@csfoy.ca"};
            //    //AjouterEtudiantsDansCours(idCours, students, client);

            //    ////*******obtenir email etudiant
            //    //string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\Revue - Iteration 1\PartieProf\Travail_Demo_10\1992473@csfoy.ca";
            //    //Console.WriteLine(ObtenirCourrielEtudiant(path));

            //    ////****obtenir sous - repertoires
            //    //string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\Revue - Iteration 1\PartieProf\Travail_Demo_10\1992473@csfoy.ca";
            //    //List<string> listeFichiers = new List<string>();
            //    //RecupererTousLesFichiers(path, listeFichiers);
            //    //Console.WriteLine("Il y a fichiers: " + listeFichiers.Count);
            //    //foreach (string fichier in listeFichiers)
            //    //{
            //    //    Console.WriteLine(fichier);
            //    //}


            //    ////**************Creer le Submission
            //    //int idSubmission = CreerSubmission(5615, "1992343@csfoy.ca", client);
            //    //Console.WriteLine("id Submission: " + idSubmission);


            //    ////******Cree le fichier dans assignment(travail)
            //    //string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\test.txt";
            //    //CreerFichier(path, idSubmission, client);


            //    ////*****Cree le travail dans Cours
            //    //CreerAssignment("TP_test_01", 25, 1220, client);


            //    ////*****Regler les parametres de Cours
            //    //ActiverLesParametresDeCours(1220, client);


            //    ////***Obtenir la liste des Courses
            //    //List<Course> listCours = ObtenirListeDesCourses(client);
            //    //foreach (Course cours in listCours)
            //    //{
            //    //    Console.WriteLine(cours);
            //    //}


            //    ////********Ajouter les etudiants dans cours
            //    //int idCours = 1220;
            //    //List<string> students = new List<string>(){
            //    //"1992343@csfoy.ca",
            //    //"1992473@csfoy.ca",
            //    //"1992178@csfoy.ca",
            //    //"1895949@csfoy.ca"};
            //    //AjouterEtudiantsDansCours(idCours, students, client);
            //}

            return View();
        }

        

        


        


        

        





        


        

        


        
        

        


        


        


        
    }
}
