using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ProyectoFinal.Models;

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
    
    public async Task<IEnumerable<ShiftDto>> GetAvailableShiftsSinceTodayAsync(int userId)
    {
        // Remove all the shifts that have an appointment with this user
        var shifts = new List<Shift>( await _shiftRepository.GetAvailableShiftsSinceTodayAsync());
        shifts.RemoveAll(s => s.Appointments.Any(a => a.UserId == userId));
        return ShiftMapper.ToDto(shifts);
    }

    public async Task<IEnumerable<ShiftDto>> GetShiftsWithThisUserAsync(int userId)
    {
        return ShiftMapper.ToDto(await _shiftRepository.GetShiftsWithThisUserAsync(userId));
    }

    public async Task CreateShiftAsync(string adminWhoRequested, ShiftCreate shift)
    {
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
        await _shiftValidator.ValidateDeleteAsync(adminWhoRequested, id);      
        await _shiftRepository.DeleteShiftAsync(id);
        _logger.LogInformation("The admin {adminWhoRequested}, deleted the shift of ID: {id}", adminWhoRequested, id);
    }
}