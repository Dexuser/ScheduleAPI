using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class EnabledDateDto
{
    public int? Id { get; set; }
    [Required] public DateOnly StartDate { get; set; }
    [Required] public DateOnly EndDate { get; set; }
}