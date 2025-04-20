using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Add(CreateUserRequest request)
    {
        var user = _mapper.Map<User>(request);
        var userId = await _userRepository.CreateUser(user);
        _logger.LogInformation("Created user with id: {userId}", userId);
        return userId;
    }

    public async Task Update(UpdateUserRequest request)
    {
        var user = await _userRepository.ReadById(request.Id);
        if (user == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        user = _mapper.Map<User>(request);
        bool isUpdated = await _userRepository.UpdateUser(user);
        if (!isUpdated)
        {
            throw new EntityUpdateException("Couldn't update the user");
        }

        _logger.LogInformation("Updated user with id: {userId}", user.Id);
    }

    public async Task Delete(int userId)
    {
        bool isDeleted = await _userRepository.DeleteUser(userId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete user");
        }

        _logger.LogInformation("Deleted user with id: {userId}", userId);
    }

    public async Task<UserResponse> GetById(int id)
    {
        var user = await _userRepository.ReadById(id);
        if (user == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAll()
    {
        var users = await _userRepository.ReadAll();
        var usersResponse = users.Select(i => _mapper.Map<UserResponse>(i));
        return usersResponse;
    }
}