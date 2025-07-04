using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class NgayKhoiHanh
{
    public int IdNkh { get; set; }

    public int IdTour { get; set; }

    public DateTime NgayKhoiHanh1 { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public int? SoChoConLai { get; set; }

    public virtual ICollection<DatTour> DatTours { get; set; } = new List<DatTour>();

    public virtual Tour IdTourNavigation { get; set; } = null!;
}
