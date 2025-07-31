using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models;

// Turnos
public class Shift
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }


    public int ServicesSlots { get; set; }

    public int MeetingDurationOnMinutes { get; set; }

    // Navigation properties

    public int ScheduleId { get; set; } // fk
    public Schedule Schedule { get; set; }

    public ICollection<Appointment> Appointments { get; set; }
}