using Microsoft.AspNetCore.Mvc;
using VNTour.Data;

namespace VNTour.ViewComponents
{
	public class DanhMucViewComponent : ViewComponent
	{
		private readonly TourDuLichContext _context;
		public DanhMucViewComponent(TourDuLichContext context)
		{
			_context = context; ;
		}
		public IViewComponentResult Invoke()
		{
			var danhMucs = _context.DanhMucTours.ToList();
			return View(danhMucs);
		}
	}
}
