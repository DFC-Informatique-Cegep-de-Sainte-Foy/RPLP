using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RplpAvecBD.Model;

namespace RplpAvecBD.Controllers
{
    public class CodePostController : Controller
    {
        public IActionResult Index() // void Index() //
        //public void Index()
        {
            string keyAPI = "e395e7df7d367ea5cc70cc802dd0f351fdafb695";

            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://api.codepost.io");
                client.DefaultRequestHeaders.Add("authorization", "Token " + keyAPI);

                //*****************upload tous les traveaux etudiants
                string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\Revue - Iteration 1\PartieProf\Travail_Demo_5";
                int assignment = 5695;
                //ReglerLesParametresDeCours(1220, client);
                //CreerAssignment("TP_test_08", 25, 1220, client);
                UploadTravauxEtudiants(assignment, path, client);
                //int idCours = 1220;
                //List<string> students = new List<string>(){
                //"1992343@csfoy.ca",
                //"1992473@csfoy.ca",
                //"1992178@csfoy.ca",
                //"1895949@csfoy.ca"};
                //AjouterEtudiantsDansCours(idCours, students, client);

                //*******obtenir email etudiant
                //string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\Revue - Iteration 1\PartieProf\Travail_Demo_10\1992473@csfoy.ca";
                //Console.WriteLine(ObtenirAdressEtudiant(path));

                //****obtenir sous-repertoires
                //string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\Revue - Iteration 1\PartieProf\Travail_Demo_10\1992473@csfoy.ca";
                //List<string> listeFichiers = new List<string>();
                //RecupererTousLesFichiers(path,listeFichiers);
                //Console.WriteLine("Il y a fichiers: " + listeFichiers.Count);
                //foreach (string fichier in listeFichiers)
                //{
                //    Console.WriteLine(fichier);
                //}


                //**************Creer le Submission
                //int idSubmission=CreeSubmission(5615,"1992343@csfoy.ca",client);
                //Console.WriteLine("id Submission: "+idSubmission);


                //******Cree le fichier dans assignment(travail)
                //string path = @"C:\Cegep Ste Foy\Session V\Projet Synthese\Revue\test.txt";
                //CreerFichier(path, idSubmission, client);


                //***** Cree le travail dans Cours
                //CreerAssignment("TP_test_01",25,1220,client);


                //***** Regler les parametres de Cours
                //ReglerLesParametresDeCours(1220, client);


                //*** Obtenir la liste des Courses
                //List<Course> listCours = ObtenirListeDesCourses(client);
                //foreach (Course cours in listCours)
                //{
                //    Console.WriteLine(cours);
                //}


                //******** Ajouter les etudiants dans cours
                //int idCours = 1220;
                //List<string> students = new List<string>(){
                //"1992343@csfoy.ca",
                //"1992473@csfoy.ca",
                //"1992178@csfoy.ca",
                //"1895949@csfoy.ca"};
                //AjouterEtudiantsDansCours(idCours, students,client);
            }
            return View();
        }

        /// <summary>
        /// procedure pour ajouter les etudiants dans Cours et les fait comme graders
        /// </summary>
        /// <param name="p_idCours">id de Cours dans CodePost</param>
        /// <param name="p_listEtudiantes">liste des tous les etudiants qui suivre de Cours</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client (de prof)</param>
        public void AjouterEtudiantsDansCours(int p_idCours, List<string> p_listEtudiantes, HttpClient p_client)
        {

            CourseRoster courseRoster = new CourseRoster(p_idCours, p_listEtudiantes);

            var json = JsonConvert.SerializeObject(courseRoster);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var task = p_client.PatchAsync("https://api.codepost.io/courses/" + p_idCours + "/roster/", content);
            task.Wait();
            var result = task.Result;
            ViewData["result"] = result;

        }


