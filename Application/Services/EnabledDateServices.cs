using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class EnabledDateServices
{
    private readonly IEnabledDateRepository _enabledDateRepository;
    private readonly EnabledDateValidator _enabledDateValidator;
    private readonly ILogger<EnabledDateServices> _logger;
    public EnabledDateServices(IEnabledDateRepository enabledDateRepository,
        EnabledDateValidator enabledDateValidator, ILogger<EnabledDateServices> logger)
    {
        _enabledDateRepository = enabledDateRepository;
        _enabledDateValidator = enabledDateValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<EnabledDateDto>> GetEnabledDatesSinceTodayAsync()
    {
        return EnabledDateMapper.ToDto(await _enabledDateRepository.GetEnabledDatesAsync());
    }

    public async Task DeleteEnabledDateAsync(string adminWhoRequested, int id)
    {
        var enabledDate = await _enabledDateRepository.GetEnabledDateByIdAsync(id);
        if (enabledDate == null)
        {
            _logger.LogInformation(
                "The Admin {adminWhoRequested} failed to delete the enabled date range of ID {id}." +
                "(That range doesn't exist)", 
                adminWhoRequested,
                id);

            throw new ValidationException("This date range doesn't exist."); 
        }
        
        await _enabledDateRepository.DeleteEnabledDateAsync(id);
        _logger.LogInformation("The admin {adminWhoRequested} deleted the enabled date range with ID {id}", adminWhoRequested, id);
    }

    public async Task CreateEnabledDateAsync(string adminWhoRequested,EnableDateCreate dateRange)
    {
        await _enabledDateValidator.ValidateAsync(adminWhoRequested, dateRange);
        await _enabledDateRepository.CreateEnabledDateAsync(EnabledDateMapper.ToEntity(dateRange));
        
        _logger.LogInformation(
            "The Admin {adminWhoRequested} provided a new Enabled dates range:" +
            " StartDate: {dateRange.StartDate}, EndDate: {dateRange.EndDate}",
            adminWhoRequested,
            dateRange.StartDate,
            dateRange.EndDate);
    }
}