using Claims.Auditing;
using Claims.Models.Enums;
using Claims.Services.AuditerServices;
using Claims.Services.CoverService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly IAuditerServices _auditerServices;
   // private readonly Container _container;
    private readonly ICoverService _coverService;

    public CoversController(AuditContext auditContext, ILogger<CoversController> logger, IAuditerServices auditerServices, ICoverService coverService)
    {
        _logger = logger;
        _auditerServices = auditerServices;
        //_container = cosmosClient?.GetContainer("ClaimDb", "Cover")
        //             ?? throw new ArgumentNullException(nameof(cosmosClient));
        _coverService = coverService;
    }
    
    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverTypeEnum coverType)
    {
        return Ok(ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        return Ok(await _coverService.GetAllAsync());
        //var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
        //var results = new List<Cover>();
        //while (query.HasMoreResults)
        //{
        //    var response = await query.ReadNextAsync();

        //    results.AddRange(response.ToList());
        //}

        //return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        try
        {
            var response = await _coverService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
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
            
            return BadRequest(res.Error);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        //cover.Id = Guid.NewGuid().ToString();
        //cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        //await _container.CreateItemAsync(cover, new PartitionKey(cover.Id));
        //_auditerServices.AuditCover(cover.Id, "POST");
        //return Ok(cover);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        _auditerServices.AuditCover(id, "DELETE");
        return _coverService.DeleteItemAsync(id);
    }

    private decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverTypeEnum coverType)
    {
        var multiplier = 1.3m;
        if (coverType == CoverTypeEnum.Yacht)
        {
            multiplier = 1.1m;
        }

        if (coverType == CoverTypeEnum.PassengerShip)
        {
            multiplier = 1.2m;
        }

        if (coverType == CoverTypeEnum.Tanker)
        {
            multiplier = 1.5m;
        }

        var premiumPerDay = 1250 * multiplier;
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30) totalPremium += premiumPerDay;
            if (i < 180 && coverType == CoverTypeEnum.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            if (i < 365 && coverType != CoverTypeEnum.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
        }

        return totalPremium;
    }
}