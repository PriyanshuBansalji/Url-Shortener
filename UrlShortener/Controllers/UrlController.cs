using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlController : ControllerBase
{
    private readonly IUrlService _urlService;

    public UrlController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    /// <summary>Shorten a URL</summary>
    [HttpPost]
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
    {
        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _urlService.CreateShortUrlAsync(request, baseUrl);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Get all shortened URLs</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var urls = await _urlService.GetAllAsync(baseUrl);
        return Ok(urls);
    }

    /// <summary>Get stats for a single short code</summary>
    [HttpGet("{code}/stats")]
    public async Task<IActionResult> GetStats(string code)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var urls = await _urlService.GetAllAsync(baseUrl);
        var match = urls.FirstOrDefault(u => u.Code == code);

        if (match == null)
            return NotFound(new { error = "Short URL not found." });

        return Ok(match);
    }

    /// <summary>Delete a short URL</summary>
    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        var deleted = await _urlService.DeleteAsync(code);
        if (!deleted)
            return NotFound(new { error = "Short URL not found." });

        return Ok(new { message = "Deleted successfully." });
    }
}
