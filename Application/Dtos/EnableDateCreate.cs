using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class EnableDateCreate
{
    [Required] public DateOnly StartDate { get; set; }
    [Required] public DateOnly EndDate { get; set; }
}