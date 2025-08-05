using Domain.Enums;
using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IAppointmentsRepository
{
    Task<bool> CreateAppointmentAsync(Appointment appointment);

    Task<Appointment?> GetAppointmentByIdAndShiftIdAsync(int userId, int shiftId);

    Task UpdateStateAsync(int userId, int shiftId, AppointmentState state);
    
    bool UserHaveAnotherAppointmentsOnThatDay(int userId, int shiftId); 

    // Es posible que este metodo sea inncesario teniendo en cuenta que
    // podria hacer un metodo en Schedules que me traiga los Appointments
    // Asociados
    //Task<IEnumerable<Appointment>> GetAppointmentsAsync();
}