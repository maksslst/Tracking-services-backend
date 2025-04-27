using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IUserService
{
    public Task<int> Add(CreateUserRequest request);
    public Task Update(UpdateUserRequest request);
    public Task Delete(int userId);
    public Task<UserResponse> GetById(int id);
    public Task<IEnumerable<UserResponse>> GetAll();
}