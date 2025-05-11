using Dapper;
using Domain.Entities;
using Infrastructure.Database.TypeMappings;
using Npgsql;

namespace Infrastructure.Repositories.UserRepository;

public class UserPostgresRepository : IUserRepository
{
    private readonly NpgsqlConnection _connection;

    public UserPostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateUser(User? user)
    {
        var userId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO users (username, first_name, last_name, patronymic, email, company_id, role, password_hash, logo_attachment_id)
                VALUES(@Username, @FirstName, @LastName, @Patronymic, @Email, @CompanyId, @Role::user_role, @PasswordHash, @LogoAttachmentId)
                RETURNING Id", user.AsDapperParams());

        return userId;
    }

    public async Task<bool> UpdateUser(User user)
    {
        var userToUpdate = await _connection.ExecuteAsync(
            @"UPDATE users
                SET username = @Username,
                    first_name = @FirstName,
                    last_name = @LastName,
                    patronymic = @Patronymic,
                    email = @Email,
                    company_id = @CompanyId,
                    role = @Role,
                    password_hash = @PasswordHash,
                    logo_attachment_id = LogoAttachmentId
                WHERE Id = @Id", user.AsDapperParams());

        return userToUpdate > 0;
    }

    public async Task<bool> DeleteUser(int userId)
    {
        var userToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM users
                WHERE id = @Id", new { Id = userId });

        return userToDelete > 0;
    }

    public async Task<User?> ReadById(int id)
    {
        var user = await _connection.QueryFirstOrDefaultAsync<User>(
            @"SELECT id, username, first_name, last_name, patronymic, email, company_id, role::text, logo_attachment_id
                FROM users
                WHERE id = @Id", new { Id = id });

        return user;
    }

    public async Task<User?> ReadByUsername(string username)
    {
        var user = await _connection.QueryFirstOrDefaultAsync<User>(
            @"SELECT id, username, first_name, last_name, patronymic, email, company_id, role::text, password_hash
                FROM users
                WHERE username = @Username", new { Username = username });

        return user;
    }

    public async Task<IEnumerable<User?>> ReadAll()
    {
        var users = await _connection.QueryAsync<User>(
            @"SELECT id, username, first_name, last_name, patronymic, email, company_id, role::text
                FROM users");

        return users;
    }
}