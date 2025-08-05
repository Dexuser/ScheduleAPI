using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task DeleteUserAsync(int id);
    
    Task<bool> ThisUserExists(string user);
    Task<bool> ThisEmailExists(string email);
    Task<User?> GetUserByUserNameAsync(string username);
}