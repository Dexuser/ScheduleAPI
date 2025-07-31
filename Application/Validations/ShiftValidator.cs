using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Validations;

public class ShiftValidator
{
    private readonly IEnabledDateRepository _enabledDateRepository;
    
    public ShiftValidator(IEnabledDateRepository enabledDateRepository)
    {
        _enabledDateRepository = enabledDateRepository;
    }

    public async Task ValidateAsync(ShiftCreate shift)
    {
        if (shift.Date < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ValidationException("The date of the shift cannot be in the past");
        }

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
            throw new ValidationException("You cannot put a shift in this Day");
        }
    }
}