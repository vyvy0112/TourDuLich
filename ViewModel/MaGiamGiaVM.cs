using Microsoft.AspNetCore.Mvc.Rendering;
using VNTour.Data;

namespace VNTour.ViewModel
{
    public class MaGiamGiaVM
    {
        public int IdGiamGia { get; set; }
        public string MaCode { get; set; }
        public string MoTa { get; set; }
        public int? PhanTramGiam { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? SoLuong { get; set; }
        public string TrangThai { get; set; }
        public int? IdTour { get; set; }
        public List<int> SelectedTours { get; set; } = new List<int>();
        public SelectList DanhSachTour { get; set; }

    }

}
