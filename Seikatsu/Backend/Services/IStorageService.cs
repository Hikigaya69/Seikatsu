namespace Seikatsu.Backend.Services
{
    public interface IStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
    }
}
