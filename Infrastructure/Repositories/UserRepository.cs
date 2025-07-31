using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;
using BCrypt.Net;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ScheduleAppContext _context;

    public UserRepository(ScheduleAppContext context)
    {
        _context = context;
    }

    public async Task CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetUserByUserNameAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username);
    }
}