namespace Fiorello1.Helpers
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, string webRootPath);
        void Delete(string fileName, string webRootPath);
        bool IsImage(IFormFile formFile);
        bool CheckSize(IFormFile formFile, int maxSize);
    }
}
