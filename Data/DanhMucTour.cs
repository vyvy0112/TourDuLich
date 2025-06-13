using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class DanhMucTour
{
    public int IdDanhMuc { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string? TrangThai { get; set; }

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
