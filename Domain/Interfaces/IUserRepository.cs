using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task DeleteUserAsync(int id);

    Task<User?> GetUserByUserNameAsync(string username);
}