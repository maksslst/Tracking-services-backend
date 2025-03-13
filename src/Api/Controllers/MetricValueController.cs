using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Mappings;
using Application.Services;
using Domain.Entities;

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
    public async Task<IActionResult> AddMetricValue([FromBody] MetricValueDto metricValueDto)
    {
        MetricValue? metricValue = await _metricValueService.AddMetricValue(metricValueDto);
        if (metricValue == null)
        {
            return BadRequest("Не удалось дабавить значение");
        }
        
        return Created(metricValue.Id.ToString(), metricValue);
    }
    #endregion

    #region HttpGet
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetAllMetricValuesServiceId(int serviceId)
    {
        IEnumerable<MetricValueDto?> metricValue = await _metricValueService.GetAllMetricValuesForService(serviceId);
        if (metricValue == null)
        {
            return BadRequest("Не удалось получить собранные заначения метрики");
        } 
        
        return Ok(metricValue);
    }
    #endregion
}