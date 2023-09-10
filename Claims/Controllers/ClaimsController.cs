using Claims.Auditing;
using Claims.Services.AuditerServices;
using Claims.Services.ClaimService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using Claims.Models;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {

        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimService _claimService;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimService claimService)
        {
            _logger = logger;
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetAsync()
        {
            try
            {
                return Ok(await _claimService.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error while Get Claims");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Claim claim)
        {
            try
            {
                var res = await _claimService.AddItemAsync(claim);
                if (res.IsSuccessful)
                    return Ok();
                else
                    return BadRequest(res.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error while Create Claim request for {claim}", claim);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            try
            {
                var res = await _claimService.DeleteItemAsync(id);
                if (res.IsSuccessful)
                    return Ok();
                else
                    return BadRequest(res.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error while Delete claim request for {id} id", id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> GetAsync(string id)
        {
            try
            {
                return Ok(await _claimService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Error while Get claim request for {id} Id", id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}