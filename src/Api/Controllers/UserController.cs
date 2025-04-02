using Application.DTOs.Mappings;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    #region HttPost

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request)
    {
        int user = await _userService.Add(request);
        return CreatedAtAction(nameof(GetById), new { userId = user }, user);
    }

    #endregion

    #region HttPut

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        var result = await _userService.Update(request);
        return Ok(result);
    }

    #endregion

    #region HttDelete

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
        await _userService.Delete(userId);
        return NoContent();
    }

    #endregion

    #region HttGet

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(int userId)
    {
        var user = await _userService.GetById(userId);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAll();
        return Ok(users);
    }

    #endregion
}