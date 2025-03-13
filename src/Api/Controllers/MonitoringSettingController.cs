using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Mappings;
using Domain.Entities;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MonitoringSettingController : ControllerBase
{
    private readonly IMonitoringSettingService _monitoringSettingService;

    public MonitoringSettingController(IMonitoringSettingService monitoringSettingService)
    {
        _monitoringSettingService = monitoringSettingService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] MonitoringSettingDto monitoringSettingDto)
    {
        MonitoringSetting? monitoringSetting =  await _monitoringSettingService.Add(monitoringSettingDto);
        if (monitoringSetting == null)
        {
            return BadRequest("Не удалось создать настройку");
        }
        
        return Created(monitoringSetting.Id.ToString(), monitoringSettingDto);
    }
    #endregion

    #region HttpPut
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] MonitoringSettingDto monitoringSettingDto)
    {
        var result = await _monitoringSettingService.Update(monitoringSettingDto);
        if (!result)
        {
            return BadRequest("Не удалось изменить настройку");
        }

        return Ok();
    }
    #endregion

    #region HttpDelete
    [HttpDelete("{monitoringSettingId}")]
    public async Task<IActionResult> Delete(int monitoringSettingId)
    {
        if (await _monitoringSettingService.GetMonitoringSetting(monitoringSettingId) == null)
        {
            return NotFound("Настройка не найдена");
        }
        
        var result = await _monitoringSettingService.Delete(monitoringSettingId);
        if (!result)
        {
            return BadRequest("Не удалось удалить настройку");
        }

        return NoContent();
    }
    #endregion

    #region HttpGet
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetByServiceId([FromQuery] int serviceId)
    {
        MonitoringSettingDto? monitoringSetting = await _monitoringSettingService.GetMonitoringSetting(serviceId);
        if (monitoringSetting == null)
        {
            return BadRequest("Не удалось найти настройку");
        }

        return Ok(monitoringSetting);
    }
    #endregion
}