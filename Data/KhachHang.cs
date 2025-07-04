using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class KhachHang
{
    public int IdKhachHang { get; set; }

    public string HoTenKh { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? MatKhau { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();

    public virtual ICollection<LienHe> LienHes { get; set; } = new List<LienHe>();
}
