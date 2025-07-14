using System;
using System.ComponentModel.DataAnnotations;
using VNTour.Data;

namespace VNTour.ViewModel
{
    public class DatTourVM
    {

        public bool giongkhachhang { get; set; } = true;
        public int IdTour { get; set; }
        public string? TenTour { get; set; }
        public string? HinhAnh { get; set; }

        public double GiaNguoiLon { get; set; }
        public double GiaTreEm { get; set; }

        public int SoLuongNguoiLon { get; set; } = 1;   // Mặc định 1
        public int SoLuongTreEm { get; set; } = 0;

        public DateTime NgayDat { get; set; }

        public double TongTienGoc { get; set; }      // Tổng chưa giảm
        public double? TienGiam { get; set; }        // Số tiền giảm (nếu có)
        public double TongTien => TongTienGoc - (TienGiam ?? 0); // Tổng sau giảm
        public double TongTienSauGiam { get; set; }
        public double TongTienSauGoc { get; set; }  
        
        public int IdNkh {  get; set; }     
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }

        public int? IdGiamGia { get; set; }  // mã từ dropdown
        public string? MaCode { get; set; }  // mã từ người dùng nhập
        public string? TrangThai { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
        public int PhanTramGiam { get; set; }
        public string? GhiChu { get; set; }
        public string? PhuongThucThanhToan { get; set; }

        public int IdDatTour { get; set; }


    }


    public class LichSuDatTourVM
    {
        public int IdDatTour { get; set; }
        public string TenTour { get; set; }
        public string HinhAnh { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
        public DateTime NgayDat { get; set; }
        public int SoNguoiLon { get; set; }
        public int SoTreEm { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } // nếu có trạng thái
    }


}
