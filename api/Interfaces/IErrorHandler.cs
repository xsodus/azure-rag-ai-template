using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    /// <summary>
    /// Interface for error handling following Single Responsibility Principle
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Handle exceptions and return appropriate HTTP response
        /// </summary>
        IActionResult HandleException(Exception exception, string context);

        /// <summary>
        /// Handle validation errors and return appropriate HTTP response
        /// </summary>
        IActionResult HandleValidationError(ValidationResult validationResult);
    }
}
