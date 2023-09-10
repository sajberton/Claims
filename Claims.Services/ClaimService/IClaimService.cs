using Claims.Models;

namespace Claims.Services.ClaimService
{
    public interface IClaimService
    {
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim> GetByIdAsync(string id);
        Task<ResponseModel> AddItemAsync(Claim item);
        Task<ResponseModel> DeleteItemAsync(string id);
    }
}