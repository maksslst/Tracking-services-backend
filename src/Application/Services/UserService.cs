using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;
using Infrastructure.Repositories.CompanyRepository;
using Npgsql;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ICompanyRepository _companyRepository;

    public UserService(IUserRepository userRepository, IMapper mapper, ICompanyRepository companyRepository)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _companyRepository = companyRepository;
    }

    public async Task<int> Add(CreateUserRequest request)
    {
        var user = _mapper.Map<User>(request);
        return await _userRepository.CreateUser(user);
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
    }

    public async Task Delete(int userId)
    {
        bool isDeleted = await _userRepository.DeleteUser(userId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete user");
        }
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