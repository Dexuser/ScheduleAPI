using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace ProyectoFinal.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentServices _appointmentServices;
    
    public AppointmentsController(AppointmentServices appointmentServices)
    {
        _appointmentServices = appointmentServices;
    }

    [HttpPost]
    [Authorize(Roles = "USER")]
    public async Task<ActionResult> CreateAppointmentCreate([FromBody] AppointmentCreate create)
    {
        try
        {
            await _appointmentServices.CreateAppointmentAsync(create);
            return Created();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch]
    [Authorize(Roles = "USER")]
    public async Task<ActionResult> UpdateAppointmentState([FromBody] AppointmentPatch patch)
    {

        try
        {
            int userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)); // Trust me.
            await _appointmentServices.UpdateStateAsync(userId, patch);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}