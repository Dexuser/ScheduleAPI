using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace ProyectoFinal.Models;

// Estas son las citas
public class Appointment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ShiftId { get; set; }
    
    public AppointmentState State { get; set; }

    // navigation properties
    public User User { get; set; }
    public Shift Shift { get; set; }
}