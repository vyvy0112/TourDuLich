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
            

        public async Task<IActionResult> ThongTin(int id)
        {
            var user = User.FindFirst(MySetting.CLAIM_CUSTOMERID);
            if (user == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            int idKhachHang = int.Parse(user.Value);

            var khachHang = await _context.KhachHangs.FindAsync(idKhachHang);
            if (khachHang == null)
            {
                return NotFound();
            }
            var model = new KhachHangVM
            {
                IdKhachHang = khachHang.IdKhachHang,
                HoTenKh = khachHang.HoTenKh,
                Sdt = khachHang.Sdt,
                Email = khachHang.Email,
                DiaChi = khachHang.DiaChi
            };
            return View(model);
        }

    }
}
