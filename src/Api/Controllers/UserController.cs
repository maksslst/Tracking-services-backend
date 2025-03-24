using Application.DTOs.Mappings;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Add([FromBody] UserDto userDto)
    {
        var user = await _userService.Add(userDto);
        if (user == null)
        {
            return BadRequest("Не удалось создать пользователя");
        }

        return CreatedAtAction(nameof(GetById), new { userId = user.Id }, user);
    }
    #endregion

    #region HttPut
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserDto userDto)
    {
        var result = await _userService.Update(userDto);
        if (!result)
        {
            return NotFound("Не удалось обновить пользователя");
        }

        return Ok(result);
    }
    #endregion

    #region HttDelete
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
        if (await _userService.GetById(userId) == null)
        {
            return NotFound("Пользователь не найден");
        }

        var result = await _userService.Delete(userId);
        if (!result)
        {
            return NotFound("Не удалось удалить пользователя");
        }

        return NoContent();
    }
    #endregion

    #region HttGet
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(int userId)
    {
        var user = await _userService.GetById(userId);
        if (user == null)
        {
            return NotFound("Такой польователь не найден");
        }

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAll();
        if (users == null)
        {
            return NotFound("Список пользователей не найден");
        }

        return Ok(users);
    }
    #endregion
}