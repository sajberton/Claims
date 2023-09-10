using Claims.Models;
using Claims.Models.Enums;
using Claims.Services.AuditerServices;
using Claims.Services.CosmosDBService;
//using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Claims.Services.CoverService
{
    public class CoverService : ICoverService
    {
        private readonly ICosmosCoverService _cosmosDBService;
        private readonly IAuditerServices _auditerServices;

        public CoverService(ICosmosCoverService cosmosDBService, IAuditerServices auditerServices)
        {
            _cosmosDBService = cosmosDBService;
            _auditerServices = auditerServices;
        }
        public async Task<ResponseModel> AddItemAsync(Cover cover)
        {
            var response = new ResponseModel();
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (cover.StartDate < today)
                {
                    response.IsSuccessful = false;
                    response.Error = "Start Date cannot be in the past";
                    return response;
                };

                int insurancePeriod = (cover.EndDate.DayNumber - cover.StartDate.DayNumber);
                if (insurancePeriod > Constants.CoverConstants.MaxPeriodDays)
                {
                    response.IsSuccessful = false;
                    response.Error = "Total insurance period cannot exceed 1 year";
                    return response;
                };

                cover.Id = Guid.NewGuid().ToString();
                cover.Premium = await ComputePremiumAsync(cover.StartDate, cover.EndDate, cover.Type);
                await _auditerServices.AuditCover(cover.Id, "POST");
                response.IsSuccessful = await _cosmosDBService.AddItemAsync(cover); ;
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
                await _auditerServices.AuditCover(id, "DELETE");
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

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
            return await _cosmosDBService.GetAllAsync();
        }

        public async Task<Cover> GetByIdAsync(string id)
        {
            return await _cosmosDBService.GetByIdAsync(id);
        }
        public async Task<decimal> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverTypeEnum coverType)
        {
            decimal basePremiumPerDay = 1250;
            decimal multiplier = 1.3m;

            switch (coverType)
            {
                case CoverTypeEnum.Yacht:
                    multiplier = 1.1m;
                    break;
                case CoverTypeEnum.PassengerShip:
                    multiplier = 1.2m;
                    break;
                case CoverTypeEnum.Tanker:
                    multiplier = 1.5m;
                    break;
            }

            decimal totalPremium = 0m;
            int insuranceLength = endDate.DayNumber - startDate.DayNumber;

            for (int i = 0; i < insuranceLength; i++)
            {
                if (i < 30)
                {
                    totalPremium += basePremiumPerDay * multiplier;
                }
                else if (i < 180)
                {
                    totalPremium += basePremiumPerDay * multiplier * (coverType == CoverTypeEnum.Yacht ? 0.95m : 0.98m);
                }
                else
                {
                    totalPremium += basePremiumPerDay * multiplier * (coverType == CoverTypeEnum.Yacht ? 0.92m : 0.97m);
                }
            }
            return totalPremium;
        }
    }
}
