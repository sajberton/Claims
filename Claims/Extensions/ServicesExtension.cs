﻿using Claims.Services.AuditerServices;
using Claims.Services.ClaimService;
using Claims.Services.CoverService;

namespace Claims.Extensions
{
    public static class ServicesExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuditerServices,  AuditerServices>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<ICoverService, CoverService>();
        }
    }
}
