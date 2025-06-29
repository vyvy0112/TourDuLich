using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
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

                // ➕ Thêm ngày khởi hành
                foreach (var item in model.NgayKhoiHanhs)
                {
                    var ngayKhoiHanh = new NgayKhoiHanh
                    {
                        IdTour = tour.IdTour,
                        NgayKhoiHanh1 = item.NgayKhoiHanh,
                        NgayKetThuc = item.NgayKhoiHanh,
                        SoChoConLai = item.SoCho
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


        //public async Task<IActionResult> CreateTour(Tour model, IFormFile? HinhAnhFile, List<IFormFile> HinhAnhPhu)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Xử lý upload hình ảnh
        //        if (HinhAnhFile != null && HinhAnhFile.Length > 0)
        //        {
        //            //var hinhPath = MyUtil.UploadHinh(HinhAnhFile, "Tour"); // ví dụ lưu vào wwwroot/hinhanh/Tour
        //            var hinhPath = MyUtil.UploadHinh(HinhAnhFile, "");
        //            if (!string.IsNullOrEmpty(hinhPath))
        //            {
        //                model.HinhAnh = hinhPath;
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "Không thể upload hình ảnh");
        //                ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
        //                return View(model);
        //            }
        //        }

        //        var tour = new Tour
        //        {
        //            TenTour = model.TenTour,
        //            MoTa = model.MoTa,
        //            GiaNguoiLon = model.GiaNguoiLon,
        //            GiaTreEm = model.GiaTreEm,
        //            TrangThai = "Hoạt Động",
        //            IdDanhMuc = model.IdDanhMuc,
        //            HinhAnh = model.HinhAnh,
        //            DiemDen = model.DiemDen,
        //            DiemKhoiHanh = model.DiemKhoiHanh,
        //            ThoiGian = model.ThoiGian,

        //        };

        //        _context.Tours.Add(tour);
        //        await _context.SaveChangesAsync();

        //        if (HinhAnhPhu != null && HinhAnhPhu.Count > 0)
        //        {
        //            foreach (var file in HinhAnhPhu)
        //            {
        //                var url = MyUtil.UploadHinh(file, "");
        //                if (!string.IsNullOrEmpty(url))
        //                {
        //                    var hinhanh = new TourHinhAnh
        //                    {
        //                        IdTour = tour.IdTour,
        //                        UrlHinhAnh = url,
        //                    };
        //                    _context.TourHinhAnhs.Add(hinhanh);
        //                }
        //            }
        //            await _context.SaveChangesAsync();
        //        }
        //        return RedirectToAction("ListTour");
        //    }

        //    ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc");
        //    return View(model);
        //}



        [HttpGet]
        public async Task<IActionResult> EditTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc", tour.IdDanhMuc);
            ViewBag.TrangThaiList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Hoạt Động", Text = "Hoạt Động" },
                new SelectListItem { Value = "Ngưng Hoạt Động", Text = "Ngưng Hoạt Động" }
            };

            ViewBag.NgayKhoiHanh = await _context.NgayKhoiHanhs
                .Where(n => n.IdTour == id)
                .OrderByDescending(n => n.NgayKhoiHanh1)
                .ToListAsync();
            return View(tour);
        }


        [HttpPost]
        public async Task<IActionResult> EditTour(Tour model, IFormFile? HinhAnhFile, List<IFormFile> HinhAnhPhu)
        {
            if (ModelState.IsValid)
            {
                var tour = await _context.Tours.FindAsync(model.IdTour);
                if (tour == null)
                {
                    return NotFound();
                }

                // Xử lý upload hình ảnh
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    var hinhPath = MyUtil.UploadHinh(HinhAnhFile, "");
                    if (!string.IsNullOrEmpty(hinhPath))
                    {
                        tour.HinhAnh = hinhPath;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể upload hình ảnh");
                        ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc", tour.IdDanhMuc);
                        return View(model);
                    }
                }

                // Cập nhật thông tin tour
                tour.TenTour = model.TenTour;
                tour.MoTa = model.MoTa;
                tour.GiaNguoiLon = model.GiaNguoiLon;
                tour.GiaTreEm = model.GiaTreEm;
                tour.TrangThai = model.TrangThai;
                
                tour.IdDanhMuc = model.IdDanhMuc;
                tour.DiemDen = model.DiemDen;
                tour.DiemKhoiHanh = model.DiemKhoiHanh;
                tour.ThoiGian = model.ThoiGian;

                _context.Tours.Update(tour);
                await _context.SaveChangesAsync();
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
                            _context.TourHinhAnhs.Update(hinhanh);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("ListTour");
            }
            // Trường hợp model lỗi validation
            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours, "IdDanhMuc", "TenDanhMuc", model.IdDanhMuc);
            ViewBag.TrangThaiList = new List<SelectListItem>
    {
        new SelectListItem { Value = "Hoạt Động", Text = "Hoạt Động", Selected = model.TrangThai == "Hoạt động" },
        new SelectListItem { Value = "Ngưng Hoạt Động", Text = "Ngưng Hoạt Động", Selected = model.TrangThai == "Ngưng Hoạt Động" }
    };
            ViewBag.IdDanhMuc = new SelectList(_context.DanhMucTours.ToList(), "IdDanhMuc", "TenDanhMuc", model.IdDanhMuc);
            return View(model);
        }


        // GET: Hiển thị giao diện xác nhận xóa
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Tours.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity); // Trả về View Delete.cshtml
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var entity = await _context.Tours.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
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

        // [HttpPost] Thực hiện xóa khi đã xác nhận
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
                return NotFound();

            _context.KhachHangs.Remove(khachhang);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListKH");
        }



        //// GET: Hiển thị giao diện xác nhận xóa
        //[HttpGet]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var entity = await _context.Tours.FindAsync(id);
        //    if (entity == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(entity); // Trả về View Delete.cshtml
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteTour(int id)
        //{
        //    var entity = await _context.Tours.FindAsync(id);
        //    if (entity == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Tours.Remove(entity);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(ListTour));
        //}
    }
}
