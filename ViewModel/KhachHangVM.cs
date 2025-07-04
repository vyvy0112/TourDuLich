namespace VNTour.ViewModel
{
    public class KhachHangVM
    {
        public int IdKhachHang { get; set; }

        public string HoTenKh { get; set; } = null!;

        public string Sdt { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? DiaChi { get; set; }

        public string? MatKhau { get; set; }
    }
}
