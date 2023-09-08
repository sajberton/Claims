using Claims.Models;
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
        Task<ResponseModel> AddItemAsync(Cover cover);
        Task DeleteItemAsync(string id);
    }
}
