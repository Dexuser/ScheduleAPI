using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ScheduleServices  
{
    private readonly IScheduleRepository _ScheduleRepository;
    private readonly ScheduleValidator _validator;
    private readonly ILogger<ScheduleServices> _logger;

    public ScheduleServices(IScheduleRepository repository, ScheduleValidator validator, ILogger<ScheduleServices> logger)
    {
        _ScheduleRepository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
    {
        return ScheduleMapper.ToDto(await _ScheduleRepository.GetAllSchedulesAsync());
    }

    public async Task CreateScheduleAsync(string adminWhoRequested, ScheduleCreate schedule)
    {
        await _validator.ValidateAsync(adminWhoRequested, schedule);
        await _ScheduleRepository.CreateScheduleAsync(ScheduleMapper.ToEntity(schedule));

        _logger.LogInformation(
            "The Admin {adminWhoRequested} provided a new schedule: StartTime: {StartTime}, EndDate: {EndTime}",
            adminWhoRequested,
            schedule.StartTime,
            schedule.EndTime);
    }

    public async Task DeleteScheduleAsync(string adminWhoRequested, int scheduleId)
    {
        await _validator.ValidateDeleteAsync(adminWhoRequested, scheduleId);
        await _ScheduleRepository.DeleteScheduleAsync(scheduleId);
        
        _logger.LogInformation(
            "The Admin {adminWhoRequested} deleted the schedule of ID: {id}",
            adminWhoRequested,
            scheduleId);
    }

    public async Task UpdateScheduleAsync(string adminWhoRequested, int scheduleId, ScheduleCreate schedule)
    {
        var sch = await _ScheduleRepository.FindByIdAsync(scheduleId);
        if (sch == null)
        {
            _logger.LogInformation(
                "The Admin {adminWhoRequested} failed to update the schedule of ID {id}. " +
                "(That schedule doesn't exist)", 
                adminWhoRequested,
                scheduleId);
            
            throw new ValidationException("This Schedule doesn't exist."); 
        }

        await _validator.ValidateAsync(adminWhoRequested, schedule, scheduleId);
        await _ScheduleRepository.UpdateScheduleAsync(scheduleId, ScheduleMapper.ToEntity(schedule));
        
        _logger.LogInformation(
            "The Admin {adminWhoRequested} updated the schedule of ID {id}." +
            " new StartTime: {StartTime}, new EndTime: {EndTime}",
            adminWhoRequested,
            scheduleId,
            schedule.StartTime,
            schedule.EndTime);
    }
}
