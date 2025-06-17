using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using VNTour.Data;

namespace VNTour.Controllers
{
    public class AdminController : Controller
    {
        private readonly TourDuLichContext _context;


        public AdminController(TourDuLichContext context)
        {
            _context = context;
        }

        public IActionResult ListTour()
        {
            var lstTour = _context.Tours
                .Where(x => x.TrangThai == "Hoạt Động")
                .OrderByDescending(x => x.IdTour)
                .ToList();


            return View(lstTour);
        }


        //[HttpGet]
        //public IActionResult CreateTour()
        //{
        //    ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
        //    return View();
        //}





        //[HttpPost]
        //public IActionResult CreateTour(Tour model)
        //{
        //    var lstDanhMuc = _context.DanhMucTours
        //        .FirstOrDefault(x => x.IdDanhMuc == model.IdDanhMuc);

        //    if (lstDanhMuc == null)
        //    {
        //        return NotFound("Danh mục không tồn tại.");
        //    }    

        //    _context.Tours.Add(model);
        //    _context.SaveChanges();
        //    return View("ListTour");

        //}


   
    }
}
