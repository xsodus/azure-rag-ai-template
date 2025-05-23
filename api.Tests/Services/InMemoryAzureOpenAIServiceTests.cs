using api.DTOs;
using api.Services;
using Xunit;

namespace api.Tests.Services
{
    /// <summary>
    /// Unit tests for InMemoryAzureOpenAIService demonstrating Liskov Substitution Principle
    /// </summary>
    public class InMemoryAzureOpenAIServiceTests
    {
        private readonly InMemoryAzureOpenAIService _service;

        public InMemoryAzureOpenAIServiceTests()
        {
            _service = new InMemoryAzureOpenAIService();
            
            // Set up some test data
            _service.AddResponse("simple query", "Simple response");
            
            _service.AddRAGResponse("rag query", new RAGResponse 
            { 
                Answer = "RAG response",
                Citations = new List<Citation> 
                { 
                    new Citation { Title = "Test Source", Url = "https://example.com/test" } 
                }
            });
            
            _service.AddImageResponse(
                "https://example.com/image.jpg", 
                "Image analysis result", 
                new RAGResponse 
                { 
                    Answer = "Follow-up result",
                    Citations = new List<Citation>() 
                }
            );
        }

        [Fact]
        public async Task GetCompletionAsync_WithSimpleResponse_ReturnsCorrectResponse()
        {
            // Act
            var result = await _service.GetCompletionAsync(
                "system prompt", 
                "simple query", 
                true, 
                0.5f);

            // Assert
            Assert.Equal("Simple response", result.Answer);
            Assert.Empty(result.Citations);
        }

        [Fact]
        public async Task GetCompletionAsync_WithRAGResponse_ReturnsCorrectResponse()
        {
            // Act
            var result = await _service.GetCompletionAsync(
                "system prompt", 
                "rag query", 
                true, 
                0.5f);

            // Assert
            Assert.Equal("RAG response", result.Answer);
            Assert.Single(result.Citations);
            Assert.Equal("Test Source", result.Citations[0].Title);
        }

        [Fact]
        public async Task GetCompletionAsync_WithUnknownQuery_ReturnsDefaultResponse()
        {
            // Act
            var result = await _service.GetCompletionAsync(
                "system prompt", 
                "unknown query", 
                true, 
                0.5f);

            // Assert
            Assert.Contains("This is a simulated response to: unknown query", result.Answer);
            Assert.Single(result.Citations);
        }

        [Fact]
        public async Task GetImageAndRAGCompletionAsync_WithKnownImageUrl_ReturnsCorrectResponse()
        {
            // Act
            var (imageResponse, followUpResponse) = await _service.GetImageAndRAGCompletionAsync(
                "system prompt",
                "What is this?",
                "https://example.com/image.jpg",
                "Tell me more about {0}",
                0.5f);

            // Assert
            Assert.Equal("Image analysis result", imageResponse);
            Assert.Equal("Follow-up result", followUpResponse.Answer);
        }

        [Fact]
        public async Task GetImageAndRAGCompletionAsync_WithUnknownImageUrl_ReturnsDefaultResponse()
        {
            // Act
            var (imageResponse, followUpResponse) = await _service.GetImageAndRAGCompletionAsync(
                "system prompt",
                "What is this?",
                "https://example.com/unknown.jpg",
                "Tell me more about {0}",
                0.5f);

            // Assert
            Assert.Contains("This is a simulated image analysis", imageResponse);
            Assert.Contains("This is a simulated follow-up", followUpResponse.Answer);
        }
    }
}
