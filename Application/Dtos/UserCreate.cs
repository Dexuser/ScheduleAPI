using System.ComponentModel.DataAnnotations;
using ProyectoFinal.Models;

namespace Application.Dtos;

public class UserCreate
{
    [Required] public string UserName { get; set; }

    // I'm going to try to use a Hash in this property.
    [Required] public string Password { get; set; }
    
    [Required] public string Email { get; set; }

    //[EnumDataType(typeof(Role))] public Role Role { get; set; }
}