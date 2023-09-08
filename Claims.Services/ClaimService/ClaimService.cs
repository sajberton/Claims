using Claims.Services.CosmosDBService;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Claims.Models;
using Claims.Services.CoverService;

namespace Claims.Services.ClaimService
{
    public class ClaimService : IClaimService
    {
        private readonly ICosmosDBService<Claim> _cosmosDBService;
        //private readonly ICoverService coverService;

        public ClaimService(ICosmosDBService<Claim> cosmosDBService)
        {
            _cosmosDBService = cosmosDBService;
        }
        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _cosmosDBService.GetAllAsync();
           // var query = _cosmosDBService.GetAllAsync<Claim>(new QueryDefinition("SELECT * FROM c"));

            //var results = new List<Claim>();
            //while (query.HasMoreResults)
            //{
            //    var response = await query.ReadNextAsync();

            //    results.AddRange(response.ToList());
            //}
            //return results;
        }

        public async Task<Claim> GetByIdAsync(string id)
        {
            try
            {
                return await _cosmosDBService.GetByIdAsync(id);
                //var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
                //return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddItemAsync(Claim item)
        {
             await _cosmosDBService.AddItemAsync(item);
        }

        public async Task DeleteItemAsync(string id)
        {
            await _cosmosDBService.DeleteItemAsync(id);
        }
    }
}
