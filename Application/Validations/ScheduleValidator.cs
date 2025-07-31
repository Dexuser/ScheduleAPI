using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Validations;

public class ScheduleValidator
{
    private readonly IScheduleRepository _repository;

    public ScheduleValidator(IScheduleRepository repository)
    {
        _repository = repository;
    }
    
    public async Task ValidateAsync(ScheduleCreate schedule, int? existingId = null)
    {
        if (schedule.EndTime < schedule.StartTime)
            throw new ValidationException("EndTime cannot be earlier than StartTime");

        var existing = await _repository.FindByStartTimeAndEndTime(schedule.StartTime, schedule.EndTime);

        if (existing != null && existing.Id != existingId)
            throw new ValidationException("Schedule already exists with those times.");
    }

}