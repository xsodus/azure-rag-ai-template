namespace api.DTOs
{
    /// <summary>
    /// Response model for image-based queries with RAG follow-up
    /// </summary>
    public class ImageQueryResponse
    {
        public string ImageResponse { get; set; } = string.Empty;
        public RAGResponse FollowUpResponse { get; set; } = new RAGResponse();
    }

    /// <summary>
    /// Response object for RAG queries including answer and citations
    /// </summary>
    public class RAGResponse
    {
        public string Answer { get; set; } = string.Empty;
        public List<Citation> Citations { get; set; } = new List<Citation>();
    }

    /// <summary>
    /// Citation from a RAG source
    /// </summary>
    public class Citation
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
