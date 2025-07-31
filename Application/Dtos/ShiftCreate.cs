using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class ShiftCreate
{
    [Required] public DateOnly Date { get; set; }
    [Required] public int ServicesSlots { get; set; }
    [Required] public int MeetingDurationOnMinutes { get; set; }
    [Required] public int ScheduleId { get; set; } // fk
}