        /// <summary>
        /// function pour separer la chaine des Cours dans table des chaines des Cours
        /// </summary>
        /// <param name="p_chaineDesCourses">le chaine des tous les Courses, qui etait retourner de CodePost</param>
        /// <returns></returns>
        public string[] SeparerChaine(string p_chaine)
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
        /// function pour obtenir la liste de tous les Cours
        /// </summary>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client (de prof)</param>
        /// <returns></returns>
        public List<Course> ObtenirListeDesCourses(HttpClient p_client)
        {
            var task = p_client.GetAsync("https://api.codepost.io/courses/");
            task.Wait();
            var result = task.Result;
            string chaineDesCourses = result.Content.ReadAsStringAsync().Result;
            string[] tableCours = SeparerChaine(chaineDesCourses);

            List<Course> listCours = new List<Course>();

            for (int i = 0; i < tableCours.Length; i++)
            {
                if (tableCours[i] != null)
                {
                    JObject objet = JObject.Parse(tableCours[i]);

                    int id = (int)objet.SelectToken("id");
                    listCours.Add(RecupererInfoDeCours(id, p_client));
                }
            }
            return listCours;
        }


        /// <summary>
        /// procedure pour régler des paramètres du cours
        /// </summary>
        /// <param name="p_id">id de Cours dans CodePost</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        public void ReglerLesParametresDeCours(int p_id, HttpClient p_client)
        {
            Course cours = RecupererInfoDeCours(p_id, p_client);
            cours.sendReleasedSubmissionsToBack = true;
            cours.emailNewUsers = true;
            cours.anonymousGradingDefault = true;

            var json = JsonConvert.SerializeObject(cours);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var task = p_client.PatchAsync("https://api.codepost.io/courses/" + p_id + "/", content);
            task.Wait();
            var result = task.Result;
            ViewData["result"] = result;
        }


