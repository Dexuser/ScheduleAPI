namespace ProyectoFinal.Models;

public class Slot
{
    public int Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int ShiftId { get; set; }
    
    public bool isTaken { get; set; }
    
    public Shift Shift { get; set; }
    public Appointment Appointment { get; set; }
    
}