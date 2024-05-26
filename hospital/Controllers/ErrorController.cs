using Microsoft.AspNetCore.Mvc;

namespace hospital.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
