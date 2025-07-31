using Application.Dtos;
using Application.Interfaces;
using Application.Mappers;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Services;

public class UserServices
{
    private readonly IUserRepository _repository;
    private readonly ITokenProvider _tokenProvider;

    public UserServices(IUserRepository repository, ITokenProvider tokenProvider)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
    }

    public async Task CreateUserAsync(UserCreate userCreate)
    {
        //We hash the password
        User newUser = UserMapper.ToEntity(userCreate);
        newUser.Role = Role.USER; // And we set this property. The day that we need Admins we will do another Method.
        newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
        await _repository.CreateUserAsync(newUser);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _repository.DeleteUserAsync(id);
    }

    public async Task<string?> LoginAsync(UserLogin userCreate)
    {
        User? user = await _repository.GetUserByUserNameAsync(userCreate.UserName);
        if (user == null) throw new Exception("User not found");


        // Hash validation
        if (BCrypt.Net.BCrypt.Verify(userCreate.Password, user.Password))
        {
            return _tokenProvider.Create(user); // return the token 
        }

        throw new Exception("Incorrect password"); // otherwise
    }
}