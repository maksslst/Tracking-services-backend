using Application.DTOs.Mappings;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
        var monitoringSetting = await _monitoringSettingService.Add(monitoringSettingDto);
        if (monitoringSetting == null)
        {
            return BadRequest("Не удалось создать настройку");
        }

        return CreatedAtAction(nameof(GetMonitoringSettingByResourceId), new { resourceId = monitoringSetting.ResourceId }, monitoringSetting);
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
    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetMonitoringSettingByResourceId(int resourceId)
    {
        var monitoringSetting = await _monitoringSettingService.GetMonitoringSetting(resourceId);
        if (monitoringSetting == null)
        {
            return BadRequest("Не удалось найти настройку");
        }

        return Ok(monitoringSetting);
    }
    #endregion
}