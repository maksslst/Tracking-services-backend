using Application.DTOs.Mappings;
using Domain.Entities;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IUserService
{
    public Task<User?> Add(CreateUserRequest request);
    public Task<bool> Update(UpdateUserRequest request);
    public Task<bool> Delete(int userId);
    public Task<UserResponse?> GetById(int id);
    public Task<IEnumerable<UserResponse?>> GetAll();
}