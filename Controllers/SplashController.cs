using Microsoft.AspNetCore.Mvc;

namespace AirlineTicketingSystem.Controllers
{
    public class SplashController : Controller
    {
        // Default action to show splash page
        public IActionResult Index()
        {
            return View();
        }
    }
}
