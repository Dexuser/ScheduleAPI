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
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ShiftValidator _shiftValidator;
    private readonly ILogger<ShiftServices> _logger;
    
    public ShiftServices(IShiftRepository shiftRepository, ShiftValidator shiftValidator, ILogger<ShiftServices> logger, IScheduleRepository scheduleRepository)
    {
        _shiftRepository = shiftRepository;
        _shiftValidator = shiftValidator;
        _logger = logger;
        _scheduleRepository = scheduleRepository;
    }
    
    
    public async Task<IEnumerable<ShiftDto>> GetAllShifts(int userId)
    {
        var shifts = new List<Shift>( await _shiftRepository.GetAllShifts());
        return ShiftMapper.ToDto(shifts);
    }


    
    public async Task<IEnumerable<ShiftDto>> GetShiftsWithOutThisUser(int userId)
    {
        var shifts = new List<Shift>( await _shiftRepository.GetShiftsWithoutThisUser(userId));
        return ShiftMapper.ToDto(shifts);
    }

    public async Task<IEnumerable<ShiftDto>> GetShiftsWithThisUserAsync(int userId)
    {
        return ShiftMapper.ToDto(await _shiftRepository.GetShiftsWithThisUserAsync(userId));
    }

    public async Task CreateShiftAsync(string adminWhoRequested, ShiftCreate shift)
    {
        await _shiftValidator.ValidateAsync(adminWhoRequested, shift);
        Shift newShift = ShiftMapper.ToEntity(shift);
        await _shiftRepository.CreateShiftAsync(newShift);
        
        _logger.LogInformation(
            "The admin {admin} added a new shift. " +
            "Date: {shift.Date}, scheduleId: {shift.ScheduleId}," +
            " ServiceSlots: {shift.ServicesSlots}, DurationOnMinutes: {shift.DurationOnMinutes}" ,
            adminWhoRequested,
            shift.Date,
            shift.ScheduleId,
            shift.ServicesSlots,
            shift.MeetingDurationOnMinutes);

        await this.GenerateTheSlotsOfThisShift(adminWhoRequested,newShift);
    }

    private async Task GenerateTheSlotsOfThisShift(string adminWhoRequested, Shift shift)
    {
        var schedule = await _scheduleRepository.FindByIdAsync(shift.ScheduleId);

        var slots = new List<Slot>();
        var time = schedule.StartTime;

        for (int i = 0; i < shift.ServicesSlots - 1; i++)
        {
            var slot = new Slot
            {
                ShiftId = shift.Id,
                isTaken = false,
                StartTime = time,
                EndTime = time.AddMinutes(shift.MeetingDurationOnMinutes) 
            };
            
            slots.Add(slot);
            time = time.AddMinutes(shift.MeetingDurationOnMinutes);
        }
        // This prevents the Extend       
        var lastSlot = new Slot
        {
            ShiftId = shift.Id,
            isTaken = false,
            StartTime = time,
            EndTime = time.AddMinutes(shift.MeetingDurationOnMinutes) 
        };
        
        // Si end está después del límite
        if (lastSlot.EndTime > schedule.EndTime)
        {
            // calcular diferencia
            var diff = lastSlot.EndTime.ToTimeSpan() - schedule.EndTime.ToTimeSpan();

            // restar esa diferencia
            lastSlot.EndTime = lastSlot.EndTime.AddMinutes(-diff.TotalMinutes);
        }
        // después de ajustar el lastSlot
        slots.Add(lastSlot); 

        await _shiftRepository.AddTheSlots(slots);

        _logger.LogInformation(
            "The admin {admin} added {count} new Slots for the Shift of ID: {shiftId}",
            adminWhoRequested,
            slots.Count,
            shift.Id);
    }

    public async Task DeleteShiftAsync(string adminWhoRequested, int id)
    {
        await _shiftValidator.ValidateDeleteAsync(adminWhoRequested, id);      
        await _shiftRepository.DeleteShiftAsync(id);
        _logger.LogInformation("The admin {adminWhoRequested}, deleted the shift of ID: {id}", adminWhoRequested, id);
    }
}