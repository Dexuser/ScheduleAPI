using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers;


[ApiController]
[Route("[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly ShiftServices _shiftServices;
    private readonly ILogger<ShiftsController> _logger;

    public ShiftsController(ShiftServices shiftServices, ILogger<ShiftsController> logger)
    {
        _shiftServices = shiftServices;
        _logger = logger;
    }
    
    [HttpGet]
    [Authorize(Roles = "ADMIN,USER")] 
    public async Task<IActionResult> GetAvailableShiftsSinceTodayAsync()
    {
        var user = GetWhoMadeTheRequest();
        var role = User.FindFirstValue(ClaimTypes.Role);
        _logger.LogInformation("the {role} {user} is requesting all the shifts available", role, user);
        
        var shifts = await _shiftServices.GetAvailableShiftsSinceTodayAsync();
        if (shifts.Count() == 0)
        {
            return NotFound();
        }
        return Ok(shifts);
    }

    [HttpGet]
    [Route("OfUser")]
    [Authorize(Roles = "USER")]
    public async Task<IActionResult> GetShiftsWithThisUserAsync()
    {
        var userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)); // taken from the token
        var username = GetWhoMadeTheRequest();
        
        _logger.LogInformation("the user {user} is requesting all the shifts where they have a slot", username);
        
        var shifts = await _shiftServices.GetShiftsWithThisUserAsync(userId);
        if (shifts.Count() == 0)
        {
            return NotFound();
        }
        return Ok(shifts);
        
    }
    
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateShift([FromBody] ShiftCreate shift)
    {
        try
        {
            var admin = GetWhoMadeTheRequest();
            _logger.LogInformation("The admin {admin} is requesting the creation of a new shift", admin);
            await _shiftServices.CreateShiftAsync(admin, shift);
            return Created();

        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
            
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteShift([FromRoute] int id)
    {
        try
        {
            var admin = GetWhoMadeTheRequest();
            _logger.LogInformation("The admin {admin} is requesting the deletion the shift of ID: {id}", admin, id);
            
            await _shiftServices.DeleteShiftAsync(admin, id);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
            
    }
    
    private string? GetWhoMadeTheRequest()
    {
        string? admin = User.FindFirstValue(JwtRegisteredClaimNames.Name); 
        return admin;
    }
}