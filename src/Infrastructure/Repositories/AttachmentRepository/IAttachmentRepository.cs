using Domain.Entities;

namespace Infrastructure.Repositories.AttachmentRepository;

public interface IAttachmentRepository
{
    public Task<int> Save(Attachment attachment);
    public Task<bool> Delete(int id);
    public Task<Attachment?> ReadById(int id);
}