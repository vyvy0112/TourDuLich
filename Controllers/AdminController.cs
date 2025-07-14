using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.ComponentModel;
using System.Globalization;

using VNTour.Data;
using VNTour.Helpers;
using VNTour.ViewModel;

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
           .Include(t => t.IdDanhMucNavigation)
           .Include(t => t.NgayKhoiHanhs)
           .OrderByDescending(t => t.IdTour)
           .ToList();
            return View(lstTour);
        }


        [HttpGet]
        public async Task<IActionResult> CreateTour()
        {
            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
            ViewBag.TrangThaiList = new List<SelectListItem>
            {
              new SelectListItem { Value = "Hoạt động", Text = "Hoạt động" },
               new SelectListItem { Value = "Ngưng hoạt động", Text = "Ngưng hoạt động" }
            };

            return View();
        }


     
        [HttpPost]
 
        public async Task<IActionResult> CreateTour(TourVM model, IFormFile? HinhAnhFile, List<IFormFile> HinhAnhPhu)
        {
            if (ModelState.IsValid)
            {
                // Upload hình chính
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    var hinhPath = MyUtil.UploadHinh(HinhAnhFile, "");
                    if (!string.IsNullOrEmpty(hinhPath))
                    {
                        model.HinhAnh = hinhPath;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể upload hình ảnh");
                        ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
                        return View(model);
                    }
                }

                // Tạo đối tượng Tour từ ViewModel
                var tour = new Tour
                {
                    TenTour = model.TenTour,
                    MoTa = model.MoTa,
                    GiaNguoiLon = model.GiaNguoiLon,
                    GiaTreEm = model.GiaTreEm,
                    TrangThai = "Hoạt Động",
                    IdDanhMuc = model.IdDanhMuc,
                    HinhAnh = model.HinhAnh,
                    DiemDen = model.DiemDen,
                    DiemKhoiHanh = model.DiemKhoiHanh,
                    ThoiGian = model.ThoiGian,
                };

                _context.Tours.Add(tour);
                await _context.SaveChangesAsync(); // để có IdTour

                
                foreach (var item in model.NgayKhoiHanhs)
                {
                    var ngayKhoiHanh = new NgayKhoiHanh
                    {
                        IdTour = tour.IdTour,
                        NgayKhoiHanh1 = item.NgayKhoiHanh,
                        NgayKetThuc = item.NgayKhoiHanh,
                        SoChoConLai = item.SoChoConLai
                    };
                    _context.NgayKhoiHanhs.Add(ngayKhoiHanh);
                }

                // ➕ Lưu hình ảnh phụ
                if (HinhAnhPhu != null && HinhAnhPhu.Count > 0)
                {
                    foreach (var file in HinhAnhPhu)
                    {
                        var url = MyUtil.UploadHinh(file, "");
                        if (!string.IsNullOrEmpty(url))
                        {
                            var hinhanh = new TourHinhAnh
                            {
                                IdTour = tour.IdTour,
                                UrlHinhAnh = url,
                            };
                            _context.TourHinhAnhs.Add(hinhanh);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("ListTour");
            }

            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
            return View(model);
        }


      
        [HttpGet]
        public async Task<IActionResult> EditTour(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.NgayKhoiHanhs)
                .FirstOrDefaultAsync(t => t.IdTour == id);

            if (tour == null)
                return NotFound();

            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc", tour.IdDanhMuc);
            ViewBag.TrangThaiList = new List<SelectListItem>
             {
               new SelectListItem { Value = "Hoạt Động", Text = "Hoạt Động" },
                  new SelectListItem { Value = "Ngưng Hoạt Động", Text = "Ngưng Hoạt Động" }
             };

            return View(tour); // tour chứa cả NgayKhoiHanhs
        }

        [HttpPost]
        public async Task<IActionResult> EditTour(Tour model, IFormFile? HinhAnhFile, List<IFormFile> HinhAnhPhu)
        {
     

            var tour = await _context.Tours
                .Include(t => t.NgayKhoiHanhs)
                .FirstOrDefaultAsync(t => t.IdTour == model.IdTour);

            if (tour == null) return NotFound();

            // Cập nhật thông tin Tour
            tour.TenTour = model.TenTour;
            tour.MoTa = model.MoTa;
            tour.GiaNguoiLon = model.GiaNguoiLon;
            tour.GiaTreEm = model.GiaTreEm;
            tour.TrangThai = model.TrangThai;
            tour.IdDanhMuc = model.IdDanhMuc;
            tour.DiemDen = model.DiemDen;
            tour.DiemKhoiHanh = model.DiemKhoiHanh;
            tour.ThoiGian = model.ThoiGian;

            // Cập nhật hình chính
            if (HinhAnhFile != null && HinhAnhFile.Length > 0)
            {
                var path = MyUtil.UploadHinh(HinhAnhFile, "");
                if (!string.IsNullOrEmpty(path))
                    tour.HinhAnh = path;
            }

            // ✅ Xử lý cập nhật Ngày Khởi Hành
            foreach (var item in model.NgayKhoiHanhs)
            {
                var existing = tour.NgayKhoiHanhs.FirstOrDefault(x => x.IdNkh == item.IdNkh);
                if (existing != null)
                {
                    // Cập nhật
                    existing.NgayKhoiHanh1 = item.NgayKhoiHanh1;
                    existing.NgayKetThuc = item.NgayKetThuc;
                    existing.SoChoConLai = item.SoChoConLai;
                }
                else
                {
                    // Thêm mới
                    tour.NgayKhoiHanhs.Add(new NgayKhoiHanh
                    {
                        IdTour = tour.IdTour,
                        NgayKhoiHanh1 = item.NgayKhoiHanh1,
                        NgayKetThuc = item.NgayKetThuc,
                        SoChoConLai = item.SoChoConLai
                    });
                }
            }


            // Hình phụ
            if (HinhAnhPhu != null && HinhAnhPhu.Count > 0)
            {
                foreach (var file in HinhAnhPhu)
                {
                    var url = MyUtil.UploadHinh(file, "");
                    if (!string.IsNullOrEmpty(url))
                    {
                        _context.TourHinhAnhs.Add(new TourHinhAnh
                        {
                            IdTour = tour.IdTour,
                            UrlHinhAnh = url
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ListTour");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Tours.Include(x=>x.NgayKhoiHanhs).FirstOrDefaultAsync(x=>x.IdTour == id);
            if (entity == null)
            {
                return NotFound();
            }
            if (entity.NgayKhoiHanhs != null && entity.NgayKhoiHanhs.Any())
            {
                _context.NgayKhoiHanhs.RemoveRange(entity.NgayKhoiHanhs);
            }
            return View(entity); // Trả về View Delete.cshtml
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var entity = await _context.Tours.Include(x=>x.NgayKhoiHanhs).FirstOrDefaultAsync(x=>x.IdTour == id);
            if (entity == null)
            {
                return NotFound();
            }
            if (entity.NgayKhoiHanhs != null && entity.NgayKhoiHanhs.Any())
            {
                _context.NgayKhoiHanhs.RemoveRange(entity.NgayKhoiHanhs);
            }
            _context.Tours.Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListTour));
        }


        [HttpGet]
        public async Task<IActionResult> LocTrangThai(string? trangthai, string? query)
        {
            ViewBag.TrangThai = trangthai;

            var tours = _context.Tours.Include(t => t.IdDanhMucNavigation).AsQueryable();

            var timkiemtour = await _context.Tours
                .Where(x => x.TenTour.Contains(query))
                .OrderByDescending(x => x.TenTour).ToListAsync();
            if (!string.IsNullOrEmpty(trangthai))
            {
                tours = tours.Where(t => t.TrangThai == trangthai);
            }

            if (!string.IsNullOrEmpty(query))
            {
                tours = tours.Where(t => t.TenTour.Contains(query));
            }
            var result = await tours.OrderByDescending(t => t.IdTour).ToListAsync();
            return View("ListTour", result);
        }



        public async Task<IActionResult> ListDanhMucTour()
        {
            var lstDanhMuc = await _context.DanhMucTours
                .OrderByDescending(x => x.IdDanhMuc)
                .ToListAsync();

            return View(lstDanhMuc);
        }



        [HttpGet]
        public async Task<IActionResult> CreateDMTour()
        {
            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDMTour(DanhMucTour model)
        {
            if (ModelState.IsValid)
            {
                _context.DanhMucTours.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListDanhMucTour");
            }
            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> EditDMTour(int id)
        {
            var danhMuc = await _context.DanhMucTours.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc); // Truyền dữ liệu danh mục cần sửa vào View
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDMTour(DanhMucTour model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả lại form nếu có lỗi nhập liệu
            }

            var danhMuc = await _context.DanhMucTours.FindAsync(model.IdDanhMuc);
            if (danhMuc == null)
            {
                return NotFound();
            }

            // Cập nhật giá trị từ form
            danhMuc.TenDanhMuc = model.TenDanhMuc;
            danhMuc.TrangThai = model.TrangThai;

            _context.DanhMucTours.Update(danhMuc);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListDanhMucTour");
        }



        [HttpGet]
        public async Task<IActionResult> LocTheoTrangThai(string trangThai)
        {
            var tours = await _context.DanhMucTours
                .Where(t => t.TrangThai == trangThai)
                .ToListAsync();

            ViewBag.TrangThai = trangThai;

            return View("ListDanhMucTour", tours); // hoặc View riêng nếu bạn muốn
        }



        [HttpPost]
        public async Task<IActionResult> TimKiemDanhMuc(string query)
        {
            var ketqua = _context.DanhMucTours
                .Where(x => x.TenDanhMuc.Contains(query))
                .OrderByDescending(x => x.IdDanhMuc)
                .ToList();
            return View("ListDanhMucTour", ketqua);
        }



        [HttpGet]
        public async Task<IActionResult> DeleteDMTour(int id)
        {
            var danhMuc = await _context.DanhMucTours.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc); // Trả về View xác nhận
        }

        
        [HttpPost, ActionName("DeleteDMTour")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhMuc = await _context.DanhMucTours.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            _context.DanhMucTours.Remove(danhMuc);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListDanhMucTour");
        }







        public async Task<IActionResult> ListGiamGia()
        {
            var listgiamgia = await _context.MaGiamGia       
                .OrderByDescending(x => x.IdGiamGia)
                .ToListAsync();
            return View(listgiamgia);
        }




        // GET: Hiển thị form
        [HttpGet]
        public IActionResult CreateMaGiamGia()
        {
            var danhSachTour = _context.Tours
             .Select(t => new
                {
                   t.IdTour,
                   TenHienThi = t.TenTour + " - " + t.IdTour // ✅ gộp thông tin
                })
                 .ToList();

            var viewModel = new MaGiamGiaVM
            {
                DanhSachTour = new SelectList(danhSachTour, "IdTour", "TenHienThi")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaGiamGia(MaGiamGiaVM model)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaGiamGium
                {
                    MaCode = model.MaCode,
                    MoTa = model.MoTa,
                    PhanTramGiam = model.PhanTramGiam,
                    NgayBatDau = model.NgayBatDau,
                    NgayKetThuc = model.NgayKetThuc,
                    SoLuong = model.SoLuong,
                    TrangThai = model.TrangThai                                 
                };

                _context.MaGiamGia.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListGiamGia");
            }

            var danhSachTour = _context.Tours
                .Select(t => new
                {
                    t.IdTour,
                    TenHienThi = t.TenTour + " - " + t.IdTour
                }).ToList();

            model.DanhSachTour = new SelectList(danhSachTour, "IdTour", "TenHienThi");
            return View(model);
        }

   


        [HttpGet]
        public async Task<IActionResult> EditMaGiamGia(int id)
        {
            var giamGia = await _context.MaGiamGia.FindAsync(id);
            if (giamGia == null)
            {
                return NotFound();
            }

            var model = new MaGiamGiaVM
            {
                IdGiamGia = giamGia.IdGiamGia,
                MaCode = giamGia.MaCode,
                MoTa = giamGia.MoTa,
                PhanTramGiam = (int?)giamGia.PhanTramGiam,
                NgayBatDau = giamGia.NgayBatDau,
                NgayKetThuc = giamGia.NgayKetThuc,
                SoLuong = giamGia.SoLuong,
                TrangThai = giamGia.TrangThai,
                IdTour = giamGia.IdTour
            };

            ViewBag.IdTour = new SelectList(_context.Tours, "IdTour", "TenTour", giamGia.IdTour);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaGiamGia(MaGiamGiaVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IdTour = new SelectList(_context.Tours, "IdTour", "TenTour", model.IdTour);
                return View(model);
            }

            // Kiểm tra trùng mã (ngoại trừ chính nó)
            bool isDuplicate = await _context.MaGiamGia
                .AnyAsync(m => m.MaCode == model.MaCode && m.IdGiamGia != model.IdGiamGia);

            if (isDuplicate)
            {
                ModelState.AddModelError("MaCode", "Mã giảm giá đã tồn tại.");
                ViewBag.IdTour = new SelectList(_context.Tours, "IdTour", "TenTour", model.IdTour);
                return View(model);
            }

            var giamGia = await _context.MaGiamGia.FindAsync(model.IdGiamGia);
            if (giamGia == null)
            {
                return NotFound();
            }

            // Cập nhật giá trị
            giamGia.MaCode = model.MaCode;
            giamGia.MoTa = model.MoTa;
            giamGia.PhanTramGiam = model.PhanTramGiam;
            giamGia.NgayBatDau = model.NgayBatDau;
            giamGia.NgayKetThuc = model.NgayKetThuc;
            giamGia.SoLuong = model.SoLuong;
            giamGia.TrangThai = model.TrangThai;
            giamGia.IdTour = model.IdTour;

            _context.MaGiamGia.Update(giamGia);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListGiamGia");
        }



        [HttpGet]
        public async Task<IActionResult> DeleteMaGiamGia(int id)
        {
            var danhMuc = await _context.MaGiamGia.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc); // Trả về View xác nhận
        }


        [HttpPost, ActionName("DeleteMaGiamGia")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaGiamGiaConfirmed(int id)
        {
            var danhMuc = await _context.MaGiamGia.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            _context.MaGiamGia.Remove(danhMuc);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListGiamGia");
        }




        [HttpGet]
        public async Task<IActionResult> LocGiamGia(string trangThai = "Tất cả", string? query = null)
        {
            var data = _context.MaGiamGia.AsQueryable();

            // Lọc trạng thái
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất Cả")
            {
                data = data.Where(x => x.TrangThai == trangThai);
            }

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(query))
            {
                data = data.Where(x =>
                    x.MaCode.Contains(query) ||
                    x.MoTa.Contains(query));
            }

            ViewBag.TrangThai = trangThai;
            ViewBag.Query = query;

            var result = await data.ToListAsync();
            return View("ListGiamGia", result);
        }

















        public async Task<IActionResult> ListKH()
        {
            var lstKhachHang = await _context.KhachHangs
            .OrderByDescending(x => x.IdKhachHang)
             .ToListAsync();
            return View(lstKhachHang);
        }


        [HttpGet]
        public async Task<IActionResult> CreateKH(int? id)
        {
            var khachhang = await _context.KhachHangs.FindAsync(id);
            return View(khachhang); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKH(KhachHang model)
        {
            if (model.IdKhachHang == 0)
            {
                // Tạo mới
                if (!string.IsNullOrEmpty(model.MatKhau))
                {
                    model.MatKhau = model.MatKhau.ToSHA256Hash("MatKhau");
                }

                if (ModelState.IsValid)
                {
                    _context.KhachHangs.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ListKH");
                }
            }
            else
            {
                // Cập nhật
                var khachhang = await _context.KhachHangs.FindAsync(model.IdKhachHang);
                if (khachhang == null)
                {
                    return NotFound();
                }

                khachhang.HoTenKh = model.HoTenKh;
                khachhang.Email = model.Email;
                khachhang.Sdt = model.Sdt;
                khachhang.DiaChi = model.DiaChi;

                // ... các trường khác

                if (!string.IsNullOrEmpty(model.MatKhau))
                {
                    khachhang.MatKhau = model.MatKhau.ToSHA256Hash("MatKhau");
                }

                if (ModelState.IsValid)
                {
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ListKH");
                }

                return View(khachhang);
            }

            return View(model);
        }







        [HttpGet]
        public async Task<IActionResult> EditKH(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.KhachHangs.FindAsync(id); // ✅ Đã await

            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang); // ✅ Trả về đúng kiểu `KhachHang`
        }

        [HttpPost]
        public async Task<IActionResult> EditKH(KhachHang model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả lại form nếu có lỗi nhập liệu
            }

            var khachhang = await _context.KhachHangs.FindAsync(model.IdKhachHang);
            if (khachhang == null)
            {
                return NotFound();
            }

            khachhang.HoTenKh = model.HoTenKh;
            khachhang.DiaChi = model.DiaChi;
            khachhang.Sdt = model.Sdt;
            khachhang.Email = model.Email;

            // Chỉ cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
            if (!string.IsNullOrEmpty(model.MatKhau))
            {
                khachhang.MatKhau = model.MatKhau.ToSHA256Hash("MatKhau");
            }

            _context.KhachHangs.Update(khachhang);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListKH"); // Trở về danh sách thay vì view lại trang Edit
        }

        [HttpGet]
        public async Task<IActionResult> DeleteKH(int? id)
        {
            if (id == null)
                return NotFound();

            var khachhang = await _context.KhachHangs.FindAsync(id);
            if (khachhang == null)
                return NotFound();

            return View(khachhang); // Không xóa ở đây, chỉ hiển thị
        }

        [HttpPost, ActionName("DeleteKH")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKHConfirmed(int id)
        {
            var khachhang = await _context.KhachHangs.FindAsync(id);
            if (khachhang == null)
                return NotFound(); // ✅ đúng phương thức có sẵn trong ASP.NET Core


            _context.KhachHangs.Remove(khachhang);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListKH");
        }



       
        [HttpGet]
        public async Task<IActionResult> ThongKe(string tuan)
        {
            // Lấy ngày đầu tuần
            //DateTime startOfWeek = DateTime.Today.AddDays(-6); // mặc định tuần trước
            DateTime startOfWeek = FirstDateOfWeekISO8601(DateTime.Today.Year,
           CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today,
           CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));

            if (!string.IsNullOrEmpty(tuan))
            {
                var parts = tuan.Split("-W");
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int week))
                {
                    startOfWeek = FirstDateOfWeekISO8601(year, week);
                }
            }




            var endOfWeek = startOfWeek.AddDays(7);

            // Lấy danh sách ngày trong tuần
            var daysInWeek = Enumerable.Range(0, 7)
                .Select(i => startOfWeek.AddDays(i))
                .ToList();

            var data = await _context.DatTours
                .Include(x => x.IdTourNavigation)
                .Where(x => x.TrangThai == "Hoàn Tất" && x.NgayDat >= startOfWeek && x.NgayDat <= endOfWeek)
                .ToListAsync();

            // Tính doanh thu theo ngày, bao gồm cả ngày không có đơn
            var thongKeTheoNgay = daysInWeek
                .Select(ngay => new ThongKeTheoNgayVM
                {
                    Ngay = ngay,
                    TongTien = data
                        .Where(x => x.NgayDat!.Value.Date == ngay.Date)
                        .Sum(x => x.TongTien ?? 0)
                })
                .ToList();

            var thongKeTheoTour = data
                .Where(x => x.IdTourNavigation != null)
                .GroupBy(x => x.IdTourNavigation.TenTour)
                .Select(g => new ThongKeTheoTourVM
                {
                    TenTour = g.Key,
                    SoLanDat = g.Count(),
                    TongTien = g.Sum(x => x.TongTien ?? 0)
                })
                .OrderByDescending(x => x.TongTien)
                .ToList();

            ViewBag.TuanSelected = tuan;


            // doanh thu ngày hiện tại 
            var doanhThuHomNay = await _context.DatTours
     .Where(x => x.TrangThai == "Hoàn Tất" && x.NgayDat!.Value.Date == DateTime.Today)
     .SumAsync(x => x.TongTien ?? 0);

            //thống kê số đơn hôm nay
            var soDonHomNay = await _context.DatTours
    .Where( x=> x.NgayDat!.Value.Date == DateTime.Today)
    .CountAsync();
            //thống kê doanh thu theo tháng 
            var homNay = DateTime.Today;
            var dauThang = new DateTime(homNay.Year, homNay.Month, 1);
            var cuoiThang = dauThang.AddMonths(1).AddDays(-1);
            var doanhThuTheoThang = await _context.DatTours
    .Where(x => x.TrangThai == "Hoàn Tất" &&
                x.NgayDat >= dauThang && x.NgayDat <= cuoiThang)
    .SumAsync(x => x.TongTien ?? 0);


            var vm = new ThongKeVM
            {
                DoanhThuTuan = thongKeTheoNgay,
                DoanhThuTheoTour = thongKeTheoTour,
                 TongDoanhThuTheoNgay = doanhThuHomNay,
                 SoDonHomNay = soDonHomNay,
                DoanhThuTheoThang = doanhThuTheoThang
            };




            return View(vm);
        }


       // Hàm phụ để lấy ngày thứ 2 đầu tiên của tuần ISO
       //tính ngày bắt đầu kết thúc của tuần
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3); // về thứ 2
        }


        [HttpGet]
        public async Task<IActionResult> XuatExcel()
        {
            var datTours = await _context.DatTours
                .Include(x => x.IdTourNavigation)
                .Include(x => x.IdKhachHangNavigation)
                .OrderBy(x => x.NgayDat)
                .Take(500) // lấy 500 đơn
                .ToListAsync();

            // 1. Thống kê theo ngày
            var thongKeNgay = datTours
                .Where(x => x.TrangThai == "Hoàn Tất")
                .GroupBy(x => x.NgayDat!.Value.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    SoDon = g.Count(),
                    TongTien = g.Sum(x => (double?)x.TongTien) ?? 0
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            // 2. Thống kê theo tour
            var thongKeTour = datTours
                .Where(x => x.TrangThai == "Hoàn Tất")
                .GroupBy(x => x.IdTourNavigation.TenTour)
                .Select(g => new
                {
                    TenTour = g.Key,
                    SoLanDat = g.Count(),
                    TongTien = g.Sum(x => (double?)x.TongTien) ?? 0
                })
                .OrderByDescending(x => x.TongTien)
                .ToList();

            using var workbook = new XLWorkbook();

            // === Sheet 1: Chi tiết đơn đặt tour ===
            var ws1 = workbook.Worksheets.Add("ChiTietDatTour");

            ws1.Cell(1, 1).Value = "STT";
            ws1.Cell(1, 2).Value = "Mã Đặt Tour";
            ws1.Cell(1, 3).Value = "Khách hàng";
            ws1.Cell(1, 4).Value = "Tên Tour";
            ws1.Cell(1, 5).Value = "Ngày Đặt";
            ws1.Cell(1, 6).Value = "Người Lớn";
            ws1.Cell(1, 7).Value = "Trẻ Em";
            ws1.Cell(1, 8).Value = "PT Thanh Toán";
            ws1.Cell(1, 9).Value = "Tổng Tiền";
            ws1.Cell(1, 10).Value = "Trạng Thái";

            int row = 2;
            int stt = 1;
            foreach (var dt in datTours)
            {
                ws1.Cell(row, 1).Value = stt++;
                ws1.Cell(row, 2).Value = "DT" + dt.IdDatTour.ToString("D4");
                ws1.Cell(row, 3).Value = dt.IdKhachHangNavigation?.HoTenKh;
                ws1.Cell(row, 4).Value = dt.IdTourNavigation?.TenTour;
                ws1.Cell(row, 5).Value = dt.NgayDat?.ToString("dd/MM/yyyy");
                ws1.Cell(row, 6).Value = dt.SoNguoiLon;
                ws1.Cell(row, 7).Value = dt.SoTreEm;
                ws1.Cell(row, 8).Value = dt.PtthanhToan;
                ws1.Cell(row, 9).Value = dt.TongTien;
                ws1.Cell(row, 9).Style.NumberFormat.Format = "#,##0";
                ws1.Cell(row, 10).Value = dt.TrangThai;
                row++;
            }

            ws1.Columns().AdjustToContents();

            // === Sheet 2: Thống kê theo ngày ===
            var ws2 = workbook.Worksheets.Add("ThongKeTheoNgay");
            ws2.Cell(1, 1).Value = "Ngày";
            ws2.Cell(1, 2).Value = "Số Đơn";
            ws2.Cell(1, 3).Value = "Tổng Doanh Thu";

            row = 2;
            foreach (var item in thongKeNgay)
            {
                ws2.Cell(row, 1).Value = item.Ngay.ToString("dd/MM/yyyy");
                ws2.Cell(row, 2).Value = item.SoDon;
                ws2.Cell(row, 3).Value = item.TongTien;
                ws2.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
                row++;
            }

            ws2.Columns().AdjustToContents();

            // === Sheet 3: Thống kê theo tour ===
            var ws3 = workbook.Worksheets.Add("ThongKeTheoTour");
            ws3.Cell(1, 1).Value = "Tên Tour";
            ws3.Cell(1, 2).Value = "Số Lần Đặt";
            ws3.Cell(1, 3).Value = "Tổng Doanh Thu";

            row = 2;
            foreach (var item in thongKeTour)
            {
                ws3.Cell(row, 1).Value = item.TenTour;
                ws3.Cell(row, 2).Value = item.SoLanDat;
                ws3.Cell(row, 3).Value = item.TongTien;
                ws3.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
                row++;
            }

            ws3.Columns().AdjustToContents();

            // Trả file
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"BaoCaoDatTour_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }






        [HttpGet]
        public IActionResult QuanLyDatTour()
        {
            var danhSach = _context.DatTours
                            .Include(d => d.IdKhachHangNavigation)
                            .Include(d => d.IdTourNavigation)
                            .Include(d => d.IdNkhNavigation)
                            .ToList();
            return View(danhSach);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDatTour(int id)
        {
            var datTour = await _context.DatTours.FindAsync(id);
            if (datTour == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái
            datTour.TrangThai = "Đã Hủy";

            // Trả lại số chỗ cho ngày khởi hành (nếu muốn)
            var ngayKhoiHanh = await _context.NgayKhoiHanhs.FindAsync(datTour.IdNkh);
            if (ngayKhoiHanh != null)
            {
                ngayKhoiHanh.SoChoConLai += (datTour.SoNguoiLon + datTour.SoTreEm);
                _context.NgayKhoiHanhs.Update(ngayKhoiHanh);
            }

            _context.DatTours.Update(datTour);
            await _context.SaveChangesAsync();
            TempData["Success"] = $" Đã hủy đơn đặt tour.\" (Mã đặt tour: {datTour.IdDatTour}) thành công!";
           
            return RedirectToAction("QuanLyDatTour"); // hoặc tên view của bạn
        }







    }
}
