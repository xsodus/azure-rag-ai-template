using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Azure;

// Add Azure OpenAI package
using Azure.AI.OpenAI;

// Flag to show citations
bool showCitations = false;

// Get configuration settings  
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
string oaiEndpoint = config["AzureOAIEndpoint"] ?? "";
string oaiKey = config["AzureOAIKey"] ?? "";
string oaiDeploymentName = config["AzureOAIDeploymentName"] ?? "";
string azureSearchEndpoint = config["AzureSearchEndpoint"] ?? "";
string azureSearchKey = config["AzureSearchKey"] ?? "";
string azureSearchIndex = config["AzureSearchIndex"] ?? "";

// Initialize the Azure OpenAI client
OpenAIClient client = new OpenAIClient(new Uri(oaiEndpoint), new AzureKeyCredential(oaiKey));

// Configure your data source
AzureSearchChatExtensionConfiguration ownDataConfig = new()
{
    SearchEndpoint = new Uri(azureSearchEndpoint),
    Authentication = new OnYourDataApiKeyAuthenticationOptions(azureSearchKey),
    IndexName = azureSearchIndex
};

// Send request to Azure OpenAI model  
Console.WriteLine("...Sending the following request to Azure OpenAI endpoint...");  

Stream imageStream = File.OpenRead("<local-path>/somewhere.png");

// Chat with image input
ChatMessageImageContentItem imageContentItem = new ChatMessageImageContentItem(
    imageStream,
    "image/png",
    ChatMessageImageDetailLevel.Low
);

/*ChatMessageImageContentItem imageContentItem = new ChatMessageImageContentItem(
    new Uri("https://upload.wikimedia.org/wikipedia/commons/8/85/Clock_Tower_-_Palace_of_Westminster%2C_London_-_May_2007_icon.png"),
    ChatMessageImageDetailLevel.Low
);*/

ChatRequestSystemMessage systemMessage = new ChatRequestSystemMessage(
    "You're travel agent assistant who can build the travel plan and optimize the budget for the customer. I need you to return the result as JSON data below:" +
    "{\"place_name\":\"<place_name>\", \"place_description\":\"<place_description>\"}"
);

// Ask the model about the place in the image
ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        systemMessage,
        new ChatRequestUserMessage(
            new ChatMessageTextContentItem("What is this place in the image?"),
            imageContentItem
        )
    },
    Temperature = 0.2f,
    DeploymentName = oaiDeploymentName
};

ChatCompletions response = client.GetChatCompletions(chatCompletionsOptions);

ChatResponseMessage responseMessage = response.Choices[0].Message;

// Convert JSON string to object
var responseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(responseMessage.Content);

if(responseJson == null)
{
    Console.WriteLine("No response from the model.");
    return;
}

var nextQuestion = $"Give me 3 hotels that is closer to this place: {responseJson["place_description"]}";

Console.WriteLine("Image Response: " + responseMessage.Content + "\n");

Console.WriteLine("Next Question: " + nextQuestion + "\n");

// Ask the model about the hotels near the place with  RAG data source
chatCompletionsOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        systemMessage,
        new ChatRequestUserMessage(nextQuestion)
    },
    Temperature = 0.2f,
    DeploymentName = oaiDeploymentName,
    // Embed a vector data source (RAG)
    AzureExtensionsOptions = new AzureChatExtensionsOptions()
    {
        Extensions = {ownDataConfig}
    }
};

response = client.GetChatCompletions(chatCompletionsOptions);

responseMessage = response.Choices[0].Message;

// Print response
Console.WriteLine("Final Response: " + responseMessage.Content + "\n");

if (showCitations)
{
    Console.WriteLine($"\n  Citations of data used:");

    foreach (AzureChatExtensionDataSourceResponseCitation citation in responseMessage.AzureExtensionsContext.Citations)
    {
        Console.WriteLine($"    Citation: {citation.Title} - {citation.Url}");
    }
}

