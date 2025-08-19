using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using ProyectoFinal.Models;

namespace Application.Dtos;

public class AppointmentPatch
{
    [Required] public int SlotId { get; set; }
    public AppointmentState? State { get; set; }
}