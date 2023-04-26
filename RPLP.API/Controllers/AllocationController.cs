using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.ENTITES.InterfacesDepots;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllocationController : ControllerBase
    {
        private readonly IDepotAllocation _depot;

        public AllocationController(IDepotAllocation p_depotAllocation)
        {
            if (p_depotAllocation == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AllocationController - Constructeur - Dépot passé en paramêtre null", 0));
            }

            this._depot = p_depotAllocation;
        }
        // GET: api/<AllocationController>
        [HttpGet]
        public ActionResult<IEnumerable<Allocation>> Get()
        {
            try
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("api/Allocation", 200, "AllocationController - GET méthode Get"));
                return Ok(this._depot.GetAllocations());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
