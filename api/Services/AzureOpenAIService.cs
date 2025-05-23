using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    public class AzureOpenAIService
    {
        private readonly OpenAIClient _client;
        private readonly string _deploymentName;
        private readonly AzureSearchChatExtensionConfiguration _dataSourceConfig;
        private readonly bool _showCitations;

        public AzureOpenAIService(IConfiguration configuration)
        {
            // Get configuration settings from appsettings.json
            string oaiEndpoint = configuration["AzureOpenAI:Endpoint"] ?? throw new ArgumentNullException("AzureOpenAI:Endpoint");
            string oaiKey = configuration["AzureOpenAI:Key"] ?? throw new ArgumentNullException("AzureOpenAI:Key");
            _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? throw new ArgumentNullException("AzureOpenAI:DeploymentName");
            
            string azureSearchEndpoint = configuration["AzureSearch:Endpoint"] ?? throw new ArgumentNullException("AzureSearch:Endpoint");
            string azureSearchKey = configuration["AzureSearch:Key"] ?? throw new ArgumentNullException("AzureSearch:Key");
            string azureSearchIndex = configuration["AzureSearch:IndexName"] ?? throw new ArgumentNullException("AzureSearch:IndexName");
            
            // Configuration for showing citations (could be moved to appsettings)
            _showCitations = true;

            // Initialize the Azure OpenAI client
            _client = new OpenAIClient(new Uri(oaiEndpoint), new AzureKeyCredential(oaiKey));

            // Configure your data source for RAG
            _dataSourceConfig = new AzureSearchChatExtensionConfiguration
            {
                SearchEndpoint = new Uri(azureSearchEndpoint),
                Authentication = new OnYourDataApiKeyAuthenticationOptions(azureSearchKey),
                IndexName = azureSearchIndex
            };
        }

        /// <summary>
        /// Send a text-based query to Azure OpenAI with RAG support
        /// </summary>
        /// <param name="systemPrompt">The system prompt to set the context for the AI</param>
        /// <param name="userQuery">The user's query</param>
        /// <param name="useRAG">Whether to use RAG (Retrieval Augmented Generation)</param>
        /// <param name="temperature">The temperature setting for the model (controls randomness)</param>
        /// <returns>The response object containing both the answer and optional citations</returns>
        public async Task<RAGResponse> GetCompletionAsync(
            string systemPrompt, 
            string userQuery, 
            bool useRAG = true, 
            float temperature = 0.2f)
        {
            // Initialize system message
            ChatRequestSystemMessage systemMessage = new ChatRequestSystemMessage(systemPrompt);

            // Set up chat options
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    systemMessage,
                    new ChatRequestUserMessage(userQuery)
                },
                Temperature = temperature,
                DeploymentName = _deploymentName
            };

            // Add RAG data source if requested
            if (useRAG)
            {
                chatCompletionsOptions.AzureExtensionsOptions = new AzureChatExtensionsOptions()
                {
                    Extensions = { _dataSourceConfig }
                };
            }

            // Get completion from Azure OpenAI
            ChatCompletions response = await _client.GetChatCompletionsAsync(chatCompletionsOptions);
            ChatResponseMessage responseMessage = response.Choices[0].Message;

            // Build response object
            RAGResponse ragResponse = new RAGResponse
            {
                Answer = responseMessage.Content
            };

            // Add citations if they exist and if configured to show them
            if (_showCitations && responseMessage.AzureExtensionsContext?.Citations != null)
            {
                foreach (AzureChatExtensionDataSourceResponseCitation citation in 
                        responseMessage.AzureExtensionsContext.Citations)
                {
                    ragResponse.Citations.Add(new Citation
                    {
                        Title = citation.Title,
                        Url = citation.Url
                    });
                }
            }

            return ragResponse;
        }

        /// <summary>
        /// Send an image-based query to Azure OpenAI followed by a RAG-enhanced follow-up query
        /// </summary>
        /// <param name="systemPrompt">The system prompt to set the context for the AI</param>
        /// <param name="initialImageQuery">The initial query about the image</param>
        /// <param name="imageUrl">URL of the image to analyze</param>
        /// <param name="followUpTemplate">Template for follow-up query, use {0} for image response insertion</param>
        /// <param name="temperature">The temperature setting for the model (controls randomness)</param>
        /// <returns>A tuple containing both the image analysis response and the RAG-enhanced follow-up response</returns>
        public async Task<(string ImageResponse, RAGResponse FollowUpResponse)> GetImageAndRAGCompletionAsync(
            string systemPrompt,
            string initialImageQuery,
            string imageUrl,
            string followUpTemplate,
            float temperature = 0.2f)
        {
            // Initialize system message
            ChatRequestSystemMessage systemMessage = new ChatRequestSystemMessage(systemPrompt);

            // Create image content item
            ChatMessageImageContentItem imageContentItem = new ChatMessageImageContentItem(
                new Uri(imageUrl),
                ChatMessageImageDetailLevel.Low
            );

            // Initial image query options
            ChatCompletionsOptions initialQueryOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    systemMessage,
                    new ChatRequestUserMessage(
                        new ChatMessageTextContentItem(initialImageQuery),
                        imageContentItem
                    )
                },
                Temperature = temperature,
                DeploymentName = _deploymentName
            };

            // Send initial image-based query
            ChatCompletions initialResponse = await _client.GetChatCompletionsAsync(initialQueryOptions);
            ChatResponseMessage initialResponseMessage = initialResponse.Choices[0].Message;
            string imageResponse = initialResponseMessage.Content;

            // Try to parse JSON response if expected, otherwise use raw response
            string descriptionForFollowUp;
            try 
            {
                var responseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(imageResponse);
                descriptionForFollowUp = responseJson?["place_description"] ?? imageResponse;
            }
            catch
            {
                // If not valid JSON, use the full response
                descriptionForFollowUp = imageResponse;
            }

            // Create follow-up query with RAG
            string followUpQuery = string.Format(followUpTemplate, descriptionForFollowUp);
            
            // Get RAG-enhanced follow-up response
            RAGResponse followUpResponse = await GetCompletionAsync(
                systemPrompt,
                followUpQuery,
                true,  // Always use RAG for follow-up
                temperature
            );

            return (imageResponse, followUpResponse);
        }
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
