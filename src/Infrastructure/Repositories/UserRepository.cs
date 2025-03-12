using Domain.Entities;
using Bogus;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private List<User> _users;
    // private readonly ICompanyRepository _companyRepository; - пришлось удалить из-за ошибки, связанной с циклической зависимостью между репозиториями User и Company

    public UserRepository()
    {
        _users = new List<User>();
        DataGeneration();
    }
    
    public Task CreateUser(User user)
    {
        _users.Add(user);
        return Task.CompletedTask;
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

    public Task<List<User?>> ReadAll()
    {
        return Task.FromResult(_users);
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