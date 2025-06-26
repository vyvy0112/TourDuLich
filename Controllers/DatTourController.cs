using Microsoft.AspNetCore.Mvc;

namespace VNTour.Controllers
{
    public class DatTourController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
