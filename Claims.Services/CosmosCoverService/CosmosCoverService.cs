using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services
{
    public class CosmosCoverService
    {
        private readonly Container _container;

        public CosmosCoverService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Cover>> GetClaimsAsync()
        {
            var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<Cover>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<Cover> GetClaimAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Cover>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddItemAsync(Cover item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public Task DeleteItemAsync(string id)
        {
            return _container.DeleteItemAsync<Cover>(id, new PartitionKey(id));
        }
    }
}
