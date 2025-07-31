using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IShiftRepository
{
    // Obtiene todos los turnos que aun acepten citas
    // Deebria retornar un Shift con Schedule
    // No se mostraran registros que ya tengan el maximo de Slots permitidos
    Task<IEnumerable<Shift>> GetAvailableShiftsSinceTodayAsync();
    
    Task<bool> ThatShiftExists(int id);
    Task CreateShiftAsync(Shift shift);
    Task DeleteShiftAsync(int id);

    // Me voy a pensar si usar Path aqui o no
    //Task Update(Shift shift);
}