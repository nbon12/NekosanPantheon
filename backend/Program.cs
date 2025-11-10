using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HelloWorldFunctionApp.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Use InMemory database for local development, Cosmos DB for production
        // Set USE_IN_MEMORY_DB=false to use Cosmos DB, or leave unset/true for InMemory
        var useInMemory = Environment.GetEnvironmentVariable("USE_IN_MEMORY_DB") != "false";

        if (useInMemory)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "HelloWorldDb"));
        }
        else
        {
            var cosmosEndpoint = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT") 
                ?? "https://localhost:8081";
            var cosmosKey = Environment.GetEnvironmentVariable("COSMOS_KEY") 
                ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            services.AddDbContext<AppDbContext>(options =>
                options.UseCosmos(
                    cosmosEndpoint,
                    cosmosKey,
                    databaseName: "HelloWorldDb"
                ));
        }
    })
    .Build();

host.Run();

