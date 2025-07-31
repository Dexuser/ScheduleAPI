using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Services;

public class ShiftServices  
{
    private readonly IShiftRepository _shiftRepository;
    private readonly ShiftValidator _shiftValidator;

    public ShiftServices(IShiftRepository shiftRepository, ShiftValidator shiftValidator)
    {
        _shiftRepository = shiftRepository;
        _shiftValidator = shiftValidator;
    }
    
    public async Task<IEnumerable<ShiftDto>> GetAvailableShiftsSinceTodayAsync()
    {
        return ShiftMapper.ToDto(await _shiftRepository.GetAvailableShiftsSinceTodayAsync());
    }

    public async Task CreateShiftAsync(ShiftCreate shift)
    {
        // validar que no se cree en el pasado
        // validar que se cree en un rango de fechas habilitada.
        await _shiftValidator.ValidateAsync(shift);
        await _shiftRepository.CreateShiftAsync(ShiftMapper.ToEntity(shift));
    }

    public async Task DeleteShiftAsync(int id)
    {
        await _shiftRepository.DeleteShiftAsync(id);
    }
}