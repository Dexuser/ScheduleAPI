using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Validations;

public class UserValidator
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserValidator> _logger;

    public UserValidator(IUserRepository userRepository, ILogger<UserValidator> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task ValidateAsync(UserCreate userCreate)
    {
        if (await _userRepository.ThisUserExists(userCreate.UserName))
        {
            _logger.LogError(
                "Someone failed to create a new user. (User {userCreate.UserName} already exists)",
                userCreate.UserName);
            
            throw new ValidationException("User already exists");
        }
        
        if (await _userRepository.ThisEmailExists(userCreate.Email))
        {
            _logger.LogError(
                "Someone failed to create a new user. (email {userCreate.Email} already exists)",
                userCreate.Email);
            
            throw new ValidationException("email already exists");
        }
    }
    
}