using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Validations;

public class ScheduleValidator
{
    private readonly IScheduleRepository _ScheduleRepository;
    private readonly ILogger<ScheduleValidator> _logger;

    public ScheduleValidator(IScheduleRepository repository, ILogger<ScheduleValidator> logger)
    {
        _ScheduleRepository = repository;
        _logger = logger;
    }
    
    public async Task ValidateAsync(string adminWhoRequested, ScheduleCreate schedule, int? existingId = null)
    {
        if (schedule.EndTime < schedule.StartTime)
        {
            _logger.LogError(
                "The admin {adminWhoRequested} failed to add or update a new schedule " +
                "(Endtime cannot be earlier than StartTime)",
                adminWhoRequested);
            
            throw new ValidationException("EndTime cannot be earlier than StartTime");
        }

        var existing = await _ScheduleRepository.FindByStartTimeAndEndTime(schedule.StartTime, schedule.EndTime);

        if (existing != null && existing.Id != existingId)
        {
            _logger.LogError(
                "The admin {adminWhoRequested} failed to add or update a new schedule " +
                "(Schedule already exists with those times.)",
                adminWhoRequested);
            
            throw new ValidationException("Schedule already exists with those times.");
        }
    }


    public async Task ValidateDeleteAsync(string adminWhoRequested, int scheduleId)
    {
        var sch = await _ScheduleRepository.FindByIdAsync(scheduleId);
        if (sch == null)
        {
            _logger.LogInformation(
                "The Admin {adminWhoRequested} failed to delete the schedule of ID {id}." +
                "(That schedule doesn't exist)", 
                adminWhoRequested,
                scheduleId);

            throw new ValidationException("This Schedule doesn't exist."); 
        }

        if (await _ScheduleRepository.IsThisScheduledUsed(scheduleId))
        {
            _logger.LogInformation(
                "The Admin {adminWhoRequested} failed to delete the schedule of ID {id}. " +
                "(That schedule is being used by a shift)", 
                adminWhoRequested,
                scheduleId);
            
            throw new ValidationException("This Schedule is being used by a shift.");
        }

    }

}