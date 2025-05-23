using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.OpenApi.Models;
using api.Services;
using api.Extensions;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add versioned API explorer (for Swagger documentation)
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Register the Swagger generator and configure options
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddHttpContextAccessor();

// Register Azure OpenAI services using the extension method
// Use in-memory service for testing environments if specified in configuration
var useInMemoryService = builder.Environment.IsEnvironment("Testing") || 
                         builder.Configuration.GetValue<bool>("UseInMemoryServices", false);

builder.Services.AddAzureOpenAIServices(builder.Configuration, useInMemoryService);

// Add controllers
builder.Services.AddControllers();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Configure HTTPS redirection
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 7222; // Set the HTTPS port to match launchSettings.json
});

var app = builder.Build();

// Configure IP rate limiting
app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiVersionDescriptionProvider>();
    
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Build a swagger endpoint for each discovered API version
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
    
    // Disable HTTPS redirection in development for easier testing
    // app.UseHttpsRedirection();
}
else
{
    app.UseHttpsRedirection();
}

// Enable routing and controllers
app.UseRouting();
app.MapControllers();

app.Run();

