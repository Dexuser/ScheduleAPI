using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly ScheduleAppContext _context;

    public ScheduleRepository(ScheduleAppContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
    {
        
        return await _context.Schedules.ToListAsync();
    }

    public async Task CreateScheduleAsync(Schedule schedule)
    {
        await _context.Schedules.AddAsync(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteScheduleAsync(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule != null)
        {
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateScheduleAsync(int id, Schedule schedule)
    {
        
        // _context.Schedules.Update(schedule)
        
        var sch =  await _context.Schedules.FindAsync(id);
        if (sch != null)
        {
            sch.StartTime = schedule.StartTime;
            sch.EndTime = schedule.EndTime;
            sch.Description = schedule.Description;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Schedule?> FindByIdAsync(int id)
    {
        Schedule? schedule = await _context.Schedules.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return schedule;
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Schedules.AnyAsync(s => s.Id == id);
    }

    public async Task<Schedule?> FindByStartTimeAndEndTime(TimeOnly startTime, TimeOnly endTime)
    {
        Schedule? schedule = await _context.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.StartTime == startTime &&
                                      u.EndTime == endTime);
        return schedule;
    }

    public async Task<bool> IsThisScheduledUsed(int id)
    {
        return  await _context.Shifts.AnyAsync(s => s.ScheduleId == id);
    }
}