using Application.DTOs.Mappings;

namespace Application.Services;

public interface IUserService
{
    public Task Add(UserDto userDto);
    public Task<bool> Update(UserDto userDto);
    public Task<bool> Delete(int userId);
    public Task<UserDto?> GetById(int? id);
    public Task<List<UserDto?>> GetAll();
}