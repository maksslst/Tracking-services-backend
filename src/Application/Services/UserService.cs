using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class UserService : IUserService
{
    private IUserRepository _userRepository;
    private IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task Add(UserDto userDto)
    {
        User mappedUser = _mapper.Map<User>(userDto);
        if (mappedUser != null)
        {
            await _userRepository.CreateUser(mappedUser);
        }
    }

    public async Task<bool> Update(UserDto userDto)
    {
        User mappedUser = _mapper.Map<User>(userDto);
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

    public async Task<List<UserDto?>> GetAll()
    {
        List<User?> users = await _userRepository.ReadAll();
        var mappedUsers = users.Select(i => _mapper.Map<UserDto>(i)).ToList();
        return mappedUsers;
    }
}