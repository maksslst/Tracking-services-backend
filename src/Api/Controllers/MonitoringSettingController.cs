using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Mappings;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MonitoringSettingController : ControllerBase
{
    private IMonitoringSettingService _monitoringSettingService;

    public MonitoringSettingController(IMonitoringSettingService monitoringSettingService)
    {
        _monitoringSettingService = monitoringSettingService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] MonitoringSettingDto monitoringSettingDto)
    {
        await _monitoringSettingService.Add(monitoringSettingDto);
        return Created();
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
    public async Task<IActionResult> Delete([FromQuery] int monitoringSettingId)
    {
        var result = await _monitoringSettingService.Delete(monitoringSettingId);
        if (!result)
        {
            return BadRequest("Не удалось удалить настройку");
        }

        return Ok();
    }
    #endregion

    #region HttpGet
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetByServiceId([FromQuery] int serviceId)
    {
        MonitoringSettingDto? monitoringSetting = await _monitoringSettingService.GetByServiceId(serviceId);
        if (monitoringSetting == null)
        {
            return BadRequest("Не удалось найти настройку");
        }

        return Ok(monitoringSetting);
    }
    #endregion
}