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
            .Include(s => s.Slots)
            .Where( s => s.Date >=  DateOnly.FromDateTime(DateTime.Today) &&
                         s.Slots.Count(slot => slot.isTaken ) < s.ServicesSlots);
        return await shifts.ToListAsync();
    }

    public async Task<IEnumerable<Shift>> GetShiftsWithoutThisUser  (int userId)
    {
        var shifts = await _context.Shifts
            .Where(s => s.Date >= DateOnly.FromDateTime(DateTime.Today) &&
                        !s.Slots.Any(slot => slot.isTaken && slot.Appointment != null && slot.Appointment.UserId == userId) &&
                        s.Slots.Any(slot => !slot.isTaken))
            .Select(s => new Shift
            {
                Id = s.Id,
                Date = s.Date,
                ServicesSlots = s.ServicesSlots,
                MeetingDurationOnMinutes = s.MeetingDurationOnMinutes,
                Schedule = s.Schedule,
                Slots = s.Slots.Where(slot => !slot.isTaken).ToList()
            })
            .ToListAsync();

        return shifts;
    }

    public async Task<IEnumerable<Shift>> GetShiftsWithThisUserAsync(int userId)
    {
        //s.Slots.A.Any(a => a.UserId == userId)).ToListAsync();
        
        // retorna todos los Shifts que sean desde hoy al futuro y que contengan una cita de ese usuario.
        // LINQ te amo.
        return await _context.Shifts
            .Include(s => s.Schedule)
            .Where(s => s.Date >= DateOnly.FromDateTime(DateTime.Today) &&
                        s.Slots.Any(slot => slot.Appointment != null && slot.Appointment.UserId == userId))
            .Select(s => new Shift
            {
                Id = s.Id,
                Date = s.Date,
                ServicesSlots = s.ServicesSlots,
                MeetingDurationOnMinutes = s.MeetingDurationOnMinutes,
                Schedule = s.Schedule,
                Slots = s.Slots.Where(slot => slot.Appointment.UserId == userId).ToList()
            })
            .ToListAsync();

    }

    public async Task<IEnumerable<Shift>> GetAllShifts()
    {
        return await _context.Shifts.Include(s => s.Schedule).ToListAsync();
    }


    public Task<Slot?> GetSlotAndShiftBySlotIdAsync(int id)
    {
        return _context.Slots
            .Include(s => s.Shift)
            .Include(s => s.Appointment)
            .AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> ThatShiftExists(int id)
    {
        return await _context.Shifts.AnyAsync(s => s.Id == id);
    }

    public async Task CreateShiftAsync(Shift shift)
    {
        await _context.Shifts.AddAsync(shift);
        await _context.SaveChangesAsync();
    }

    public async Task AddTheSlots(List<Slot> slots)
    {
        await this._context.Slots.AddRangeAsync(slots);
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

    public async Task<bool> ThatSlotExists(int id)
    {
        return await this._context.Slots.AnyAsync(s => s.Id == id);
    }
    public async Task<bool> ThatSlotIsTaken(int id)
    {
        var slot =  await this._context.Slots.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return slot.isTaken;
        
        
        
    }
    
    // Knowing that the slots are something created from a Shift, its obvios to write these methods here
    public async Task<bool> ThisShiftHaveAppointmentsSuscribed(int id)
    {
        return await  _context.Shifts.AnyAsync(s => s.Id == id && s.Slots.Any(slot => slot.isTaken));
    }

    public async Task<Shift?> GetShiftByIdAsync(int id)
    {
        return await _context.Shifts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }
}