using api.DTOs;
using api.Interfaces;

namespace api.Services
{
    /// <summary>
    /// In-memory mock implementation of IAzureOpenAIService for testing
    /// Demonstrates the Liskov Substitution Principle (LSP) by being a valid substitute
    /// for the original AzureOpenAIService
    /// </summary>
    public class InMemoryAzureOpenAIService : IAzureOpenAIService
    {
        private readonly Dictionary<string, string> _responses = new();
        private readonly Dictionary<string, RAGResponse> _ragResponses = new();
        private readonly Dictionary<string, (string, RAGResponse)> _imageResponses = new();

        /// <summary>
        /// Add or update a pre-defined response for a specific query
        /// </summary>
        public void AddResponse(string query, string response)
        {
            _responses[query] = response;
        }

        /// <summary>
        /// Add or update a pre-defined RAG response for a specific query
        /// </summary>
        public void AddRAGResponse(string query, RAGResponse response)
        {
            _ragResponses[query] = response;
        }

        /// <summary>
        /// Add or update a pre-defined image response for a specific URL
        /// </summary>
        public void AddImageResponse(string imageUrl, string imageResponse, RAGResponse followUpResponse)
        {
            _imageResponses[imageUrl] = (imageResponse, followUpResponse);
        }

        /// <summary>
        /// Get a completion response based on pre-defined responses or generate a default one
        /// </summary>
        public Task<RAGResponse> GetCompletionAsync(
            string systemPrompt, 
            string userQuery, 
            bool useRAG = true, 
            float temperature = 0.2f)
        {
            // First try to find an exact match for the query
            if (_ragResponses.TryGetValue(userQuery, out var ragResponse))
            {
                return Task.FromResult(ragResponse);
            }

            // If no RAG response is found, try to find a simple text response
            if (_responses.TryGetValue(userQuery, out var simpleResponse))
            {
                return Task.FromResult(new RAGResponse
                {
                    Answer = simpleResponse,
                    Citations = new List<Citation>()
                });
            }

            // Default response if nothing matches
            return Task.FromResult(new RAGResponse
            {
                Answer = $"This is a simulated response to: {userQuery}",
                Citations = new List<Citation>
                {
                    new Citation { Title = "Simulated Source", Url = "https://example.com/simulated" }
                }
            });
        }

        /// <summary>
        /// Get an image analysis response based on pre-defined responses or generate a default one
        /// </summary>
        public Task<(string ImageResponse, RAGResponse FollowUpResponse)> GetImageAndRAGCompletionAsync(
            string systemPrompt,
            string initialImageQuery,
            string imageUrl,
            string followUpTemplate,
            float temperature = 0.2f)
        {
            // Try to find a pre-defined response for this image URL
            if (_imageResponses.TryGetValue(imageUrl, out var response))
            {
                return Task.FromResult(response);
            }

            // Default response if nothing matches
            var defaultImageResponse = $"This is a simulated image analysis for: {imageUrl}";
            var defaultFollowUp = new RAGResponse
            {
                Answer = $"This is a simulated follow-up information about the image.",
                Citations = new List<Citation>
                {
                    new Citation { Title = "Simulated Image Source", Url = "https://example.com/images" }
                }
            };

            return Task.FromResult((defaultImageResponse, defaultFollowUp));
        }
    }
}
