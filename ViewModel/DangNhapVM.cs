using System.ComponentModel.DataAnnotations;

namespace VNTour.ViewModel
{
    public class DangNhapVM
    {
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc Số điện thoại")]
      
        public string EmailOrPhone { get; set; }  // Nhập email hoặc số điện thoại

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }
}
