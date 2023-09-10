using Claims.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.CosmosDbServices.CosmosCoverService
{
    public interface ICosmosCoverService
    {
        Task<IEnumerable<Cover>> GetAllAsync();
        Task<Cover> GetByIdAsync(string id);
        Task<bool> AddItemAsync(Cover item);
        Task<bool> DeleteItemAsync(string id);
    }
}
