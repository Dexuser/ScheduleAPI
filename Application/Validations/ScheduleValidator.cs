using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ProyectoFinal.Models;

namespace Application.Validations;

public class ScheduleValidator
{
    private readonly IScheduleRepository _repository;
    private readonly ILogger<ScheduleValidator> _logger;

    public ScheduleValidator(IScheduleRepository repository, ILogger<ScheduleValidator> logger)
    {
        _repository = repository;
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

        var existing = await _repository.FindByStartTimeAndEndTime(schedule.StartTime, schedule.EndTime);

        if (existing != null && existing.Id != existingId)
        {
            _logger.LogError(
                "The admin {adminWhoRequested} failed to add or update a new schedule " +
                "(Schedule already exists with those times.)",
                adminWhoRequested);
            
            throw new ValidationException("Schedule already exists with those times.");
        }
    }

}