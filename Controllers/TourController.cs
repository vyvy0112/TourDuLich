using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNTour.Data;
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
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 6;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            // Query danh sách tour (bất đồng bộ)
            var listtour = await _context.Tours
             .AsNoTracking()
             .Where(x => x.TrangThai == "Hoạt Động")
             .Include(x => x.IdDanhMucNavigation) // << thêm dòng này
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


        public async Task<IActionResult> TourTheoDM(int? id, int? page)
        {
            if (id == null || !await _context.DanhMucTours.AnyAsync(x => x.IdDanhMuc == id))
            {
                return NotFound("Không tìm thấy danh mục tour này");
            }

            var danhMuc = await _context.DanhMucTours.FirstOrDefaultAsync(x => x.IdDanhMuc == id);
            ViewBag.TenDanhMuc = danhMuc?.TenDanhMuc; // để hiện tên danh mục trên View
            ViewBag.IdDanhMuc = id;                   // chính chỗ này => để phân trang giữ id

            int pageSize = 6;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var listtour = await _context.Tours
                .AsNoTracking()
                .Where(x => x.IdDanhMuc == id && x.TrangThai == "Hoạt Động")
                .Include(x => x.IdDanhMucNavigation)
                .OrderBy(x => x.TenTour)
                .ToListAsync();

            PagedList<Tour> tours = new PagedList<Tour>(listtour, pageNumber, pageSize);

            if (tours == null || !tours.Any())
            {
                return NotFound("Không có tour nào hoạt động trong danh mục này.");
            }

            return View(tours);

        }


        
        public async Task<IActionResult> TimKiemTour(string? query, int? giaTu, int? giaDen, int? idDanhMuc,DateTime? ngayBatDau)
        {
            var queryTour = _context.Tours.AsQueryable();

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
                //queryTour = queryTour.Where(x => x.NgayKhoiHanh.HasValue && x.NgayKhoiHanh.Value >= ngayBatDau.Value);
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
                NgayKhoiHanh = p.NgayKhoiHanh,
                HinhAnh = p.HinhAnh,
                SoCho = p.SoCho,
                TrangThai = p.TrangThai,
                IdDanhMuc = p.IdDanhMuc,
                TenDanhMuc = p.IdDanhMucNavigation.TenDanhMuc
            }).ToListAsync();

            // Đổ ViewBag
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;
            ViewBag.TuKhoa = query;
            ViewBag.IdDanhMuc = idDanhMuc;
                    ViewBag.NgayBatDau = ngayBatDau?.ToString("yyyy-MM-dd");
            ViewBag.DanhMucTours = await _context.DanhMucTours
                .AsNoTracking()
                .OrderBy(x => x.TenDanhMuc)
                .ToListAsync();

            return View(lsttour);
        }

        public IActionResult ChiTiet(int id)
        {
            var tour = _context.Tours
                .AsNoTracking()
                .Include(t => t.IdDanhMucNavigation)
                .SingleOrDefault(t => t.IdTour == id);

            if (tour == null)
            {
                return NotFound("Tour không tồn tại hoặc đã bị xóa.");
            }

            var hinhanhs = _context.TourHinhAnhs.Where(x=>x.IdTour == id)
                .Select(x=>x.UrlHinhAnh).ToList();
            var danhgias = _context.DanhGia.Where(x=>x.IdTour == id)
                .OrderByDescending(x=>x.NgayTao)
                .ToList();
            var result = new ChiTietTour
            {
                IdTour = tour.IdTour,
                TenTour = tour.TenTour,
                MoTa = tour.MoTa,
                GiaNguoiLon = tour.GiaNguoiLon,
                GiaTreEm = tour.GiaTreEm,
                ThoiGian = tour.ThoiGian,
                DiemKhoiHanh = tour.DiemKhoiHanh,
                DiemDen = tour.DiemDen,
                NgayKhoiHanh = tour.NgayKhoiHanh,
                HinhAnh = tour.HinhAnh,
                SoCho = tour.SoCho,
                TrangThai = tour.TrangThai,
                IdDanhMuc = tour.IdDanhMuc,
                TenDanhMuc = tour.IdDanhMucNavigation?.TenDanhMuc,
                //TourHinhAnhs = _context.TourHinhAnhs
                //    .Where(th => th.IdTour == tour.IdTour)
                //    .Select(th => new TourHinhAnh
                //    {
                //        IdHinhAnh = th.IdHinhAnh,
                //        HinhAnh = th.HinhAnh
                //    }).ToList()

            };

            ViewBag.DanhGias = danhgias;
            ViewBag.HinhAnhs = hinhanhs;

            return View(result);
        }


        [HttpPost]
        public IActionResult DanhGia(DanhGiaVM model)
        {
            if (!ModelState.IsValid)
            {
                var danhgia = new DanhGium
                {
                    IdTour = model.IdTour,
                    HoTen = model.HoTen,
                    NoiDung = model.NoiDung,    
                    DiemDanhGia = model.DiemDanhGia,
                    NgayTao = model.NgayTao,
                };
                 _context.DanhGia.Add(danhgia);
                _context.SaveChanges();

            }
            var lstdanhgia =  _context.DanhGia
                .Where(x=>x.IdTour == model.IdTour)
                .OrderByDescending(x => x.NgayTao)
                .ToList();
            return RedirectToAction("ChiTiet", new { id = model.IdTour });
        }


    }

}

