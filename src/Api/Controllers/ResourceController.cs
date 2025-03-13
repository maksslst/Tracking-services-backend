using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs.Mappings;
using Domain.Entities;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ResourceController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ResourceDto resourceDto)
    {
        Resource? service = await _resourceService.Add(resourceDto);
        if (service == null)
        {
            return BadRequest("Не удалось добавить сервис");
        }
        
        return Created(resourceDto.Id.ToString(), service);
    }

    [HttpPost("{companyId}/{serviceId}")]
    public async Task<IActionResult> AddCompanyService([FromBody] ResourceDto? serviceDto,int companyId)
    {
        var result = await _resourceService.AddCompanyService(companyId, serviceDto);
        if (!result)
        {
            return BadRequest("Не удалось добавить сервис компании");
        }

        return Ok();
    }
    #endregion

    #region HttpPut
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ResourceDto resourceDto)
    {
        var result = await _resourceService.Update(resourceDto);
        if (!result)
        {
            return BadRequest("Не удалось обновить сервис");
        }

        return Ok();
    }
    
    [HttpPut("{companyId}/{serviceUpdateId}")]
    public async Task<IActionResult> UpdateCompanyService([FromBody] ResourceDto resourceDto, int companyId, int serviceUpdateId)
    {
        var result = await _resourceService.UpdateCompanyService(companyId, resourceDto, serviceUpdateId);
        if (!result)
        {
            return BadRequest("Не удалось изменить сервис");
        }

        return Ok();
    }
    #endregion
    
    #region HttpDelete
    [HttpDelete("{serviceId}")]
    public async Task<IActionResult> Delete(int serviceId)
    {
        if (await _resourceService.GetService(serviceId) == null)
        {
            return NotFound("Сервис не найден");
        }
        
        var result = await _resourceService.Delete(serviceId);
        if (!result)
        {
            return BadRequest("Не удалось удалить сервис");
        }

        return NoContent();
    }
    
    [HttpDelete("{serviceId}/{companyId}")]
    public async Task<IActionResult> DeleteCompanyService(int serviceId, int companyId)
    {
        var result = await _resourceService.DeleteCompanyService(serviceId, companyId);
        if (!result)
        {
            return BadRequest("Не удалось удалить сервис");
        }

        return NoContent();
    }
    #endregion

    #region HttpGet
    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetByServiceId(int serviceId)
    {
        ResourceDto? service = await _resourceService.GetService(serviceId);
        if (service == null)
        {
            return BadRequest("Не удалось найти сервис");
        }

        return Ok(service);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        IEnumerable<ResourceDto?> services = await _resourceService.GetAllServices();
        if (services == null)
        {
            return BadRequest("Не удалось получить список сервисов");
        }

        return Ok(services);
    }

    [HttpGet("GetCompanyServices/{companyId}")]
    public async Task<IActionResult> GetCompanyServices(int companyId)
    {
        IEnumerable<ResourceDto?> services = await _resourceService.GetCompanyServices(companyId);
        if (services == null)
        {
            return BadRequest("Не удалось получить список сервисов компании");
        }

        return Ok(services);
    }
    #endregion
}