using api.Services;

namespace api.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configure Azure OpenAI and related services
        /// </summary>
        public static IServiceCollection AddAzureOpenAIServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Validate configuration
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

            // Register AzureOpenAIService
            services.AddSingleton<AzureOpenAIService>();

            return services;
        }
    }
}
