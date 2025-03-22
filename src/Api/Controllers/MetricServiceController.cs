using Application.DTOs.Mappings;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> AddMetric([FromBody] MetricDto metricDto)
    {
        Metric? metric = await _metricService.AddMetric(metricDto);
        if (metric == null)
        {
            return BadRequest("Не удалось создать метрику");
        }

        return Created(metricDto.Id.ToString(), metricDto);
    }
    #endregion

    #region HttpPut
    [HttpPut]
    public async Task<IActionResult> UpdateMetric([FromBody] MetricDto metricDto)
    {
        var result = await _metricService.UpdateMetric(metricDto);
        if (!result)
        {
            return BadRequest("Не удалось обновить метрику");
        }

        return Ok();
    }
    #endregion

    #region HttpDelete
    [HttpDelete("{metricId}")]
    public async Task<IActionResult> DeleteMetric(int metricId)
    {
        if (await _metricService.GetMetricByServiceId(metricId) == null)
        {
            return NotFound("Метрика не найдена");
        }

        var result = await _metricService.DeleteMetric(metricId);
        if (!result)
        {
            return BadRequest("Не удалось удалить метрику");
        }
        return NoContent();
    }
    #endregion

    #region HttpGet
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetMetricServiceId(int serviceId)
    {
        MetricDto? metric = await _metricService.GetMetricByServiceId(serviceId);
        if (metric == null)
        {
            return BadRequest("Не удалось получить метрику");
        }

        return Ok(metric);
    }

    [HttpGet("GetAllMetricServiceId/{serviceId}")]
    public async Task<IActionResult> GetAllMetricServiceId(int serviceId)
    {
        IEnumerable<MetricDto?> metrics = await _metricService.GetAllMetricsByServiceId(serviceId);
        if (metrics == null)
        {
            return BadRequest("Не удалось получить метрики сервиса");
        }

        return Ok(metrics);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<MetricDto?> metrics = await _metricService.GetAll();
        if (metrics == null)
        {
            return BadRequest("Не удалось получить метрики");
        }
        return Ok(metrics);
    }
    #endregion
}