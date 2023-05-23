using Microsoft.AspNetCore.Mvc;
using RPLP.JOURNALISATION;
using RPLP.MVC.Models;
using System.Diagnostics;

namespace RPLP.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error(int? statusCode = null)
        {
            List<int> statues = new List<int>() { 400, 401, 402, 403, 404, 405, 406, 407, 408, 500, 501, 502, 503, 504 };

            if (statusCode.HasValue)
            {
                if (statues.Contains((int)statusCode))
                {
                    var viewName = statusCode.ToString();
                    return View(viewName);
                }
                else
                {
                    return View((object)400);
                }
            }
            else
            {
                return View((object)400);
            }
        }
    }
}