# Unofficial Fluent C#/.NET SDK for accessing the OpenAI API (Easy swap among OpenAi and Azure OpenAi)

## Last update with Cost and Tokens calculation

A simple C# .NET wrapper library to use with [OpenAI](https://openai.com/)'s API.

[![MIT License](https://img.shields.io/github/license/dotnet/aspnetcore?color=%230b0&style=flat-square)](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/LICENSE.txt) 
[![Discord](https://img.shields.io/discord/732297728826277939?style=flat-square&label=Discord&logo=discord&logoColor=white&color=7289DA)](https://discord.gg/wUh2fppr)


## Help the project

### Contribute: https://www.buymeacoffee.com/keyserdsoze
### Contribute: https://patreon.com/Rystem


## Requirements

This library targets .NET standard 2.1 and above.

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

- [Startup Setup](#startup-setup)
  - [Dependency Injection](#dependency-injection)
  - [Azure](#dependency-injection-with-azure)
  - [Use different version](#use-different-version)
  - [Factory](#dependency-injection-with-factory)
- [Without Dependency Injection](#without-dependency-injection)
- [Models](#models)
  - [List Models](#list-models)
  - [Retrieve Models](#retrieve-model)
- [Completions](#completions)
  - [Streaming](#streaming)
- [Chat](#chat)
  - [Streaming](#chat-streaming)
- [Edits](#edits)
- [Images](#images)
  - [Create Image](#create-image)
  - [Create Image Edit](#create-image-edit)
  - [Create Image Variation](#create-image-variation)
- [Embeddings](#embeddings)
  - [Create Embedding](#create-embedding)
- [Audio](#audio)
  - [Create Transcription](#create-transcription)
  - [Create Translation](#create-translation)
- [File](#file)
  - [List Files](#list-files)
  - [Upload File](#upload-file)
  - [Delete File](#delete-file)
  - [Retrieve File Info](#retrieve-file)
  - [Retrieve File Content](#retrieve-file-content)
- [Fine-Tunes](#fine-tunes)
  - [Create Fine Tune](#create-fine-tune)
  - [List Fine Tune](#list-fine-tunes)
  - [Retrieve Fine Tune](#retrieve-fine-tune)
  - [Cancel Fine Tune](#cancel-fine-tune)
  - [List Fine Tune Events](#list-fine-tune-events)
  - [List Fine Tune Events As Stream](#List-fine-tune-events-as-stream)
  - [Delete Fine Tune](#delete-fine-tune-model)
- [Moderations](#moderations)
  - [Create Moderation](#create-moderation)
- [Utilities](#utilities)
  - [Cosine similarity](#cosine-similarity)
  - [Tokens](#tokens)
  - [Cost](#cost)
  - [Setup Price](#setup-price)


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
    });

## Dependency Injection With Azure

### Add to service collection the OpenAi service in your DI with Azure integration
When you want to use the integration with Azure, you need to specify all the models you're going to use. In the example you may find the model name for DavinciText3.
You still may add a custom model, with AddDeploymentCustomModel.

    builder.Services.AddOpenAi(settings =>
    {
        settings.ApiKey = apiKey;
        settings.Azure.ResourceName = "AzureResourceName (Name of your deployed service on Azure)";
        settings.Azure
            .AddDeploymentTextModel("Test (The name from column 'Model deployment name' in Model deployments blade in your Azure service)", TextModelType.DavinciText3);
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
            .AddDeploymentTextModel("Test", TextModelType.CurieText)
            .AddDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
            .AddDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
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
            .AddDeploymentTextModel("Test", TextModelType.CurieText)
            .AddDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
            .AddDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
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
            .AddDeploymentTextModel("Test", TextModelType.CurieText)
            .AddDeploymentTextModel("text-davinci-002", TextModelType.DavinciText2)
            .AddDeploymentEmbeddingModel("Test", EmbeddingModelType.AdaTextEmbedding);
    });

## Use different version
[📖 Back to summary](#documentation)\
You may install different version for each endpoint.

     services.AddOpenAi(settings =>
            {
                settings.ApiKey = azureApiKey;
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
            .AddDeploymentTextModel("text-curie-001", TextModelType.CurieText)
            .AddDeploymentTextModel("text-davinci-003", TextModelType.DavinciText3)
            .AddDeploymentEmbeddingModel("OpenAiDemoModel", EmbeddingModelType.AdaTextEmbedding)
            .AddDeploymentChatModel("gpt35turbo", ChatModelType.Gpt35Turbo0301);
    }, "Azure");

I can retrieve the integration with IOpenAiFactory interface and the name of the integration.
    
    private readonly IOpenAiFactory _openAiFactory;

    public CompletionEndpointTests(IOpenAiFactory openAiFactory)
    {
        _openAiFactory = openAiFactory;
    }

    public async ValueTask DoSomethingWithDefaultIntegrationAsync()
    {
        var openAiApi = _openAiFactory.Create();
        openAiApi.Completion.........
    }

    public async ValueTask DoSomethingWithAzureIntegrationAsync()
    {
        var openAiApi = _openAiFactory.Create("Azure");
        openAiApi.Completion.........
    }

or get the more specific service

    public async ValueTask DoSomethingWithAzureIntegrationAsync()
    {
        var openAiEmbeddingApi = _openAiFactory.CreateEmbedding(name);
        openAiEmbeddingApi.Request(....);
    }

## Without Dependency Injection
[📖 Back to summary](#documentation)\
You may configure in a static constructor or during startup your integration without the dependency injection pattern.

      OpenAiService.Setup(settings =>
        {
            settings.ApiKey = apiKey;
        }, "NoDI");

and you can use it with the same static class OpenAiService and the static Create method

    var openAiApi = OpenAiService.Create(name);
    openAiApi.Embedding......

or get the more specific service

    var openAiEmbeddingApi = OpenAiService.CreateEmbedding(name);
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


## Completions
[📖 Back to summary](#documentation)\
Given a prompt, the model will return one or more predicted completions, and can also return the probabilities of alternative tokens at each position.\
You may find more details [here](https://platform.openai.com/docs/api-reference/completions),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/CompletionEndpointTests.cs) samples from unit test

    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Completion
        .Request("One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight")
        .WithModel(TextModelType.CurieText)
        .WithTemperature(0.1)
        .SetMaxTokens(5)
        .ExecuteAsync();

### Streaming

    var openAiApi = _openAiFactory.Create(name);
    var results = new List<CompletionResult>();
            await foreach (var x in openAiApi.Completion
               .Request("Today is Monday, tomorrow is", "10 11 12 13 14")
               .WithTemperature(0)
               .SetMaxTokens(3)
               .ExecuteAsStreamAsync())
            {
                results.Add(x);
            }

## Chat
[📖 Back to summary](#documentation)\
Given a chat conversation, the model will return a chat completion response.\
You may find more details [here](https://platform.openai.com/docs/api-reference/chat),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ChatEndpointTests.cs) samples from unit test.

    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Chat
            .Request(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
            .WithModel(ChatModelType.Gpt4)
            .WithTemperature(1)
            .ExecuteAsync();

### Chat Streaming 

    var openAiApi = _openAiFactory.Create(name);
    var results = new List<ChatResult>();
        await foreach (var x in openAiApi.Chat
            .Request(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
            .WithModel(ChatModelType.Gpt35Turbo)
            .WithTemperature(1)
            .ExecuteAsStreamAsync())
            {
                results.Add(x);
            }

## Edits
[📖 Back to summary](#documentation)\
Given a prompt and an instruction, the model will return an edited version of the prompt.
You may find more details [here](https://platform.openai.com/docs/api-reference/edits),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/EditEndpointTests.cs) samples from unit test.

    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Edit
            .Request("Fix the spelling mistakes")
            .WithModel(EditModelType.TextDavinciEdit)
            .SetInput("What day of the wek is it?")
            .WithTemperature(0.5)
            .ExecuteAsync();

## Images
[📖 Back to summary](#documentation)\
Given a prompt and/or an input image, the model will generate a new image.\
You may find more details [here](https://platform.openai.com/docs/api-reference/images),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ImageEndpointTests.cs) samples from unit test.

### Create Image
Creates an image given a prompt.

    var openAiApi = _openAiFactory.Create(name);
    var response = await openAiApi.Image
        .Generate("A cute baby sea otter")
        .WithSize(ImageSize.Small)
        .ExecuteAsync();    

Download directly and save as stream

    var openAiApi = _openAiFactory.Create(name);
    var streams = new List<Stream>();
    await foreach (var image in openAiApi.Image
        .Generate("A cute baby sea otter")
        .WithSize(ImageSize.Small)
        .DownloadAsync())
    {
        streams.Add(image);
    }

### Create Image Edit
Creates an edited or extended image given an original image and a prompt.

    var openAiApi = _openAiFactory.Create(name);
    var response = await openAiApi.Image
        .Generate("A cute baby sea otter wearing a beret")
        .EditAndTrasformInPng(editableFile, "otter.png")
        .WithSize(ImageSize.Small)
        .WithNumberOfResults(2)
        .ExecuteAsync();

### Create Image Variation
Creates a variation of a given image.

    var openAiApi = _openAiFactory.Create(name);
    var response = await openAiApi.Image
        .VariateAndTransformInPng(editableFile, "otter.png")
        .WithSize(ImageSize.Small)
        .WithNumberOfResults(1)
        .ExecuteAsync();

## Embeddings
[📖 Back to summary](#documentation)\
Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.\
You may find more details [here](https://platform.openai.com/docs/api-reference/embeddings),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/EmbeddingEndpointTests.cs) samples from unit test.


### Create Embedding
Creates an embedding vector representing the input text.

    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Embedding
        .Request("A test text for embedding")
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
    
    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Audio
        .Request(editableFile, "default.mp3")
        .TranscriptAsync();

### Create Translation
Translates audio into English.

    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Audio
        .Request(editableFile, "default.mp3")
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
    var deleteResult = await openAiApi.File
        .DeleteAsync(fileId);

## Moderations
[📖 Back to summary](#documentation)\
Given a input text, outputs if the model classifies it as violating OpenAI's content policy.\
You may find more details [here](https://platform.openai.com/docs/api-reference/moderations),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ModerationEndpointTests.cs) samples from unit test.

### Create moderation
Classifies if text violates OpenAI's Content Policy
    
    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Moderation
            .Create("I want to kill them.")
            .WithModel(ModerationModelType.TextModerationStable)
            .ExecuteAsync();


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
    Assert.True(resultOfCosineSimilarity >= 1);

Without DI, you need to setup an OpenAiService [without Dependency Injection](#without-dependency-injection) and after that you can use

    IOpenAiUtility openAiUtility = OpenAiService.Utility();

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

    IOpenAiCost _openAiCost;
    var integrationName = "Azure";
    var manualCostCalculator = _openAiCost.Configure(x =>
    {
        x
        .WithFamily(ModelFamilyType.Gpt3_5)
        .WithType(OpenAiType.Chat);
    }, integrationName);
    var manualCalculatedPrice = manualCostCalculator.Invoke(new OpenAiUsage
    {
        PromptTokens = numberOfTokens * times,
    });

You may get price for your request directly from any endpoint

     var chat = openAiApi.Chat
            .Request(new ChatMessage { Role = ChatRole.User, Content = content })
            .WithModel(chatModel)
            .WithTemperature(1);
     var costForRequest = chat.CalculateCost();

You can get the cost for current request
    
    var chat = openAiApi.Chat
            .Request(new ChatMessage { Role = ChatRole.User, Content = content })
            .WithModel(chatModel)
            .WithTemperature(1);
     var responseForChatWithCost = await chat.ExecuteAndCalculateCostAsync();
     var costForRequestAndResponse = responseForChatWithCost.CalculateCost();

### Setup price
[📖 Back to summary](#documentation)\
During setup of your OpenAi service you may add your custom price table with settings.Price property.

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
            .AddDeploymentTextModel("text-curie-001", TextModelType.CurieText)
            .AddDeploymentTextModel("text-davinci-003", TextModelType.DavinciText3)
            .AddDeploymentEmbeddingModel("OpenAiDemoModel", EmbeddingModelType.AdaTextEmbedding)
            .AddDeploymentChatModel("gpt35turbo", ChatModelType.Gpt35Turbo0301);
        settings.Price
            .SetFineTuneForAda(0.2M, 0.2M)
            .SetAudioForTranslation(0.2M);
    }, "Azure");