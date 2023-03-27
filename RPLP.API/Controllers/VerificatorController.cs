using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPLP.DAL.SQL.Depots;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificatorController : ControllerBase
    {
        private readonly IVerificatorForDepot _verificator;
        public VerificatorController(IVerificatorForDepot verificatorForDepot)
        {
            if (verificatorForDepot == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                "VerificatorController - Constructeur - verificatorForDepot passé en paramêtre est null", 0));
            }
            this._verificator = verificatorForDepot;
        }

        [HttpGet("UserType/{email}")]
        public ActionResult<string> GetUserTypeByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "VerificatorForDepot - GetUserTypeByEmail - email passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Verificator/UserType/{email}", 0, "VerificatorController - GET méthode GetUserTypeByEmail"));

                Type type = this._verificator.GetUserTypeByEmail(email);

                if (type == null)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "VerificatorForDepot - GetUserTypeByEmail - variable type assigné par la méthode GetUserTypeByEmail est null", 0));

                    return BadRequest();
                }

                return type.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("UsernameTaken/{username}")]
        public ActionResult<bool> CheckUsernameTaken(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "VerificatorForDepot - CheckUsernameTaken - username passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Verificator/UsernameTaken/{username}", 0, "VerificatorController - GET méthode CheckUsernameTaken"));

                return this._verificator.CheckUsernameTaken(username);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("EmailTaken/{email}")]
        public ActionResult<bool> CheckEmailTaken(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "VerificatorForDepot - CheckEmailTaken - email passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Verificator/EmailTaken/{email}", 0, "VerificatorController - GET méthode CheckEmailTaken"));

                return this._verificator.CheckEmailTaken(email);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
