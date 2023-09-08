using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Claims.Models;

namespace Claims.Services.CosmosDBService
{
    public interface ICosmosDBService<T> where T : ModelBase
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);

        Task<bool> AddItemAsync(T entity);
        Task DeleteItemAsync(string id);
    };
}
