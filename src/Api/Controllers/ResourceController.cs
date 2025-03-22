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
        int? resourceId = await _resourceService.Add(resourceDto);
        if (resourceId == null)
        {
            return BadRequest("Не удалось добавить сервис");
        }
        
        return Created(resourceId.ToString(), resourceId);
    }

    [HttpPost("{companyId}")]
    public async Task<IActionResult> AddCompanyResource([FromBody] ResourceDto? resourceDto, int companyId)
    {
        var result = await _resourceService.AddCompanyResource(companyId, resourceDto);
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
    
    [HttpPut("{companyId}/{resourceUpdateId}")]
    public async Task<IActionResult> UpdateCompanyResource([FromBody] ResourceDto resourceDto, int companyId, int resourceUpdateId)
    {
        var result = await _resourceService.UpdateCompanyResource(companyId, resourceDto, resourceUpdateId);
        if (!result)
        {
            return BadRequest("Не удалось изменить сервис");
        }

        return Ok();
    }
    #endregion
    
    #region HttpDelete
    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> Delete(int resourceId)
    {
        if (await _resourceService.GetResource(resourceId) == null)
        {
            return NotFound("Сервис не найден");
        }
        
        var result = await _resourceService.Delete(resourceId);
        if (!result)
        {
            return BadRequest("Не удалось удалить сервис");
        }

        return NoContent();
    }
    
    [HttpDelete("{resourceId}/{companyId}")]
    public async Task<IActionResult> DeleteCompanyResource(int resourceId, int companyId)
    {
        var result = await _resourceService.DeleteCompanyResource(resourceId, companyId);
        if (!result)
        {
            return BadRequest("Не удалось удалить сервис");
        }

        return NoContent();
    }
    #endregion

    #region HttpGet
    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetByResourceId(int resourceId)
    {
        ResourceDto? service = await _resourceService.GetResource(resourceId);
        if (service == null)
        {
            return BadRequest("Не удалось найти сервис");
        }

        return Ok(service);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllResources()
    {
        IEnumerable<ResourceDto?> resources = await _resourceService.GetAllResources();
        if (resources == null)
        {
            return BadRequest("Не удалось получить список сервисов");
        }

        return Ok(resources);
    }

    [HttpGet("GetCompanyResources/{companyId}")]
    public async Task<IActionResult> GetCompanyResources(int companyId)
    {
        IEnumerable<ResourceDto?> resources = await _resourceService.GetCompanyResources(companyId);
        if (resources == null)
        {
            return BadRequest("Не удалось получить список сервисов компании");
        }

        return Ok(resources);
    }
    #endregion
}