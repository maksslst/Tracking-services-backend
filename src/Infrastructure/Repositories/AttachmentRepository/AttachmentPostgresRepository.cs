using Dapper;
using Domain.Entities;
using Npgsql;

namespace Infrastructure.Repositories.AttachmentRepository;

public class AttachmentPostgresRepository : IAttachmentRepository
{
    private readonly NpgsqlConnection _connection;

    public AttachmentPostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> Save(Attachment attachment)
    {
        var attachmentId = await _connection.ExecuteScalarAsync<int>(
            @"INSERT INTO attachments (file_name, stored_path, content_type, size, created_at)
                VALUES (@FileName, @StoredPath, @ContentType, @Size, @CreatedAt)
                RETURNING id", attachment);
        
        attachment.Id = attachmentId;
        return attachmentId;
    }

    public async Task<bool> Delete(int id)
    {
        var attachmentToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM attachments
                WHERE id = @AttachmentId", new { AttachmentId = id });
        
        return attachmentToDelete > 0;
    }

    public async Task<Attachment?> ReadById(int id)
    {
        var attachment = await _connection.QueryFirstOrDefaultAsync<Attachment>(
            @"SELECT id, file_name, stored_path, content_type, size, created_at
                FROM attachments
                WHERE id = @AttachmentId", new { AttachmentId = id });
        
        return attachment;
    }
}