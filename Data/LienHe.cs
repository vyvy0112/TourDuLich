using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class LienHe
{
    public int IdLienHe { get; set; }

    public int? IdKhachHang { get; set; }

    public string? HoTen { get; set; }

    public string? SoDienThoai { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual KhachHang? IdKhachHangNavigation { get; set; }
}
