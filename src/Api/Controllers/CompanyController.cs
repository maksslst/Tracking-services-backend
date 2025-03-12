using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs.Mappings;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    private ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody]CompanyDto companyDto)
    {
        await _companyService.Add(companyDto);
        return Created();
    }
    
    [HttpPost("{userId}/{companyId}")]
    public async Task<IActionResult> AddUserToCompany([FromQuery]int userId, int companyId)
    {
        var result = await _companyService.AddUserToCompany(userId, companyId);
        if (!result)
        {
            return BadRequest("Не удалось добавить пользователя в компанию");
        }

        return Ok();
    }
    #endregion

    #region HttpPut
    [HttpPut]
    public async Task<IActionResult> Update([FromBody]CompanyDto companyDto)
    {
        var result = await _companyService.Update(companyDto);
        if (!result)
        {
            return BadRequest("Не удалось обновить компанию");
        }
        
        return Ok();
    }
    #endregion

    #region HttpDelete
    [HttpDelete("{companyId}")]
    public async Task<IActionResult> Delete([FromQuery]int companyId)
    {
        var result = await _companyService.Delete(companyId);
        if (!result)
        {
            return BadRequest("Не удалось удалить компанию");
        }

        return Ok();
    }
    
    [HttpDelete("{userId}/{companyId}")]
    public async Task<IActionResult> DeleteUserFromCompany([FromQuery]int userId, int companyId)
    {
        var result = await _companyService.DeleteUserFromCompany(userId, companyId);
        if (!result)
        {
            return BadRequest("Не удалось удалить пользователя из компании");
        }

        return Ok();
    }
    #endregion

    #region HttpGet
    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetByCompanyId([FromQuery] int companyId)
    {
        var company = await _companyService.GetByCompanyId(companyId);
        if (company == null)
        {
            return BadRequest("Не удалось найти компанию");
        }
        return Ok(company);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompany()
    {
        List<CompanyDto?> companies = await _companyService.GetAllCompany();
        if (companies.Count == 0)
        {
            return BadRequest("Не удалось получить список компаний");
        }

        return Ok(companies);
    }

    [HttpGet("GetCompanyUsers/{companyId}")]
    public async Task<IActionResult> GetCompanyUsers([FromQuery]int companyId)
    {
        List<UserDto?> users = await _companyService.GetCompanyUsers(companyId);
        if (users.Count == 0)
        {
            return BadRequest("Не удалось найти список пользователей");
        }
        return Ok(users);
    }
    #endregion
}