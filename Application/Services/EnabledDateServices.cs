using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Services;

public class EnabledDateServices
{
    private readonly IEnabledDateRepository _enabledDateRepository;
    private readonly EnabledDateValidator _enabledDateValidator;

    public EnabledDateServices(IEnabledDateRepository enabledDateRepository,
        EnabledDateValidator enabledDateValidator)
    {
        _enabledDateRepository = enabledDateRepository;
        _enabledDateValidator = enabledDateValidator;
    }

    public async Task<IEnumerable<EnabledDateDto>> GetEnabledDatesSinceTodayAsync()
    {
        return EnabledDateMapper.ToDto(await _enabledDateRepository.GetEnabledDatesAsync());
    }

    public async Task DeleteEnabledDateAsync(int id)
    {
        await _enabledDateRepository.DeleteEnabledDateAsync(id);
    }

    public async Task CreateEnabledDateAsync(EnableDateCreate dateRangue)
    {
        Console.WriteLine(dateRangue);
        _enabledDateValidator.ValidateThatStartDate(dateRangue);
        await _enabledDateRepository.CreateEnabledDateAsync(EnabledDateMapper.ToEntity(dateRangue));
    }
}