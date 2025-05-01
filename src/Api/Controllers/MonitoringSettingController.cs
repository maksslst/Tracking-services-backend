using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MonitoringSettingController(IMonitoringSettingService monitoringSettingService) : ControllerBase
{
    #region HttpPost

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateMonitoringSettingRequest request)
    {
        int monitoringSetting = await monitoringSettingService.Add(request);

        return CreatedAtAction(nameof(GetMonitoringSettingByResourceId), new { resourceId = monitoringSetting },
            monitoringSetting);
    }

    #endregion

    #region HttpPut

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateMonitoringSettingRequest request)
    {
        await monitoringSettingService.Update(request);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [Authorize(Roles = "Admin, Moderator")]
    [HttpDelete("{monitoringSettingId}")]
    public async Task<IActionResult> Delete(int monitoringSettingId)
    {
        await monitoringSettingService.Delete(monitoringSettingId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetMonitoringSettingByResourceId(int resourceId)
    {
        var monitoringSetting = await monitoringSettingService.GetMonitoringSetting(resourceId);

        return Ok(monitoringSetting);
    }

    #endregion
}