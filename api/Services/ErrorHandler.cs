using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Services
{
    /// <summary>
    /// Implementation of error handling following Single Responsibility Principle
    /// </summary>
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger<ErrorHandler> _logger;

        public ErrorHandler(ILogger<ErrorHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult HandleException(Exception exception, string context)
        {
            _logger.LogError(exception, "Error in {Context}: {Message}", context, exception.Message);

            return exception switch
            {
                ArgumentNullException => new BadRequestObjectResult(new { error = "Invalid request parameters." }),
                ArgumentException => new BadRequestObjectResult(new { error = exception.Message }),
                UnauthorizedAccessException => new UnauthorizedObjectResult(new { error = "Unauthorized access." }),
                NotSupportedException => new BadRequestObjectResult(new { error = "Operation not supported." }),
                TimeoutException => new ObjectResult(new { error = "Request timeout. Please try again." }) { StatusCode = 408 },
                _ => new ObjectResult(new { error = "An error occurred while processing your request." }) { StatusCode = 500 }
            };
        }

        public IActionResult HandleValidationError(ValidationResult validationResult)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", validationResult.Errors));
            
            return new BadRequestObjectResult(new 
            { 
                error = validationResult.ErrorMessage, 
                details = validationResult.Errors 
            });
        }
    }
}
