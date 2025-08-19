using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IShiftRepository
{
    
    /*
     * Obtiene todos los turnos que aún acepten citas.
     * Retorna un objeto Shift que tiene objeto Schedule incrustado.
     * No se mostrarán los registros que ya alcansaron el maximo de citas permitidos.
     */
    Task<IEnumerable<Shift>> GetShiftsWithoutThisUser(int userId);
    Task<IEnumerable<Shift>> GetShiftsWithThisUserAsync(int userId);
    Task<IEnumerable<Shift>> GetAllShifts();
    Task CreateShiftAsync(Shift shift);
    Task AddTheSlots(List<Slot> slots);
    Task DeleteShiftAsync(int id);
    
    Task<bool> ThatSlotExists(int id);
    Task<bool> ThatSlotIsTaken(int id);
    Task<Slot?> GetSlotAndShiftBySlotIdAsync(int id);
    Task<bool> ThatShiftExists(int id);
    Task<bool> ThisShiftHaveAppointmentsSuscribed(int id);
    Task<Shift?> GetShiftByIdAsync(int id);
}