using Claims.Auditing;
using Claims.Services.AuditerServices;
using Claims.Services.ClaimService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {

        private readonly ILogger<ClaimsController> _logger;
        private readonly IAuditerServices _auditerServices;
        private readonly IClaimService _claimService;

        public ClaimsController(ILogger<ClaimsController> logger, AuditContext auditContext, IAuditerServices auditerServices, IClaimService claimService)
        {
            _logger = logger;
            _auditerServices = auditerServices;
            _claimService = claimService;
        }

        [HttpGet]
        public Task<IEnumerable<Claim>> GetAsync()
        {
            return _claimService.GetAllAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
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
                return BadRequest(ex.Message);
            }
            //claim.Id = Guid.NewGuid().ToString();
            //await _cosmosDbService.AddItemAsync(claim);
            //_auditerServices.AuditClaim(claim.Id, "POST");
            //return Ok(claim);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id)
        {
            _auditerServices.AuditClaim(id, "DELETE");
            return _claimService.DeleteItemAsync(id);
        }

        [HttpGet("{id}")]
        public Task<Claim> GetAsync(string id)
        {
            return _claimService.GetByIdAsync(id);
        }
    }

    //public class CosmosDbService
    //{
    //    private readonly Container _container;

    //    public CosmosDbService(CosmosClient dbClient,
    //        string databaseName,
    //        string containerName)
    //    {
    //        if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
    //        _container = dbClient.GetContainer(databaseName, containerName);
    //    }

    //    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    //    {
    //        var query = _container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
    //        var results = new List<Claim>();
    //        while (query.HasMoreResults)
    //        {
    //            var response = await query.ReadNextAsync();

    //            results.AddRange(response.ToList());
    //        }
    //        return results;
    //    }

    //    public async Task<Claim> GetClaimAsync(string id)
    //    {
    //        try
    //        {
    //            var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
    //            return response.Resource;
    //        }
    //        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    //        {
    //            return null;
    //        }
    //    }

    //    public Task AddItemAsync(Claim item)
    //    {
    //        return _container.CreateItemAsync(item, new PartitionKey(item.Id));
    //    }

    //    public Task DeleteItemAsync(string id)
    //    {
    //        return _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
    //    }
    //}
}