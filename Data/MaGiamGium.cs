using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class MaGiamGium
{
    public int IdGiamGia { get; set; }

    public string? MaCode { get; set; }

    public string? MoTa { get; set; }

    public double? PhanTramGiam { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public int? SoLuong { get; set; }

    public string? TrangThai { get; set; }

    public int? IdTour { get; set; }

    public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();

    public virtual Tour? IdTourNavigation { get; set; }
}
