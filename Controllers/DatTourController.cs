using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using VNTour.Data;
using VNTour.Helpers;
using VNTour.Services;
using VNTour.ViewModel;

namespace VNTour.Controllers
{
    public class DatTourController : Controller
    {
        private readonly TourDuLichContext _context;
        private readonly IVnPayService _vnPayService;

        public DatTourController(TourDuLichContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }







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
                    .Where(m => m.IdTour == id &&
                  m.NgayBatDau <= DateTime.Today &&
                  m.NgayKetThuc >= DateTime.Today &&
                  m.SoLuong > 0)
                    .Select(m => new SelectListItem
                     {
             Value = m.IdGiamGia.ToString(),
                  Text = $"{m.MaCode} - Giảm {m.PhanTramGiam}%"
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

            // === Trường hợp: Nhập mã giảm giá thủ công ===
            if (!string.IsNullOrEmpty(model.MaCode))
            {
                var giamGia = _context.MaGiamGia.FirstOrDefault(m =>
                    m.MaCode == model.MaCode &&
                    m.IdTour == model.IdTour &&
                    m.NgayBatDau <= DateTime.Today &&
                    m.NgayKetThuc >= DateTime.Today &&
                    m.SoLuong > 0 &&
                    m.PhanTramGiam.HasValue);

                if (giamGia != null)
                {
                    tienGiam = tongTienGoc * giamGia.PhanTramGiam.Value / 100.0;
                    tongTienSauGiam -= tienGiam.Value;

                    giamGia.SoLuong -= 1;
                    _context.MaGiamGia.Update(giamGia);

                    model.IdGiamGia = giamGia.IdGiamGia; // cập nhật lại ID
                }
                else
                {
                    model.IdGiamGia = null;
                }
            }
            // === Trường hợp: Chọn từ dropdown ===
            else if (model.IdGiamGia.HasValue)
            {
                var giamGia = _context.MaGiamGia.FirstOrDefault(m =>
                    m.IdGiamGia == model.IdGiamGia &&
                    m.IdTour == model.IdTour &&
                    m.NgayBatDau <= DateTime.Today &&
                    m.NgayKetThuc >= DateTime.Today &&
                    m.SoLuong > 0 &&
                    m.PhanTramGiam.HasValue);

                if (giamGia != null)
                {
                    tienGiam = tongTienGoc * giamGia.PhanTramGiam.Value / 100.0;
                    tongTienSauGiam -= tienGiam.Value;

                    giamGia.SoLuong -= 1;
                    _context.MaGiamGia.Update(giamGia);
                }
                else
                {
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
                //TrangThai = "Chờ Xác Nhận",
                TrangThai = (payment == "VNPay") ? "Đã thanh toán" : "Chờ Xác Nhận",
                IdNhanVien = null
            };
            if ((tongTienSauGiam * 100) < 5000)
            {
                ModelState.AddModelError("", "Số tiền quá nhỏ để thanh toán qua VNPAY. Vui lòng chọn phương thức khác.");
                return View(model);
            }


            //thanh toán VNPAY  
            //if (payment == "VNPay")
            //{
            //    // Ghi tạm đơn hàng với trạng thái "Chờ thanh toán"
            //    datTour.TrangThai = "Đã Thanh Toán";
            //    _context.DatTours.Add(datTour);
            //    await _context.SaveChangesAsync(); // ⚠️ Lúc này IdDatTour mới có giá trị

            //    var vnpay = new VnPaymentRequestModel
            //    {
            //        Amount = (int)tongTienSauGiam,

            //        CreatedDate = DateTime.Now,
            //        Description = $"{model.HoTen} {model.DienThoai}",
            //        FullName = model.HoTen,
            //        OrderId = datTour.IdDatTour, // ✅ Đã có ID
            //        ReturnUrl = Url.Action("PaymentCallBack", "DatTour", null, Request.Scheme)
            //    };

            //    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnpay));
            //}
            if (payment == "VNPay")
            {
                datTour.TrangThai = "Đã Thanh Toán";
                _context.Database.BeginTransaction();

                _context.DatTours.Add(datTour);
                await _context.SaveChangesAsync(); // Lúc này có datTour.IdDatTour

                var chiTiet = new ChiTietDatTour
                {
                    IdDatTour = datTour.IdDatTour,
                    SoLuong = model.SoLuongNguoiLon + model.SoLuongTreEm,
                    DonGia = model.GiaNguoiLon,
                    ThanhTien = tongTienSauGiam,
                };

                _context.ChiTietDatTours.Add(chiTiet);
                _context.NgayKhoiHanhs.Update(ngayKhoiHanh);

                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();

                var vnpay = new VnPaymentRequestModel
                {
                    Amount = (int)tongTienSauGiam,
                    CreatedDate = DateTime.Now,
                    Description = $"{model.HoTen} {model.DienThoai}",
                    FullName = model.HoTen,
                    OrderId = datTour.IdDatTour,
                    ReturnUrl = Url.Action("PaymentCallBack", "DatTour", null, Request.Scheme)
                };

                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnpay));
            }


            try
            {

                _context.Database.BeginTransaction();

                _context.DatTours.Add(datTour);

                // 🟢 Lưu lại để EF sinh ra datTour.IdDatTour
                await _context.SaveChangesAsync();

                var chiTiet = new ChiTietDatTour
                {
                    IdDatTour = datTour.IdDatTour,
                    SoLuong = model.SoLuongNguoiLon + model.SoLuongTreEm,
                    DonGia = model.GiaNguoiLon,
                    ThanhTien = tongTienSauGiam,
                };

                _context.ChiTietDatTours.Add(chiTiet);
                _context.NgayKhoiHanhs.Update(ngayKhoiHanh);

                // Lưu toàn bộ
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();

                return RedirectToAction("ThanhCong", new { id = datTour.IdDatTour });
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                ModelState.AddModelError("", "Lỗi khi đặt tour: " + ex.Message);
                return View(model);
            }

            TempData["Success"] = "Đặt tour thành công!";
            return RedirectToAction("ThanhCong", new { id = datTour.IdDatTour });
        }


