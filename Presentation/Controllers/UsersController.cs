using System.ComponentModel.DataAnnotations;
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
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserServices userServices, ILogger<UsersController> logger)
    {
        _userServices = userServices;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<ActionResult> CreateUser([FromBody] UserCreate user)
    {
        try
        {
            _logger.LogInformation("Someone requested to create (register) a new user");
            // well, if we are in this point, the model received is valid.
            await _userServices.CreateUserAsync(user);
            return Created();
        }
        catch (ValidationException ex)
        {
            return BadRequest( new {message =  ex.Message});
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<ActionResult<Object>> Login([FromBody] UserLogin user)
    {
        try
        {
            _logger.LogInformation("The user: {UserName} requested access to the system", user.UserName);
            string? tokenJWT = await _userServices.LoginAsync(user);
            return Ok(new { token = tokenJWT });
        }
        catch (ValidationException ex)
        {
            return Unauthorized( new {message =  ex.Message});
        }
    }
}