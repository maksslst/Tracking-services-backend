using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs.Mappings;
using Domain.Entities;

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
    public async Task<IActionResult> Add([FromBody]CompanyDto companyDto)
    {
        Company? company =  await _companyService.Add(companyDto);
        if (company == null)
        {
            return BadRequest("Не удалось создать компанию");
        }
        
        return Created(company.Id.ToString(), company);
    }
    
    [HttpPost("{userId}/{companyId}")]
    public async Task<IActionResult> AddUserToCompany(int userId, int companyId)
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
    public async Task<IActionResult> Delete(int companyId)
    {
        if (await _companyService.GetCompany(companyId) == null)
        {
            return NotFound("Такой компании не найдено");
        }
        
        var result = await _companyService.Delete(companyId);
        if (!result)
        {
            return BadRequest("Не удалось удалить компанию");
        }

        return NoContent();
    }
    
    [HttpDelete("{userId}/{companyId}")]
    public async Task<IActionResult> DeleteUserFromCompany(int userId, int companyId)
    {
        var result = await _companyService.DeleteUserFromCompany(userId, companyId);
        if (!result)
        {
            return BadRequest("Не удалось удалить пользователя из компании");
        }

        return NoContent();
    }
    #endregion

    #region HttpGet
    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetByCompanyId(int companyId)
    {
        var company = await _companyService.GetCompany(companyId);
        if (company == null)
        {
            return BadRequest("Не удалось найти компанию");
        }
        return Ok(company);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCompanies()
    {
        IEnumerable<CompanyDto?> companies = await _companyService.GetAllCompanies();
        if (companies == null)
        {
            return BadRequest("Не удалось получить список компаний");
        }

        return Ok(companies);
    }

    [HttpGet("GetCompanyUsers/{companyId}")]
    public async Task<IActionResult> GetCompanyUsers(int companyId)
    {
        IEnumerable<UserDto?> users = await _companyService.GetCompanyUsers(companyId);
        if (users == null)
        {
            return BadRequest("Не удалось найти список пользователей");
        }
        return Ok(users);
    }
    #endregion
}