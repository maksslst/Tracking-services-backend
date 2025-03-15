using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface IUserService
{
    public Task<User?> Add(UserDto userDto);
    public Task<bool> Update(UserDto userDto);
    public Task<bool> Delete(int userId);
    public Task<UserDto?> GetById(int? id);
    public Task<IEnumerable<UserDto?>> GetAll();
}