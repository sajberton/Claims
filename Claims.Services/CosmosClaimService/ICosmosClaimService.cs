using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services
{
    public interface ICosmosClaimService
    {
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim> GetClaimAsync(string id);
        Task<bool> AddItemAsync(Claim item);

        Task<bool> DeleteItemAsync(string id);
    }
}
