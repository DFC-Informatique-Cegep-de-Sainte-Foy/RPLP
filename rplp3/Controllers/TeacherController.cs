using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rplp3.Models;

namespace rplp3.Controllers
{
    public class TeacherController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        //[Authorize("estProfesseur")]
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Key()
        {
            return View();
        }

        //[Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult Key(ApiKey apiKey)
        {
            if (ModelState.IsValid)
            {
            Console.WriteLine(apiKey.PrivateKey);
            }
            else
            {
                Console.WriteLine("model invalide");
            }
            //initialisation a faux
            //ViewBag.Key = false;
            // verifie si on a le key du prof ... on ignore , VALIDATION A AMELIORER
            //if (Regex.IsMatch(apiKey.Key, "[a-z0-9]{40}"))
            //{
            //    ViewBag.Key = true;
            //}
            
            return View();
        }




        public IActionResult Upload()
        {
            return View();
        }


        // action Upload qui recoit en post le fichier 
        //[Authorize("estProfesseur")]
        [HttpPost]
        public IActionResult Upload(Microsoft.AspNetCore.Http.IFormFile file)
        {
            // obtenir le nom du fichier
            string fileName = System.IO.Path.GetFileName(file.FileName);

            // si le fichier existe deja, on efface celui qui etait present 
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            // Creation du nouveau fichier local et copie le contenu du fichier dedans
            using (FileStream localFile = System.IO.File.OpenWrite(fileName))
            using (Stream uploadedFile = file.OpenReadStream())
            {
                uploadedFile.CopyTo(localFile);
            }
            //confirmation de succes
            ViewBag.Message = "Téléchargement effectué avec succès";

            //decompresser le fichier recu (fichier source, destination)
            Decompresser(fileName, ObtenirPathDestinationFichier());

            return View();
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
            //".\\Fichiers"
            ZipFile.ExtractToDirectory(fileName, destinationFolder);
            }
        }

        private string ObtenirPathDestinationFichier()
        {
            string nomRepertoire = User.Identity.Name;
            string pathDestination = ".\\Fichiers\\" + nomRepertoire;
            return pathDestination;
        }
    }
}