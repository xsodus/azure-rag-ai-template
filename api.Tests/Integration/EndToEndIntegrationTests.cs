using api.Controllers.V1_0;
using api.DTOs;
using api.Interfaces;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace api.Tests.Integration
{
    /// <summary>
    /// Integration tests verifying the end-to-end flow of components
    /// </summary>
    public class EndToEndIntegrationTests
    {
        // Mock services for the test
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<AzureOpenAIController>> _mockControllerLogger;
        private readonly Mock<ILogger<ChatRequestHandler>> _mockHandlerLogger;
        private readonly Mock<ILogger<ErrorHandler>> _mockErrorLogger;
        
        // Actual implementations for integration test
        private readonly RequestValidator _validator;
        private readonly ErrorHandler _errorHandler;
        private readonly IAzureOpenAIService _mockOpenAIService;
        private readonly IChatRequestHandler _chatHandler;
        private readonly AzureOpenAIController _controller;

        public EndToEndIntegrationTests()
        {
            // Set up mocks
            _mockConfiguration = new Mock<IConfiguration>();
            _mockControllerLogger = new Mock<ILogger<AzureOpenAIController>>();
            _mockHandlerLogger = new Mock<ILogger<ChatRequestHandler>>();
            _mockErrorLogger = new Mock<ILogger<ErrorHandler>>();
            _mockOpenAIService = Mock.Of<IAzureOpenAIService>();
                        
            // Create actual implementations of services
            _validator = new RequestValidator();
            _errorHandler = new ErrorHandler(_mockErrorLogger.Object);
            _chatHandler = new ChatRequestHandler(_mockOpenAIService, _mockHandlerLogger.Object);
            
            // Create controller with real implementations of validator and error handler
            _controller = new AzureOpenAIController(
                _chatHandler,
                _validator,
                _errorHandler,
                _mockControllerLogger.Object
            );
        }

        [Fact]
        public async Task CompleteFlow_QueryWithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange - Create an invalid request (empty query)
            var request = new QueryRequest
            {
                SystemPrompt = "You are a helpful assistant.",
                UserQuery = "",  // Invalid - empty
                UseRAG = true,
                Temperature = 0.2f
            };

            // Act - Execute the full flow
            var result = await _controller.Query(request);

            // Assert - Verify the proper error response
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            // Verify the response contains the expected error message
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var errorObj = JsonConvert.DeserializeObject<JObject>(json);
            
            Assert.Equal("User query is required", errorObj?["error"]?.ToString());
        }

        [Fact]
        public async Task CompleteFlow_ImageQueryWithInvalidUrl_ReturnsBadRequest()
        {
            // Arrange - Create an invalid request (invalid image URL)
            var request = new ImageQueryRequest
            {
                SystemPrompt = "You are a helpful assistant.",
                InitialImageQuery = "What is in this image?",
                ImageUrl = "invalid-url",  // Invalid URL format
                FollowUpTemplate = "Tell me more about {0}",
                Temperature = 0.2f
            };

            // Act - Execute the full flow
            var result = await _controller.ImageQuery(request);

            // Assert - Verify the proper error response
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            // Verify the response contains the expected error message
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var errorObj = JsonConvert.DeserializeObject<JObject>(json);
            
            Assert.Contains("Invalid image URL format", errorObj?["error"]?.ToString());
        }

        [Fact]
        public async Task CompleteFlow_QueryWithValidRequest_DelegatesToService()
        {
            // Arrange
            var request = new QueryRequest
            {
                SystemPrompt = "You are a helpful assistant.",
                UserQuery = "Tell me about Azure AI services.",
                UseRAG = true,
                Temperature = 0.2f
            };

            var expectedResponse = new RAGResponse
            {
                Answer = "Azure AI services include various cognitive capabilities.",
                Citations = new List<Citation>()
            };

            // Set up the mock service to return the expected response
            Mock.Get(_mockOpenAIService)
                .Setup(s => s.GetCompletionAsync(
                    request.SystemPrompt,
                    request.UserQuery,
                    request.UseRAG,
                    request.Temperature))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Query(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RAGResponse>(okResult.Value);
            Assert.Equal(expectedResponse.Answer, response.Answer);
            
            // Verify the service was called with the correct parameters
            Mock.Get(_mockOpenAIService).Verify(
                s => s.GetCompletionAsync(
                    request.SystemPrompt,
                    request.UserQuery,
                    request.UseRAG,
                    request.Temperature),
                Times.Once);
        }
    }
}
