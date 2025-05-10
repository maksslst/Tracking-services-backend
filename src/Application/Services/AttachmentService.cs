using Domain.Entities;
using Infrastructure.Repositories.AttachmentRepository;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class AttachmentService(
    IAttachmentRepository repository,
    IHttpContextAccessor httpContextAccessor,
    IFileStorageService fileStorage)
    : IAttachmentService
{
    public async Task<Attachment> Upload(IFormFile file, string category)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var storedPath = await fileStorage.SaveFile(file, category, fileName);

        var attachment = new Attachment
        {
            FileName = file.FileName,
            StoredPath = storedPath,
            ContentType = file.ContentType,
            Size = file.Length,
            CreatedAt = DateTime.Now
        };

        await repository.Save(attachment);
        return attachment;
    }

    public async Task<byte[]> GetFileContent(int id)
    {
        var attachment = await repository.ReadById(id);
        if (attachment == null || !fileStorage.FileExists(attachment.StoredPath))
        {
            throw new FileNotFoundException("Attachment not found");
        }
        
        return await fileStorage.ReadFile(attachment.StoredPath);
    }

    public async Task Delete(int id)
    {
        var attachment = await repository.ReadById(id);
        if (attachment == null)
        {
            return;
        }
        
        fileStorage.DeleteFile(attachment.StoredPath);
        await repository.Delete(id);
    }

    public async Task<string> GetPublicLink(int id)
    {
        var context = httpContextAccessor.HttpContext;
        var request = context?.Request;
        
        var attachment = await repository.ReadById(id);
        if (attachment == null)
        {
            throw new FileNotFoundException("Attachment not found");
        }
        
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        return $"{baseUrl}/api/attachments/{id}/download";
    }

    public async Task<Attachment?> GetMetadata(int id)
    {
        return await repository.ReadById(id);
    }
}