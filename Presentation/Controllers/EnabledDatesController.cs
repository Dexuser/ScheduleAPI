using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ProyectoFinal.Controllers;

[ApiController]
[Route("[controller]")]
public class EnabledDatesController : ControllerBase
{
    private readonly EnabledDateServices _dateServices;
    private readonly ILogger<EnabledDatesController> _logger;

    public EnabledDatesController(EnabledDateServices dateServices, ILogger<EnabledDatesController> logger)
    {
        _dateServices = dateServices;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetEnabledDates()
    {
        var admin = GetWhoMadeTheRequest();
        _logger.LogInformation("The Admin {Name} is requesting all the enabled dates", admin);
        
        var dates = await _dateServices.GetEnabledDatesSinceTodayAsync();
        if (dates.Count() == 0)
            return NotFound();
        return Ok(dates);
    }


    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateEnabledDate([FromBody] EnableDateCreate dateRange)
    {
        try
        {
            var admin= GetWhoMadeTheRequest();
            _logger.LogInformation("The Admin {admin} is requesting to create a enabled dates range", admin);
            
            await _dateServices.CreateEnabledDateAsync(admin, dateRange);
            return Created();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteEnabledDate(int id)
    {
        try
        {
            var admin= GetWhoMadeTheRequest();
            _logger.LogInformation("The Admin {admin} is requesting to delete a enabled dates range", admin);
            
            await _dateServices.DeleteEnabledDateAsync(admin, id);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    private string? GetWhoMadeTheRequest()
    { 
        string? admin = User.FindFirstValue(JwtRegisteredClaimNames.Name); 
        return admin;
    }
}