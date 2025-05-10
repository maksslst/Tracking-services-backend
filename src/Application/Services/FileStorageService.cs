using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    private readonly string _baseFolder = Path.Combine(environment.ContentRootPath, "Storage");

    public async Task<string> SaveFile(IFormFile file, string subFolder, string fileName)
    {
        var directory = Path.Combine(_baseFolder, subFolder);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var filePath = Path.Combine(directory, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public async Task<byte[]> ReadFile(string path)
    {
        return await File.ReadAllBytesAsync(path);
    }

    public string GetFilePublicLink(string storedPath)
    {
        return storedPath;
    }
}