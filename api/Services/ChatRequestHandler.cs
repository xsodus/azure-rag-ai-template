using api.DTOs;
using api.Interfaces;

namespace api.Services
{
    /// <summary>
    /// Implementation of chat request handling following Single Responsibility Principle
    /// </summary>
    public class ChatRequestHandler : IChatRequestHandler
    {
        private readonly IAzureOpenAIService _openAIService;
        private readonly ILogger<ChatRequestHandler> _logger;

        public ChatRequestHandler(IAzureOpenAIService openAIService, ILogger<ChatRequestHandler> logger)
        {
            _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RAGResponse> HandleChatRequestAsync(QueryRequest request)
        {
            _logger.LogInformation("Processing chat request for user query: {UserQuery}", request.UserQuery);

            try
            {
                var response = await _openAIService.GetCompletionAsync(
                    request.SystemPrompt,
                    request.UserQuery,
                    request.UseRAG,
                    request.Temperature);

                _logger.LogInformation("Successfully processed chat request");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat request for query: {UserQuery}", request.UserQuery);
                throw;
            }
        }

        public async Task<ImageQueryResponse> HandleImageChatRequestAsync(ImageQueryRequest request)
        {
            _logger.LogInformation("Processing image chat request for image: {ImageUrl}", request.ImageUrl);

            try
            {
                var (imageResponse, followUpResponse) = await _openAIService.GetImageAndRAGCompletionAsync(
                    request.SystemPrompt,
                    request.InitialImageQuery,
                    request.ImageUrl,
                    request.FollowUpTemplate,
                    request.Temperature);

                var response = new ImageQueryResponse
                {
                    ImageResponse = imageResponse,
                    FollowUpResponse = followUpResponse
                };

                _logger.LogInformation("Successfully processed image chat request");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image chat request for image: {ImageUrl}", request.ImageUrl);
                throw;
            }
        }
    }
}
