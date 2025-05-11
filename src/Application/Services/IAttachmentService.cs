using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IAttachmentService
{
    public Task<Attachment> Upload(IFormFile file, string category);
    public Task<byte[]> GetFileContent(int id);
    public Task Delete(int id);
    public Task<string> GetPublicLink(int id);
    public Task<Attachment?> GetMetadata(int id);
}