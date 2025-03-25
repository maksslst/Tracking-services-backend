using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.UserRepository;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<User?> Add(UserDto userDto)
    {
        var mappedUser = _mapper.Map<User>(userDto);
        if (mappedUser != null)
        {
            await _userRepository.CreateUser(mappedUser);
            return mappedUser;
        }

        return null;
    }

    public async Task<bool> Update(UserDto userDto)
    {
        var mappedUser = _mapper.Map<User>(userDto);
        if (mappedUser == null)
        {
            return false;
        }

        return await _userRepository.UpdateUser(mappedUser);
    }

    public async Task<bool> Delete(int userId)
    {
        return await _userRepository.DeleteUser(userId);
    }

    public async Task<UserDto?> GetById(int? id)
    {
        var user = await _userRepository.ReadById(id);
        var mappedUser = _mapper.Map<UserDto>(user);
        return mappedUser;
    }

    public async Task<IEnumerable<UserDto?>> GetAll()
    {
        var users = await _userRepository.ReadAll();
        var mappedUsers = users.Select(i => _mapper.Map<UserDto>(i));
        return mappedUsers;
    }
}