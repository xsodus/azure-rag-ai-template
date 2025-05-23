using api.Services;
using api.DTOs;
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
        [HttpPost("chat")]
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
        [HttpPost("chat-with-image")]
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

                return Ok(new ImageQueryResponse 
                { 
                    ImageResponse = imageResponse, 
                    FollowUpResponse = followUpResponse 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image query with Azure OpenAI");
                return StatusCode(500, new { error = "An error occurred while processing your image query." });
            }
        }
    }
}
