using api.DTOs;
using api.Interfaces;
using api.Services;
using Xunit;

namespace api.Tests.Services
{
    /// <summary>
    /// Unit tests for RequestValidator demonstrating Single Responsibility Principle
    /// </summary>
    public class RequestValidatorTests
    {
        private readonly RequestValidator _validator;

        public RequestValidatorTests()
        {
            _validator = new RequestValidator();
        }

        #region QueryRequest Tests

        [Fact]
        public void ValidateQueryRequest_ValidRequest_ReturnsValidResult()
        {
            // Arrange
            var request = new QueryRequest
            {
                UserQuery = "This is a valid query",
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateQueryRequest(request);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void ValidateQueryRequest_NullRequest_ReturnsInvalidResult()
        {
            // Arrange
            QueryRequest? request = null;

            // Act
            #pragma warning disable CS8604 // Possible null reference argument.
            var result = _validator.ValidateQueryRequest(request);
            #pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("Request is null", result.Errors);
            Assert.Equal("Request cannot be null", result.ErrorMessage);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateQueryRequest_EmptyUserQuery_ReturnsInvalidResult(string userQuery)
        {
            // Arrange
            var request = new QueryRequest
            {
                UserQuery = userQuery,
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("UserQuery is required and cannot be empty", result.Errors);
            Assert.Equal("User query is required", result.ErrorMessage);
        }
        
        [Fact]
        public void ValidateQueryRequest_NullUserQuery_ReturnsInvalidResult()
        {
            // Arrange
            var request = new QueryRequest
            {
                UserQuery = null!,
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("UserQuery is required and cannot be empty", result.Errors);
            Assert.Equal("User query is required", result.ErrorMessage);
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(2.1f)]
        public void ValidateQueryRequest_InvalidTemperature_ReturnsInvalidResult(float temperature)
        {
            // Arrange
            var request = new QueryRequest
            {
                UserQuery = "Valid query",
                Temperature = temperature
            };

            // Act
            var result = _validator.ValidateQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Temperature must be between 0 and 2", result.Errors);
            Assert.Equal("Temperature must be between 0 and 2", result.ErrorMessage);
        }

        [Fact]
        public void ValidateQueryRequest_MultipleErrors_ReturnsCombinedErrors()
        {
            // Arrange
            var request = new QueryRequest
            {
                UserQuery = "",
                Temperature = 2.5f
            };

            // Act
            var result = _validator.ValidateQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);
            Assert.Contains("UserQuery is required and cannot be empty", result.Errors);
            Assert.Contains("Temperature must be between 0 and 2", result.Errors);
            // The implementation might set any error as the first one, so we'll just check that it's one of the errors
            Assert.Contains(result.ErrorMessage, new[]
            {
                "User query is required",
                "Temperature must be between 0 and 2"
            });
        }

        #endregion

        #region ImageQueryRequest Tests

        [Fact]
        public void ValidateImageQueryRequest_ValidRequest_ReturnsValidResult()
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = "https://example.com/image.jpg",
                InitialImageQuery = "What is in this image?",
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Equal(string.Empty, result.ErrorMessage);
        }

        [Fact]
        public void ValidateImageQueryRequest_NullRequest_ReturnsInvalidResult()
        {
            // Arrange
            ImageQueryRequest? request = null;

            // Act
            #pragma warning disable CS8604 // Possible null reference argument.
            var result = _validator.ValidateImageQueryRequest(request);
            #pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("Request is null", result.Errors);
            Assert.Equal("Request cannot be null", result.ErrorMessage);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateImageQueryRequest_EmptyImageUrl_ReturnsInvalidResult(string imageUrl)
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = imageUrl,
                InitialImageQuery = "What is in this image?",
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("ImageUrl is required and cannot be empty", result.Errors);
            Assert.Equal("Image URL is required", result.ErrorMessage);
        }
        
        [Fact]
        public void ValidateImageQueryRequest_NullImageUrl_ReturnsInvalidResult()
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = null!,
                InitialImageQuery = "What is in this image?",
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("ImageUrl is required and cannot be empty", result.Errors);
            Assert.Equal("Image URL is required", result.ErrorMessage);
        }

        [Theory]
        [InlineData("invalid-url")]
        [InlineData("just some text")]
        public void ValidateImageQueryRequest_InvalidImageUrl_ReturnsInvalidResult(string imageUrl)
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = imageUrl,
                InitialImageQuery = "What is in this image?",
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("ImageUrl must be a valid URL", result.Errors);
            Assert.Equal("Invalid image URL format", result.ErrorMessage);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateImageQueryRequest_EmptyInitialImageQuery_ReturnsInvalidResult(string query)
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = "https://example.com/image.jpg",
                InitialImageQuery = query,
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("InitialImageQuery is required and cannot be empty", result.Errors);
            Assert.Equal("Initial image query is required", result.ErrorMessage);
        }
        
        [Fact]
        public void ValidateImageQueryRequest_NullInitialImageQuery_ReturnsInvalidResult()
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = "https://example.com/image.jpg",
                InitialImageQuery = null!,
                Temperature = 0.5f
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("InitialImageQuery is required and cannot be empty", result.Errors);
            Assert.Equal("Initial image query is required", result.ErrorMessage);
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(2.1f)]
        public void ValidateImageQueryRequest_InvalidTemperature_ReturnsInvalidResult(float temperature)
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = "https://example.com/image.jpg",
                InitialImageQuery = "What is in this image?",
                Temperature = temperature
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Temperature must be between 0 and 2", result.Errors);
            Assert.Equal("Temperature must be between 0 and 2", result.ErrorMessage);
        }

        [Fact]
        public void ValidateImageQueryRequest_MultipleErrors_ReturnsCombinedErrors()
        {
            // Arrange
            var request = new ImageQueryRequest
            {
                ImageUrl = "",
                InitialImageQuery = "",
                Temperature = 2.5f
            };

            // Act
            var result = _validator.ValidateImageQueryRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
            Assert.Contains("ImageUrl is required and cannot be empty", result.Errors);
            Assert.Contains("InitialImageQuery is required and cannot be empty", result.Errors);
            Assert.Contains("Temperature must be between 0 and 2", result.Errors);
            // The implementation might set any error as the first one, so we'll just check that it's one of the errors
            Assert.Contains(result.ErrorMessage, new[]
            {
                "Image URL is required",
                "Initial image query is required",
                "Temperature must be between 0 and 2"
            });
        }

        #endregion
    }
}