        //[HttpGet]
        //public IActionResult PaymentCallBack(VnPaymentResponseModel model)
        //{
        //    var reponse =  _vnPayService.PaymentExecute(Request.Query);
        //    return RedirectToAction("PaymentCallBack", reponse);
        //}

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            // Kiểm tra kết quả thanh toán
            if (response.Success)
            {
                // Có thể cập nhật trạng thái đơn hàng tại đây (nếu chưa làm)
                return View("PaymentSuccess", response); // Trả về trang thanh toán thành công
            }
            else
            {
                return View("PaymentFailed", response); // Trả về trang thất bại
            }
        }


        [HttpPost]
        public IActionResult ApDungMaGiamGia(string maCode, double tongTienGoc)
        {
            if (string.IsNullOrEmpty(maCode))
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });

            var giamGia = _context.MaGiamGia.FirstOrDefault(m =>
                m.MaCode == maCode &&
                m.NgayBatDau <= DateTime.Today &&
                m.NgayKetThuc >= DateTime.Today &&
                m.SoLuong > 0 &&
                m.PhanTramGiam.HasValue);

            if (giamGia == null)
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });

            var tienGiam = tongTienGoc * giamGia.PhanTramGiam.Value / 100.0;
            var tongTienSauGiam = tongTienGoc - tienGiam;

            return Json(new
            {
                success = true,
                tienGiam = tienGiam,
                tongTienSauGiam = tongTienSauGiam,
                idGiamGia = giamGia.IdGiamGia,
                phanTramGiam = giamGia.PhanTramGiam
            });
        }
      

        public async Task<IActionResult> ThanhCong(int id)
        {
            if (TempData["DatTourVM"] != null)
            {
                var model = JsonConvert.DeserializeObject<DatTourVM>(TempData["DatTourVM"].ToString());
                ViewBag.Message = "Đặt tour thành công!";
                return View(model);
            }

            // Trường hợp người dùng F5 → fallback lấy từ DB
            var thongtin = await _context.DatTours
                .Include(x => x.IdTourNavigation)
                .Include(x => x.IdKhachHangNavigation)
                .FirstOrDefaultAsync(x => x.IdDatTour == id);

            if (thongtin == null)
            {
                return NotFound();
            }

            var vm = new DatTourVM
            {
                IdDatTour = thongtin.IdDatTour,
                HoTen = thongtin.TenNguoiDat,
                DiaChi = thongtin.DiaChi,
                SoLuongNguoiLon = (int)thongtin.SoNguoiLon,
                SoLuongTreEm = (int)thongtin.SoTreEm,
                DienThoai = thongtin.IdKhachHangNavigation.Sdt,
                IdTour = (int)thongtin.IdTour,
                Email = thongtin.IdKhachHangNavigation.Email,
                NgayDat = DateTime.Now,
                TrangThai = thongtin.TrangThai,
                //NgayKhoiHanh = thongtin.NgayKhoiHanh,
                HinhAnh = thongtin.IdTourNavigation.HinhAnh,
                GiaNguoiLon = thongtin.IdTourNavigation.GiaNguoiLon,
                GiaTreEm = thongtin.IdTourNavigation.GiaTreEm,
                TongTienGoc = (double)thongtin.TongTien,
                GhiChu = thongtin.GhiChu
            };

            ViewBag.Message = "Đặt tour thành công!";
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> LichSuDatTour()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID);
            if (claim == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            int idKhachHang = int.Parse(claim.Value);

            var lichSu = await _context.DatTours
                .Where(x => x.IdKhachHang == idKhachHang)
                 .Include(x => x.IdTourNavigation)
                .Include(x => x.IdNkhNavigation) // Nếu bạn dùng thông tin từ bảng này
                .OrderByDescending(x => x.NgayDat)
                 .ToListAsync();
            return View(lichSu); // truyền trực tiếp List<DatTour>
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyTour(int id)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID);
            if (claim == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idKhachHang = int.Parse(claim.Value);
            var datTour = await _context.DatTours
                .FirstOrDefaultAsync(dt => dt.IdDatTour == id && dt.IdKhachHang == idKhachHang);

            if (datTour == null)
            {
                return NotFound();
            }           
            datTour.TrangThai = "Đã Hủy";
            _context.Update(datTour);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"Chỉnh mã tour {datTour.IdDatTour} thay đổi trạng thái {datTour.TrangThai}";
            return RedirectToAction("LichSuDatTour");
        }


        public async Task<IActionResult> XemChiTiet(int id)
        {
            var datTour = await _context.DatTours
                .Include(d => d.IdTourNavigation)
                .Include(d => d.IdGiamGiaNavigation)
                .Include(d => d.IdKhachHangNavigation)
                .Include(d => d.IdNkhNavigation) // Bao gồm ngày khởi hành
                .FirstOrDefaultAsync(d => d.IdDatTour == id);

            if (datTour == null)
            {
                return NotFound();
            }

            return View(datTour);
        }



    }
}
