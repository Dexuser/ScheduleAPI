using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace Infrastructure.Repositories;

public class EnabledDateRepository : IEnabledDateRepository
{
    private readonly ScheduleAppContext _context;

    public EnabledDateRepository(ScheduleAppContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<EnabledDate>> GetEnabledDatesAsync()
    {
        var dates = _context.EnabledDates
            .Where(date => date.EndDate >= DateOnly.FromDateTime(DateTime.Today));
        return await dates.ToListAsync();
    }

    public async Task DeleteEnabledDateAsync(int id)
    {
        var date = await _context.EnabledDates.FindAsync(id);
        if (date != null)
        {
            _context.EnabledDates.Remove(date);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateEnabledDateAsync(EnabledDate enabledDate)
    {
        await _context.EnabledDates.AddAsync(enabledDate);
        await _context.SaveChangesAsync();
    }
}