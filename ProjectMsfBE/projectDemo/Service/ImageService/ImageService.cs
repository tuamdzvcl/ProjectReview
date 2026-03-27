namespace projectDemo.Service.ImageService
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void Delete(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var fileName = Path.GetFileName(imageUrl);
            var path = Path.Combine(_env.WebRootPath, "images", fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Exception("File không tìm thấy ảnh");
                }
                if (!file.ContentType.StartsWith("image/"))
                    throw new Exception("Invalid file type");

                if (file.Length > 2 * 1024 * 1024)
                    throw new Exception("Max 2MB");

                var folder = Path.Combine(_env.WebRootPath, "images");
                Console.WriteLine(folder);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                Console.WriteLine(file?.FileName);
                Console.WriteLine(file?.ContentType);
                Console.WriteLine(fileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.CreateNew);
                await file.CopyToAsync(stream);

                return $"/images/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.ToString());
            }
        }
    }
}
