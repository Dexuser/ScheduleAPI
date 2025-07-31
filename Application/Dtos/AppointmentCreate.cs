using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class AppointmentCreate
{
    [Required] public int UserId { get; set; }
    [Required] public int ShiftId { get; set; }
}