using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Validations;

public class EnabledDateValidator
{
    private readonly ILogger<EnabledDateValidator> _logger;
    private readonly IEnabledDateRepository _enabledDateRepository;
    
    public EnabledDateValidator(ILogger<EnabledDateValidator> logger, IEnabledDateRepository enabledDateRepository)
    {
        _logger = logger;
        _enabledDateRepository = enabledDateRepository;
    }

    public async Task ValidateAsync(string adminWhoRequested, EnableDateCreate dateRange)
    {
        if (dateRange.StartDate < DateOnly.FromDateTime(DateTime.Today))
        {
            _logger.LogError(
                "The Admin {adminWhoRequested} failed to add a date range (StartDate cannot be earlier that today)",
                adminWhoRequested
                );
            
            throw new ValidationException("The start date cannot be earlier that today");
        }

        if (dateRange.EndDate < dateRange.StartDate)
        {
            _logger.LogError(
                "The Admin {adminWhoRequested} failed to add a date range (EndDate cannot be earlier that StartDate)",
                adminWhoRequested
            );
            
            throw new ValidationException("EndDate cannot be earlier that StartDate");

        }

        if (await _enabledDateRepository.AlreadyExistsThatDateRange(dateRange.StartDate, dateRange.EndDate))
        {
            _logger.LogError(
                "The Admin {adminWhoRequested} failed to add a date range (That date already exists)",
                adminWhoRequested
            );

            throw new ValidationException("That Date range already exists");
        }
    }
    
    public async Task ValidateDeleteAsync(string adminWhoRequested, int id)
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
    }
}