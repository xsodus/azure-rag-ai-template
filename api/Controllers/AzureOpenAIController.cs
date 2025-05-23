using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AzureOpenAIController : ControllerBase
    {
        private readonly AzureOpenAIService _openAIService;
        private readonly ILogger<AzureOpenAIController> _logger;

        public AzureOpenAIController(AzureOpenAIService openAIService, ILogger<AzureOpenAIController> logger)
        {
            _openAIService = openAIService;
            _logger = logger;
        }

        /// <summary>
        /// Send a text query to Azure OpenAI with optional RAG support
        /// </summary>
        [HttpPost("query")]
        public async Task<IActionResult> Query([FromBody] QueryRequest request)
        {
            try
            {
                var response = await _openAIService.GetCompletionAsync(
                    request.SystemPrompt, 
                    request.UserQuery, 
                    request.UseRAG, 
                    request.Temperature);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying Azure OpenAI");
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Process an image and follow up with a RAG-powered query
        /// </summary>
        [HttpPost("image-query")]
        public async Task<IActionResult> ImageQuery([FromBody] ImageQueryRequest request)
        {
            try
            {
                var (imageResponse, followUpResponse) = await _openAIService.GetImageAndRAGCompletionAsync(
                    request.SystemPrompt,
                    request.InitialImageQuery,
                    request.ImageUrl,
                    request.FollowUpTemplate,
                    request.Temperature);

                return Ok(new { 
                    imageResponse, 
                    followUpResponse 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image query with Azure OpenAI");
                return StatusCode(500, new { error = "An error occurred while processing your image query." });
            }
        }
    }

    /// <summary>
    /// Request model for text-based queries
    /// </summary>
    public class QueryRequest
    {
        public string SystemPrompt { get; set; } = "You are a helpful assistant.";
        public string UserQuery { get; set; } = "";
        public bool UseRAG { get; set; } = true;
        public float Temperature { get; set; } = 0.2f;
    }

    /// <summary>
    /// Request model for image-based queries with RAG follow-up
    /// </summary>
    public class ImageQueryRequest
    {
        public string SystemPrompt { get; set; } = "You are a helpful assistant.";
        public string InitialImageQuery { get; set; } = "What is shown in this image?";
        public string ImageUrl { get; set; } = "";
        public string FollowUpTemplate { get; set; } = "Tell me more about: {0}";
        public float Temperature { get; set; } = 0.2f;
    }
}
