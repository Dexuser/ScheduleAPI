using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class UserLogin
{
    [Required] public string UserName { get; set; }

    // I'm going to try to use a Hash in this property.
    [Required] public string Password { get; set; }
}