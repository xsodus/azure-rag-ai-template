using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// Request model for text-based queries
    /// </summary>
    public class QueryRequest
    {
        [StringLength(1000, ErrorMessage = "System prompt cannot exceed 1000 characters")]
        public string SystemPrompt { get; set; } = "You are a helpful assistant.";
        
        [Required(ErrorMessage = "User query is required")]
        [StringLength(4000, MinimumLength = 1, ErrorMessage = "User query must be between 1 and 4000 characters")]
        public string UserQuery { get; set; } = "";
        
        public bool UseRAG { get; set; } = true;
        
        [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0 and 2")]
        public float Temperature { get; set; } = 0.2f;
    }

    /// <summary>
    /// Request model for image-based queries with RAG follow-up
    /// </summary>
    public class ImageQueryRequest
    {
        [StringLength(1000, ErrorMessage = "System prompt cannot exceed 1000 characters")]
        public string SystemPrompt { get; set; } = "You are a helpful assistant.";
        
        [Required(ErrorMessage = "Initial image query is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Initial image query must be between 1 and 1000 characters")]
        public string InitialImageQuery { get; set; } = "What is shown in this image?";
        
        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "A valid image URL is required")]
        public string ImageUrl { get; set; } = "";
        
        [Required(ErrorMessage = "Follow-up template is required")]
        [StringLength(1000, ErrorMessage = "Follow-up template cannot exceed 1000 characters")]
        public string FollowUpTemplate { get; set; } = "Tell me more about: {0}";
        
        [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0 and 2")]
        public float Temperature { get; set; } = 0.2f;
    }
}
