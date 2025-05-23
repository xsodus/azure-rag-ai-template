using api.DTOs;

namespace api.Interfaces
{
    /// <summary>
    /// Interface for request validation following Single Responsibility Principle
    /// </summary>
    public interface IRequestValidator
    {
        /// <summary>
        /// Validate query request
        /// </summary>
        ValidationResult ValidateQueryRequest(QueryRequest request);

        /// <summary>
        /// Validate image query request
        /// </summary>
        ValidationResult ValidateImageQueryRequest(ImageQueryRequest request);
    }

    /// <summary>
    /// Result of request validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
