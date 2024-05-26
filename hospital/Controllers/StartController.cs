using hospital.DAO;
using Microsoft.AspNetCore.Mvc;

namespace hospital.Controllers
{
    public class StartController : Controller
    {
       
        public IActionResult StartPage()
        {

            return View("~/Views/Start/StartPage.cshtml");
        }
    }
}
