using Claims.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services.CosmosDBService
{
    public class CosmosDBService<T> : ICosmosDBService<T> where T : ModelBase
    {
        private readonly Container _container;

        public CosmosDBService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        //public async Task<IEnumerable<Claim>> GetClaimsAsync()
        //{
        //    var query = _container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
        //    var results = new List<Claim>();
        //    while (query.HasMoreResults)
        //    {
        //        var response = await query.ReadNextAsync();

        //        results.AddRange(response.ToList());
        //    }
        //    return results;
        //}

        //public async Task<Claim> GetClaimAsync(string id)
        //{
        //    try
        //    {
        //        var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
        //        return response.Resource;
        //    }
        //    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        return null;
        //    }
        //}

        //public Task AddItemAsync(Claim item)
        //{
        //    return _container.CreateItemAsync(item, new PartitionKey(item.Id));
        //}

        //public Task DeleteItemAsync(string id)
        //{
        //    return _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
        //}

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<bool> AddItemAsync(T item)
        {
            var response = await _container.CreateItemAsync(item, new PartitionKey(item.Id));
            if(response.StatusCode == System.Net.HttpStatusCode.OK 
                || response.StatusCode == System.Net.HttpStatusCode.Accepted) 
            { 
                return true;
            }
            return false;
        }

        public Task DeleteItemAsync(string id)
        {
            return _container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }
    }
}
