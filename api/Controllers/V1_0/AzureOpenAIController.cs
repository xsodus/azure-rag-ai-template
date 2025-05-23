using api.DTOs;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.V1_0
{
    /// <summary>
    /// Controller for Azure OpenAI operations following SOLID principles
    /// - Single Responsibility: Only handles HTTP requests/responses
    /// - Open/Closed: Extensible through interfaces
    /// - Liskov Substitution: Depends on abstractions
    /// - Interface Segregation: Uses focused interfaces
    /// - Dependency Inversion: Depends on abstractions, not concretions
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AzureOpenAIController : ControllerBase
    {
        private readonly IChatRequestHandler _chatRequestHandler;
        private readonly IRequestValidator _requestValidator;
        private readonly IErrorHandler _errorHandler;
        private readonly ILogger<AzureOpenAIController> _logger;

        public AzureOpenAIController(
            IChatRequestHandler chatRequestHandler,
            IRequestValidator requestValidator,
            IErrorHandler errorHandler,
            ILogger<AzureOpenAIController> logger)
        {
            _chatRequestHandler = chatRequestHandler ?? throw new ArgumentNullException(nameof(chatRequestHandler));
            _requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Send a text query to Azure OpenAI with optional RAG support
        /// </summary>
        [HttpPost("chat")]
        [ProducesResponseType(typeof(RAGResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> Query([FromBody] QueryRequest request)
        {
            // Check ModelState validation from data annotations
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for query request");
                return BadRequest(new 
                { 
                    error = "Validation failed", 
                    details = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList() 
                });
            }

            // Additional custom validation
            var validationResult = _requestValidator.ValidateQueryRequest(request);
            if (!validationResult.IsValid)
            {
                return _errorHandler.HandleValidationError(validationResult);
            }

            try
            {
                var response = await _chatRequestHandler.HandleChatRequestAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException(ex, "Query processing");
            }
        }

        /// <summary>
        /// Process an image and follow up with a RAG-powered query
        /// </summary>
        [HttpPost("chat-with-image")]
        [ProducesResponseType(typeof(ImageQueryResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 500)]
        public async Task<IActionResult> ImageQuery([FromBody] ImageQueryRequest request)
        {
            // Check ModelState validation from data annotations
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for image query request");
                return BadRequest(new 
                { 
                    error = "Validation failed", 
                    details = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList() 
                });
            }

            // Additional custom validation
            var validationResult = _requestValidator.ValidateImageQueryRequest(request);
            if (!validationResult.IsValid)
            {
                return _errorHandler.HandleValidationError(validationResult);
            }

            try
            {
                var response = await _chatRequestHandler.HandleImageChatRequestAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return _errorHandler.HandleException(ex, "Image query processing");
            }
        }
    }
}
