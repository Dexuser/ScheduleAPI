using Domain.Enums;
using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IAppointmentsRepository
{
    Task<bool> CreateAppointmentAsync(Appointment appointment);

    Task<Appointment?> GetThatAppointmentByIdAndSlotId(int userId, int slotId);

    Task UpdateStateAsync(int userId, int slotId, AppointmentState state);
    
    bool UserHaveAnotherAppointmentsOnThatDay(int userId, int slotId ); 

}