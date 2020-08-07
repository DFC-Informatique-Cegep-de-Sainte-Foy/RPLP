using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rplp3.Models;
using System.Security.Principal;
using System.Security.Claims;
using System.IO;

namespace rplp3.Controllers
{
    //test/
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Teacher()
        {
            //bool isTeacher = true; // pour test en developpement
            bool isTeacher = false;
            List<System.Security.Claims.Claim> groups = User.Claims.Where(c => c.Type == "groups").ToList();
            //IdentityReferenceCollection groupe = System.Security.Principal.WindowsIdentity.GetCurrent().Groups;
            foreach (System.Security.Claims.Claim groupeUtilisateur in groups)
            {
                if (groupeUtilisateur.Value == "c0d32534-918b-44bd-a2c9-b21e292e6cf7")
                {
                    isTeacher = true;
                }
            }

            if (!isTeacher)
            {
                return View("BadAuth", "Home");
            }
            return View();
        }

        [Authorize]
        public IActionResult Student()
        {
            bool isStudent = false;
            List<System.Security.Claims.Claim> groups = User.Claims.Where(c => c.Type == "groups").ToList();
            foreach (System.Security.Claims.Claim groupeUtilisateur in groups)
            {
                if (groupeUtilisateur.Value == "76d2a1f1-fa8a-4a15-8ada-2724d74ad571")
                {
                    isStudent = true;
                }
            }
            if (!isStudent)
            {
                return View("BadAuth", "Home");
            }
            return View();
        }

        public IActionResult BadAuth()
        {
            return View();
        }

        public IActionResult CodePost()
        {
            return Redirect("http://www.codepost.io");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Upload()
        {
            return View();
        }

        // action Upload qui recoit en post le fichier 
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

            return View();
        }

    }
}
