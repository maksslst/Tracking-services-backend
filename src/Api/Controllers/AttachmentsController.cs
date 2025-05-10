using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AttachmentsController(IAttachmentService attachmentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string category = "attachments")
    {
        if (file.Length == 0)
        {
            return BadRequest("File is required");
        }

        var attachment = await attachmentService.Upload(file, category);
        return Ok(attachment);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var file = await attachmentService.GetMetadata(id);
        if (file == null)
        {
            return NotFound();
        }

        var bytes = await attachmentService.GetFileContent(id);
        return File(bytes, file.ContentType, file.FileName);
    }

    [HttpGet("{id}/meta")]
    public async Task<IActionResult> GetMetadata(int id)
    {
        var attachment = await attachmentService.GetMetadata(id);
        if (attachment == null)
        {
            return NotFound();
        }

        return Ok(attachment);
    }

    [HttpGet("{id}/link")]
    public async Task<IActionResult> GetLink(int id)
    {
        var link = await attachmentService.GetPublicLink(id);
        return Ok(new { url = link });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await attachmentService.Delete(id);
        return Ok(new { message = "Attachment deleted" });
    }
}