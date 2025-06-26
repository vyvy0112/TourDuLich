using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Data;
using System.Security.Claims;
using VNTour.Data;
using VNTour.Helpers;
using VNTour.ViewModel;

namespace VNTour.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly IMapper _mapper;
        private readonly TourDuLichContext _context;

        public TaiKhoanController(IMapper mapper, TourDuLichContext context)
        {
            _mapper = mapper;
            _context = context;
        }



        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(DangKyVM model)
        {
            if (ModelState.IsValid)
            {
                var kq = new KhachHang
                {
                    HoTenKh = model.HoTenKh,
                    Sdt = model.Sdt,
                    Email = model.Email,
                    DiaChi = model.DiaChi,
                    MatKhau = model.MatKhau.ToSHA256Hash("MatKhau") // Mã hóa mật khẩu
                };

                _context.KhachHangs.Add(kq);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập để tiếp tục.";

                return RedirectToAction("DangNhap");
            }
            return View(model);

        }


        [HttpGet]
        public IActionResult DangNhap(string? ReturrnUrl)
        {
            ViewBag.ReturnUrl = ReturrnUrl;
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> DangNhap([Bind("EmailOrPhone", "MatKhau")] DangNhapVM model, string? ReturrnUrl)
        {
            ViewBag.ReturnUrl = ReturrnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string hashedPassword = model.MatKhau.ToSHA256Hash("MatKhau");

            KhachHang? khachhang = null;
            NhanVien? user = null;

            // Kiểm tra nếu là Email
            bool isEmail = model.EmailOrPhone.Contains("@");

            if (isEmail)
            {
                khachhang = _context.KhachHangs.FirstOrDefault(x => x.Email == model.EmailOrPhone && x.MatKhau == hashedPassword);
                user = _context.NhanViens.FirstOrDefault(x => x.Email == model.EmailOrPhone && x.MatKhau == hashedPassword);
            }
            else
            {
                khachhang = _context.KhachHangs.FirstOrDefault(x => x.Sdt == model.EmailOrPhone && x.MatKhau == hashedPassword);
                user = _context.NhanViens.FirstOrDefault(x => x.Sdt == model.EmailOrPhone && x.MatKhau == hashedPassword);
            }

            // Nếu là nhân viên (admin)
            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.HoTenNv),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(MySetting.CLAIM_STAFFID, user.IdNhanVien.ToString())
        };

                HttpContext.Session.SetString("HoTenNv", user.HoTenNv);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role);
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);

                TempData["loginSuccess"] = "Đăng nhập quản trị thành công!";
                return RedirectToAction("ListTour", "Admin");
            }

            // Nếu là khách hàng
            if (khachhang != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, khachhang.HoTenKh),
            new Claim(ClaimTypes.Email, khachhang.Email),
            new Claim(MySetting.CLAIM_CUSTOMERID, khachhang.IdKhachHang.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);

                HttpContext.Session.SetString("HoTenKh", khachhang.HoTenKh);
                HttpContext.Session.SetString("Email", khachhang.Email);
                TempData["loginSuccess"] = "Đăng nhập thành công!";

                if (Url.IsLocalUrl(ReturrnUrl))
                {
                    return Redirect(ReturrnUrl);
                }

                return RedirectToAction("Index", "Tour");
            }

            ModelState.AddModelError("", "Email/Số điện thoại hoặc mật khẩu không đúng!");
            return View(model);
        }




        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear(); // Xóa session
            return Redirect("/");
        }
             //public IActionResult ThongTin()
             //{
             //   int maKh = int.Parse(User.FindFirst(MySetting.CLAIM_STAFFID).Value);

             //   var kh = _context.KhachHangs.FirstOrDefault(k => k.IdKhachHang == maKh);
             //   if (kh == null)
             //     return NotFound();

             //    return View(kh);
             //}

        //[HttpPost]
        //public async Task<IActionResult> QuenMatKhau(string email)
        //{
        //   var kh = await _context.KhachHangs.FirstOrDefaultAsync(x => x.Email == email);
        //    if (kh == null)
        //    {
        //        // Không tiết lộ email không tồn tại (bảo mật)
        //        TempData["Message"] = "Nếu email hợp lệ, liên kết đặt lại mật khẩu đã được gửi.";
        //        return RedirectToAction("DangNhap");
        //    }
        //    var token = Guid.NewGuid().ToString();

        //    // Lưu token vào DB (thêm cột ResetToken vào KhachHang hoặc bảng riêng)
        //    kh.ResetToken = token;
        //    kh.ResetTokenExpiry = DateTime.Now.AddMinutes(30); // token hết hạn sau 30 phút
        //    await _context.SaveChangesAsync();

        //    // Tạo link reset
        //    var resetLink = Url.Action("DatLaiMatKhau", "TaiKhoan", new { token = token }, Request.Scheme);

        //    // Gửi email (bạn cần cấu hình SMTP, tạm ghi ra log)
        //    Console.WriteLine($"Link reset: {resetLink}");

        //    TempData["Message"] = "Nếu email hợp lệ, liên kết đặt lại mật khẩu đã được gửi.";
        //    return RedirectToAction("DangNhap");
        //    return View(kh);
        //}


    }
}

