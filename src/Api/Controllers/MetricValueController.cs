using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Mappings;
using Application.Services;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MetricValueController : ControllerBase
{
    private IMetricValueService _metricValueService;

    public MetricValueController(IMetricValueService metricValueService)
    {
        _metricValueService = metricValueService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> AddMetricValue([FromBody] MetricValueDto metricValueDto)
    {
        await _metricValueService.AddMetricValue(metricValueDto);
        return Created();
    }
    #endregion

    #region HttpGet
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetAllMetricValuesServiceId([FromQuery]int serviceId)
    {
        List<MetricValueDto?> metricValue = await _metricValueService.GetAllMetricValuesServiceId(serviceId);
        if (metricValue.Count == 0)
        {
            return BadRequest("Не удалось получить собранные заначения метрики");
        } 
        
        return Ok(metricValue);
    }
    #endregion
}