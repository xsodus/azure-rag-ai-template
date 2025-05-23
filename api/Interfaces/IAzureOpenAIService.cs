using api.DTOs;

namespace api.Interfaces
{
    /// <summary>
    /// Interface for Azure OpenAI operations following Interface Segregation Principle
    /// </summary>
    public interface IAzureOpenAIService
    {
        /// <summary>
        /// Send a text-based query to Azure OpenAI with RAG support
        /// </summary>
        Task<RAGResponse> GetCompletionAsync(
            string systemPrompt, 
            string userQuery, 
            bool useRAG = true, 
            float temperature = 0.2f);

        /// <summary>
        /// Send an image-based query to Azure OpenAI followed by a RAG-enhanced follow-up query
        /// </summary>
        Task<(string ImageResponse, RAGResponse FollowUpResponse)> GetImageAndRAGCompletionAsync(
            string systemPrompt,
            string initialImageQuery,
            string imageUrl,
            string followUpTemplate,
            float temperature = 0.2f);
    }
}
