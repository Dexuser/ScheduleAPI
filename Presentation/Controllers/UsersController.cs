using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserServices _userServices;

    public UsersController(UserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreate user)
    {
        try
        {
            // well, if we are in this point, the model received is valid.
            await _userServices.CreateUserAsync(user);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLogin user)
    {
        try
        {
            string? token = await _userServices.LoginAsync(user);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}