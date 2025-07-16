using VNTour.Data;

namespace VNTour.ViewModel
{
    public class DanhGiaVM
    {
        public int IdDanhGia { get; set; }

        public string HoTen { get; set; } = null!;

        public string? Email { get; set; }

        public DateTime? NgayTao { get; set; }

        public int? DiemDanhGia { get; set; }

        public string? NoiDung { get; set; }

        public int IdTour { get; set; }

        public virtual Tour IdTourNavigation { get; set; } = null!;
        public int IdKhachHang { get; set; }
    }
}
