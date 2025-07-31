using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models;

// Usuarios
public class User
{
    public int Id { get; set; }

    public string UserName { get; set; }

    // i'm going to try to use a Hash in this property.
    public string Password { get; set; }
    
    public string Email { get; set; }
    
    public Role Role { get; set; }

    // navigation properties
    public ICollection<Appointment> Appointments { get; set; }
}