        /// <summary>
        /// Function pour recuperer tous les information necessaire sur le Cours
        /// </summary>
        /// <param name="p_id">id de Cours dans CodePost</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        /// <returns></returns>
        public Course RecupererInfoDeCours(int p_id, HttpClient p_client)
        {

            var task = p_client.GetAsync("https://api.codepost.io/courses/" + p_id + "/");
            task.Wait();
            var result = task.Result;

            string shaineInfoSurCours = result.Content.ReadAsStringAsync().Result;
            JObject objet = JObject.Parse(shaineInfoSurCours);

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
        /// Le procedure pour creer assignment(travail) sur CodePost 
        /// </summary>
        /// <param name="p_name">le nom de travail</param>
        /// <param name="p_points">le points pour travail</param>
        /// <param name="p_idCourse">id de cours dans quelle on créer le travail</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        public void CreerAssignment(string p_name, int p_points, int p_idCourse, HttpClient p_client)
        {
            bool estCree = AssignmentEstDejaCreer(p_name, p_points, p_idCourse, p_client);

            if (!estCree)
            {
                Assignment assignment = new Assignment(p_name, p_points, p_idCourse, true, true);
                var json = JsonConvert.SerializeObject(assignment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var task = p_client.PostAsync("https://api.codepost.io/assignments/", content);
                task.Wait();
                var result = task.Result;
                ViewData["result"] = result;
            }
            else
            {
                throw new ArgumentException("Le travail avec cet nom deja existe", p_name);
            }

        }


        public bool AssignmentEstDejaCreer(string p_name, int p_points, int p_idCourse, HttpClient p_client)
        {
            bool estCreer = false;

            return estCreer;
        }


        /// <summary>
        /// Le function pour recupere information sur Assignment
        /// </summary>
        /// <param name="p_id">id d'essignment</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        /// <returns>objet de type Assignment</returns>
        public Assignment RecupererInfoSurAssignment(int p_id, HttpClient p_client)
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
        /// La fonction de création de Submision pour le chargement ulterieur du travail d'etudiant
        /// </summary>
        /// <param name="p_assignment">id d'Assignment(travail)</param>
        /// <param name="p_emailEtudiant">email d'etudiant</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        /// <returns>id de Submission</returns>
        public int CreeSubmission(int p_assignment, string p_emailEtudiant, HttpClient p_client)
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
            ViewData["result"] = result;

            string resultCreationSubmission = result.Content.ReadAsStringAsync().Result;
            JObject objet = JObject.Parse(resultCreationSubmission);
            idSubmission = (int)objet.SelectToken("id");

            return idSubmission;
        }


        /// <summary>
        /// La procedure de determination de tous les fichiers dans un repertoire donne
        /// </summary>
        /// <param name="p_path">le repertoire dans lequel on collecte tous les fichiers</param>
        /// <param name="p_fichiers">liste de tous les fichiers presentes dans le repertoire donne</param>
        public void RecupererTousLesFichiers(string p_path, List<string> p_fichiers)
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
        /// Le fonction pour convertire le path complete dans le path relativ
        /// </summary>
        /// <param name="p_path">Le path complete</param>
        /// <param name="p_separator">le nom de repertoire pour couper le chemin (tout apres ce repertoire est reflete)</param>
        /// <returns>le path relative</returns>
        public string ConvertirePathEnRelative(string p_path, string p_separator)
        {

            string[] table = new string[2];
            table = p_path.Split(p_separator, StringSplitOptions.None);

            string pathRelative = table[1];
            return pathRelative;
        }


        /// <summary>
        /// Le function pour obtenir tous les chemins vers les repertoires avec les travaux de chaque etudiante
        /// </summary>
        /// <param name="p_path">le path vers le repertoire des travaux de tous les etudiantes pour upload</param>
        /// <returns>le table avec les path vers le repertoire des travaux de tous les etudiantes</returns>
        public string[] ObtenirSousRepertoiresEtudiantsAvecTraveaux(string p_path)
        {
            string[] tableRepertoires = Directory.GetDirectories(p_path);

            if (tableRepertoires.Length == 0)
            {
                Console.WriteLine("Erreur. Il n'y a pas le travaux d'etudiants dans repertoire " + p_path);
            }
            return tableRepertoires;
        }


        /// <summary>
        /// Le function pour obtenir e-mail d'etudiante
        /// </summary>
        /// <param name="p_path">le path vers le repertoire de travail d'etudiante pour upload</param>
        /// <returns>l'email d'etudiante comme chaine de caracteres</returns>
        public string ObtenirAdressEtudiant(string p_path)
        {
            return new DirectoryInfo(p_path).Name;
        }


        /// <summary>
        /// Le function pour obtenir le Dictionary avec key=id de submission et valeur=objet Submission
        /// </summary>
        /// <param name="p_assignment">id d'Assignment sur CodePost</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        /// <returns></returns>
        public Dictionary<int, Submission> ObtenirDictionarySubmissionDansTravail(int p_assignment, HttpClient p_client)
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
        /// Le procedure pour upload tous les fichiers de une seule etudient
        /// </summary>
        /// <param name="p_assignment">l'id d'assignment(travail) sur CodePost</param>
        /// <param name="p_dictionary">le dictionary ou le key c'est l'id de Submission et valeur - c'est l'objet de type Submission</param>
        /// <param name="p_path">le path vers le repertoire de travail d'une seule etudiante pour upload</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        public void UploadTousLesFichiersEtudiant(int p_assignment, Dictionary<int, Submission> p_dictionary, string p_path, HttpClient p_client)
        {
            List<string> listFichiersEtudiant = new List<string>();
            RecupererTousLesFichiers(p_path, listFichiersEtudiant);
            string emailEtudiant = ObtenirAdressEtudiant(p_path);

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
                idSubmission = CreeSubmission(p_assignment, emailEtudiant, p_client);
            }


            foreach (string fichier in listFichiersEtudiant)
            {
                CreerFichier(fichier, emailEtudiant, idSubmission, p_client);
            }
        }


        /// <summary>
        /// Le procedure pour upload tous les travaux de tous les etudiants
        /// </summary>
        /// <param name="p_assignment">l'id d'assignment(travail) sur CodePost</param>
        /// <param name="p_path">le path vers le repertoire des travaux d tous les etudiantes pour upload</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        public void UploadTravauxEtudiants(int p_assignment, string p_path, HttpClient p_client)
        {

            Dictionary<int, Submission> dictionarySubmission = ObtenirDictionarySubmissionDansTravail(p_assignment, p_client);
            string[] tableRepertoiresEtudiants = ObtenirSousRepertoiresEtudiantsAvecTraveaux(p_path);
            foreach (string repertoire in tableRepertoiresEtudiants)
            {
                UploadTousLesFichiersEtudiant(p_assignment, dictionarySubmission, repertoire, p_client);
            }
        }


