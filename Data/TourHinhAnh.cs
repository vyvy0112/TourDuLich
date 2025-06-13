using System;
using System.Collections.Generic;

namespace VNTour.Data;

public partial class TourHinhAnh
{
    public int Id { get; set; }

    public int? IdTour { get; set; }

    public string? UrlHinhAnh { get; set; }

    public virtual Tour? IdTourNavigation { get; set; }
}
