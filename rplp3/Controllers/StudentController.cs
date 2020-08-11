using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace rplp3.Controllers
{
    public class StudentController : Controller
    {
        [Authorize("estEtudiant")]
        public IActionResult Student()
        {
            return View();
        }
    }
}