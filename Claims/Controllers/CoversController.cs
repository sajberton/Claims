using Claims.Auditing;
using Claims.Models;
using Claims.Models.Enums;
using Claims.Services.CoverService;
using Microsoft.AspNetCore.Mvc;


namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoverService _coverService;

    public CoversController(ILogger<CoversController> logger, ICoverService coverService)
    {
        _logger = logger;
        _coverService = coverService;
    }

    [HttpPost("compute-premium")]
    public async Task<ActionResult<decimal>> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverTypeEnum coverType)
    {
        try
        {
            return Ok(await _coverService.ComputePremiumAsync(startDate, endDate, coverType));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        try
        {
            return Ok(await _coverService.GetAllAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "Error while Get Covers");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        try
        {
            return Ok(await _coverService.GetByIdAsync(id));
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "Error while Get cover request for {id} Id", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        try
        {
            var res = await _coverService.AddItemAsync(cover);
            if (res.IsSuccessful)
                return Ok();
            else
                return BadRequest(res.Error);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        try
        {
            var res = await _coverService.DeleteItemAsync(id);
            if (res.IsSuccessful)
                return Ok();
            else
                return BadRequest(res.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "Error while delete cover request for cover id {id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}