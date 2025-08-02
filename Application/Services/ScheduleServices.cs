using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ProyectoFinal.Models;

namespace Application.Services;

public class ScheduleServices  
{
    private readonly IScheduleRepository _repository;
    private readonly ScheduleValidator _validator;
    private readonly ILogger<ScheduleServices> _logger;

    public ScheduleServices(IScheduleRepository repository, ScheduleValidator validator, ILogger<ScheduleServices> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
    {
        return ScheduleMapper.ToDto(await _repository.GetAllSchedulesAsync());
    }

    public async Task CreateScheduleAsync(string adminWhoRequested, ScheduleCreate schedule)
    {
        await _validator.ValidateAsync(adminWhoRequested, schedule);
        await _repository.CreateScheduleAsync(ScheduleMapper.ToEntity(schedule));

        _logger.LogInformation(
            "The Admin {adminWhoRequested} provided a new schedule: StartTime: {StartTime}, EndDate: {EndTime}",
            adminWhoRequested,
            schedule.StartTime,
            schedule.EndTime);
    }

    public async Task DeleteScheduleAsync(string adminWhoRequested, int scheduleId)
    {
        var sch = await _repository.FindByIdAsync(scheduleId);
        if (sch == null)
        {
            _logger.LogInformation(
                "The Admin {adminWhoRequested} failed to delete the schedule of ID {id}." +
                "(That schedule doesn't exist)", 
                adminWhoRequested,
                scheduleId);

            throw new ValidationException("This Schedule doesn't exist."); 
        }
        
        await _repository.DeleteScheduleAsync(scheduleId);
        
        _logger.LogInformation(
            "The Admin {adminWhoRequested} deleted the schedule of ID: {id}",
            adminWhoRequested,
            scheduleId);
    }

    public async Task UpdateScheduleAsync(string adminWhoRequested, int scheduleId, ScheduleCreate schedule)
    {
        var sch = await _repository.FindByIdAsync(scheduleId);
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
        await _repository.UpdateScheduleAsync(scheduleId, ScheduleMapper.ToEntity(schedule));
        
        _logger.LogInformation(
            "The Admin {adminWhoRequested} updated the schedule of ID {id}." +
            " new StartTime: {StartTime}, new EndTime: {EndTime}",
            adminWhoRequested,
            scheduleId,
            schedule.StartTime,
            schedule.EndTime);
    }
}
