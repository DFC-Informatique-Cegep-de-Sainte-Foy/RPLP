using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPLP.DAL.SQL.Depots;
using RPLP.JOURNALISATION;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificatorController : ControllerBase
    {
        private readonly VerificatorForDepot _verificator;
        public VerificatorController()
        {
            this._verificator = new VerificatorForDepot();
        }

        [HttpGet("UserType/{email}")]
        public ActionResult<string> GetUserTypeByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "VerificatorForDepot - GetUserTypeByEmail - email passé en paramêtre est vide"));
                }

                Type type = this._verificator.GetUserTypeByEmail(email);

                if (type == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "VerificatorForDepot - GetUserTypeByEmail - variable type assigné par la méthode GetUserTypeByEmail est null"));

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "VerificatorForDepot - CheckUsernameTaken - username passé en paramêtre est vide"));
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "VerificatorForDepot - CheckEmailTaken - email passé en paramêtre est vide"));
                }

                return this._verificator.CheckEmailTaken(email);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
