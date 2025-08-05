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
public class SchedulesController : ControllerBase
{
    private readonly ScheduleServices _scheduleServices;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(ScheduleServices scheduleServices, ILogger<SchedulesController> logger)
    {
        _scheduleServices = scheduleServices;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllSchedules()
    {

        var admin = GetWhoMadeTheRequest();
        _logger.LogInformation("The Admin {Name} is requesting all the schedules", admin);       
        
        var schedules = await _scheduleServices.GetAllSchedulesAsync();
        if (schedules.Count() == 0)
            return NotFound();
        return Ok(schedules);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateSchedule([FromBody] ScheduleCreate schedule)
    {
        try
        {
            var admin = GetWhoMadeTheRequest();
            _logger.LogInformation("The Admin {Name} is requesting to create a new schedule", admin);
            
            await _scheduleServices.CreateScheduleAsync(admin, schedule);
            return Created();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }



    [HttpPut("{scheduleId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateSchedule([FromBody] ScheduleCreate schedule, [FromRoute] int scheduleId)
    {
        try
        {
            var admin= GetWhoMadeTheRequest();
            _logger.LogInformation("The Admin {Name} is requesting to update the schedule of ID: {scheduleId}", admin, scheduleId);
            await _scheduleServices.UpdateScheduleAsync(admin, scheduleId, schedule);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{scheduleId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteSchedule([FromRoute] int scheduleId)
    {
        try
        {
            var admin = GetWhoMadeTheRequest();
            _logger.LogInformation("The Admin {Name} is requesting to delete the schedule of ID: {scheduleId}", admin, scheduleId);
            await _scheduleServices.DeleteScheduleAsync(admin, scheduleId);
            return NoContent();
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