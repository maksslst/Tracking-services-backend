using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MetricValueController(IMetricValueService metricValueService) : ControllerBase
{
    #region HttpPost

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpPost]
    public async Task<IActionResult> AddMetricValue([FromBody] CreateMetricValueRequest request)
    {
        int metricValue = await metricValueService.AddMetricValue(request);
        return CreatedAtAction(nameof(GetByMetricValueId), new { metricValueId = metricValue }, metricValue);
    }

    #endregion

    #region HttpGet

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("GetAllMetricValuesByResourceId/{resourceId}")]
    public async Task<IActionResult> GetAllMetricValuesByResourceId(int resourceId)
    {
        var metricValue = await metricValueService.GetAllMetricValuesForResource(resourceId);
        return Ok(metricValue);
    }

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("{metricValueId}")]
    public async Task<IActionResult> GetByMetricValueId(int metricValueId)
    {
        var metricValue = await metricValueService.GetMetricValue(metricValueId);
        return Ok(metricValue);
    }

    #endregion
}