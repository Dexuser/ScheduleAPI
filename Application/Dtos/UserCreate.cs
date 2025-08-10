using System.ComponentModel.DataAnnotations;
namespace Application.Dtos;

public class UserCreate
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Email { get; set; }
}