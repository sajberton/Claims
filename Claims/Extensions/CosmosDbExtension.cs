using Claims.CosmosDbServices.CosmosCoverService;
using Claims.Services;
using Claims.Services.AuditerServices;
using Claims.Services.ClaimService;
using Claims.Services.CoverService;
using Microsoft.Azure.Cosmos;

namespace Claims.Extensions
{
    public static class CosmosDbExtension
    {
        public static async void AddCosmosDbServices(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            CosmosClient client = new CosmosClient(account, key);

            services.AddSingleton(
            InitializeCosmosClaimServiceAsync(configurationSection, client).GetAwaiter().GetResult());

            services.AddSingleton(
            InitializeCosmosCoverServiceAsync(configurationSection, client).GetAwaiter().GetResult());
        }

        static async Task<ICosmosClaimService> InitializeCosmosClaimServiceAsync(IConfigurationSection configurationSection, CosmosClient client)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            CosmosClaimService cosmosDbService = new CosmosClaimService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }

        static async Task<ICosmosCoverService> InitializeCosmosCoverServiceAsync(IConfigurationSection configurationSection, CosmosClient client)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerNameCover").Value;

            CosmosCoverService cosmosDbService = new CosmosCoverService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }
    }
}
