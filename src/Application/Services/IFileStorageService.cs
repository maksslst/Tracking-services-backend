using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IFileStorageService
{
    public Task<string> SaveFile(IFormFile file, string subFolder, string fileName);
    public bool FileExists(string path);
    public void DeleteFile(string path);
    public Task<byte[]> ReadFile(string path);
    public string GetFilePublicLink(string storedPath);
}