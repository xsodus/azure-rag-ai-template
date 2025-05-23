using api.DTOs;
using api.Interfaces;

namespace api.Services
{
    /// <summary>
    /// Implementation of request validation following Single Responsibility Principle
    /// </summary>
    public class RequestValidator : IRequestValidator
    {
        public ValidationResult ValidateQueryRequest(QueryRequest request)
        {
            var result = new ValidationResult { IsValid = true };

            if (request == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Request cannot be null";
                result.Errors.Add("Request is null");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.UserQuery))
            {
                result.IsValid = false;
                result.ErrorMessage = "User query is required";
                result.Errors.Add("UserQuery is required and cannot be empty");
            }

            if (request.Temperature < 0 || request.Temperature > 2)
            {
                result.IsValid = false;
                result.ErrorMessage = "Temperature must be between 0 and 2";
                result.Errors.Add("Temperature must be between 0 and 2");
            }

            return result;
        }

        public ValidationResult ValidateImageQueryRequest(ImageQueryRequest request)
        {
            var result = new ValidationResult { IsValid = true };

            if (request == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Request cannot be null";
                result.Errors.Add("Request is null");
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                result.IsValid = false;
                result.ErrorMessage = "Image URL is required";
                result.Errors.Add("ImageUrl is required and cannot be empty");
            }
            else if (!Uri.TryCreate(request.ImageUrl, UriKind.Absolute, out var uri))
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid image URL format";
                result.Errors.Add("ImageUrl must be a valid URL");
            }

            if (string.IsNullOrWhiteSpace(request.InitialImageQuery))
            {
                result.IsValid = false;
                result.ErrorMessage = "Initial image query is required";
                result.Errors.Add("InitialImageQuery is required and cannot be empty");
            }

            if (request.Temperature < 0 || request.Temperature > 2)
            {
                result.IsValid = false;
                result.ErrorMessage = "Temperature must be between 0 and 2";
                result.Errors.Add("Temperature must be between 0 and 2");
            }

            return result;
        }
    }
}
