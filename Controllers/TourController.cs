using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VNTour.Data;
using VNTour.Helpers;
using VNTour.ViewModel;
using X.PagedList;

namespace VNTour.Controllers
{
    public class TourController : Controller
    {
        private readonly TourDuLichContext _context;

        public TourController(TourDuLichContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? page,int? id)
        {        
            var danhMucs = await _context.DanhMucTours
           .Where(x => x.TrangThai == "Hoạt Động")
            .ToListAsync();


            ViewBag.DanhMucList = new SelectList(danhMucs, "IdDanhMuc", "TenDanhMuc", id);

            int pageSize = 6;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;       
            var listtour = await _context.Tours
    .AsNoTracking()
    .Where(x => x.TrangThai == "Hoạt Động" && x.IdDanhMucNavigation.TrangThai == "Hoạt Động")
    .Include(x => x.IdDanhMucNavigation)
    .Include(x => x.NgayKhoiHanhs) // << Thêm dòng này
    .OrderBy(x => x.TenTour)
    .ToListAsync();

            PagedList<Tour> tours = new PagedList<Tour>(listtour, pageNumber, pageSize);

            // Kiểm tra nếu không có dữ liệu
            if (tours == null || !tours.Any())
            {
                return NotFound("Không có tour nào hoạt động.");
            }
            return View(tours);
        }


        
        public async Task<IActionResult> TourTheoDM(int? id, int? page, string tuKhoa, DateTime? ngayBatDau, int? giaTu, int? giaDen)
        {
            if (id == null || !await _context.DanhMucTours.AnyAsync(x => x.IdDanhMuc == id))
            {
                return NotFound("Không tìm thấy danh mục tour này");
            }

            var danhMuc = await _context.DanhMucTours.FirstOrDefaultAsync(x => x.IdDanhMuc == id);
            ViewBag.TenDanhMuc = danhMuc?.TenDanhMuc;
            ViewBag.IdDanhMuc = id;
            var danhMucs = await _context.DanhMucTours
           .Where(x => x.TrangThai == "Hoạt Động")
           
           .ToListAsync();
            ViewBag.DanhMucList = new SelectList(danhMucs, "IdDanhMuc", "TenDanhMuc", id); // id nếu đã chọn

            int pageSize = 6;
            int pageNumber = page ?? 1;

            var query = _context.Tours
                .AsNoTracking()
                .Include(x => x.IdDanhMucNavigation)
                .Include(x=>x.NgayKhoiHanhs)
                .Where(x => x.IdDanhMuc == id
                            && x.TrangThai == "Hoạt Động"
                            && x.IdDanhMucNavigation.TrangThai == "Hoạt Động");

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(x => x.TenTour.Contains(tuKhoa) || x.DiemDen.Contains(tuKhoa));
                ViewBag.TuKhoa = tuKhoa;
            }

          

            if (giaTu.HasValue)
            {
                query = query.Where(x => x.GiaNguoiLon >= giaTu.Value);
                ViewBag.GiaTu = giaTu;
            }

            if (giaDen.HasValue)
            {
                query = query.Where(x => x.GiaNguoiLon <= giaDen.Value);
                ViewBag.GiaDen = giaDen;
            }

            if (ngayBatDau.HasValue)
            {
                DateTime ngay = ngayBatDau.Value.Date;
                query = query.Where(x =>
                    x.NgayKhoiHanhs.Any(n => n.NgayKhoiHanh1.Date >= ngay));
                ViewBag.NgayBatDau = ngayBatDau.Value.ToString("yyyy-MM-dd");
            }


            var listTour = await query.OrderBy(x => x.TenTour).ToListAsync();
            var tours = new PagedList<Tour>(listTour, pageNumber, pageSize);

            return View(tours);
        }



        public async Task<IActionResult> TimKiemTour(string? query, int? giaTu, int? giaDen, int? idDanhMuc,DateTime? ngayBatDau)
        {           
            var queryTour = _context.Tours
             .Include(x => x.IdDanhMucNavigation) // Nếu cần lấy TenDanhMuc
             .Include(x=>x.NgayKhoiHanhs) // Nếu cần lấy NgayKhoiHanh
             .Where(x => x.TrangThai == "Hoạt Động") // Nếu bạn chỉ muốn lấy tour hoạt động
             .AsQueryable();
            if (!string.IsNullOrEmpty(query))
            {
                queryTour = queryTour.Where(x =>
                    x.TenTour.Contains(query) ||
                    x.DiemDen.Contains(query) ||     
                    x.DiemKhoiHanh.Contains(query) ||
                    x.MoTa.Contains(query)
                );
            }

            if (giaTu.HasValue)
            {
                queryTour = queryTour.Where(x => x.GiaNguoiLon >= giaTu.Value);
            }

            if (giaDen.HasValue)
            {
                queryTour = queryTour.Where(x => x.GiaNguoiLon <= giaDen.Value);
            }

            if (idDanhMuc.HasValue)
            {
                queryTour = queryTour.Where(x => x.IdDanhMuc == idDanhMuc.Value);
            }
            if (ngayBatDau.HasValue)
            {
                var ngay = ngayBatDau.Value.Date;

                // Lọc các tour có ít nhất một ngày khởi hành sau ngày lọc
                queryTour = queryTour.Where(t => t.NgayKhoiHanhs.Any(nkh => nkh.NgayKhoiHanh1.Date >= ngay));
            }

           


            var lsttour = await queryTour.Select(p => new TourVM
            {
                IdTour = p.IdTour,
                TenTour = p.TenTour,
                MoTa = p.MoTa,
                GiaNguoiLon = p.GiaNguoiLon,
                GiaTreEm = p.GiaTreEm,
                ThoiGian = p.ThoiGian,
                DiemKhoiHanh = p.DiemKhoiHanh,
                DiemDen = p.DiemDen,               
                TrangThai = p.TrangThai,              
                TenDanhMuc = p.IdDanhMucNavigation.TenDanhMuc
            }).ToListAsync();

            // Đổ ViewBag
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;
            ViewBag.TuKhoa = query;
            ViewBag.IdDanhMuc = idDanhMuc;
            ViewBag.NgayBatDau = ngayBatDau;
            ViewBag.DanhMucTours = await _context.DanhMucTours
                .AsNoTracking()
                .OrderBy(x => x.TenDanhMuc)
                .ToListAsync();

            return View(lsttour);
        }

