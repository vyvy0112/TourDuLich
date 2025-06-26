using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class MaGiamGium
{
    public int IdGiamGia { get; set; }

    public string? MaCode { get; set; }

    public string? MoTa { get; set; }

    public double? PhanTramGiam { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public int? SoLuong { get; set; }

    public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();
}
