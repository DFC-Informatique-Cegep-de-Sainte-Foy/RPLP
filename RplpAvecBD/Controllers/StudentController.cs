using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RplpAvecBD.Data;
using RplpAvecBD.Model;


namespace RplpAvecBD.Controllers
{
    [Authorize(Roles = "student,teacher")]
    public class StudentController : Controller
    {

        private readonly RplpContext _rplpContext;

        public StudentController(RplpContext rplpContext)
        {
            _rplpContext = rplpContext;
        }

        public IActionResult Index()
        {
            List<Professeur> professeurs = new List<Professeur>();

            professeurs = _rplpContext.Professeurs.ToList();

            ViewBag.listeProfesseurs = professeurs;

            return View();
        }

     
        [HttpPost]
        public IActionResult Index(Unclaim p_unclaim)
        {
            if (p_unclaim.idProfesseur == 0 || string.IsNullOrEmpty(p_unclaim.idProfesseur.ToString()))
            {
                ModelState.AddModelError("idProfesseur", "Vous devez sélectionner un professeur !");
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("ResultatUnclaim", new { idProfesseur = p_unclaim.idProfesseur, codeEtudiant = p_unclaim.codeEtudiant });
            }

            List<Professeur> professeurs = new List<Professeur>();

            professeurs = _rplpContext.Professeurs.ToList();

            ViewBag.listeProfesseurs = professeurs;

            return View();
        }
    
        [HttpGet]
        public IActionResult ResultatUnclaim(int idProfesseur, string codeEtudiant)
        {
            Professeur professeurSelectionne = _rplpContext.Professeurs.SingleOrDefault(p => p.id == idProfesseur);

            using (HttpClient c = new HttpClient())
            {
                c.BaseAddress = new Uri("https://api.codepost.io");
                c.DefaultRequestHeaders.Add("authorization", "Token " + professeurSelectionne.apiKey);

                var values = new Dictionary<string, string>
                {
                    {"grader", null}, 
                    {"id", codeEtudiant}
                };

                var submissionUpdate = new FormUrlEncodedContent(values);
                string address = "https://api.codepost.io/submissions/" + codeEtudiant + "/";
                var task = c.PatchAsync(address, submissionUpdate);
                task.Wait();

                var result = task.Result;
                var resultUpdateSubmission = result.Content.ReadAsStringAsync().Result;

                // Conversion du résultat dans JObjet
                JObject objet = JObject.Parse(resultUpdateSubmission);

                // Afficher succès ou échec
                try
                {
                    int id = (int)objet.SelectToken("id");
                    ViewData["resultUnclaim"] = "";
                }
                catch (Exception)
                {
                    ViewData["resultUnclaim"] = "Erreur";
                }

                ViewData["id"] = codeEtudiant;

                return View();
            }
        }

    
        public IActionResult GuideCodePostEtudiant()
        {
            return View();
        }
    }
}
