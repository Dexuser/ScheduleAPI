using System.ComponentModel.DataAnnotations;
using ProyectoFinal.Models;

namespace Application.Dtos;

public class AppointmentPatch
{
    [Required] public int ShiftId { get; set; }
    public AppointmentState? State { get; set; }
}