using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Validations;

public class ShiftValidator
{
    private readonly IEnabledDateRepository _enabledDateRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IShiftRepository _shiftRepository;
    private readonly ILogger<ShiftValidator> _logger;
    
    public ShiftValidator(IEnabledDateRepository enabledDateRepository, ILogger<ShiftValidator> logger,
        IScheduleRepository scheduleRepository, IShiftRepository shiftRepository)
    {
        _enabledDateRepository = enabledDateRepository;
        _logger = logger;
        _scheduleRepository = scheduleRepository;
        _shiftRepository = shiftRepository;
    }

    public async Task ValidateAsync(string adminWhoRequested, ShiftCreate shift)
    {
        // prevents shifts in the past
        if (shift.Date < DateOnly.FromDateTime(DateTime.Today))
        {
            _logger.LogError(
                "The admin {adminWhoRequested} failed to add a new shift. " +
                "(The date of the shift cannot be in the past)",
                adminWhoRequested);
            
            throw new ValidationException("The date of the shift cannot be in the past");
        }

        // prevents negative or zero Services slots
        if (shift.ServicesSlots < 1)
        {
            _logger.LogError(
                "The admin {adminWhoRequested} failed to add a new shift. " +
                "(The services slot cannot be less than 1)",
                adminWhoRequested);

            throw new ValidationException("The service slots cannot be less than 1");
        }
        
        // prevents invalid schedules.
        if (!await _scheduleRepository.ExistsAsync(shift.ScheduleId))
        {
            _logger.LogError(
                "The admin {adminWhoRequested} failed to add a new shift. " +
                "(That schedule doesn't exist)",
                adminWhoRequested);

            throw new ValidationException("That schedule doesn't exist");
        }

        // prevents the creation of a shift in an invalid day
        var enabledDates = await _enabledDateRepository.GetEnabledDatesAsync();
        bool isContainedInADateRange = false;
        foreach (var enabledDate in enabledDates)
        {
            if (shift.Date >= enabledDate.StartDate && shift.Date <= enabledDate.EndDate)
            {
                isContainedInADateRange = true;
                break;
            }
        }

        if (!isContainedInADateRange)
        {
            _logger.LogError(
                "The admin {adminWhoRequested} failed to add a new shift. " +
                "(The day wasn't in a enabled range of dates)",
                adminWhoRequested);
            
            throw new ValidationException("You cannot put a shift in this Day");
        }
    }

    public async Task ValidateDeleteAsync(string adminWhoRequested, int id)
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
    }
}