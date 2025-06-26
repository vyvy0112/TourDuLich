using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class DanhGium
{
    public int IdDanhGia { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime? NgayTao { get; set; }

    public int? DiemDanhGia { get; set; }

    public string? NoiDung { get; set; }

    public int IdTour { get; set; }

    public int? IdKhachHang { get; set; }

    public virtual KhachHang? IdKhachHangNavigation { get; set; }

    public virtual Tour IdTourNavigation { get; set; } = null!;
}
