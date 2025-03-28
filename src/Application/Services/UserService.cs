using Application.DTOs.Mappings;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;
using Infrastructure.Repositories.CompanyRepository;

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

    public async Task<User?> Add(CreateUserRequest request)
    {
        if (await _companyRepository.ReadByCompanyId(request.CompanyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var user = new User()
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic,
            CompanyId = request.CompanyId
        };

        await _userRepository.CreateUser(user);
        return user;
    }

    public async Task<bool> Update(UpdateUserRequest request)
    {
        if (await _companyRepository.ReadByCompanyId(request.CompanyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        if (await _userRepository.ReadById(request.Id) == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        var user = new User()
        {
            Id = request.Id,
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Patronymic = request.Patronymic,
            CompanyId = request.CompanyId
        };

        return await _userRepository.UpdateUser(user);
    }

    public async Task<bool> Delete(int userId)
    {
        if (await _userRepository.ReadById(userId) == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        return await _userRepository.DeleteUser(userId);
    }

    public async Task<UserResponse?> GetById(int id)
    {
        var user = await _userRepository.ReadById(id);
        if (user == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        var userResponse = new UserResponse()
        {
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Patronymic = user.Patronymic,
            CompanyId = user.CompanyId
        };

        return userResponse;
    }

    public async Task<IEnumerable<UserResponse?>> GetAll()
    {
        var users = await _userRepository.ReadAll();
        if (users == null || users.Count() == 0)
        {
            throw new NotFoundApplicationException("Users not found");
        }
        
        var usersResponse = new List<UserResponse>();
        foreach (var user in users)
        {
            usersResponse.Add(new UserResponse()
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                CompanyId = user.CompanyId
            });
        }

        return usersResponse;
    }
}