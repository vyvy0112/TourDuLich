namespace VNTour.Helpers
{
    public class MyUtil
    {
        //public static string UploadHinh(IFormFile hinhanh, string folder)
        //{
        //    try
        //    {
        //        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hinhanh", folder);
        //        using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
        //        {
        //            hinhanh.CopyTo(myfile);
        //        }
        //        return hinhanh.FileName;
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }
        //}

        public static string UploadHinh(IFormFile hinhanh, string folder)
        {
            try
            {
                // Tạo thư mục nếu chưa có
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hinhanh", folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Đặt tên file tránh trùng lặp
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(hinhanh.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var myfile = new FileStream(fullPath, FileMode.Create))
                {
                    hinhanh.CopyTo(myfile);
                }

                return Path.Combine(folder, fileName); // trả về đường dẫn tương đối
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

    }

}
