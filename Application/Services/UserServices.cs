using Application.Dtos;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ProyectoFinal.Models;

namespace Application.Services;

public class UserServices
{
    private readonly IUserRepository _repository;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<UserServices> _logger;

    public UserServices(IUserRepository repository, ITokenProvider tokenProvider, ILogger<UserServices> logger)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task CreateUserAsync(UserCreate userCreate)
    {
        //We hash the password
        User newUser = UserMapper.ToEntity(userCreate);
        newUser.Role = Role.USER; // And we set this property. The day that we need Admins we will do another Method.
        newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
        await _repository.CreateUserAsync(newUser);
        
        _logger.LogInformation("User {UserName} created",  userCreate.UserName);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _repository.DeleteUserAsync(id);
        
        _logger.LogInformation("User of ID {id} Eliminated", id);
    }

    public async Task<string?> LoginAsync(UserLogin credentials)
    {
        User? user = await _repository.GetUserByUserNameAsync(credentials.UserName);
        if (user == null) throw new Exception("User not found");


        // Hash validation
        if (BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password))
        {
            string? token = _tokenProvider.Create(user);
            _logger.LogInformation(
                "The user {UserName} logs in successfully. Token returned: {token}",
                credentials.UserName, token);
            return token;
        }

        _logger.LogWarning(
            "The user {UserName} tried to log in to the system but failed (incorrect password)",
            credentials.UserName);
        throw new Exception("Incorrect password"); // otherwise
    }
}