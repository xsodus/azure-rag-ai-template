using api.Services;
using api.Interfaces;

namespace api.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configure Azure OpenAI and related services following Dependency Inversion Principle
        /// </summary>
        public static IServiceCollection AddAzureOpenAIServices(this IServiceCollection services, IConfiguration configuration, bool useInMemoryService = false)
        {
            // Register services following SOLID principles
            // - Single Responsibility: Each service has one reason to change
            // - Open/Closed: Services can be extended through interfaces
            // - Dependency Inversion: Depending on abstractions, not concretions
            
            // If useInMemoryService is true, register the in-memory implementation for testing
            if (useInMemoryService || configuration.GetValue<bool>("UseInMemoryServices", false))
            {
                services.AddSingleton<IAzureOpenAIService, InMemoryAzureOpenAIService>();
            }
            else
            {
                // Validate configuration for the real Azure OpenAI service
                var oaiEndpoint = configuration["AzureOpenAI:Endpoint"];
                var oaiKey = configuration["AzureOpenAI:Key"];
                var deploymentName = configuration["AzureOpenAI:DeploymentName"];
                var searchEndpoint = configuration["AzureSearch:Endpoint"];
                var searchKey = configuration["AzureSearch:Key"];
                var searchIndex = configuration["AzureSearch:IndexName"];

                if (string.IsNullOrEmpty(oaiEndpoint) || string.IsNullOrEmpty(oaiKey) || string.IsNullOrEmpty(deploymentName))
                {
                    throw new ArgumentException("Azure OpenAI configuration is incomplete. Please check your appsettings.json file.");
                }

                if (string.IsNullOrEmpty(searchEndpoint) || string.IsNullOrEmpty(searchKey) || string.IsNullOrEmpty(searchIndex))
                {
                    throw new ArgumentException("Azure Search configuration is incomplete. Please check your appsettings.json file.");
                }

                services.AddScoped<IAzureOpenAIService, AzureOpenAIService>();
            }
            
            // Register other services
            services.AddScoped<IChatRequestHandler, ChatRequestHandler>();
            services.AddScoped<IRequestValidator, RequestValidator>();
            services.AddScoped<IErrorHandler, ErrorHandler>();

            return services;
        }
    }
}
