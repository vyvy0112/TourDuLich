namespace VNTour.ViewModel
{
    public class ThongKeVM
    {
        public DateTime NgayDat { get; set; }
        public double TongDoanhThuTheoNgay { get; set; }

        public double DoanhThuTheoThang { get; set; }
        public int SoDonHomNay { get; set; }
        public double TongTien { get; set; }
        public int TongKhachHang { get; set; }

        //public List<DoanhThuNgayVM> DoanhThuTuan { get; set; }
        /////thống kê theo tour
        //public List<DoanhThuTheoTourVM> DoanhThuTheoTour { get; set; }
        public string? TuanSelected { get; set; } // <-- thêm dòng này

        public List<ThongKeTheoNgayVM> DoanhThuTuan { get; set; }
        public List<ThongKeTheoTourVM> DoanhThuTheoTour { get; set; }
    }

    public class ThongKeTheoNgayVM
    {
        public DateTime Ngay { get; set; }
        public double TongTien { get; set; }
    }

    public class ThongKeTheoTourVM
    {
        public string TenTour { get; set; }
        public double TongTien { get; set; }
        public int SoLanDat { get; set; }

    }

}
