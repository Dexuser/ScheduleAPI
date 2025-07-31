using System.ComponentModel.DataAnnotations;
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

    public ShiftsController(ShiftServices shiftServices)
    {
        _shiftServices = shiftServices;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableShiftsSinceTodayAsync()
    {
        var shifts = await _shiftServices.GetAvailableShiftsSinceTodayAsync();
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
            await _shiftServices.CreateShiftAsync(shift);
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
            await _shiftServices.DeleteShiftAsync(id);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
            
    }
}