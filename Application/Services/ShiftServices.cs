using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ShiftServices  
{
    private readonly IShiftRepository _shiftRepository;
    private readonly ShiftValidator _shiftValidator;
    private readonly ILogger<ShiftServices> _logger;
    
    public ShiftServices(IShiftRepository shiftRepository, ShiftValidator shiftValidator, ILogger<ShiftServices> logger)
    {
        _shiftRepository = shiftRepository;
        _shiftValidator = shiftValidator;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ShiftDto>> GetAvailableShiftsSinceTodayAsync()
    {
        return ShiftMapper.ToDto(await _shiftRepository.GetAvailableShiftsSinceTodayAsync());
    }

    public async Task<IEnumerable<ShiftDto>> GetShiftsWithThisUserAsync(int userId)
    {
        return ShiftMapper.ToDto(await _shiftRepository.GetShiftsWithThisUserAsync(userId));
    }

    public async Task CreateShiftAsync(string adminWhoRequested, ShiftCreate shift)
    {
        // validar que no se cree en el pasado
        // validar que se cree en un rango de fechas habilitada.
        await _shiftValidator.ValidateAsync(adminWhoRequested, shift);
        await _shiftRepository.CreateShiftAsync(ShiftMapper.ToEntity(shift));
        
        _logger.LogInformation(
            "The admin {admin} added a new shift. " +
            "Date: {shift.Date}, scheduleId: {shift.ScheduleId}," +
            " ServiceSlots: {shift.ServicesSlots}, DurationOnMinutes: {shift.DurationOnMinutes}" ,
            adminWhoRequested,
            shift.Date,
            shift.ScheduleId,
            shift.ServicesSlots,
            shift.MeetingDurationOnMinutes);
    }
    

    public async Task DeleteShiftAsync(string adminWhoRequested, int id)
    {
        if (!await _shiftRepository.ThatShiftExists(id))
        {
            _logger.LogError("The admin {adminWhoRequested}, failed to delete the shift of ID: {id} (That Shift doesn't exist)", adminWhoRequested, id);
            throw new ValidationException("That Shift doesn't exist");
        }

        if (await _shiftRepository.ThisShiftHaveAppointmentsSuscribed(id))
        {
            _logger.LogError("The admin {adminWhoRequested}, failed to delete the shift of ID: {id} (That Shift has appointments)", adminWhoRequested, id);
            throw new ValidationException("That Shift has appointments");
        }
        
        await _shiftRepository.DeleteShiftAsync(id);
        _logger.LogInformation("The admin {adminWhoRequested}, deleted the shift of ID: {id}", adminWhoRequested, id);
    }
}