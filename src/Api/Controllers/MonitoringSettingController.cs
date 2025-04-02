using Application.DTOs.Mappings;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;

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
    public async Task<IActionResult> Add([FromBody] CreateMonitoringSettingRequest request)
    {
        int monitoringSetting = await _monitoringSettingService.Add(request);

        return CreatedAtAction(nameof(GetMonitoringSettingByResourceId), new { resourceId = monitoringSetting },
            monitoringSetting);
    }

    #endregion

    #region HttpPut

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateMonitoringSettingRequest request)
    {
        await _monitoringSettingService.Update(request);
        return Ok();
    }

    #endregion

    #region HttpDelete

    [HttpDelete("{monitoringSettingId}")]
    public async Task<IActionResult> Delete(int monitoringSettingId)
    {
        await _monitoringSettingService.Delete(monitoringSettingId);
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