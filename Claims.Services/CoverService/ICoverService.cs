using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services.CoverService
{
    public interface ICoverService
    {
        Task<IEnumerable<Cover>> GetAllAsync();
        Task<Cover> GetByIdAsync(string id);
        Task AddItemAsync(Cover item);
        Task DeleteItemAsync(string id);
    }
}
