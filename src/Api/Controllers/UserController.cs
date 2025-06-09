using Api.Extensions;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    #region HttPost

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request)
    {
        int user = await userService.Add(request);
        return CreatedAtAction(nameof(GetById), new { userId = user }, user);
    }

    #endregion

    #region HttPut

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        await userService.Update(request);
        return NoContent();
    }

    #endregion

    #region HttDelete

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
        await userService.Delete(userId);
        return NoContent();
    }

    #endregion

    #region HttGet

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(int userId)
    {
        var user = await userService.GetById(userId);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await userService.GetAll();
        return Ok(users);
    }

    [HttpGet("UserInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return NotFound("User not found");
        }
        
        var user = await userService.GetById(userId.Value);
        return Ok(user);
    }

    #endregion
}