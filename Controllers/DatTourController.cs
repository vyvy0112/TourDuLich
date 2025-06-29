using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNTour.Data;
using VNTour.ViewModel;

namespace VNTour.Controllers
{
    public class DatTourController : Controller
    {
        private readonly TourDuLichContext _context;

        public DatTourController(TourDuLichContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
