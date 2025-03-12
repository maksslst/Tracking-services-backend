using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Mappings;
using Application.Services;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MetricServiceController : ControllerBase
{
    private IMetricService _metricService;

    public MetricServiceController(IMetricService metricService)
    {
        _metricService = metricService;
    }

    [HttpPost]
    public async Task<IActionResult> AddMetric([FromBody]MetricDto metricDto)
    {
        await _metricService.AddMetric(metricDto);
        return Created();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMetric([FromBody]MetricDto metricDto)
    {
        var result = await _metricService.UpdateMetric(metricDto);
        if (!result)
        {
            return BadRequest("Не удалось обновить метрику");
        }
        
        return Ok();
    }

    [HttpDelete("{metricId}")]
    public async Task<IActionResult> DeleteMetric([FromQuery]int metricId)
    {
        var result = await _metricService.DeleteMetric(metricId);
        if (!result)
        {
            return BadRequest("Не удалось удалить метрику");
        }
        return Ok();
    }

    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetMetricServiceId([FromQuery]int serviceId)
    {
        MetricDto? metric = await _metricService.GetMetricServiceId(serviceId);
        if (metric == null)
        {
            return BadRequest("Не удалось получить метрику");
        }

        return Ok(metric);
    }

    [HttpGet("GetAllMetricServiceId/{serviceId}")]
    public async Task<IActionResult> GetAllMetricServiceId([FromQuery]int serviceId)
    {
        List<MetricDto?> metrics = await _metricService.GetAllMetricServiceId(serviceId);
        if (metrics.Count == 0)
        {
            return BadRequest("Не удалось получить метрики сервиса");
        }
        
        return Ok(metrics);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<MetricDto?> metrics = await _metricService.GetAll();
        if (metrics.Count == 0)
        {
            return BadRequest("Не удалось получить метрики");
        }
        return Ok(metrics);
    }
}