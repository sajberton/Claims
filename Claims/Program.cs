using System.Configuration;
using System.Text.Json.Serialization;
using Claims.Auditing;
using Claims.Controllers;
using Claims.Extensions;
using Claims.Services;
using Claims.Services.CosmosDBService;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

var cosmosClient = InitializeCosmosClientInstanceAsync(builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult();

services.AddSingleton(
    InitializeCosmosClaimServiceAsync(builder.Configuration.GetSection("CosmosDb"), cosmosClient).GetAwaiter().GetResult());

services.AddSingleton(
    InitializeCosmosCoverServiceAsync(builder.Configuration.GetSection("CosmosDb"), cosmosClient).GetAwaiter().GetResult());

//services.AddSingleton(x =>
//{
//    return new CosmosClaimService(cosmosClient, "ClaimDb", "Claim");
//});

//services.AddSingleton(x =>
//{
//    return new CosmosCoverService(cosmosClient, "ClaimDb", "Cover");
//});

services.AddServices();

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

//static async Task<CosmosClaimService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
//{
//    string databaseName = configurationSection.GetSection("DatabaseName").Value;
//    string containerName = configurationSection.GetSection("ContainerName").Value;
//    string account = configurationSection.GetSection("Account").Value;
//    string key = configurationSection.GetSection("Key").Value;
//    Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
//    CosmosClaimService cosmosDbService = new CosmosClaimService(client, databaseName, containerName);
//    Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
//    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

//    return cosmosDbService;
//}

static async Task<CosmosClient> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value;
    string containerName = configurationSection.GetSection("ContainerName").Value;
    string account = configurationSection.GetSection("Account").Value;
    string key = configurationSection.GetSection("Key").Value;
    Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
   // CosmosClaimService cosmosDbService = new CosmosClaimService(client, databaseName, containerName);
    Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return client;
}

static async Task<CosmosClaimService> InitializeCosmosClaimServiceAsync(IConfigurationSection configurationSection, CosmosClient client)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value;
    string containerName = configurationSection.GetSection("ContainerName").Value;
    string account = configurationSection.GetSection("Account").Value;
    string key = configurationSection.GetSection("Key").Value;
   // Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    CosmosClaimService cosmosDbService = new CosmosClaimService(client, databaseName, containerName);
    Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return cosmosDbService;
}

static async Task<CosmosCoverService> InitializeCosmosCoverServiceAsync(IConfigurationSection configurationSection, CosmosClient client)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value;
    string containerName = configurationSection.GetSection("ContainerNameCover").Value;
    string account = configurationSection.GetSection("Account").Value;
    string key = configurationSection.GetSection("Key").Value;
    // Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    CosmosCoverService cosmosDbService = new CosmosCoverService(client, databaseName, containerName);
    Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return cosmosDbService;
}

public partial class Program { }