using Azure;
using Claims.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.CosmosDbServices.CosmosCoverService
{
    public class CosmosCoverService : ICosmosCoverService
    {
        private readonly Container _container;

        public CosmosCoverService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
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

        public async Task<Cover> GetByIdAsync(string id)
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

        public async Task<bool> AddItemAsync(Cover item)
        {
            var response = await _container.CreateItemAsync(item, new PartitionKey(item.Id));
            if (response.StatusCode == System.Net.HttpStatusCode.OK
               || response.StatusCode == System.Net.HttpStatusCode.Accepted
               || response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var response = await _container.DeleteItemAsync<Cover>(id, new PartitionKey(id));
            if (response.StatusCode == System.Net.HttpStatusCode.OK
               || response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return true;
            }
            return false;
        }
    }
}
