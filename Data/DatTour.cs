using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class DatTour
{
    public int IdDatTour { get; set; }

    public int? IdKhachHang { get; set; }

    public int? IdTour { get; set; }

    public int? IdGiamGia { get; set; }

    public DateOnly? NgayDat { get; set; }

    public int? SoNguoiLon { get; set; }

    public int? SoTreEm { get; set; }

    public double? DonGia { get; set; }

    public double? TongTien { get; set; }

    public string? GhiChu { get; set; }

    public string? PtthanhToan { get; set; }

    public string? TrangThai { get; set; }

    public int? IdNhanVien { get; set; }

    public virtual ICollection<ChiTietDatTour> ChiTietDatTours { get; set; } = new List<ChiTietDatTour>();

    public virtual MaGiamGium? IdGiamGiaNavigation { get; set; }

    public virtual KhachHang? IdKhachHangNavigation { get; set; }

    public virtual NhanVien? IdNhanVienNavigation { get; set; }

    public virtual Tour? IdTourNavigation { get; set; }
}
