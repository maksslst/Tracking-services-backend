using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MetricServiceController(IMetricService metricService) : ControllerBase
{
    #region HttpPost

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpPost]
    public async Task<IActionResult> AddMetric([FromBody] CreateMetricRequest request)
    {
        int metric = await metricService.AddMetric(request);
        return CreatedAtAction(nameof(GetMetricByResourceId), new { resourceId = metric }, metric);
    }

    #endregion

    #region HttpPut

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpPut]
    public async Task<IActionResult> UpdateMetric([FromBody] UpdateMetricRequest request)
    {
        await metricService.UpdateMetric(request);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpDelete("{metricId}")]
    public async Task<IActionResult> DeleteMetric(int metricId)
    {
        await metricService.DeleteMetric(metricId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetMetricByResourceId(int resourceId)
    {
        var metric = await metricService.GetMetricByResourceId(resourceId);
        return Ok(metric);
    }

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("GetAllMetricResourceId/{resourceId}")]
    public async Task<IActionResult> GetAllMetricResourceId(int resourceId)
    {
        var metrics = await metricService.GetAllMetricsByResourceId(resourceId);
        return Ok(metrics);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var metrics = await metricService.GetAll();
        return Ok(metrics);
    }

    #endregion
}