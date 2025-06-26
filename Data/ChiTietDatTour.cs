using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class ChiTietDatTour
{
    public int Idctdt { get; set; }

    public int? IdDatTour { get; set; }

    public int? SoLuong { get; set; }

    public double? DonGia { get; set; }

    public double? ThanhTien { get; set; }

    public virtual DatTour? IdDatTourNavigation { get; set; }
}
