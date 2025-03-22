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
        await _connection.OpenAsync();

        var userId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO users (username, ""firstName"", ""lastName"", patronymic, email, company_id)
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
        
        await _connection.CloseAsync();
        return userId;
    }

    public async Task<bool> UpdateUser(User user)
    {
        await _connection.OpenAsync();
        
        var userToUpdate = await _connection.ExecuteAsync(
            @"UPDATE users
                SET username = @Username,
                    firstName = @FirstName,
                    lastName = @LastName,
                    patronymic = @Patronymic,
                    email = @Email,
                    company_id = @CompanyId
                WHERE Id = @Id", user);
        
        await _connection.CloseAsync();
        return userToUpdate > 0;
    }

    public async Task<bool> DeleteUser(int userId)
    {
        await _connection.OpenAsync();

        var userToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM users
                WHERE id = @Id", new {Id = userId});
        
        await _connection.CloseAsync();
        return userToDelete > 0;
    }

    public async Task<User?> ReadById(int? id)
    {
        await _connection.OpenAsync();

        User? user = await _connection.QueryFirstOrDefaultAsync<User>(
            @"SELECT id, username, ""firstName"", ""lastName"", patronymic, email, company_id as CompanyId
                FROM users
                WHERE id = @Id", new { Id = id });
        
        await _connection.CloseAsync();
        return user;
    }

    public async Task<IEnumerable<User?>> ReadAll()
    {
        await _connection.OpenAsync();
        IEnumerable<User?> users = await _connection.QueryAsync<User>(
            @"SELECT id, username, ""firstName"", ""lastName"", patronymic, email, company_id as CompanyId
                FROM users");
        
        await _connection.CloseAsync();
        return users;
    }
}