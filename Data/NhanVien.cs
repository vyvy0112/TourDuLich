using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class NhanVien
{
    public int IdNhanVien { get; set; }

    public string HoTenNv { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? MatKhau { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();
}
