using System.ComponentModel.DataAnnotations;
using Application.Dtos;

namespace Application.Validations;

public class EnabledDateValidator
{
    public void ValidateThatStartDate(EnableDateCreate dateRangue)
    {
        if (dateRangue.StartDate < DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException("The start date cannot be earlier that today");
    }
}