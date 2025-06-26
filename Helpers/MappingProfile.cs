using AutoMapper;
using VNTour.Data;
using VNTour.ViewModel;

namespace VNTour.Helpers
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<DangKyVM, KhachHang>();
        }
    }
}
