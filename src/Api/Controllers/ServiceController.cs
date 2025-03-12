using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs.Mappings;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceController : ControllerBase
{
    private IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ServiceDto serviceDto)
    {
        await _serviceService.Add(serviceDto);
        return Created();
    }

    [HttpPost("{companyId}/{serviceId}")]
    public async Task<IActionResult> AddCompanyService([FromBody] ServiceDto? serviceDto,int companyId, int serviceId = -1)
    {
        var result = await _serviceService.AddCompanyService(serviceId, companyId, serviceDto);
        if (!result)
        {
            return BadRequest("Не удалось добавить сервис компании");
        }

        return Ok();
    }
    #endregion

    #region HttpPut
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ServiceDto serviceDto)
    {
        var result = await _serviceService.Update(serviceDto);
        if (!result)
        {
            return BadRequest("Не удалось обновить сервис");
        }

        return Ok();
    }
    
    [HttpPut("{companyId}/{serviceUpdateId}")]
    public async Task<IActionResult> UpdateCompanyService([FromBody] ServiceDto serviceDto, int companyId, int serviceUpdateId)
    {
        var result = await _serviceService.UpdateCompanyService(companyId, serviceDto, serviceUpdateId);
        if (!result)
        {
            return BadRequest("Не удалось изменить сервис");
        }

        return Ok();
    }
    #endregion
    
    #region HttpDelete
    [HttpDelete("{serviceId}")]
    public async Task<IActionResult> Delete([FromQuery]int serviceId)
    {
        var result = await _serviceService.Delete(serviceId);
        if (!result)
        {
            return BadRequest("Не удалось удалить сервис");
        }

        return Ok();
    }
    
    [HttpDelete("{serviceId}/{companyId}")]
    public async Task<IActionResult> DeleteCompanyService([FromQuery]int serviceId, int companyId)
    {
        var result = await _serviceService.DeleteCompanyService(serviceId, companyId);
        if (!result)
        {
            return BadRequest("Не удалось удалить сервис");
        }

        return Ok();
    }
    #endregion

    #region HttpGet
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetByServiceId([FromQuery]int serviceId)
    {
        ServiceDto? service = await _serviceService.GetByServiceId(serviceId);
        if (service == null)
        {
            return BadRequest("Не удалось найти сервис");
        }

        return Ok(service);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        List<ServiceDto?> services = await _serviceService.GetAllServices();
        if (services.Count == 0)
        {
            return BadRequest("Не удалось получить список сервисов");
        }

        return Ok(services);
    }

    [HttpGet("GetCompanyServices/{companyId}")]
    public async Task<IActionResult> GetCompanyServices([FromQuery]int companyId)
    {
        List<ServiceDto?> services = await _serviceService.GetCompanyServices(companyId);
        if (services.Count == 0)
        {
            return BadRequest("Не удалось получить список сервисов компании");
        }

        return Ok(services);
    }
    #endregion
}