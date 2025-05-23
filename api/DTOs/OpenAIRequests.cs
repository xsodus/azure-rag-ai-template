namespace api.DTOs
{
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
