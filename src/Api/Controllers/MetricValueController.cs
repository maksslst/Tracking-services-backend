using Application.DTOs.Mappings;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MetricValueController : ControllerBase
{
    private readonly IMetricValueService _metricValueService;

    public MetricValueController(IMetricValueService metricValueService)
    {
        _metricValueService = metricValueService;
    }

    #region HttpPost

    [HttpPost]
    public async Task<IActionResult> AddMetricValue([FromBody] CreateMetricValueRequest request)
    {
        int metricValue = await _metricValueService.AddMetricValue(request);
        return CreatedAtAction(nameof(GetByMetricValueId), new { metricValueId = metricValue }, metricValue);
    }

    #endregion

    #region HttpGet

    [HttpGet("GetAllMetricValuesByResourceId/{resourceId}")]
    public async Task<IActionResult> GetAllMetricValuesByResourceId(int resourceId)
    {
        var metricValue = await _metricValueService.GetAllMetricValuesForResource(resourceId);
        return Ok(metricValue);
    }

    [HttpGet("{metricValueId}")]
    public async Task<IActionResult> GetByMetricValueId(int metricValueId)
    {
        var metricValue = await _metricValueService.GetMetricValue(metricValueId);
        return Ok(metricValue);
    }

    #endregion
}