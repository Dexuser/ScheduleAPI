using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers;

[ApiController]
[Route("[controller]")]
public class EnabledDatesController : ControllerBase
{
    private readonly EnabledDateServices _dateServices;

    public EnabledDatesController(EnabledDateServices dateServices)
    {
        _dateServices = dateServices;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetEnabledDates()
    {
        var dates = await _dateServices.GetEnabledDatesSinceTodayAsync();
        if (dates.Count() == 0) return NotFound();
        return Ok(dates);
    }


    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateEnabledDate([FromBody] EnableDateCreate dateRangue)
    {
        try
        {
            await _dateServices.CreateEnabledDateAsync(dateRangue);
            return Created();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteEnabledDate(int id)
    {
        try
        {
            await _dateServices.DeleteEnabledDateAsync(id);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}