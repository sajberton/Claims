using Azure;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services
{
    public class CosmosClaimService
    {
        private readonly Container _container;

        public CosmosClaimService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            var query = _container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<Claim>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<Claim> GetClaimAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddItemAsync(Claim item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var response = await _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
            if (response.StatusCode == System.Net.HttpStatusCode.OK
               || response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return true;
            }
            return false;
        }
    }
}
