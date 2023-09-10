using Claims.Models;
using Claims.Services.CoverService;
using Claims.Services.AuditerServices;
using Utils;

namespace Claims.Services.ClaimService
{
    public class ClaimService : IClaimService
    {
        private readonly ICosmosClaimService _cosmosDBService;
        private readonly ICoverService _coverService;
        private readonly IAuditerServices _auditerServices;

        public ClaimService(ICosmosClaimService cosmosDBService, ICoverService coverService, IAuditerServices auditerServices)
        {
            _cosmosDBService = cosmosDBService;
            _coverService = coverService;
            _auditerServices = auditerServices;
        }
        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _cosmosDBService.GetAllAsync();
        }

        public async Task<Claim> GetByIdAsync(string id)
        {
            return await _cosmosDBService.GetClaimAsync(id);
        }

        public async Task<ResponseModel> AddItemAsync(Claim claim)
        {
            var response = new ResponseModel();
            try
            {
                if (claim.DamageCost > Constants.ClaimsConstants.MaxDamageCost)
                {
                    response.IsSuccessful = false;
                    response.Error = "Exceeded Maximum damage cost";
                    return response;
                };

                var cover = await _coverService.GetByIdAsync(claim.CoverId);
                if (cover == null)
                {
                    response.IsSuccessful = false;
                    response.Error = "This cover does not exists";
                    return response;
                };

                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (cover.EndDate < today)
                {
                    response.IsSuccessful = false;
                    response.Error = "This cover has expired";
                    return response;
                };

                claim.Id = Guid.NewGuid().ToString();
                await _auditerServices.AuditClaim(claim.Id, "POST");
                response.IsSuccessful = await _cosmosDBService.AddItemAsync(claim); ;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel> DeleteItemAsync(string id)
        {
            var response = new ResponseModel();
            try
            {
                await _auditerServices.AuditClaim(id, "DELETE");
                var res = await _cosmosDBService.DeleteItemAsync(id);
                response.IsSuccessful = res;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = ex.Message;
                return response;
            }
        }
    }
}
