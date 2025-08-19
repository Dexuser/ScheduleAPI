using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace ProyectoFinal.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentServices _appointmentServices;
    private readonly ILogger<AppointmentsController> _logger;
    
    public AppointmentsController(AppointmentServices appointmentServices, ILogger<AppointmentsController> logger)
    {
        _appointmentServices = appointmentServices;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "USER")]
    public async Task<ActionResult> CreateAppointmentCreate([FromBody] AppointmentCreate create)
    {
        int userId = Int32.Parse( User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        if (userId != create.UserId)
        {
            string text = "The userId in the header doesn't match the userId on the body";
            return BadRequest(new {message = text});
        }
        
        try
        {
            var user = GetWhoMadeTheRequest();
            _logger.LogInformation("The user {user} is requesting the creation of a new appointment", user);
            
            await _appointmentServices.CreateAppointmentAsync(user, create);
            return Created();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch]
    [Authorize(Roles = "USER")]
    public async Task<ActionResult> UpdateAppointmentState([FromBody] AppointmentPatch patch)
    {

        try
        {
            int userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)); // Trust me.
            var userName = GetWhoMadeTheRequest();
            _logger.LogInformation(
                "The user {user} is requesting the update of the state of their appointment associated " +
                "to the shift of ID {patch.ShiftId}", userName, patch.SlotId);
            
            await _appointmentServices.UpdateStateAsync(userName, userId, patch);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    private string? GetWhoMadeTheRequest()
    {
        string? user = User.FindFirstValue(JwtRegisteredClaimNames.Name); 
        return user;
    }
    
}