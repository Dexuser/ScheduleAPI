using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class ScheduleCreate
{
    [Required] public TimeOnly StartTime { get; set; }
    [Required] public TimeOnly EndTime { get; set; }
    [Required] public string Description { get; set; }
}