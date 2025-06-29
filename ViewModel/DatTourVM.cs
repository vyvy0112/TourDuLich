using System;
using System.ComponentModel.DataAnnotations;

namespace VNTour.ViewModel
{
    public class DatTourVM
    {
        // Thông tin tour
        public int IdTour { get; set; }
        public string TenTour { get; set; }

        [Display(Name = "Hình ảnh")]
        public string HinhAnh { get; set; }

        [Display(Name = "Ngày khởi hành")]
        [Required(ErrorMessage = "Vui lòng chọn ngày khởi hành.")]
        public DateTime NgayKhoiHanh { get; set; }

        [Display(Name = "Giá người lớn")]
        public decimal GiaNguoiLon { get; set; }

        [Display(Name = "Giá trẻ em")]
        public decimal GiaTreEm { get; set; }

        [Display(Name = "Số lượng người lớn")]
        [Range(1, 100, ErrorMessage = "Tối thiểu 1 người lớn")]
        public int SoLuongNguoiLon { get; set; }

        [Display(Name = "Số lượng trẻ em")]
        [Range(0, 100, ErrorMessage = "Số lượng trẻ em không hợp lệ")]
        public int SoLuongTreEm { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TongTien
        {
            get
            {
                return (SoLuongNguoiLon * GiaNguoiLon) + (SoLuongTreEm * GiaTreEm);
            }
        }

        // Thông tin người đặt (nếu có đăng nhập thì có thể bỏ)
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
    }
}
