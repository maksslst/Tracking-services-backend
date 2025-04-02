using Application.DTOs.Mappings;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;

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
    public async Task<IActionResult> Add([FromBody] CreateResourceRequest request)
    {
        int resource = await _resourceService.Add(request);
        return CreatedAtAction(nameof(GetByResourceId), new { resourceId = resource }, resource);
    }

    [HttpPost("{companyId}")]
    public async Task<IActionResult> AddCompanyResource(int companyId, [FromBody] CreateResourceRequest request)
    {
        await _resourceService.AddCompanyResource(companyId, request);
        return Ok();
    }

    #endregion

    #region HttpPut

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateResourceRequest request)
    {
        await _resourceService.Update(request);
        return Ok();
    }

    [HttpPut("{companyId}/{resourceId}")]
    public async Task<IActionResult> UpdateCompanyResource(int companyId, int resourceId,
        [FromBody] UpdateResourceRequest request)
    {
        await _resourceService.UpdateCompanyResource(companyId, request, resourceId);
        return Ok();
    }

    #endregion

    #region HttpDelete

    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> Delete(int resourceId)
    {
        await _resourceService.Delete(resourceId);
        return NoContent();
    }

    [HttpDelete("{resourceId}/{companyId}")]
    public async Task<IActionResult> DeleteCompanyResource(int resourceId, int companyId)
    {
        await _resourceService.DeleteCompanyResource(resourceId, companyId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetByResourceId(int resourceId)
    {
        var service = await _resourceService.GetResource(resourceId);
        return Ok(service);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllResources()
    {
        var resources = await _resourceService.GetAllResources();
        return Ok(resources);
    }

    [HttpGet("GetCompanyResources/{companyId}")]
    public async Task<IActionResult> GetCompanyResources(int companyId)
    {
        var resources = await _resourceService.GetCompanyResources(companyId);
        return Ok(resources);
    }

    #endregion
}