using Claims.Models.Enums;
using Claims.Services.AuditerServices;
using Claims.Services.CosmosDBService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services.CoverService
{
    public class CoverService : ICoverService
    {
        private readonly ICosmosDBService<Cover> _cosmosDBService;
        private readonly IAuditerServices _auditerServices;

        public CoverService(ICosmosDBService<Cover> cosmosDBService, IAuditerServices auditerServices)
        {
            _cosmosDBService = cosmosDBService;
            _auditerServices = auditerServices;
        }
        public async Task AddItemAsync(Cover cover)
        {
            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
            await _cosmosDBService.AddItemAsync(cover);
            _auditerServices.AuditCover(cover.Id, "POST");
          //  return Ok(cover);
        }

        public async Task DeleteItemAsync(string id)
        {
            _auditerServices.AuditCover(id, "DELETE");
           await _cosmosDBService.DeleteItemAsync(id);
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
            return await _cosmosDBService.GetAllAsync();
        }

        public async Task<Cover> GetByIdAsync(string id)
        {
            return await _cosmosDBService.GetByIdAsync(id);
        }

        private decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverTypeEnum coverType)
        {
            var multiplier = 1.3m;
            if (coverType == CoverTypeEnum.Yacht)
            {
                multiplier = 1.1m;
            }

            if (coverType == CoverTypeEnum.PassengerShip)
            {
                multiplier = 1.2m;
            }

            if (coverType == CoverTypeEnum.Tanker)
            {
                multiplier = 1.5m;
            }

            var premiumPerDay = 1250 * multiplier;
            var insuranceLength = endDate.DayNumber - startDate.DayNumber;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                if (i < 30) totalPremium += premiumPerDay;
                if (i < 180 && coverType == CoverTypeEnum.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                if (i < 365 && coverType != CoverTypeEnum.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
            }

            return totalPremium;
        }
    }
}
