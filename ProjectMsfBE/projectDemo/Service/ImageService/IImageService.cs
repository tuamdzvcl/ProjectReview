namespace projectDemo.Service.ImageService
{
    public interface IImageService
    {
        Task<string> UploadAsync(IFormFile file);
        void Delete(string imageUrl);
    }
}
