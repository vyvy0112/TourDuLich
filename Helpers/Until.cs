namespace VNTour.Helpers
{
    public class Until
    {
        public static string UpLoadHinh(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var uploadPath = Path.Combine("wwwroot", folderName);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }
    }
}
