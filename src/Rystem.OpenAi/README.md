# Unofficial Fluent C#/.NET SDK for accessing the OpenAI API (Easy swap among OpenAi and Azure OpenAi)

## Last update with Cost and Tokens calculation

A simple C# .NET wrapper library to use with [OpenAI](https://openai.com/)'s API.

[![MIT License](https://img.shields.io/github/license/dotnet/aspnetcore?color=%230b0&style=flat-square)](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/LICENSE.txt)
[![Discord](https://img.shields.io/discord/732297728826277939?style=flat-square&label=Discord&logo=discord&logoColor=white&color=7289DA)](https://discord.gg/wUh2fppr)
[![OpenAi.Nuget](https://github.com/KeyserDSoze/Rystem.OpenAi/actions/workflows/PackageDeploy.OpenAi.yml/badge.svg)](https://github.com/KeyserDSoze/Rystem.OpenAi/actions/workflows/PackageDeploy.OpenAi.yml)

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/summary/new_code?id=KeyserDSoze_Rystem.OpenAi)
![image](https://user-images.githubusercontent.com/40713438/236708265-6e53c2f4-f8fd-47d4-ad68-ed92ee31024b.png)

## Help the project

### Contribute: https://www.buymeacoffee.com/keyserdsoze
### Contribute: https://patreon.com/Rystem

## [Stars](https://github.com/KeyserDSoze/Rystem.OpenAi/stargazers)

## Requirements

This library targets .NET 9 and above.

### Adv
Watch out my Rystem framework to be able to do .Net webapp faster (easy integration with repository pattern or CQRS for your Azure services).
### [What is Rystem?](https://github.com/KeyserDSoze/Rystem)

## Setup

Install package Rystem.OpenAi [from Nuget](https://www.nuget.org/packages/Rystem.OpenAi/).
Here's how via command line:

```powershell
Install-Package Rystem.OpenAi
```

## Documentation

### Table of Contents

- [Unofficial Fluent C#/.NET SDK for accessing the OpenAI API (Easy swap among OpenAi and Azure OpenAi)](#unofficial-fluent-cnet-sdk-for-accessing-the-openai-api-easy-swap-among-openai-and-azure-openai)
  - [Last update with Cost and Tokens calculation](#last-update-with-cost-and-tokens-calculation)
  - [Help the project](#help-the-project)
    - [Contribute: https://www.buymeacoffee.com/keyserdsoze](#contribute-httpswwwbuymeacoffeecomkeyserdsoze)
    - [Contribute: https://patreon.com/Rystem](#contribute-httpspatreoncomrystem)
  - [Stars](#stars)
  - [Requirements](#requirements)
    - [Adv](#adv)
    - [What is Rystem?](#what-is-rystem)
  - [Setup](#setup)
  - [Documentation](#documentation)
    - [Table of Contents](#table-of-contents)
  - [Startup Setup](#startup-setup)
  - [Dependency Injection](#dependency-injection)
    - [Add to service collection the OpenAi service in your DI](#add-to-service-collection-the-openai-service-in-your-di)
  - [Dependency Injection With Azure](#dependency-injection-with-azure)
    - [Add to service collection the OpenAi service in your DI with Azure integration](#add-to-service-collection-the-openai-service-in-your-di-with-azure-integration)
    - [Add to service collection the OpenAi service in your DI with Azure integration and app registration](#add-to-service-collection-the-openai-service-in-your-di-with-azure-integration-and-app-registration)
    - [Add to service collection the OpenAi service in your DI with Azure integration and system assigned managed identity](#add-to-service-collection-the-openai-service-in-your-di-with-azure-integration-and-system-assigned-managed-identity)
    - [Add to service collection the OpenAi service in your DI with Azure integration and user assigned managed identity](#add-to-service-collection-the-openai-service-in-your-di-with-azure-integration-and-user-assigned-managed-identity)
  - [Use different version](#use-different-version)
  - [Dependency Injection With Factory](#dependency-injection-with-factory)
  - [Without Dependency Injection](#without-dependency-injection)
  - [Models](#models)
    - [List Models](#list-models)
    - [Retrieve Models](#retrieve-models)
    - [Delete fine-tune model](#delete-fine-tune-model)
  - [Chat](#chat)
    - [Chat Streaming](#chat-streaming)
    - [Chat functions](#chat-functions)
      - [Simple function configuration](#simple-function-configuration)
  - [Images](#images)
    - [Create Image](#create-image)
    - [Create Image Edit](#create-image-edit)
    - [Create Image Variation](#create-image-variation)
  - [Embeddings](#embeddings)
    - [Create Embedding](#create-embedding)
    - [Distance for embedding](#distance-for-embedding)
    - [Which distance function should I use?](#which-distance-function-should-i-use)
  - [Audio](#audio)
    - [Create Transcription](#create-transcription)
    - [Create Translation](#create-translation)
  - [File](#file)
    - [List files](#list-files)
    - [Upload file](#upload-file)
    - [Delete file](#delete-file)
    - [Retrieve file](#retrieve-file)
    - [Retrieve file content](#retrieve-file-content)
  - [Fine-Tunes](#fine-tunes)
    - [Create fine-tune](#create-fine-tune)
    - [List fine-tunes](#list-fine-tunes)
    - [Retrieve fine-tune](#retrieve-fine-tune)
    - [Cancel fine-tune](#cancel-fine-tune)
    - [List fine-tune events](#list-fine-tune-events)
    - [List fine-tune events as stream](#list-fine-tune-events-as-stream)
    - [Delete fine-tune model](#delete-fine-tune-model-1)
  - [Moderations](#moderations)
    - [Create moderation](#create-moderation)
  - [Utilities](#utilities)
    - [Cosine Similarity](#cosine-similarity)
    - [Tokens](#tokens)
    - [Cost](#cost)
    - [Setup price](#setup-price)
  - [Management](#management)
    - [Billing](#billing)
    - [Deployments](#deployments)


## Startup Setup
[📖 Back to summary](#documentation)\
You may install with Dependency Injection one or more than on integrations at the same time. Furthermore you don't need to use the Dependency Injection pattern and use a custom Setup.

## Dependency Injection
[📖 Back to summary](#documentation)

### Add to service collection the OpenAi service in your DI

    var apiKey = configuration["Azure:ApiKey"];
    services.AddOpenAi(settings =>
    {
        settings.ApiKey = apiKey;
        //add a default model for chatClient, you can add everything in this way to prepare at the best your
        //client for the request
        settings.DefaultRequestConfiguration.Chat = chatClient =>
        {
            chatClient.WithModel(configuration["OpenAi2:ModelName"]!);
        };
    });

## Dependency Injection With Azure

### Add to service collection the OpenAi service in your DI with Azure integration
When you want to use the integration with Azure, you need to specify all the models you're going to use. In the example you may find the model name for DavinciText3.
You still may add a custom model, with MapDeploymentCustomModel.

    builder.Services.AddOpenAi(settings =>
    {
        settings.ApiKey = apiKey;
        settings.Azure.ResourceName = "AzureResourceName (Name of your deployed service on Azure)";
        settings.Azure
            .MapDeploymentTextModel("Test (The name from column 'Model deployment name' in Model deployments blade in your Azure service)", TextModelType.DavinciText3);
    });

### Add to service collection the OpenAi service in your DI with Azure integration and app registration
See how to create an app registration [here](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app).

    var resourceName = builder.Configuration["Azure:ResourceName"];
    var clientId = builder.Configuration["AzureAd:ClientId"];
    var clientSecret = builder.Configuration["AzureAd:ClientSecret"];
    var tenantId = builder.Configuration["AzureAd:TenantId"];
    builder.Services.AddOpenAi(settings =>
    {
        settings.Azure.ResourceName = resourceName;
        settings.Azure.AppRegistration.ClientId = clientId;
        settings.Azure.AppRegistration.ClientSecret = clientSecret;
        settings.Azure.AppRegistration.TenantId = tenantId;
        settings.Azure
            .MapDeploymentTextModel("Test", TextModelType.CurieText)
            .MapDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
            .MapDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
    });

### Add to service collection the OpenAi service in your DI with Azure integration and system assigned managed identity
See how to create a managed identity [here](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview).\
[System Assigned Managed Identity](https://learn.microsoft.com/en-us/azure/automation/enable-managed-identity-for-automation)

    var resourceName = builder.Configuration["Azure:ResourceName"];
    builder.Services.AddOpenAi(settings =>
    {
        settings.Azure.ResourceName = resourceName;
        settings.Azure.ManagedIdentity.UseDefault = true;
        settings.Azure
            .MapDeploymentTextModel("Test", TextModelType.CurieText)
            .MapDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
            .MapDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
    });

### Add to service collection the OpenAi service in your DI with Azure integration and user assigned managed identity
See how to create a managed identity [here](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview).\
[User Assigned Managed Identity](https://learn.microsoft.com/en-us/azure/automation/add-user-assigned-identity)

    var resourceName = builder.Configuration["Azure:ResourceName"];
    var managedIdentityId = builder.Configuration["ManagedIdentity:ClientId"];
    builder.Services.AddOpenAi(settings =>
    {
        settings.Azure.ResourceName = resourceName;
        settings.Azure.ManagedIdentity.Id = managedIdentityId;
        settings.Azure
            .MapDeploymentTextModel("Test", TextModelType.CurieText)
            .MapDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
            .MapDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
    });

## Use different version
[📖 Back to summary](#documentation)\
You may install different version for each endpoint.

     services.AddOpenAi(settings =>
            {
                settings.ApiKey = azureApiKey;
                settings.Version = "2024-08-01-preview";
                settings
                    .UseVersionForChat("2023-03-15-preview");
            });

In this example We are adding a different version only for chat, and all the other endpoints will use the same (in this case the default version).

## Dependency Injection With Factory
[📖 Back to summary](#documentation)\
You may install more than one OpenAi integration, using name parameter in configuration.
In the next example we have two different configurations, one with OpenAi and a default name and with Azure OpenAi and name "Azure"

    var apiKey = context.Configuration["OpenAi:ApiKey"];
    services
        .AddOpenAi(settings =>
        {
            settings.ApiKey = apiKey;
        });
    var azureApiKey = context.Configuration["Azure:ApiKey"];
    var resourceName = context.Configuration["Azure:ResourceName"];
    var clientId = context.Configuration["AzureAd:ClientId"];
    var clientSecret = context.Configuration["AzureAd:ClientSecret"];
    var tenantId = context.Configuration["AzureAd:TenantId"];
    services.AddOpenAi(settings =>
    {
        settings.ApiKey = azureApiKey;
        settings
            .UseVersionForChat("2023-03-15-preview");
        settings.Azure.ResourceName = resourceName;
        settings.Azure.AppRegistration.ClientId = clientId;
        settings.Azure.AppRegistration.ClientSecret = clientSecret;
        settings.Azure.AppRegistration.TenantId = tenantId;
        settings.Azure
            .MapDeploymentTextModel("text-curie-001", TextModelType.CurieText)
            .MapDeploymentTextModel("text-davinci-003", TextModelType.DavinciText3)
            .MapDeploymentEmbeddingModel("OpenAiDemoModel", EmbeddingModelType.AdaTextEmbedding)
            .MapDeploymentChatModel("gpt35turbo", ChatModelType.Gpt35Turbo0301);
    }, "Azure");

I can retrieve the integration with IFactory<> interface (from Rystem) and the name of the integration.

    private readonly IFactory<IOpenAi> _openAiFactory;

    public CompletionEndpointTests(IFactory<IOpenAi> openAiFactory)
    {
        _openAiFactory = openAiFactory;
    }

    public async ValueTask DoSomethingWithDefaultIntegrationAsync()
    {
        var openAiApi = _openAiFactory.Create();
        openAiApi.Chat.........
    }

    public async ValueTask DoSomethingWithAzureIntegrationAsync()
    {
        var openAiApi = _openAiFactory.Create("Azure");
        openAiApi.Chat.........
    }

or get the more specific service

    private readonly IFactory<IOpenAiChat> _chatFactory;
    public Constructor(IFactory<IOpenAiChat> chatFactory)
    {
        _chatFactory = chatFactory;
    }
    public async ValueTask DoSomethingWithAzureIntegrationAsync()
    {
        var chat = _chatFactory.Create(name);
        chat.ExecuteRequestAsync(....);
    }

## Without Dependency Injection
[📖 Back to summary](#documentation)\
You may configure in a static constructor or during startup your integration without the dependency injection pattern.

      OpenAiService.Instance.AddOpenAi(settings =>
        {
            settings.ApiKey = apiKey;
        }, "NoDI");

and you can use it with the same static class OpenAiService and the static Create method

    var openAiApi = OpenAiService.Instance.Create(name);
    openAiApi.Embedding......

or get the more specific service

    var openAiEmbeddingApi = OpenAiService.Instance.CreateEmbedding(name);
    openAiEmbeddingApi.Request(....);

## Models
[📖 Back to summary](#documentation)\
List and describe the various models available in the API. You can refer to the [Models documentation](https://platform.openai.com/docs/models/overview) to understand what models are available and the differences between them.\
You may find more details [here](https://platform.openai.com/docs/api-reference/models),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ModelEndpointTests.cs) samples from unit test.

### List Models
Lists the currently available models, and provides basic information about each one such as the owner and availability.

    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Model.ListAsync();

### Retrieve Models
Retrieves a model instance, providing basic information about the model such as the owner and permissioning.

    var openAiApi = _openAiFactory.Create(name);
    var result = await openAiApi.Model.RetrieveAsync(TextModelType.DavinciText3.ToModelId());

### Delete fine-tune model
Delete a fine-tuned model. You must have the Owner role in your organization.

    var openAiApi = _openAiFactory.Create(name);
    var deleteResult = await openAiApi.Model
        .DeleteAsync(fineTuneModelId);

## Chat
[📖 Back to summary](#documentation)\
Given a chat conversation, the model will return a chat completion response.\
You may find more details [here](https://platform.openai.com/docs/api-reference/chat),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ChatEndpointTests.cs) samples from unit test.

    var openAiApi = _openAiFactory.Create(name)!;
    var results = await openAiApi.Chat
        .AddMessage(new ChatMessageRequest { Role = ChatRole.User, Content = "Hello!! How are you?" })
        .WithModel(ChatModelName.Gpt4_o)
        .WithTemperature(1)
        .ExecuteAsync();

### Chat Streaming

    var openAiApi = _openAiFactory.Create(name)!;
    var results = new List<ChunkChatResult>();
    await foreach (var x in openAiApi.Chat
        .AddMessage(new ChatMessageRequest { Role = ChatRole.System, Content = "You are a friend of mine." })
        .AddMessage(new ChatMessageRequest { Role = ChatRole.User, Content = "Hello!! How are you?" })
        .WithModel(ChatModelName.Gpt4_o)
        .WithTemperature(1)
        .WithStopSequence("alekud")
        .AddStopSequence("coltello")
        .WithNumberOfChoicesPerPrompt(1)
        .WithFrequencyPenalty(0)
        .WithPresencePenalty(0)
        .WithNucleusSampling(1)
        .SetMaxTokens(1200)
        .WithUser("KeyserDSoze")
        .ExecuteAsStreamAsync())
        {
            results.Add(x);
        }


### Chat functions
You may find the update [here](https://openai.com/blog/function-calling-and-other-api-updates)

#### Simple function configuration
Here an example based on the link, you may find it in unit test.

    var request = openAiApi.Chat
                .RequestWithUserMessage("What is the weather like in Boston?")
                .WithModel(ChatModelType.Gpt35Turbo)
                .WithFunction(new JsonFunction
                {
                    Name = functionName,
                    Description = "Get the current weather in a given location",
                    Parameters = new JsonFunctionNonPrimitiveProperty()
                        .AddPrimitive("location", new JsonFunctionProperty
                        {
                            Type = "string",
                            Description = "The city and state, e.g. San Francisco, CA"
                        })
                        .AddEnum("unit", new JsonFunctionEnumProperty
                        {
                            Type = "string",
                            Enums = new List<string> { "celsius", "fahrenheit" }
                        })
                        .AddRequired("location")
                });
    var response = await request
        .ExecuteAndCalculateCostAsync();
    var function = response.Result.Choices[0].Message.Function;
    var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(function.Arguments);
    request
        .AddFunctionMessage(functionName, "{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
    response = await request
        .ExecuteAndCalculateCostAsync();
    var content = response.Result.Choices[0].Message.Content;

In this case you receive as finish reason instead of "stop" the word "functionExecuted".

    Assert.Equal("functionExecuted", response.Result.Choices[0].FinishReason);

You can use some JsonProperty attribute like:

- JsonPropertyName: name of the property
- JsonPropertyDescription: description of what the property is.
- JsonRequired: to set as Required for OpenAi
- JsonPropertyAllowedValues: to have only a range of possible values for the property.
- JsonPropertyRange: to have a range of values
- JsonPropertyMaximum: to have a maximum value for the property
- JsonPropertyMinimum: to have a minimum value for the property
- JsonPropertyMultipleOf: to have only a multiple of a value for the property

After the configuration you can use this function framework in this way:

    var openAiApi = _openAiFactory.Create(name);
    var response = await openAiApi.Chat
        .RequestWithUserMessage("What is the weather like in Boston?")
        .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
        .WithFunction(WeatherFunction.NameLabel)
        .ExecuteAndCalculateCostAsync(true);

    var content = response.Result.Choices[0].Message.Content;

## Function chaining
You may find the PlayFramework [here](https://github.com/KeyserDSoze/Rystem.OpenAi/tree/master/src/Rystem.PlayFramework)

## Images
[📖 Back to summary](#documentation)\
Given a prompt and/or an input image, the model will generate a new image.\
You may find more details [here](https://platform.openai.com/docs/api-reference/images),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ImageEndpointTests.cs) samples from unit test.

### Create Image
Creates an image given a prompt.

    var openAiApi = _openAiFactory.Create(name)!;
    var response = await openAiApi.Image
        .WithSize(ImageSize.Large)
        .GenerateAsync("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.");
    var uri = response.Data?.FirstOrDefault();

Download directly and save as stream

    var openAiApi = _openAiFactory.Create(name)!;

    var response = await openAiApi.Image
        .WithSize(ImageSize.Large)
        .GenerateAsBase64Async("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.");

    var image = response.Data?.FirstOrDefault();
    var imageAsStream = image.ConvertToStream();

### Create Image Edit
Creates an edited or extended image given an original image and a prompt.

    var openAiApi = _openAiFactory.Create(name)!;
    var location = Assembly.GetExecutingAssembly().Location;
    location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
    using var readableStream = File.OpenRead($"{location}\\Files\\otter.png");
    var editableFile = new MemoryStream();
    await readableStream.CopyToAsync(editableFile);
    editableFile.Position = 0;

    var response = await openAiApi.Image
        .WithSize(ImageSize.Small)
        .WithNumberOfResults(2)
        .EditAsync("A cute baby sea otter wearing a beret", editableFile, "otter.png");

    var uri = response.Data?.FirstOrDefault();

### Create Image Variation
Creates a variation of a given image.

    var openAiApi = _openAiFactory.Create(name)!;

    var location = Assembly.GetExecutingAssembly().Location;
    location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
    using var readableStream = File.OpenRead($"{location}\\Files\\otter.png");
    var editableFile = new MemoryStream();
    await readableStream.CopyToAsync(editableFile);
    editableFile.Position = 0;
    var response = await openAiApi.Image
        .WithSize(ImageSize.Small)
        .WithNumberOfResults(1)
        .VariateAsync(editableFile, "otter.png");

    var uri = response.Data?.FirstOrDefault();

## Embeddings
[📖 Back to summary](#documentation)\
Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.\
You may find more details [here](https://platform.openai.com/docs/api-reference/embeddings),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/EmbeddingEndpointTests.cs) samples from unit test.


### Create Embedding
Creates an embedding vector representing the input text.

    var openAiApi = name == "NoDI" ? OpenAiServiceLocator.Instance.Create(name) : _openAiFactory.Create(name)!;

    var results = await openAiApi.Embeddings
        .WithInputs("A test text for embedding")
        .ExecuteAsync();

    var resultOfCosineSimilarity = _openAiUtility.CosineSimilarity(results.Data.First().Embedding!, results.Data.First().Embedding!);

### Create Embedding with custom dimensions
Creates an embedding with custom dimensions vector representing the input text.
Only supported in text-embedding-3 and later models.

    var openAiApi = name == "NoDI" ? OpenAiServiceLocator.Instance.Create(name) : _openAiFactory.Create(name)!;

    var results = await openAiApi.Embeddings
        .AddPrompt("A test text for embedding")
        .WithModel("text-embedding-3-large")
        .WithDimensions(999)
        .ExecuteAsync();

### Distance for embedding
For searching over many vectors quickly, we recommend using a vector database. You can find examples of working with vector databases and the OpenAI API in [our Cookbook](https://github.com/openai/openai-cookbook/tree/main/examples/vector_databases) on GitHub.
Vector database options include:

- [Pinecone](https://github.com/openai/openai-cookbook/tree/main/examples/vector_databases/pinecone), a fully managed vector database
- [Weaviate](https://github.com/openai/openai-cookbook/tree/main/examples/vector_databases/weaviate), an open-source vector search engine
- [Redis](https://github.com/openai/openai-cookbook/tree/main/examples/vector_databases/redis) as a vector database
- [Qdrant](https://github.com/openai/openai-cookbook/tree/main/examples/vector_databases/qdrant), a vector search engine
- [Milvus](https://github.com/openai/openai-cookbook/blob/main/examples/vector_databases/Using_vector_databases_for_embeddings_search.ipynb), a vector database built for scalable similarity search
- [Chroma](https://github.com/chroma-core/chroma), an open-source embeddings store

### Which distance function should I use?
We recommend cosine similarity. The choice of distance function typically doesn’t matter much.

OpenAI embeddings are normalized to length 1, which means that:

Cosine similarity can be computed slightly faster using just a dot product
Cosine similarity and Euclidean distance will result in the identical rankings

You may use the utility service in this repository to calculate in C# the distance with [Cosine similarity](#cosine-similarity)

## Audio
[📖 Back to summary](#documentation)\
You may find more details [here](https://platform.openai.com/docs/api-reference/audio),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/AudioEndpointTests.cs) samples from unit test.

### Create Transcription
Transcribes audio into the input language.

    var openAiApi = _openAiFactory.Create(name)!;
    var location = Assembly.GetExecutingAssembly().Location;
    location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
    using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");

    var editableFile = new MemoryStream();
    readableStream.CopyTo(editableFile);
    editableFile.Position = 0;

    var results = await openAiApi.Audio
        .WithFile(editableFile.ToArray(), "default.mp3")
        .WithTemperature(1)
        .WithLanguage(Language.Italian)
        .WithPrompt("Incidente")
        .TranscriptAsync();

### Create Translation
Translates audio into English.

    var openAiApi = _openAiFactory.Create(name)!;

    var location = Assembly.GetExecutingAssembly().Location;
    location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
    using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");
    var editableFile = new MemoryStream();
    await readableStream.CopyToAsync(editableFile);
    editableFile.Position = 0;

    var results = await openAiApi.Audio
        .WithTemperature(1)
        .WithPrompt("sample")
        .WithFile(editableFile.ToArray(), "default.mp3")
        .TranslateAsync();


## File
[📖 Back to summary](#documentation)\
Files are used to upload documents that can be used with features like Fine-tuning.\
You may find more details [here](https://platform.openai.com/docs/api-reference/files),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/FileEndpointTests.cs) samples from unit test.

### List files
Returns a list of files that belong to the user's organization.

      var openAiApi = _openAiFactory.Create(name);
      var results = await openAiApi.File
                .AllAsync();

### Upload file
Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.

    var openAiApi = _openAiFactory.Create(name);
    var uploadResult = await openAiApi.File
            .UploadFileAsync(editableFile, name);

### Delete file
Delete a file.

    var openAiApi = _openAiFactory.Create(name);
    var deleteResult = await openAiApi.File
            .DeleteAsync(uploadResult.Id);

### Retrieve file
Returns information about a specific file.

    var openAiApi = _openAiFactory.Create(name);
    var retrieve = await openAiApi.File
            .RetrieveAsync(uploadResult.Id);

### Retrieve file content
Returns the contents of the specified file

    var openAiApi = _openAiFactory.Create(name);
    var contentRetrieve = await openAiApi.File
            .RetrieveFileContentAsStringAsync(uploadResult.Id);

## Fine-Tunes
[📖 Back to summary](#documentation)\
Manage fine-tuning jobs to tailor a model to your specific training data.\
You may find more details [here](https://platform.openai.com/docs/api-reference/fine-tunes),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/FineTuneEndpointTests.cs) samples from unit test.

### Create fine-tune
Creates a job that fine-tunes a specified model from a given dataset.
Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.

    var openAiApi = _openAiFactory.Create(name);
    var createResult = await openAiApi.FineTune
                                .Create(fileId)
                                .ExecuteAsync();

### List fine-tunes
List your organization's fine-tuning jobs

    var openAiApi = _openAiFactory.Create(name);
    var allFineTunes = await openAiApi.FineTune
                        .ListAsync();

### Retrieve fine-tune
Gets info about the fine-tune job.

    var openAiApi = _openAiFactory.Create(name);
    var retrieveFineTune = await openAiApi.FineTune
                            .RetrieveAsync(fineTuneId);

### Cancel fine-tune
Immediately cancel a fine-tune job.

    var openAiApi = _openAiFactory.Create(name);
    var cancelResult = await openAiApi.FineTune
                            .CancelAsync(fineTuneId);

### List fine-tune events
Get fine-grained status updates for a fine-tune job.

    var openAiApi = _openAiFactory.Create(name);
    var events = await openAiApi.FineTune
                        .ListEventsAsync(fineTuneId);

### List fine-tune events as stream
Get fine-grained status updates for a fine-tune job.

    var openAiApi = _openAiFactory.Create(name);
    var events = await openAiApi.FineTune
                        .ListEventsAsStreamAsync(fineTuneId);

### Delete fine-tune model
Delete a fine-tuned model. You must have the Owner role in your organization.

    var openAiApi = _openAiFactory.Create(name);
    var deleteResult = await openAiApi.FineTune
        .DeleteAsync(fineTuneModelId);

## Moderations
[📖 Back to summary](#documentation)\
Given a input text, outputs if the model classifies it as violating OpenAI's content policy.\
You may find more details [here](https://platform.openai.com/docs/api-reference/moderations),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ModerationEndpointTests.cs) samples from unit test.

### Create moderation
Classifies if text violates OpenAI's Content Policy

    var openAiApi = _openAiFactory.Create(name)!;
    var results = await openAiApi.Moderation
        .WithModel("testModel")
        .WithModel(ModerationModelName.OmniLatest)
        .ExecuteAsync("I want to kill them and everyone else.");

    var categories = results.Results?.FirstOrDefault()?.Categories;


## Utilities
[📖 Back to summary](#documentation)\
Utilities for OpenAi, you can inject the interface IOpenAiUtility everywhere you need it.
In IOpenAiUtility you can find:

### Cosine Similarity
[📖 Back to embeddings](#embeddings)\
In data analysis, cosine similarity is a measure of similarity between two non-zero vectors defined in an inner product space. Cosine similarity is the cosine of the angle between the vectors; that is, it is the dot product of the vectors divided by the product of their lengths. It follows that the cosine similarity does not depend on the magnitudes of the vectors, but only on their angle. The cosine similarity always belongs to the interval [−1,1]. For example, two proportional vectors have a cosine similarity of 1, two orthogonal vectors have a similarity of 0, and two opposite vectors have a similarity of -1. In some contexts, the component values of the vectors cannot be negative, in which case the cosine similarity is bounded in [0,1].
Here an example from Unit test.

    IOpenAiUtility _openAiUtility;
    var resultOfCosineSimilarity = _openAiUtility.CosineSimilarity(results.Data.First().Embedding, results.Data.First().Embedding);
    var resultOfEuclideanDinstance = _openAiUtility.EuclideanDistance(results.Data.First().Embedding, results.Data.First().Embedding);
    Assert.True(resultOfCosineSimilarity >= 1);

Without DI, you need to setup an OpenAiService [without Dependency Injection](#without-dependency-injection) and after that you can use

    IOpenAiUtility openAiUtility = OpenAiService.Instance.Utility();

### Tokens
[📖 Back to summary](#documentation)\
You can think of tokens as pieces of words, where 1,000 tokens is about 750 words. You can calculate your request tokens with the Tokenizer service in Utility.

    IOpenAiUtility _openAiUtility
    var encoded = _openAiUtility.Tokenizer
        .WithChatModel(ChatModelType.Gpt4)
        .Encode(value);
    Assert.Equal(numberOfTokens, encoded.NumberOfTokens);
    var decoded = _openAiUtility.Tokenizer.Decode(encoded.EncodedTokens);
    Assert.Equal(value, decoded);

### Cost
[📖 Back to summary](#documentation)\
You can think of tokens as pieces of words, where 1,000 tokens is about 750 words.

    var openAiApi = _openAiFactory.Create(name)!;
    var results = await openAiApi.Chat
        .AddMessage(new ChatMessageRequest { Role = ChatRole.User, Content = "Hello!! How are you?" })
        .WithModel(ChatModelName.Gpt4_o)
        .WithTemperature(1)
        .ExecuteAsync();
    //calculate cost works only if you added the price during setup.
    var cost = openAiApi.Chat.CalculateCost();

### Setup price
[📖 Back to summary](#documentation)\
During setup of your OpenAi service you may add your custom price table with settings.PriceBuilder property.

    services.AddOpenAi(settings =>
    {
        //custom version for chat endpoint
        settings
            .UseVersionForChat("2024-08-01-preview");
        //resource name for Azure
        settings.Azure.ResourceName = resourceName;
        //app registration configuration for Azure authentication
        settings.Azure.AppRegistration.ClientId = clientId;
        settings.Azure.AppRegistration.ClientSecret = clientSecret;
        settings.Azure.AppRegistration.TenantId = tenantId;
        //map deployment for Azure for every request for chat endpoint with gpt-4 model.
        settings
            .MapDeploymentForEveryRequests(OpenAiType.Chat, "gpt-4");
        //default request configuration for chat endpoint, this method is ran during the creation of the chat service.
        settings.DefaultRequestConfiguration.Chat = chatClient =>
        {
            chatClient.ForceModel("gpt-4");
        };
        //add a price for kind of cost for model you want to add. Here an example with gpt-4 model.
        settings.PriceBuilder
            .AddModel("gpt-4",
            new OpenAiCost { Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.0000025m },
            new OpenAiCost { Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00000125m },
            new OpenAiCost { Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00001m });
    }, "Azure");

## Management
[📖 Back to summary](#documentation)\
In your openai dashboard you may get the billing usage, or users, or taxes, or similar. Here you have an api to retrieve this kind of data.

### Billing
[📖 Back to summary](#documentation)\
You may use the management endpoint to retrieve data for your usage. Here an example on how to get the usage for the month of april.

    var management = _openAiFactory.CreateManagement(integrationName);
    var usages = await management
        .Billing
        .From(new DateTime(2023, 4, 1))
        .To(new DateTime(2023, 4, 30))
        .GetUsageAsync();
    Assert.NotEmpty(usages.DailyCosts);

### Deployments
[📖 Back to summary](#documentation)\
Only for Azure you have to deploy a model to use model in your application. You can configure Deployment during startup of your application.

    services.AddOpenAi(settings =>
    {
        settings.ApiKey = azureApiKey;
        settings
            .UseVersionForChat("2023-03-15-preview");
        settings.Azure.ResourceName = resourceName;
        settings.Azure.AppRegistration.ClientId = clientId;
        settings.Azure.AppRegistration.ClientSecret = clientSecret;
        settings.Azure.AppRegistration.TenantId = tenantId;
        settings.Azure
            .MapDeploymentTextModel("text-curie-001", TextModelType.CurieText)
            .MapDeploymentTextModel("text-davinci-003", TextModelType.DavinciText3)
            .MapDeploymentEmbeddingModel("OpenAiDemoModel", EmbeddingModelType.AdaTextEmbedding)
            .MapDeploymentChatModel("gpt35turbo", ChatModelType.Gpt35Turbo0301)
            .MapDeploymentCustomModel("ada001", "text-ada-001");
        settings.Price
            .SetFineTuneForAda(0.0004M, 0.0016M)
            .SetAudioForTranslation(0.006M);
    }, "Azure");

During startup you can configure other deployments on your application or on Azure.

    var app = builder.Build();
    await app.Services.MapDeploymentsAutomaticallyAsync(true);

or a specific integration or list of integration that you setup previously.

    await app.Services.MapDeploymentsAutomaticallyAsync(true, "Azure", "Azure2");

You can do this step with No dependency injection integration too.

MapDeploymentsAutomaticallyAsync is a extensions method for IServiceProvider, with true you can automatically install on Azure the deployments you setup on application.
In the other parameter you can choose which integration runs this automatic update. In the example it's running for the default integration.
With the Management endpoint you can programatically configure or manage deployments on Azure.

You can create a new deployment

    var createResponse = await openAiApi.Management.Deployment
        .Create(deploymentId)
        .WithCapacity(2)
        .WithDeploymentTextModel("ada", TextModelType.AdaText)
        .WithScaling(Management.DeploymentScaleType.Standard)
        .ExecuteAsync();

Get a deployment by Id

    var deploymentResult = await openAiApi.Management.Deployment.RetrieveAsync(createResponse.Id);

List of all deployments by status

    var listResponse = await openAiApi.Management.Deployment.ListAsync();

Update a deployment

    var updateResponse = await openAiApi.Management.Deployment
        .Update(createResponse.Id)
        .WithCapacity(1)
        .WithDeploymentTextModel("ada", TextModelType.AdaText)
        .WithScaling(Management.DeploymentScaleType.Standard)
        .ExecuteAsync();

Delete a deployment by Id

    var deleteResponse = await openAiApi.Management.Deployment
        .DeleteAsync(createResponse.Id);
