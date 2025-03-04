using Application.DTOs.Mappings;

namespace Application.Services;

public class UserService : IUserService
{
    public Task Add(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> GetById(int? id)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDto?>> GetAll()
    {
        throw new NotImplementedException();
    }
}