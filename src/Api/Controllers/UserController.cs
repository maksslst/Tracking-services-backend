using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Mappings;
using Application.Services;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    #region HttPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] UserDto userDto)
    {
        await _userService.Add(userDto);
        return Created();
    }
    #endregion

    #region HttPut
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserDto userDto)
    {
        var result = await _userService.Update(userDto);
        if (result)
        {
            return Ok(result);
        }

        return BadRequest("Не удалось обновить пользователя");
    }
    #endregion
    
    #region HttDelete
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete([FromQuery]int userId)
    {
        var result = await _userService.Delete(userId);
        if (result)
        {
            return Ok(result);
        }

        return BadRequest("Не удалось удалить пользователя");
    }
    #endregion

    #region HttGet
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById([FromQuery] int userId)
    {
        UserDto? user = await _userService.GetById(userId);
        if (user == null)
        {
            return BadRequest("Такой польователь не найден");
        }

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<UserDto?> users = await _userService.GetAll();
        if (users.Count == 0)
        {
            return BadRequest("Список пользователей не найден");
        }

        return Ok(users);
    }
    #endregion
}