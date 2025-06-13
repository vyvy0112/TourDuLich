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

    public double? TongTien { get; set; }

    public string? TrangThai { get; set; }

    public virtual MaGiamGium? IdGiamGiaNavigation { get; set; }

    public virtual KhachHang? IdKhachHangNavigation { get; set; }

    public virtual Tour? IdTourNavigation { get; set; }
}
