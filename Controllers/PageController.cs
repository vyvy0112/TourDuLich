using Microsoft.AspNetCore.Mvc;
using VNTour.Data;

namespace VNTour.Controllers
{
    public class PageController : Controller
    {
        private readonly TourDuLichContext _context;


        public PageController(TourDuLichContext context)
        {
            _context = context;
        }



        // GET: Hiển thị form liên hệ
        [HttpGet]
        public IActionResult LienHe()
        {
            return View();
        }

        // POST: Xử lý gửi form liên hệ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LienHe(LienHe model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về lại form nếu có lỗi nhập
            }

            model.NgayTao = DateTime.Now;
            _context.LienHes.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
            return RedirectToAction("LienHe");
        }


        public IActionResult ChinhSachRiengTu()
        {
            return View();
        }



        public IActionResult TinTuc()
        {
            return View();
        }




    }
}
