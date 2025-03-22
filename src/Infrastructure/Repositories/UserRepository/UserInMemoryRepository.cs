using Bogus;
using Domain.Entities;

namespace Infrastructure.Repositories.UserRepository;

public class UserInMemoryRepository : IUserRepository
{
    private List<User> _users;

    public UserInMemoryRepository()
    {
        _users = new List<User>();
        DataGeneration();
    }

    public Task<int> CreateUser(User user)
    {
        _users.Add(user);
        return Task.FromResult(user.Id);
    }

    public Task<bool> UpdateUser(User user)
    {
        User userToUpdate = _users.Find(i => i.Id == user.Id);
        if (userToUpdate == null)
        {
            return Task.FromResult(false);
        }

        userToUpdate.Username = user.Username;
        userToUpdate.FirstName = user.FirstName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.Patronymic = user.Patronymic;
        userToUpdate.Company = user.Company;
        userToUpdate.Email = user.Email;
        userToUpdate.CompanyId = user.CompanyId;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteUser(int userId)
    {
        User userToDelete = _users.Find(i => i.Id == userId);
        if (userToDelete == null)
        {
            return Task.FromResult(false);
        }

        _users.Remove(userToDelete);
        return Task.FromResult(true);
    }

    public Task<User?> ReadById(int? id)
    {
        User user = _users.Find(i => i.Id == id);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User?>> ReadAll()
    {
        return Task.FromResult<IEnumerable<User?>>(_users);
    }

    private void DataGeneration()
    {
        var faker = new Faker();
        for (int i = 0; i < 5; i++)
        {
            User user = new User()
            {
                Id = i + 1,
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                Username = faker.Person.UserName,
                Patronymic = faker.Name.LastName(),
                Email = faker.Person.Email
            };

            _users.Add(user);
        }
    }
}