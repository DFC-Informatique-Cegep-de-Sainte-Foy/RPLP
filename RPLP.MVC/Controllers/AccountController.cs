using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.MVC.Models;
using System.Security.Claims;
using RPLP.JOURNALISATION;

namespace RPLP.MVC.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string returnUrl = "/")
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
            RPLP.JOURNALISATION.Logging.Journal(new Log(User.Identity.Name, "undefined",
                "Connexion a partir de AccountController.cs/Login"));
        }

        [Authorize]
        public async Task Logout()
        {
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri(Url.Action("Index", "Home"))
                .Build();

            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            RPLP.JOURNALISATION.Logging.Journal(new Log(User.Identity.Name, "undefined",
                "Deconnexion a partir de AccountController.cs/Logout"));
        }

        [Authorize]
        public IActionResult Profile()
        {
            RPLP.JOURNALISATION.Logging.Journal(new Log(User.Identity.Name, "undefined",
                "Recuperation des donnees de client connecter a partir de AccountController.cs/Profile"));
            return View(new TeacherProfileViewModel()
            {
                FullName = User.Identity.Name,
                EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value
            });
        }
    }
}