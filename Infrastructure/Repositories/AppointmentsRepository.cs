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

        var aux = await _context.Shifts.Include(s => s.Appointments)
            .Where(s => s.Id == appointment.ShiftId).FirstAsync();

        if (aux.Appointments.Count() < aux.ServicesSlots)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        await transaction.RollbackAsync();
        return false;
    }

    public async Task<Appointment?> GetAppointmentByIdAndShiftIdAsync(int userId, int shiftId)
    {
        var appointment =  await _context.Appointments.Where( a => a.UserId == userId && a.ShiftId == shiftId).FirstOrDefaultAsync();
        return appointment;
    }


    public async Task UpdateStateAsync(int userId, int shiftId, AppointmentState state)
    {
        var appointment = await this.GetAppointmentByIdAndShiftIdAsync(userId, shiftId);
        if (appointment != null)
        {
            appointment.State = state;
            await _context.SaveChangesAsync();
        }
    }

    public bool UserHaveAnotherAppointmentsOnThatDay(int userId, int shiftId)
    {
        var date = _context.Shifts
            .Where(s => s.Id == shiftId)
            .Select(s => s.Date)
            .FirstOrDefault();

        if (date == default)
            return false;

        return _context.Appointments
            .Any(a => a.UserId == userId && a.Shift.Date == date);
    }
}

