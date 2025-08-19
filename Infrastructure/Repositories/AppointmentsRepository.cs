using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace Infrastructure.Repositories;

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly ScheduleAppContext _context;

    public AppointmentsRepository(ScheduleAppContext context)
    {
        _context = context;
    }

    // In order to prevent overBooking of the Appointments, we use a Transaction
    public async Task<bool> CreateAppointmentAsync(Appointment appointment)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        
        // Retorna el shift que contenga ese Slot
        var shift = await _context.Shifts.Include(shift => shift.Slots)
            .FirstOrDefaultAsync(s => s.Slots.Any( slot => slot.Id == appointment.SlotId));
        
        if (shift.Slots.Count(s => s.isTaken) < shift.ServicesSlots)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            await _context.Slots.Where(s => s.Id == appointment.SlotId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.isTaken, true));
            
            await transaction.CommitAsync();
            return true;
        }
        await transaction.RollbackAsync();
        return false;
    }

    public async Task<Appointment?> GetThatAppointmentByIdAndSlotId(int userId, int slotId)
    {
        var appointment =  await _context.Appointments.Where( a => a.UserId == userId && a.SlotId == slotId).FirstOrDefaultAsync();
        return appointment;
    }



    public async Task UpdateStateAsync(int userId, int slotId, AppointmentState state)
    {
        var appointment = await this.GetThatAppointmentByIdAndSlotId(userId, slotId);
        if (appointment != null)
        {
            appointment.State = state;
            await _context.SaveChangesAsync();
        }
    }

    public bool UserHaveAnotherAppointmentsOnThatDay(int userId, int slotId)
    {
        var shiftDate = _context.Slots
            .Where(s => s.Id == slotId)
            .Select(s => s.Shift.Date)
            .FirstOrDefault();

        return  _context.Appointments
            .Any(a => a.UserId == userId && a.Slot.Shift.Date == shiftDate);
  
    }
}

