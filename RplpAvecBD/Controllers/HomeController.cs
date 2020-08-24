using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RplpAvecBD.Models;

namespace RplpAvecBD.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public static bool estProfesseurConnecte = false;

        private IAuthorizationService _authorization;

        public HomeController(ILogger<HomeController> logger, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _authorization = authorizationService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var allowedProfesseur = await _authorization.AuthorizeAsync(User, "estProfesseur");

                var allowedEtudiant = await _authorization.AuthorizeAsync(User, "estEtudiant");

                if (allowedProfesseur.Succeeded)
                {
                    estProfesseurConnecte = true;
                    return RedirectToAction("Index", "Teacher");
                }
                else if (allowedEtudiant.Succeeded)
                {
                    estProfesseurConnecte = false;
                    return RedirectToAction("Index", "Student");
                }
            }

            return View();
        }

        [AllowAnonymous]
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
    }
}
