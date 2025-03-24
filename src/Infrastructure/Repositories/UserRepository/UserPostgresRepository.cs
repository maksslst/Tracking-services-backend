using Dapper;
using Domain.Entities;
using Npgsql;

namespace Infrastructure.Repositories.UserRepository;

public class UserPostgresRepository : IUserRepository
{
    private readonly NpgsqlConnection _connection;

    public UserPostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateUser(User user)
    {
        var userId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO users (username, ""first_name"", ""last_name"", patronymic, email, company_id)
                VALUES(@Username, @FirstName, @LastName, @Patronymic, @Email, @CompanyId)
                RETURNING Id",
            new
            {
                user.Username,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.Email,
                user.CompanyId
            });

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
                    company_id = @CompanyId
                WHERE Id = @Id", user);

        return userToUpdate > 0;
    }

    public async Task<bool> DeleteUser(int userId)
    {
        var userToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM users
                WHERE id = @Id", new { Id = userId });

        return userToDelete > 0;
    }

    public async Task<User?> ReadById(int? id)
    {
        var user = await _connection.QueryFirstOrDefaultAsync<User>(
            @"SELECT id, username, ""first_name"", ""last_name"", patronymic, email, company_id as CompanyId
                FROM users
                WHERE id = @Id", new { Id = id });

        return user;
    }

    public async Task<IEnumerable<User?>> ReadAll()
    {
        var users = await _connection.QueryAsync<User>(
            @"SELECT id, username, ""first_name"", ""last_name"", patronymic, email, company_id as CompanyId
                FROM users");

        return users;
    }
}