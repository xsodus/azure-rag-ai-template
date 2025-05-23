# Azure RAG AI Template

This template helps you build Retrieval-Augmented Generation (RAG) AI applications using Azure OpenAI and Azure AI Search. Follow the steps below to understand RAG, set up Azure, and run the code or API.

---

## 1. What is Retrieval-Augmented Generation (RAG)?

RAG combines information retrieval and AI generation. When a user asks a question, the system:
1. Retrieves relevant documents from a large data source (like PDFs or images in Azure Blob Storage) using Azure Cognitive Search and vector embeddings.
2. Uses Azure OpenAI (e.g., GPT-4) to generate a response based on both the user’s question and the retrieved documents.

This approach gives more accurate, context-aware answers by grounding AI responses in your own data.

---

## 2. Azure RAG AI Architecture Diagram

![Azure RAG AI Diagram](image/azure-rag-ai-diagram.png)

**How it works:**

1. **User / Application**: The user sends a message (text or image) to the application.
2. **OpenAI gpt-4o-mini**: The message is processed by the GPT-4o-mini model, which can use context from the vector data (retrieved documents) via AI Search.
3. **AI Search with Vector Index**: The system searches for relevant content using a vector index built from your documents. This index is created using embeddings from the OpenAI text-embedding-ada-002 model.
4. **OpenAI text-embedding-ada-002**: This model generates vector embeddings for your documents, making them searchable by semantic meaning.
5. **Blob Storage**: All your source documents (PDFs, images, Excel files, etc.) are stored here. The embedding model processes these files to create the vector index.

The flow: User → GPT-4o-mini → (AI Search) → Vector Index (built from Blob Storage via embeddings) → Response to User

---

## 3. How to Configure Azure

You’ll need:
- An Azure subscription
- Azure OpenAI resource (with GPT-4o-mini and text-embedding-ada-002 models)
- Azure Blob Storage (for your documents)
- Azure Cognitive Search (for vector search)

**Step-by-step setup:**

1. **Create Azure OpenAI Resource**
   - In the Azure portal, create an OpenAI resource in the `East US` region.
   - Deploy the `gpt-4o-mini` model (supports text and image input).
   - Deploy the `text-embedding-ada-002` model for vector embeddings.

2. **Create Storage Account**
   - Create a Storage Account (Standard, LRS) in the same region.
   - In the storage account, create a container and upload your data (e.g., brochures.zip from [this link](https://raw.githubusercontent.com/MicrosoftLearning/mslearn-openai/refs/heads/main/Labfiles/06-use-own-data/data/brochures.zip)).

3. **Create Azure Cognitive Search Resource**
   - Create a Cognitive Search resource (Free tier is fine) in the same region.
   - In the Cognitive Search portal, use “Import and vectorize data” to connect to your Blob Storage and set up a vector index using the `text-embedding-ada-002` model.

4. **Configure Keys and Endpoints**
   - In the Azure portal, copy the following values:
     - OpenAI: `KEY1`, `Endpoint`, and deployment name
     - Cognitive Search: `Url`, `Primary admin key`, and index name
   - Paste these into the `appsettings.json` file in both `api/` and `simple-code/` folders:
     - `AzureOAIKey`, `AzureOAIEndpoint`, `AzureOAIDeploymentName`
     - `AzureSearchEndpoint`, `AzureSearchKey`, `AzureSearchIndex`

---

## 4. How to Run the Simple Code Example

The `simple-code/` folder contains a minimal example (`RAGAI.cs`) showing how to use Azure OpenAI and Cognitive Search together.

**Steps:**
1. Open a terminal and navigate to the folder:
   ```sh
   cd simple-code
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Build the project:
   ```sh
   dotnet build
   ```
4. Run the example:
   ```sh
   dotnet run
   ```

---

## 5. How to Run the REST API

The `api/` folder contains a REST API with endpoints for chat and RAG operations.

**Steps:**
1. Navigate to the API folder:
   ```sh
   cd api
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Build the API:
   ```sh
   dotnet build
   ```
4. Run the API:
   ```sh
   dotnet run
   ```
5. Access the API documentation at:
   - [https://localhost:7222/swagger/index.html](https://localhost:7222/swagger/index.html)

**Main Endpoints:**
- `POST /api/v1/AzureOpenAI/chat` — Text-based RAG chat
- `POST /api/v1/AzureOpenAI/chat-with-image` — Image-based RAG chat

---

## 6. How to Run the Unit Tests

Unit and integration tests are in the `api.Tests/` folder.

**Steps:**
1. Navigate to the test folder:
   ```sh
   cd api.Tests
   ```
2. Run the tests:
   ```sh
   dotnet test
   ```

---

## Additional Features
- SOLID architecture for maintainability
- Rate limiting and validation
- In-memory mode for local development
- Swagger UI for API exploration

For more details, see the code comments and the architecture diagram in `image/azure-rag-ai-diagram.png`.
