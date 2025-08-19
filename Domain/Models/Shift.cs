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
    public int ScheduleId { get; set; } // fk
    
    // Navigation properties

    public Schedule Schedule { get; set; }
    
    public ICollection<Slot> Slots { get; set; }
    
    // TODO ELIMINAR y CAMBIAR
   // public ICollection<Appointment> Appointments { get; set; }
}