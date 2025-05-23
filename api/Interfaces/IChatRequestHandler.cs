using api.DTOs;

namespace api.Interfaces
{
    /// <summary>
    /// Interface for handling OpenAI chat requests following Single Responsibility Principle
    /// </summary>
    public interface IChatRequestHandler
    {
        /// <summary>
        /// Handle text-based chat request
        /// </summary>
        Task<RAGResponse> HandleChatRequestAsync(QueryRequest request);

        /// <summary>
        /// Handle image-based chat request with follow-up
        /// </summary>
        Task<ImageQueryResponse> HandleImageChatRequestAsync(ImageQueryRequest request);
    }
}
