using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly ScheduleAppContext _context;
    
    public ShiftRepository(ScheduleAppContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<Shift>> GetAvailableShiftsSinceTodayAsync()
    {
        var shifts = _context.Shifts
            .Include(s => s.Schedule)
            .Include(s => s.Appointments)
            .Where( s => s.Date >=  DateOnly.FromDateTime(DateTime.Today) &&
                         s.Appointments.Count < s.ServicesSlots);
        return await shifts.ToListAsync();
    }

    public async Task<IEnumerable<Shift>> GetShiftsWithThisUserAsync(int userId)
    {
        // retorna todos los Shifts que sean desde hoy al futuro y que contengan una cita de ese usuario.
        // LINQ te amo.
        return await _context.Shifts
            .Include(s => s.Schedule)
            .Include(s => s.Appointments)
            .Where( s => s.Date >= DateOnly.FromDateTime(DateTime.Today) && 
                         s.Appointments.Any(a => a.UserId == userId)).ToListAsync();
    }

    public async Task<bool> ThatShiftExists(int id)
    {
        return await _context.Shifts.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> ThatShiftStillAcceptsAppointments(int shiftId)
    {
        var shift = await _context.Shifts
            .Include(s => s.Appointments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == shiftId);
        
        return (shift.Appointments.Count < shift.ServicesSlots);
        
    }

    public async Task CreateShiftAsync(Shift shift)
    {
        await _context.Shifts.AddAsync(shift);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteShiftAsync(int id)
    {
        var shift = await _context.Shifts.FindAsync(id);
        if (shift != null)
        {
            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ThisShiftHaveAppointmentsSuscribed(int id)
    {
        return await  _context.Shifts.AnyAsync(s => s.Id == id && s.Appointments.Count > 0);
    }

    public async Task<Shift?> GetShiftByIdAsync(int id)
    {
        return await _context.Shifts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }
}