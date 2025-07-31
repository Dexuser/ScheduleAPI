using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers;

[ApiController]
[Route("[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly ScheduleServices _scheduleServices;

    public SchedulesController(ScheduleServices scheduleServices)
    {
        _scheduleServices = scheduleServices;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllSchedules()
    {
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
            await _scheduleServices.CreateScheduleAsync(schedule);
            return Created();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateSchedule([FromBody] ScheduleCreate schedule, [FromRoute] int id)
    {
        try
        {
            await _scheduleServices.UpdateScheduleAsync(id, schedule);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteSchedule([FromRoute] int id)
    {
        try
        {
            await _scheduleServices.DeleteScheduleAsync(id);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}