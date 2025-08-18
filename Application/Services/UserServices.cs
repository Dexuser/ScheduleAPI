using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ProyectoFinal.Models;

namespace Application.Services;

public class UserServices
{
    private readonly IUserRepository _repository;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<UserServices> _logger;
    private readonly UserValidator _userValidator;

    public UserServices(
        IUserRepository repository,
        ITokenProvider tokenProvider,
        ILogger<UserServices> logger,
        UserValidator userValidator)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
        _logger = logger;
        _userValidator = userValidator;
    }

    public async Task CreateUserAsync(UserCreate userCreate)
    {
        await _userValidator.ValidateAsync(userCreate);
        
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
        if (user == null) throw new ValidationException("User not found");


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
            "The user {UserName} tried to log in to the system but failed " +
            "(incorrect password)",
            credentials.UserName);
        throw new ValidationException("Incorrect password"); // otherwise
    }
}