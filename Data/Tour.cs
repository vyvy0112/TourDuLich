using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class Tour
{
    public int IdTour { get; set; }

    public string TenTour { get; set; } = null!;

    public string? MoTa { get; set; }

    public double GiaNguoiLon { get; set; }

    public double GiaTreEm { get; set; }

    public string? ThoiGian { get; set; }

    public string? DiemKhoiHanh { get; set; }

    public string? DiemDen { get; set; }

    public DateOnly? NgayKhoiHanh { get; set; }

    public DateOnly? NgayVe { get; set; }

    public string? HinhAnh { get; set; }

    public int? SoCho { get; set; }

    public string? TrangThai { get; set; }

    public int? IdDanhMuc { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();

    public virtual DanhMucTour? IdDanhMucNavigation { get; set; }

    public virtual ICollection<TourHinhAnh> TourHinhAnhs { get; set; } = new List<TourHinhAnh>();
}
