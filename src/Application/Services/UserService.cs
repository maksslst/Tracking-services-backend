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
        try
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

            return await _userRepository.CreateUser(user);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add user");
        }
    }

    public async Task<bool> Update(UpdateUserRequest request)
    {
        try
        {
            if (await _companyRepository.ReadByCompanyId(request.CompanyId) == null)
            {
                throw new NotFoundApplicationException("Company not found");
            }

            var user = await _userRepository.ReadById(request.Id);
            if (user == null)
            {
                throw new NotFoundApplicationException("User not found");
            }

            user.Username = request.Username;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Patronymic = request.Patronymic;
            user.CompanyId = request.CompanyId;    

            return await _userRepository.UpdateUser(user);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't update user");
        }
    }

    public async Task<bool> Delete(int userId)
    {
        try
        {
            bool isDeleted = await _userRepository.DeleteUser(userId);
            if (!isDeleted)
            {
                throw new NotFoundApplicationException("User not found");
            }

            return true;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete user");
        }
    }

    public async Task<UserResponse> GetById(int id)
    {
        try
        {
            var user = await _userRepository.ReadById(id);
            if (user == null)
            {
                throw new NotFoundApplicationException("User not found");
            }

            return _mapper.Map<UserResponse>(user);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get user");
        }
    }

    public async Task<IEnumerable<UserResponse>> GetAll()
    {
        try
        {
            var users = await _userRepository.ReadAll();
            if (users == null || users.Count() == 0)
            {
                throw new NotFoundApplicationException("Users not found");
            }

            var usersResponse = users.Select(i => _mapper.Map<UserResponse>(i));
            return usersResponse;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get users");
        }
    }
}