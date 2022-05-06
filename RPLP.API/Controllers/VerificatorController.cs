using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPLP.DAL.SQL.Depots;

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
            Type type = this._verificator.GetUserTypeByEmail(email);

            if (type == null)
            {
                return BadRequest();
            }

            return type.ToString();

        }

        [HttpGet("UsernameTaken/{username}")]
        public ActionResult<bool> CheckUsernameTaken(string username)
        {
            return this._verificator.CheckUsernameTaken(username);
        }

        [HttpGet("EmailTaken/{email}")]
        public ActionResult<bool> CheckEmailTaken(string email)
        {
            return this._verificator.CheckEmailTaken(email);
        }
    }
}