        //public IActionResult ChiTiet(int id)
        //{
        //    var tour = _context.Tours
        //        .AsNoTracking()
        //        .Include(t => t.IdDanhMucNavigation)
        //        .Include(t => t.NgayKhoiHanhs) // Thêm dòng này nếu cần 
        //        .SingleOrDefault(t => t.IdTour == id);

        //    if (tour == null)
        //    {
        //        return NotFound("Tour không tồn tại hoặc đã bị xóa.");
        //    }

        //    //lọc bảng hình anh
        //    var hinhanhs = _context.TourHinhAnhs.Where(x => x.IdTour == id)
        //        .Select(x => x.UrlHinhAnh).ToList();

        //    var danhgias = _context.DanhGia.Where(x => x.IdTour == id)
        //        .OrderByDescending(x => x.NgayTao)
        //        .ToList();

        //    //var ngayhientai = DateTime.Now.Date;
        //    //var listngaykhoihanh = _context.NgayKhoiHanhs
        //    //    .Where(x => x.NgayKhoiHanh1 >= ngayhientai)
        //    //    .OrderBy(x => x.NgayKhoiHanh1)
        //    //    .ToList();


        //    var result = new ChiTietTour
        //    {
        //        IdTour = tour.IdTour,
        //        TenTour = tour.TenTour,
        //        MoTa = tour.MoTa,
        //        GiaNguoiLon = tour.GiaNguoiLon,
        //        GiaTreEm = tour.GiaTreEm,
        //        ThoiGian = tour.ThoiGian,
        //        DiemKhoiHanh = tour.DiemKhoiHanh,
        //        DiemDen = tour.DiemDen,
        //        NgayKhoiHanhs = tour.NgayKhoiHanhs?.ToList(),
        //        HinhAnh = tour.HinhAnh,
        //        TrangThai = tour.TrangThai,
        //        IdDanhMuc = tour.IdDanhMuc,
        //        TenDanhMuc = tour.IdDanhMucNavigation?.TenDanhMuc,


        //    };

        //    ViewBag.DanhGias = danhgias;
        //    ViewBag.HinhAnhs = hinhanhs;

        //    return View(result);
        //}




        public IActionResult ChiTiet(int id)
        {
            var tour = _context.Tours
                .Include(t => t.NgayKhoiHanhs)
                .FirstOrDefault(t => t.IdTour == id);

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // Mặc định không cho đánh giá
            bool coTheDanhGia = false;

            if (!string.IsNullOrEmpty(email))
            {
                coTheDanhGia = _context.DatTours.Any(dt =>
                    dt.IdTour == id &&
                    dt.IdKhachHangNavigation.Email == email &&
                    dt.TrangThai == "Đã Hoàn Thành" || dt.TrangThai == "Đã Thanh Toán" // hoặc "Đã thanh toán", "Thành công", tuỳ hệ thống
                );
            }

            ViewBag.CoTheDanhGia = coTheDanhGia;

            // Load các đánh giá
            ViewBag.DanhGias = _context.DanhGia
                .Where(dg => dg.IdTour == id)
                .OrderByDescending(dg => dg.NgayTao)
                .ToList();

            var vm = new ChiTietTour
            {
                IdTour = tour.IdTour,
                TenTour = tour.TenTour,
                MoTa = tour.MoTa,
                GiaNguoiLon = tour.GiaNguoiLon,
                GiaTreEm = tour.GiaTreEm,
                HinhAnh = tour.HinhAnh,
                NgayKhoiHanhs = tour.NgayKhoiHanhs.ToList(),
                ThoiGian = tour.ThoiGian
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult DanhGia(int idTour)
        {
            var khachHang = User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);

            if (khachHang == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idkhachhang = int.Parse(khachHang.Value);

            // Kiểm tra khách đã đặt tour này và đã hoàn tất hay đã thanh toán
            var daDatTour = _context.DatTours.Any(d =>
                d.IdKhachHang == idkhachhang &&
                d.IdTour == idTour &&
                (d.TrangThai == "Đã Hoàn Thành" || d.TrangThai == "Đã Thanh Toán"));

            if (!daDatTour)
            {
                TempData["Error"] = "Bạn cần đặt tour này thành công mới có thể đánh giá.";
                return RedirectToAction("ChiTiet", "Tour", new { id = idTour });
            }

            // Cho phép vào form đánh giá
            var model = new DanhGiaVM
            {
                IdTour = idTour,
                IdKhachHang = idkhachhang,
            };

            return View(model);
        }

       
        [HttpPost]
        public IActionResult DanhGia(DanhGium model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }
            model.IdKhachHangNavigation = _context.KhachHangs.FirstOrDefault(x=>x.IdKhachHang == model.IdKhachHang);
            model.Email = email;
            model.NgayTao = DateTime.Now;

            _context.DanhGia.Add(model);
            _context.SaveChanges();

            return RedirectToAction("ChiTiet", new { id = model.IdTour });
        }


    }

}

