using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(int? page,int? id)
        {        
            var danhMucs = await _context.DanhMucTours
           .Where(x => x.TrangThai == "Hoạt Động")
            .ToListAsync();


            ViewBag.DanhMucList = new SelectList(danhMucs, "IdDanhMuc", "TenDanhMuc", id);

            int pageSize = 6;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            // Query danh sách tour (bất đồng bộ)
            //var listtour = await _context.Tours
            // .AsNoTracking()
            // .Where(x => x.TrangThai == "Hoạt Động" && x.IdDanhMucNavigation.TrangThai == "Hoạt Động")
            // .Include(x => x.IdDanhMucNavigation) // << thêm dòng này
            // .OrderBy(x => x.TenTour)
            // .ToListAsync();

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


        //public async Task<IActionResult> TourTheoDM(int? id, int? page)
        //{
        //    if (id == null || !await _context.DanhMucTours.AnyAsync(x => x.IdDanhMuc == id))
        //    {
        //        return NotFound("Không tìm thấy danh mục tour này");
        //    }

        //    var danhMuc = await _context.DanhMucTours.FirstOrDefaultAsync(x => x.IdDanhMuc == id);
        //    ViewBag.TenDanhMuc = danhMuc?.TenDanhMuc; // để hiện tên danh mục trên View
        //    ViewBag.IdDanhMuc = id;                   // chính chỗ này => để phân trang giữ id

        //    int pageSize = 6;
        //    int pageNumber = page == null || page < 1 ? 1 : page.Value;

        //    var listtour = await _context.Tours
        //      .AsNoTracking()
        //      .Include(x => x.IdDanhMucNavigation)
        //      .Where(x => x.IdDanhMuc == id
        //     && x.TrangThai == "Hoạt Động"
        //     && x.IdDanhMucNavigation.TrangThai == "Hoạt Động")
        //      .OrderBy(x => x.TenTour).ToListAsync();

        //    PagedList<Tour> tours = new PagedList<Tour>(listtour, pageNumber, pageSize);

        //    if (tours == null || !tours.Any())
        //    {
        //        return NotFound("Không có tour nào hoạt động trong danh mục này.");
        //    }

        //    return View(tours);

        //}      @*      kích thước ảnh 1349 x 736 *@
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

            var listTour = await query.OrderBy(x => x.TenTour).ToListAsync();
            var tours = new PagedList<Tour>(listTour, pageNumber, pageSize);
            //Cách này không tối ưu nếu có nhiều dữ liệu vì load toàn bộ rồi mới phân trang.


            return View(tours);
        }



        public async Task<IActionResult> TimKiemTour(string? query, int? giaTu, int? giaDen, int? idDanhMuc,DateOnly? ngayBatDau)
        {
            //   var queryTour = _context.Tours
            //.Include(x => x.IdDanhMucNavigation)
            //   .AsQueryable();

            //var queryTour = _context.Tours
            //.AsQueryable();

            var queryTour = _context.Tours
       .Include(x => x.IdDanhMucNavigation) // Nếu cần lấy TenDanhMuc
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

            //if (ngayBatDau.HasValue)
            //{
            //    queryTour = queryTour.Where(x =>
            //        x.NgayKhoiHanh.HasValue &&
            //        x.NgayKhoiHanh.Value == ngayBatDau.Value
            //    );
            //}



            
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
                //NgayKhoiHanh = p.NgayKhoiHanh,
                //HinhAnh = p.HinhAnh,
                //SoCho = p.SoCho,
                TrangThai = p.TrangThai,
                //IdDanhMuc = p.IdDanhMuc,
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

        public IActionResult ChiTiet(int id)
        {
            var tour = _context.Tours
                .AsNoTracking()
                .Include(t => t.IdDanhMucNavigation)
                .Include(t => t.NgayKhoiHanhs) // Thêm dòng này nếu cần 
                .SingleOrDefault(t => t.IdTour == id);

            if (tour == null)
            {
                return NotFound("Tour không tồn tại hoặc đã bị xóa.");
            }

            //lọc bảng hình anh
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

                NgayKhoiHanhs = tour.NgayKhoiHanhs?.ToList(),
                HinhAnh = tour.HinhAnh,
                //SoCho = tour.SoCho,
                TrangThai = tour.TrangThai,
                IdDanhMuc = tour.IdDanhMuc,
                TenDanhMuc = tour.IdDanhMucNavigation?.TenDanhMuc,
               

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

        public async Task<IActionResult> ListDat()
        {
            if (ModelState.IsValid)
            {
                var lstDatTour = await _context.DatTours
                    .AsNoTracking()
                    .Include(x => x.IdKhachHangNavigation)
                    .Include(x => x.IdTourNavigation)
                    .Include(x => x.IdGiamGiaNavigation)
                    .OrderByDescending(x => x.NgayDat)
                    .ToListAsync();

                return View(lstDatTour);
            }
            else
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

        }


    }

}

