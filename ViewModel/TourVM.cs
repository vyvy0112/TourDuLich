using System.ComponentModel.DataAnnotations.Schema;
using VNTour.Data;

namespace VNTour.ViewModel
{
    public class TourVM
    {
           public int IdTour { get; set; }
       
        public string TenTour { get; set; }
            public string MoTa { get; set; }
            public double GiaNguoiLon { get; set; }
            public double GiaTreEm { get; set; }
            public int IdDanhMuc { get; set; }
            public string? HinhAnh { get; set; }
            public string DiemDen { get; set; }
            public string DiemKhoiHanh { get; set; }
            public string ThoiGian { get; set; }
            public string TrangThai { get; set; }
        public List<NgayKhoiHanhVM> NgayKhoiHanhs { get; set; } = new();







        public string? TenDanhMuc { get; set; }

        public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();

        public virtual DanhMucTour? IdDanhMucNavigation { get; set; }

        public virtual ICollection<TourHinhAnh> TourHinhAnhs { get; set; } = new List<TourHinhAnh>();
        
    }


    public class ChiTietTour
    {
        public int IdTour { get; set; }

        public string TenTour { get; set; } = null!;

        public string? MoTa { get; set; }

        public double GiaNguoiLon { get; set; }

        public double GiaTreEm { get; set; }

        public string? ThoiGian { get; set; }

        public string? DiemKhoiHanh { get; set; }

        public string? DiemDen { get; set; }

        public List<NgayKhoiHanh>? NgayKhoiHanhs { get; set; } // ✅ đúng


        public string? HinhAnh { get; set; }

        public int? SoChoConLai { get; set; }

        public string? TrangThai { get; set; }

        public int? IdDanhMuc { get; set; }
        public string? TenDanhMuc { get; set; }
        public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

        public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();

        public virtual DanhMucTour? IdDanhMucNavigation { get; set; }

        public virtual ICollection<TourHinhAnh> TourHinhAnhs { get; set; } = new List<TourHinhAnh>();

    }
}
