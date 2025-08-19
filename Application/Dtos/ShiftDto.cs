using System.ComponentModel.DataAnnotations;
using ProyectoFinal.Models;

namespace Application.Dtos;

public class ShiftDto
{
    [Required] public int Id { get; set; }
    [Required] public DateOnly Date { get; set; }
    [Required] public int ServicesSlots { get; set; }
    [Required] public int MeetingDurationOnMinutes { get; set; }
    [Required] public ScheduleDto Schedule { get; set; }
    
    [Required] public IEnumerable<SlotDto> Slots { get; set; }
}