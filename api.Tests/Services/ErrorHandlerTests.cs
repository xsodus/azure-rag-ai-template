using api.Interfaces;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace api.Tests.Services
{
    /// <summary>
    /// Unit tests for ErrorHandler demonstrating Single Responsibility Principle
    /// </summary>
    public class ErrorHandlerTests
    {
        private readonly Mock<ILogger<ErrorHandler>> _mockLogger;
        private readonly ErrorHandler _errorHandler;

        public ErrorHandlerTests()
        {
            _mockLogger = new Mock<ILogger<ErrorHandler>>();
            _errorHandler = new ErrorHandler(_mockLogger.Object);
        }

        [Fact]
        public void HandleException_ArgumentNullException_ReturnsBadRequest()
        {
            // Arrange
            var exception = new ArgumentNullException("parameter");
            var context = "Test context";

            // Act
            var result = _errorHandler.HandleException(exception, context);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var error = Assert.IsType<dynamic>(badRequestResult.Value);
            Assert.Equal("Invalid request parameters.", (string)error.error);
        }

        [Fact]
        public void HandleException_UnauthorizedAccessException_ReturnsUnauthorized()
        {
            // Arrange
            var exception = new UnauthorizedAccessException("Unauthorized");
            var context = "Test context";

            // Act
            var result = _errorHandler.HandleException(exception, context);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            var error = Assert.IsType<dynamic>(unauthorizedResult.Value);
            Assert.Equal("Unauthorized access.", (string)error.error);
        }

        [Fact]
        public void HandleException_GenericException_ReturnsInternalServerError()
        {
            // Arrange
            var exception = new Exception("Generic error");
            var context = "Test context";

            // Act
            var result = _errorHandler.HandleException(exception, context);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var error = Assert.IsType<dynamic>(objectResult.Value);
            Assert.Equal("An error occurred while processing your request.", (string)error.error);
        }

        [Fact]
        public void HandleValidationError_WithErrors_ReturnsBadRequest()
        {
            // Arrange
            var validationResult = new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Validation failed",
                Errors = new List<string> { "Error 1", "Error 2" }
            };

            // Act
            var result = _errorHandler.HandleValidationError(validationResult);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var error = Assert.IsType<dynamic>(badRequestResult.Value);
            Assert.Equal("Validation failed", (string)error.error);
            Assert.Equal(2, ((string[])error.details).Length);
        }
    }
}
