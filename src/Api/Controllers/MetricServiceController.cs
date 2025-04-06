using Application.DTOs.Mappings;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MetricServiceController : ControllerBase
{
    private readonly IMetricService _metricService;

    public MetricServiceController(IMetricService metricService)
    {
        _metricService = metricService;
    }

    #region HttpPost

    [HttpPost]
    public async Task<IActionResult> AddMetric([FromBody] CreateMetricRequest request)
    {
        int metric = await _metricService.AddMetric(request);
        return CreatedAtAction(nameof(GetMetricByResourceId), new { resourceId = metric }, metric);
    }

    #endregion

    #region HttpPut

    [HttpPut]
    public async Task<IActionResult> UpdateMetric([FromBody] UpdateMetricRequest request)
    {
        await _metricService.UpdateMetric(request);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [HttpDelete("{metricId}")]
    public async Task<IActionResult> DeleteMetric(int metricId)
    {
        await _metricService.DeleteMetric(metricId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetMetricByResourceId(int resourceId)
    {
        var metric = await _metricService.GetMetricByResourceId(resourceId);
        return Ok(metric);
    }

    [HttpGet("GetAllMetricServiceId/{serviceId}")]
    public async Task<IActionResult> GetAllMetricServiceId(int serviceId)
    {
        var metrics = await _metricService.GetAllMetricsByServiceId(serviceId);
        return Ok(metrics);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var metrics = await _metricService.GetAll();
        return Ok(metrics);
    }

    #endregion
}