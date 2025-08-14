using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IShiftRepository
{
    
    /*
     * Obtiene todos los turnos que aún acepten citas.
     * Retorna un objeto Shift que tiene objeto Schedule incrustado.
     * No se mostrarán los registros que ya alcansaron el maximo de citas permitidos.
     */
    Task<IEnumerable<Shift>> GetAvailableShiftsSinceTodayAsync();
    Task<IEnumerable<Shift>> GetShiftsWithThisUserAsync(int userId);
    Task CreateShiftAsync(Shift shift);
    Task DeleteShiftAsync(int id);

    Task<bool> ThatShiftExists(int id);
    Task<bool> ThisShiftHaveAppointmentsSuscribed(int id);
    Task<bool> ThatShiftStillAcceptsAppointments(int shiftId);
    Task<Shift?> GetShiftByIdAsync(int id);
}