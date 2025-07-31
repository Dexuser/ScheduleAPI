using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Services;

public class ScheduleServices  
{
    private readonly IScheduleRepository _repository;
    private readonly ScheduleValidator _validator;

    public ScheduleServices(IScheduleRepository repository, ScheduleValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
    {
        return ScheduleMapper.ToDto(await _repository.GetAllSchedulesAsync());
    }

    public async Task CreateScheduleAsync(ScheduleCreate schedule)
    {
        await _validator.ValidateAsync(schedule);
        await _repository.CreateScheduleAsync(ScheduleMapper.ToEntity(schedule));
    }

    public async Task DeleteScheduleAsync(int id)
    {
        var sch = await _repository.FindByIdAsync(id);
        if (sch == null)
        {
            throw new ValidationException("This Schedule doesn't exist."); 
        }
        
        await _repository.DeleteScheduleAsync(id);
    }

    public async Task UpdateScheduleAsync(int id, ScheduleCreate schedule)
    {
        var sch = await _repository.FindByIdAsync(id);
        if (sch == null)
        {
            throw new ValidationException("This Schedule doesn't exist."); 
        }

        await _validator.ValidateAsync(schedule, id);
        await _repository.UpdateScheduleAsync(id, ScheduleMapper.ToEntity(schedule));
    }
}