        /// <summary>
        /// Function pour convertire le chemin de fichier dans formet qui convien CodePost
        /// </summary>
        /// <param name="p_path">le chemin vers le fichier</param>
        /// <returns>le path dans le format qui convien CodePost</returns>
        public string ConvertirePathPourCodePost(string p_path)
        {
            string newPath = p_path.Replace(@"\", @"/");
            return newPath;
        }


        /// <summary>
        /// Le procedure pour cree le fichier dans CodePost
        /// </summary>
        /// <param name="p_nameAvecPath">le nom de fichier avec le chemin </param>
        /// <param name="p_submission">l'id de submission (=le numero d'etudiant) sur CodePost pour upload le fichier </param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        public void CreerFichier(string p_nameAvecPath, string p_emailEtudiant, int p_submission, HttpClient p_client)
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
        /// Le procedure pour creer le fichier dans assignment(Travail) = upload le fichier d'etudiant 
        /// </summary>
        /// <param name="p_name">le nom de fichier</param>
        /// <param name="p_code">le code de fichier en format string</param>
        /// <param name="p_extansion">l'extention de fichier</param>
        /// <param name="p_submission">l'id de submissin (le numero d'etudiant)</param>
        /// <param name="p_path">le chemin vers le fichier pour creer arboresence</param>
        /// <param name="p_client">HttpClient qui etait cree pour API CodePost avec le KeyApi de client(de prof)</param>
        public void CreerFichier(string p_name, string p_code, string p_extansion, int p_submission, string p_path, HttpClient p_client)
        {

            Model.File file = new Model.File(p_name, p_extansion, p_code, p_submission, p_path);

            var json = JsonConvert.SerializeObject(file);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var task = p_client.PostAsync(" https://api.codepost.io/files/", content);
            task.Wait();

            var result = task.Result;
            ViewData["result"] = result;
        }


        /// <summary>
        /// function pour convertire le fichier avec le text dans string
        /// </summary>
        /// <param name="p_path">le chemin vers le fichier avec son nom</param>
        /// <returns>le contenu du fichier comme le shaine des caracteres</returns>
        public string ConvertireFichierTextDansString(string p_path)
        {
            string textDeFichier = "";

            if (string.IsNullOrEmpty(p_path))
            {
                Console.WriteLine("Erreur.Le chemin vers le fichier ne peux pas etre vide ou null.");
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
        /// function pour convertire le fichier avec l'image dans string
        /// </summary>
        /// <param name="p_path">le chemin vers le fichier avec son nom</param>
        /// <returns>le contenu du fichier comme le shaine des caracteres</returns>
        public string ConvertireFichierImageDansString(string p_path)
        {
            string fichier_B64 = "";

            if (string.IsNullOrEmpty(p_path))
            {
                Console.WriteLine("Erreur.Le chemin vers le fichier ne peux pas etre vide ou null.");
                Console.ReadKey();
            }
            else
            {
                fichier_B64 = "data:application/binary;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(p_path));
            }
            return fichier_B64;
        }


        /// <summary>
        /// function pour convertire tous les types des fichiers (avec text et image) dans string
        /// </summary>
        /// <param name="p_path"></param>
        /// <returns></returns>
        public string ConvertireFichierDansString(string p_path)
        {
            string stringARetourner = "";

            if (string.IsNullOrEmpty(p_path))
            {
                Console.WriteLine("Erreur.Le chemin vers le fichier ne peux pas etre vide ou null.");
                Console.ReadKey();
            }
            else
            {
                List<string> extansionsFichierImage = new List<string> { ".jpeg", ".jpg", ".pdf", ".gif", ".png" };
                string extansionFichier = Path.GetExtension(p_path);

                if (extansionsFichierImage.Contains(extansionFichier))
                {
                    stringARetourner = ConvertireFichierImageDansString(p_path);
                }
                else stringARetourner = ConvertireFichierTextDansString(p_path);
            }
            return stringARetourner;
        }  
    }
}
