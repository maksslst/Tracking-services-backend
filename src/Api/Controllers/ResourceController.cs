using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ResourceController(IResourceService resourceService) : ControllerBase
{
    #region HttpPost

    [Authorize(Roles = "Admin, Moderator")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateResourceRequest request)
    {
        int resource = await resourceService.Add(request);
        return CreatedAtAction(nameof(GetByResourceId), new { resourceId = resource }, resource);
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpPost("{companyId}")]
    public async Task<IActionResult> AddCompanyResource(int companyId, [FromBody] CreateResourceRequest request)
    {
        await resourceService.AddCompanyResource(companyId, request);
        return NoContent();
    }

    #endregion

    #region HttpPut

    [Authorize(Roles = "Admin, Moderator")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateResourceRequest request)
    {
        await resourceService.Update(request);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpPut("{companyId}/{resourceId}")]
    public async Task<IActionResult> UpdateCompanyResource(int companyId, int resourceId,
        [FromBody] UpdateResourceRequest request)
    {
        await resourceService.UpdateCompanyResource(companyId, request, resourceId);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> Delete(int resourceId)
    {
        await resourceService.Delete(resourceId);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpDelete("{resourceId}/{companyId}")]
    public async Task<IActionResult> DeleteCompanyResource(int resourceId, int companyId)
    {
        await resourceService.DeleteCompanyResource(resourceId, companyId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [Authorize(Roles = "Admin, Moderator")]
    [HttpGet("{resourceId}")]
    public async Task<IActionResult> GetByResourceId(int resourceId)
    {
        var service = await resourceService.GetResource(resourceId);
        return Ok(service);
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpGet]
    public async Task<IActionResult> GetAllResources()
    {
        var resources = await resourceService.GetAllResources();
        return Ok(resources);
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpGet("GetCompanyResources/{companyId}")]
    public async Task<IActionResult> GetCompanyResources(int companyId)
    {
        var resources = await resourceService.GetCompanyResources(companyId);
        return Ok(resources);
    }

    #endregion
}