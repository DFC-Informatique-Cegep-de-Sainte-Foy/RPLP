using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace rplp3.Controllers
{
    public class UnclaimController : Controller
    {
        [HttpGet]
        public IActionResult UnclaimResult(String IdSubmission)
        {
            using (HttpClient c = new HttpClient())
            {
                c.BaseAddress = new Uri("https://api.codepost.io");
                // olena e395e7df7d367ea5cc70cc802dd0f351fdafb695
                // pf    7d9acea046111298c0971cb6b437ef74e6b89625
                c.DefaultRequestHeaders.Add("authorization", "Token 7d9acea046111298c0971cb6b437ef74e6b89625");

                var values = new Dictionary<string, string>
                {
                    { "grader" ,null },//  { "grader" , "1992473@csfoy.ca" }
                    { "id", IdSubmission}
                };

                var submissionUpdate = new FormUrlEncodedContent(values);
                string adress = "https://api.codepost.io/submissions/" + IdSubmission + "/";
                var task = c.PatchAsync(adress, submissionUpdate);
                task.Wait();

                var result = task.Result;
                var resultUpdateSubmission = result.Content.ReadAsStringAsync().Result;

                //Convertation du resultat dans JObjet
                JObject objet = JObject.Parse(resultUpdateSubmission);
                //pour afficher si le Unclaim a ete executer avec succes ou non
                try
                {
                    int id = (int)objet.SelectToken("id");
                    ViewData["resultUnclaim"] = "";
                }
                catch (Exception)
                {
                    ViewData["resultUnclaim"] = "Erreur";
                }

                //IList<string> detail = objet.SelectToken("id").Select(s => (string)s).ToList();                            
                // return Content($"Votre travail a ete unclaimed" + IdSubmission);
                ViewData["id"] = IdSubmission;
                return View();
            }
        }
    }
}