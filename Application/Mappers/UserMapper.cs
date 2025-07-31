using Application.Dtos;
using ProyectoFinal.Models;

namespace Application.Mappers;

public static class UserMapper
{
    public static User ToEntity(UserCreate create)
    {
        return new User()
        {
            UserName = create.UserName,
            Password = create.Password,
            Email = create.Email
            // Role = create.Role
        };
    }

}