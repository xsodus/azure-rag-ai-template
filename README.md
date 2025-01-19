Collecting workspace information

To set up and run this project, follow these steps:

## Prerequisites

1. [.NET SDK 7.0](https://dotnet.microsoft.com/download/dotnet/7.0) or later
2. [Azure Subscription](https://azure.microsoft.com/en-us/free/)
3. [Azure OpenAI Service](https://azure.microsoft.com/en-us/services/cognitive-services/openai-service/)
4. [Azure Cognitive Search](https://azure.microsoft.com/en-us/services/search/)

## Azure Resource Setup

1. **Create Azure OpenAI Resource**:
    - Go to the Azure portal and create a new resource in the `East US` region with the `gpt-4o-mini` model, which supports both text and image input at the lowest price.
    - On the Azure OpenAI page, create a new resource with the following details:
        - **Subscription**: Your subscription group
        - **Resource group**: The resource group name from step 1
        - **Name**: New resource name
        - **Pricing tier**: Standard S0

2. **Create Storage Account**:
    - Open the Storage accounts page and create a new resource with the following details:
        - **Subscription**: Your subscription group
        - **Resource group**: The resource group name from step 1
        - **Storage account name**: New account name
        - **Region**: East US
        - **Primary service**: Azure Blob Storage or Azure Data Lake Storage Gen 2
        - **Performance**: Standard
        - **Redundancy**: Locally-redundant storage (LRS)

3. **Setup Storage Container**:
    - Go to the new storage account page, then navigate to Data storage > Containers.
    - Create a new container.
    - Enter the new container and upload the data source from [this link](https://raw.githubusercontent.com/MicrosoftLearning/mslearn-openai/refs/heads/main/Labfiles/06-use-own-data/data/brochures.zip). These files contain travel brochures that will be vectorized as embedded data for AI.

4. **Create Azure Cognitive Search Resource**:
    - Open the AI Search page and create a new resource with the following details:
        - **Resource Group**: The resource group name from step 1
        - **Service name**: New service name
        - **Location**: East US
        - **Pricing Tier**: Free

5. **Configure Azure Cognitive Search**:
    - Go to the new AI Search resource and click on `Import and vectorize data`.
    - Select `Azure Blob Storage` on the `Set up your data connect` page.
    - Configure your Azure Blob Storage with the following details:
        - **Subscription**: Your subscription group
        - **Storage account**: The storage account from step 2
        - **Blob container**: The blob container from step 3
        - **Blob folder**: Leave it empty
        - **Parsing mode**: Default
        - **Enable deletion tracking**: Uncheck
        - **Authenticate using managed identity**: Uncheck
    - Click `Next` to proceed to the `Vectorize your text` step and fill in the following details:
        - **Kind**: Azure OpenAI
        - **Subscription**: Your subscription group
        - **Azure OpenAI service**: The OpenAI resource from step 1
        - **Model deployment**: Select `text-embedding-ada-002` (Deploy this model through Azure Foundry Service if not available)
        - Check the acknowledgment box for additional costs.
    - Click `Next` on the `Vectorize and enrich your images` step (no changes needed).
    - Click `Next` on the `Advance settings` step (no changes needed).
    - Click `Next` on the `Review and create` step to create the vector index.

6. **Deploy and Configure OpenAI Model**:
    - Go back to the OpenAI resource from step 1 and click `Go to Azure AI Foundry portal`.
    - Deploy `gpt-4o-mini` with `Global Standard` and set the rate limit to 16K tokens per minute.
    - Copy the `KEY1` and `Endpoint` from Resource Management > Keys and Endpoint, then paste them into `AzureOAIKey` and `AzureOAIEndpoint` in `appsettings.json`.
    - Copy the AI model deployment name and paste it into `AzureOAIDeploymentName` in `appsettings.json`.

7. **Configure Azure Cognitive Search in appsettings.json**:
    - Go to the AI Search resource from step 4.
    - Copy the `Url` from the `Overview` page and paste it into `AzureSearchEndpoint` in `appsettings.json`.
    - Go to `Settings` > `Keys`, then copy the `Primary admin key` and paste it into `AzureSearchKey` in `appsettings.json`.
    - Go to `Search management` > `Indexes`, then copy the `vector-xxx` name and paste it into `AzureSearchIndex` in `appsettings.json`.


## Setup

1. Clone the repository:
    ```sh
    git clone git@github.com:xsodus/azure-rag-ai-template.git
    cd azure-rag-ai-template
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Update the 

appsettings.json

 file with your Azure OpenAI and Azure Cognitive Search credentials:
    ```json
    {
        "AzureOAIEndpoint": "<your-azure-openai-endpoint>",
        "AzureOAIKey": "<your-azure-openai-key>",
        "AzureOAIDeploymentName": "<your-azure-openai-deployment-name>",
        "AzureSearchEndpoint": "<your-azure-search-endpoint>",
        "AzureSearchKey": "<your-azure-search-key>",
        "AzureSearchIndex": "<your-azure-search-index>"
    }
    ```

## Running the Project

1. Build the project:
    ```sh
    dotnet build
    ```

2. Run the project:
    ```sh
    dotnet run
    ```

## Usage

The project will initialize the Azure OpenAI client and send a request to the Azure OpenAI endpoint. It will then process the response and print the results to the console.

For more details, refer to the source code in 

RAGAI.cs

