using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VNTour.Data;
using VNTour.Helpers;
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



        //public List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(MySetting.CART_KEY) ?? new List<CartItemVM>();

        //public IActionResult Index()
        //{
        //    return View(Cart);
        //}
        //[Authorize]
        //public IActionResult TourDat(int id, int quantity = 1)
        //{
        //    var gioHang = Cart;
        //    var item = gioHang.SingleOrDefault(p => p.IdTour == id);
        //    if (item == null)
        //    {
        //        var hangHoa = _context.Tours.SingleOrDefault(p => p.IdTour == id);
        //        if (hangHoa == null)
        //        {
        //            TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
        //            return Redirect("/404");
        //        }
        //        item = new CartItemVM
        //        {
        //            IdTour = hangHoa.IdTour,
        //            TenTour = hangHoa.TenTour,
        //            //GiaNguoiLon = hangHoa.GiaNguoiLon ?? 0,
        //            //GiaTreEm = hangHoa.GiaTreEm ?? 0,
        //            HinhAnh = hangHoa.HinhAnh,
        //            SoNguoiLon = quantity,
        //            SoTreEm = quantity
        //        };



        //        gioHang.Add(item);
        //    }
        //    else
        //    {
        //        item.SoNguoiLon += quantity;
        //        item.SoTreEm += quantity;
        //    }
        //    HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
        //    return RedirectToAction("Index");

        //}

        //[HttpGet]
        //public IActionResult KiemTraMaGiamGia(string code, int soNguoiLon, int soTreEm, int idTour)
        //{
        //    var tour = _context.Tours.FirstOrDefault(t => t.IdTour == idTour);
        //    if (tour == null)
        //        return Json(new { success = false, message = "Tour không tồn tại" });

        //    var today = DateOnly.FromDateTime(DateTime.Now);
        //    //var ma = _context.MaGiamGia
        //    //    .FirstOrDefault(m => m.MaCode.ToUpper() == code.ToUpper()
        //    //                      && m.NgayBatDau <= today && m.NgayKetThuc >= today
        //    //                      && m.SoLuong > 0 && m.TrangThai == "Hoạt Động") ;

        //    if (ma == null)
        //    {
        //        return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
        //    }

        //    var tongTien = (tour.GiaNguoiLon) * soNguoiLon + (tour.GiaTreEm) * soTreEm;
        //    var tienGiam = tongTien * ma.PhanTramGiam / 100;
        //    var tongConLai = tongTien - tienGiam;

        //    return Json(new
        //    {
        //        success = true,
        //        tongTien = tongConLai,
        //        tienGiam = tienGiam,
        //        phanTram = ma.PhanTramGiam,
        //        message = $"Áp dụng mã giảm {ma.PhanTramGiam}% thành công!"
        //    });

        //}







        //[HttpGet]
        //public async Task<IActionResult> DatTour(int id,int idNgayKhoiHanh)
        //{
        //    var tour = _context.Tours.SingleOrDefault(p => p.IdTour == id).;
        //    if (tour == null) return NotFound();

        //    var vm = new DatTourVM
        //    {
        //        IdTour = tour.IdTour,
        //        TenTour = tour.TenTour,
        //        HinhAnh = tour.HinhAnh,
        //        GiaNguoiLon = tour.GiaNguoiLon,
        //        GiaTreEm = tour.GiaTreEm,
        //        SoLuongNguoiLon = 1,
        //        SoLuongTreEm = 0
        //    };

        //    return View(vm);


        //}





        

        [Authorize]
        [HttpGet]  // thêm attribute này nếu chưa có
        public IActionResult GetLoggedInCustomerInfo()
        {
            var staffId = int.Parse(User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_STAFFID)?.Value ?? "0");
            var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.IdKhachHang == staffId);

            if (khachHang == null)
                return NotFound();

            return Json(new
            {
                hoTenKh = khachHang.HoTenKh,
                diaChi = khachHang.DiaChi,
                email = khachHang.Email,
                sdt = khachHang.Sdt
            });
        }

        //[HttpGet]
        //public IActionResult DatTour(int id, int? idNgayKhoiHanh)
        //{
        //    var tour = _context.Tours.SingleOrDefault(p => p.IdTour == id);
        //    if (tour == null) return NotFound();

        //    DateTime ngayKhoiHanh = DateTime.MinValue;
        //    if (idNgayKhoiHanh.HasValue)
        //    {
        //        var ngay = _context.NgayKhoiHanhs
        //                    .FirstOrDefault(n => n.IdNkh == idNgayKhoiHanh.Value);
        //        if (ngay != null)
        //        {
        //            ngayKhoiHanh = ngay.NgayKhoiHanh1;
        //        }
        //    }
        //    ViewBag.MaGiamGias = _context.MaGiamGia
        //     .Where(m => m.NgayBatDau.Value.Date <= DateTime.Today
        //     && m.NgayKetThuc.Value.Date >= DateTime.Today
        //     && m.SoLuong > 0)
        //     .Select(m => new SelectListItem
        //      {
        //       Value = m.IdGiamGia.ToString(),
        //       Text = $"{m.MaCode} - Giảm {m.PhanTramGiam}% (còn {m.SoLuong})"
        //         }).ToList();

        //    var vm = new DatTourVM
        //    {
        //        IdTour = tour.IdTour,
        //        //IdNgayKhoiHanh = idNgayKhoiHanh ?? 0,
        //        //NgayKhoiHanh = ngayKhoiHanh, // ⬅️ GÁN ĐÚNG NGÀY ĐÃ CHỌN
        //        TenTour = tour.TenTour,
        //        HinhAnh = tour.HinhAnh,
        //        GiaNguoiLon = tour.GiaNguoiLon,
        //        GiaTreEm = tour.GiaTreEm,
        //        SoLuongNguoiLon = 1,
        //        SoLuongTreEm = 0
        //    };

        //    return View(vm);
        //}


        [HttpGet]
        public IActionResult DatTour(int id, int? idNgayKhoiHanh)
        {
            var tour = _context.Tours.SingleOrDefault(p => p.IdTour == id);
            if (tour == null) return NotFound();

            // Lấy danh sách ngày khởi hành còn chỗ
            var ngayKhoiHanhList = _context.NgayKhoiHanhs
                .Where(n => n.IdTour == id && n.SoChoConLai > 0)
                .Select(n => new SelectListItem
                {
                    Value = n.IdNkh.ToString(),
                    Text = $"{n.NgayKhoiHanh1:dd/MM/yyyy} ({n.SoChoConLai} chỗ)"
                }).ToList();

            ViewBag.NgayKhoiHanhList = ngayKhoiHanhList;

            // Mã giảm giá còn hạn
            ViewBag.MaGiamGias = _context.MaGiamGia
                .Where(m => m.NgayBatDau <= DateTime.Today &&
                            m.NgayKetThuc >= DateTime.Today &&
                            m.SoLuong > 0)
                .Select(m => new SelectListItem
                {
                    Value = m.IdGiamGia.ToString(),
                    Text = $"{m.MaCode} - Giảm {m.PhanTramGiam}% (còn {m.SoLuong})"
                }).ToList();

            // Nếu người dùng chọn ngày khởi hành
            DateTime? ngayKhoiHanhValue = null;
            if (idNgayKhoiHanh.HasValue)
            {
                var ngay = _context.NgayKhoiHanhs.FirstOrDefault(n => n.IdNkh == idNgayKhoiHanh.Value);
                if (ngay != null)
                {
                    ngayKhoiHanhValue = ngay.NgayKhoiHanh1;
                }
            }



            // Tạo viewmodel
            var vm = new DatTourVM
            {
                IdTour = tour.IdTour,
                IdNkh = idNgayKhoiHanh ?? 0, // gán để lưu trong POST
                TenTour = tour.TenTour,
                HinhAnh = tour.HinhAnh,
                GiaNguoiLon = tour.GiaNguoiLon,
                GiaTreEm = tour.GiaTreEm,
                SoLuongNguoiLon = 1,
                SoLuongTreEm = 0,
                //NgayKhoiHanh = ngayKhoiHanhValue // nếu cần hiển
                // ✅ Gán vào ViewModel

                NgayKhoiHanh = (DateTime)ngayKhoiHanhValue

            };

            var claim = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID);
            if (claim != null && int.TryParse(claim.Value, out int idKhachHang))
            {
                var khach = _context.KhachHangs.FirstOrDefault(k => k.IdKhachHang == idKhachHang);
                if (khach != null)
                {
                    vm.HoTen = khach.HoTenKh;
                    vm.Email = khach.Email;
                    vm.DienThoai = khach.Sdt;
                    vm.DiaChi = khach.DiaChi;
                }
            }

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatTour(DatTourVM model, string payment = "COD")
        {
            var claim = User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if (claim == null || !int.TryParse(claim.Value, out int khachHangId))
                return Unauthorized();

            var khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.IdKhachHang == khachHangId);
            if (khachhang == null) return NotFound();

            if (model.giongkhachhang)
            {
                khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.IdKhachHang == khachHangId);
            }


            if (claim != null)
            {
                int idKhach = int.Parse(claim.Value);
                var khach = _context.KhachHangs.FirstOrDefault(k => k.IdKhachHang == idKhach);
                if (khach != null)
                {
                    model.HoTen = khach.HoTenKh;
                    model.Email = khach.Email;
                    model.DiaChi = khach.DiaChi;
                    model.DienThoai = khach.Sdt;
                }
            }
            // Kiểm tra ngày khởi hành
            // Kiểm tra ngày khởi hành hợp lệ
            var ngayKhoiHanh = _context.NgayKhoiHanhs
                .FirstOrDefault(n => n.IdNkh == model.IdNkh && n.IdTour == model.IdTour);

            if (ngayKhoiHanh == null)
            {
                ModelState.AddModelError("", "Không tìm thấy ngày khởi hành phù hợp.");
                return View(model);
            }

            // Kiểm tra còn đủ chỗ
            int tongSoNguoi = model.SoLuongNguoiLon + model.SoLuongTreEm;
            if (ngayKhoiHanh.SoChoConLai < tongSoNguoi)
            {
                ModelState.AddModelError("", "Không đủ chỗ.");
                return View(model);
            }
            ngayKhoiHanh.SoChoConLai -= tongSoNguoi;
            _context.NgayKhoiHanhs.Update(ngayKhoiHanh);



            double tongTienGoc = (model.SoLuongNguoiLon * model.GiaNguoiLon) +
                      (model.SoLuongTreEm * model.GiaTreEm);

            double tongTienSauGiam = tongTienGoc;
            double? tienGiam = null;

            // Kiểm tra nếu người dùng đã chọn mã giảm giá từ dropdown
            if (model.IdGiamGia.HasValue)
            {
                var giamGia = _context.MaGiamGia.FirstOrDefault(m => m.IdGiamGia == model.IdGiamGia.Value);
                if (giamGia != null &&
                    giamGia.NgayBatDau <= DateTime.Today &&
                    giamGia.NgayKetThuc >= DateTime.Today &&
                    giamGia.SoLuong > 0 &&
                    giamGia.PhanTramGiam.HasValue)
                {
                    tienGiam = tongTienGoc * giamGia.PhanTramGiam.Value / 100.0;
                    tongTienSauGiam -= tienGiam.Value;

                    // Trừ lượt sử dụng mã
                    giamGia.SoLuong -= 1;
                    _context.MaGiamGia.Update(giamGia);
                }
                else
                {
                    // Nếu không hợp lệ thì bỏ mã giảm giá
                    model.IdGiamGia = null;
                }
            }


            // Tạo bản ghi đặt tour
            var datTour = new DatTour
            {
                IdTour = model.IdTour,
                IdNkh = model.IdNkh,
                //IdNkh = model.IdNkh, // <-- gán Id ngày khởi hành
                IdKhachHang = khachhang.IdKhachHang,
                IdGiamGia = model.IdGiamGia,
                TenNguoiDat = khachhang.HoTenKh ?? model.HoTen,
                DiaChi = model.DiaChi ?? khachhang.DiaChi,
                NgayDat = DateTime.Now,
                SoNguoiLon = model.SoLuongNguoiLon,
                SoTreEm = model.SoLuongTreEm,
                DonGia = model.GiaNguoiLon,
                TongTien = tongTienSauGiam,
                GhiChu = model.GhiChu,
                PtthanhToan = payment == "COD" ? "COD" : "Thanh toán VnPay",
                TrangThai = "Chờ Xác Nhận",
                IdNhanVien = null
            };

            try
            {
                _context.Database.BeginTransaction();

                _context.DatTours.Add(datTour);
                _context.NgayKhoiHanhs.Update(ngayKhoiHanh);

                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
                return RedirectToAction("ThanhCong");
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                ModelState.AddModelError("", "Lỗi khi đặt tour: " + ex.Message);
                return View(model);
            }

            TempData["Success"] = "Đặt tour thành công!";
            return RedirectToAction("ThanhCong");
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DatTour(DatTourVM model, string payment = "COD")
        //{
        //    var claim = User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
        //    if (claim == null || !int.TryParse(claim.Value, out int khachHangId))
        //    {
        //        return Unauthorized(); // Chưa đăng nhập hoặc không đúng loại tài khoản
        //    }

        //    var khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.IdKhachHang == khachHangId);
        //    if (khachhang == null) return NotFound();

        //    if (model.giongkhachhang)
        //    {
        //        // Gợi ý: xử lý nếu claim không tồn tại hoặc giá trị không hợp lệ
        //        khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.IdKhachHang == khachHangId);
        //    }


        //    // Tính tổng tiền gốc chưa giảm
        //    double tongTienGoc = (model.SoLuongNguoiLon * model.GiaNguoiLon) +
        //                         (model.SoLuongTreEm * model.GiaTreEm);

        //    double tongTienSauGiam = tongTienGoc;
        //    double? tienGiam = null;

        //    // Kiểm tra mã giảm giá nếu có chọn
        //    if (model.IdGiamGia.HasValue)
        //    {
        //        var giamGia = _context.MaGiamGia.FirstOrDefault(m => m.IdGiamGia == model.IdGiamGia.Value);
        //        if (giamGia != null &&
        //            giamGia.NgayBatDau <= DateTime.Today &&
        //            giamGia.NgayKetThuc >= DateTime.Today &&
        //            giamGia.SoLuong > 0 &&
        //            giamGia.PhanTramGiam.HasValue)
        //        {
        //            tienGiam = tongTienGoc * giamGia.PhanTramGiam.Value / 100.0;
        //            tongTienSauGiam -= tienGiam.Value;

        //            // Trừ mã giảm giá
        //            giamGia.SoLuong -= 1;
        //            _context.MaGiamGia.Update(giamGia);
        //        }
        //        else
        //        {
        //            // Nếu mã giảm không hợp lệ, hủy chọn
        //            model.IdGiamGia = null;
        //        }
        //    }
        //    // Tính tổng chưa giảm         
        //    var datTour = new DatTour
        //    {
        //        IdTour = model.IdTour,
        //        IdKhachHang = khachhang.IdKhachHang,
        //        IdGiamGia = model.IdGiamGia,
        //        TenNguoiDat = khachhang.HoTenKh ?? model.HoTen,
        //        DiaChi = model.DiaChi ?? khachhang.DiaChi,               
        //        NgayDat = DateTime.Now,
        //        SoNguoiLon = model.SoLuongNguoiLon,
        //        SoTreEm = model.SoLuongTreEm,
        //        DonGia = model.GiaNguoiLon,
        //        TongTien = tongTienSauGiam, // Đã trừ giảm giá
        //        GhiChu = model.GhiChu,
        //        PtthanhToan = payment == "COD" ? "COD" : "Thanh toán VnPay",
        //        TrangThai = "Chờ Xác Nhận",

        //    };
        //    try
        //    {
        //        _context.Database.BeginTransaction();
        //        _context.DatTours.Add(datTour);
        //        await _context.SaveChangesAsync();
        //        _context.Database.CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        _context.Database.RollbackTransaction();
        //        ModelState.AddModelError("", "Lỗi khi đặt tour: " + ex.Message);
        //        return RedirectToAction("ThanhCong");
        //    }

        //    TempData["Success"] = "Đặt tour thành công!";
        //    return RedirectToAction("ThanhCong");

        //}



        [HttpGet("ApDung")]
        public IActionResult ApDung(string maCode)
        {
            if (string.IsNullOrWhiteSpace(maCode))
            {
                return BadRequest(new { isValid = false, message = "Vui lòng nhập mã giảm giá." });
            }

            var giamGia = _context.MaGiamGia.FirstOrDefault(m => m.MaCode.ToLower() == maCode.ToLower());

            if (giamGia == null)
            {
                return NotFound(new { isValid = false, message = "Mã giảm giá không tồn tại." });
            }

            if (giamGia.NgayBatDau > DateTime.Today)
            {
                return BadRequest(new { isValid = false, message = "Mã giảm giá chưa có hiệu lực." });
            }

            if (giamGia.NgayKetThuc < DateTime.Today)
            {
                return BadRequest(new { isValid = false, message = "Mã giảm giá đã hết hạn." });
            }

            if (giamGia.SoLuong <= 0)
            {
                return BadRequest(new { isValid = false, message = "Mã giảm giá đã hết lượt sử dụng." });
            }

            return Ok(new
            {
                isValid = true,
                message = "Áp dụng mã thành công.",
                idGiamGia = giamGia.IdGiamGia,
                phanTramGiam = giamGia.PhanTramGiam
            });
        }



        public async Task<IActionResult> ThanhCong()
        {
            // Hiển thị thông báo thành công
            ViewBag.Message = "Đặt tour thành công!";
            return View();
        }

    }
}
