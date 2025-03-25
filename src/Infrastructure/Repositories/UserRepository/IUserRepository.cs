using Domain.Entities;

namespace Infrastructure.Repositories.UserRepository;

public interface IUserRepository
{
    public Task<int> CreateUser(User user);
    public Task<bool> UpdateUser(User user);
    public Task<bool> DeleteUser(int userId);
    public Task<User?> ReadById(int? id);
    public Task<IEnumerable<User?>> ReadAll();
}