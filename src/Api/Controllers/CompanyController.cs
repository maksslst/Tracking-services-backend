using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    #region HttpPost

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateCompanyRequest request)
    {
        int company = await _companyService.Add(request);
        return CreatedAtAction(nameof(GetByCompanyId), new { companyId = company }, company);
    }

    [HttpPost("AddUserToCompany/{userId}/{companyId}")]
    public async Task<IActionResult> AddUserToCompany(int userId, int companyId)
    {
        await _companyService.AddUserToCompany(userId, companyId);
        return NoContent();
    }

    #endregion

    #region HttpPut

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCompanyRequest request)
    {
        await _companyService.Update(request);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [HttpDelete("{companyId}")]
    public async Task<IActionResult> Delete(int companyId)
    {
        await _companyService.Delete(companyId);
        return NoContent();
    }

    [HttpDelete("DeleteUserFromCompany/{userId}/{companyId}")]
    public async Task<IActionResult> DeleteUserFromCompany(int userId, int companyId)
    {
        await _companyService.DeleteUserFromCompany(userId, companyId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetByCompanyId(int companyId)
    {
        var company = await _companyService.GetCompany(companyId);
        return Ok(company);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await _companyService.GetAllCompanies();
        return Ok(companies);
    }

    [HttpGet("GetCompanyUsers/{companyId}")]
    public async Task<IActionResult> GetCompanyUsers(int companyId)
    {
        var users = await _companyService.GetCompanyUsers(companyId);
        return Ok(users);
    }

    #endregion
}