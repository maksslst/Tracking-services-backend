using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    #region HttpPost

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateCompanyRequest request)
    {
        int company = await companyService.Add(request);
        return CreatedAtAction(nameof(GetByCompanyId), new { companyId = company }, company);
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpPost("AddUserToCompany/{userId}/{companyId}")]
    public async Task<IActionResult> AddUserToCompany(int userId, int companyId)
    {
        await companyService.AddUserToCompany(userId, companyId);
        return NoContent();
    }

    #endregion

    #region HttpPut

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCompanyRequest request)
    {
        await companyService.Update(request);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [Authorize(Roles = "Admin")]
    [HttpDelete("{companyId}")]
    public async Task<IActionResult> Delete(int companyId)
    {
        await companyService.Delete(companyId);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpDelete("DeleteUserFromCompany/{userId}/{companyId}")]
    public async Task<IActionResult> DeleteUserFromCompany(int userId, int companyId)
    {
        await companyService.DeleteUserFromCompany(userId, companyId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetByCompanyId(int companyId)
    {
        var company = await companyService.GetCompany(companyId);
        return Ok(company);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await companyService.GetAllCompanies();
        return Ok(companies);
    }

    [HttpGet("GetCompanyUsers/{companyId}")]
    public async Task<IActionResult> GetCompanyUsers(int companyId)
    {
        var users = await companyService.GetCompanyUsers(companyId);
        return Ok(users);
    }

    #endregion
}