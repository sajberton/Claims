using System.Security.Claims;

namespace Claims.Services.ClaimService
{
    public interface IClaimService
    {
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim> GetByIdAsync(string id);
        Task AddItemAsync(Claim item);
        Task DeleteItemAsync(string id);
    }
}