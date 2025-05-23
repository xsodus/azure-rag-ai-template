using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.OpenApi.Models;
using api.Services;
using api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Azure RAG AI API",
        Version = "v1",
        Description = "API for Azure RAG AI Template"
    });
});

// Register Azure OpenAI services using the extension method
builder.Services.AddAzureOpenAIServices(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Configure HTTPS redirection
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 7222; // Set the HTTPS port to match launchSettings.json
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Azure RAG AI API v1"));
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

