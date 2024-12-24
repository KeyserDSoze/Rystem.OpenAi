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

This library targets .NET 9 or above.

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
    - [Usage examples](#usage-examples) 
    - [Function chaining](#function-chaining)
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
    - [Speech](#speech)
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

```csharp
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
    }, "custom integration name");

    var openAiApi = serviceProvider.GetRequiredService<IFactory<IOpenAi>>();
    var firstInstanceOfChatClient = openAiApi.Create("custom integration name").Chat;
    var openAiChatApi = serviceProvider.GetRequiredService<IFactory<IOpenAiChat>>();
    var anotherInstanceOfChatClient = openAiChatApi.Create("custom integration name");
```

## Dependency Injection With Azure

### Add to service collection the OpenAi service in your DI with Azure integration
When you want to use the integration with Azure.

```csharp
    builder.Services.AddOpenAi(settings =>
    {
        settings.ApiKey = apiKey;
        settings.Azure.ResourceName = "AzureResourceName (Name of your deployed service on Azure)";
    });
```

### Add to service collection the OpenAi service in your DI with Azure integration and app registration
See how to create an app registration [here](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app).

```csharp
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
    });
```

### Add to service collection the OpenAi service in your DI with Azure integration and system assigned managed identity
See how to create a managed identity [here](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview).\
[System Assigned Managed Identity](https://learn.microsoft.com/en-us/azure/automation/enable-managed-identity-for-automation)

```csharp
    var resourceName = builder.Configuration["Azure:ResourceName"];
    builder.Services.AddOpenAi(settings =>
    {
        settings.Azure.ResourceName = resourceName;
        settings.Azure.ManagedIdentity.UseDefault = true;
    });
```

### Add to service collection the OpenAi service in your DI with Azure integration and user assigned managed identity
See how to create a managed identity [here](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview).\
[User Assigned Managed Identity](https://learn.microsoft.com/en-us/azure/automation/add-user-assigned-identity)

```csharp
    var resourceName = builder.Configuration["Azure:ResourceName"];
    var managedIdentityId = builder.Configuration["ManagedIdentity:ClientId"];
    builder.Services.AddOpenAi(settings =>
    {
        settings.Azure.ResourceName = resourceName;
        settings.Azure.ManagedIdentity.Id = managedIdentityId;
    });
```

## Use different version
[📖 Back to summary](#documentation)\
You may install different version for each endpoint.

```csharp
     services.AddOpenAi(settings =>
            {
                settings.ApiKey = azureApiKey;
                //default version for all endpoints
                settings.Version = "2024-08-01-preview";
                //different version for chat endpoint
                settings
                    .UseVersionForChat("2023-03-15-preview");
            });
```

In this example We are adding a different version only for chat, and all the other endpoints will use the same (in this case the default version).

## Dependency Injection With Factory
[📖 Back to summary](#documentation)\
You may install more than one OpenAi integration, using name parameter in configuration.
In the next example we have two different configurations, one with OpenAi and a default name and with Azure OpenAi and name "Azure"

```csharp
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
    }, "Azure");
```

I can retrieve the integration with IFactory<> interface (from Rystem) and the name of the integration.

```csharp
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
```

or get the more specific service

```csharp
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
```

## Without Dependency Injection
[📖 Back to summary](#documentation)\
You may configure in a static constructor or during startup your integration without the dependency injection pattern.

```csharp
      OpenAiServiceLocator.Configuration.AddOpenAi(settings =>
        {
            settings.ApiKey = apiKey;
        }, "NoDI");
```

and you can use it with the same static class OpenAiServiceLocator and the static Create method

```csharp
    var openAiApi = OpenAiServiceLocator.Instance.Create(name);
    openAiApi.Embedding......
```

or get the more specific service

```csharp
    var openAiEmbeddingApi = OpenAiServiceLocator.Instance.CreateEmbedding(name);
    openAiEmbeddingApi.Request(....);
```

## Models
[📖 Back to summary](#documentation)\
List and describe the various models available in the API. You can refer to the [Models documentation](https://platform.openai.com/docs/models/overview) to understand what models are available and the differences between them.\
You may find more details [here](https://platform.openai.com/docs/api-reference/models),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/ModelEndpointTests.cs) samples from unit test.

### List Models
Lists the currently available models, and provides basic information about each one such as the owner and availability.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var results = await openAiApi.Model.ListAsync();
```

### Retrieve Models
Retrieves a model instance, providing basic information about the model such as the owner and permissions.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var result = await openAiApi.Model.RetrieveAsync("insert here the model name you need to retrieve");
```

### Delete fine-tune model
Delete a fine-tuned model. You must have the Owner role in your organization.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var deleteResult = await openAiApi.Model
        .DeleteAsync(fineTuneModelId);
```

## Chat
[📖 Back to summary](#documentation)\
Given a chat conversation, the model will return a chat completion response.\
You may find more details [here](https://platform.openai.com/docs/api-reference/chat),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/ChatEndpointTests.cs) samples from unit test.

The `IOpenAiChat` interface provides a robust framework for interacting with OpenAI Chat models. This documentation includes method details and usage explanations, followed by 20 distinct examples that demonstrate real-world applications.

## Methods Overview

### 1. **Execution Methods**
#### `ExecuteAsync(CancellationToken cancellationToken = default)`
- Executes the configured request and retrieves the result in a single response.
- **Usage**: Best for one-off requests where the response can be processed at once.

#### `ExecuteAsStreamAsync(bool withUsage = true, CancellationToken cancellationToken = default)`
- Streams the results progressively, enabling real-time processing.
- **Usage**: Ideal for scenarios where partial results need to be displayed or acted upon immediately.

### 2. **Message Management**
#### Adding Messages
- **`AddMessage(ChatMessageRequest message)`**  
  Adds a message with detailed configuration (`Role`, `Content`).
- **`AddMessages(params ChatMessageRequest[] messages)`**  
  Adds multiple messages at once.
- **`AddMessage(string content, ChatRole role = ChatRole.User)`**  
  A simplified method to add a single message.

#### Specialized Messages
- **`AddUserMessage(string content)`**  
  Adds a user-specific message.  
- **`AddSystemMessage(string content)`**  
  Adds a system-specific message for setting context.  
- **`AddAssistantMessage(string content)`**  
  Adds an assistant-specific message.  

#### Retrieving Messages
- **`GetCurrentMessages()`**  
  Retrieves all messages added to the current request.

#### Content Builder
- **`AddContent(ChatRole role = ChatRole.User)`**  
  Adds content dynamically with a builder.  
- **`AddUserContent()`**, **`AddSystemContent()`**, **`AddAssistantContent()`**  
  Builders for specific message roles.

### 3. **Parameter Configuration**
#### Generation Parameters
- **`WithTemperature(double value)`**  
  Adjusts randomness (range: 0 to 2).  
- **`WithNucleusSampling(double value)`**  
  Enables nucleus sampling (range: 0 to 1).  
- **`WithPresencePenalty(double value)`**  
  Penalizes repeating tokens (range: -2 to 2).  
- **`WithFrequencyPenalty(double value)`**  
  Penalizes frequent tokens (range: -2 to 2).

#### Token and Choice Limits
- **`SetMaxTokens(int value)`**  
  Sets the maximum tokens for the response.  
- **`WithNumberOfChoicesPerPrompt(int value)`**  
  Sets how many response options to generate.

#### Stop Sequences
- **`WithStopSequence(params string[] values)`**  
  Adds one or more stop sequences.  
- **`AddStopSequence(string value)`**  
  Adds a single stop sequence.

#### Bias and User Context
- **`WithBias(string key, int value)`**, **`WithBias(Dictionary<string, int> bias)`**  
  Adjusts the likelihood of specific tokens appearing.  
- **`WithUser(string user)`**  
  Adds a unique user identifier for tracking.  
- **`WithSeed(int? seed)`**  
  Sets a seed for deterministic responses.

### 4. **Response Format Management**
- **`ForceResponseFormat(FunctionTool function)`**, **`ForceResponseFormat(MethodInfo function)`**  
  Forces responses to follow specific function-based formats.  
- **`ForceResponseAsJsonFormat()`**, **`ForceResponseAsText()`**  
  Ensures responses are structured as JSON or plain text.

### 5. **Tool and Function Management**
- **`AvoidCallingTools()`**, **`ForceCallTools()`**, **`CanCallTools()`**  
  Configures tool-calling behavior.  
- **`ClearTools()`**, **`ForceCallFunction(string name)`**  
  Manages specific tools and their calls.

## Usage Examples

### **Basic Interaction**
**Description**: A simple user message and response.  

```csharp
var chat = openAiApi.Chat
    .AddUserMessage("Hello, how are you?")
    .WithModel(ChatModelName.Gpt4_o);

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

### **Streaming Interaction**
**Description**: Streaming a response progressively.  

```csharp
await foreach (var chunk in openAiApi.Chat
    .AddUserMessage("Tell me a story.")
    .WithModel(ChatModelName.Gpt4_o)
    .ExecuteAsStreamAsync())
{
    Console.Write(chunk.Choices?.FirstOrDefault()?.Delta?.Content);
}
```

### **Configuring Temperature**
**Description**: Adjusting response randomness.  

```csharp
var chat = openAiApi.Chat
    .AddUserMessage("What is your opinion on AI?")
    .WithTemperature(0.9);

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

### **Adding Multiple Messages**
**Description**: Sending multiple messages to set context.  

```csharp
var chat = openAiApi.Chat
    .AddSystemMessage("You are a helpful assistant.")
    .AddUserMessage("Who won the soccer match yesterday?")
    .AddUserMessage("What are the latest updates?");

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

### **Using Stop Sequences**
**Description**: Limiting the response with stop sequences.  

```csharp
var chat = openAiApi.Chat
    .AddUserMessage("Explain the theory of relativity.")
    .WithStopSequence("end");

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

### **Adding a Function Tool**
**Description**: Using functions for structured responses.  

```csharp
var functionTool = new FunctionTool
{
    Name = "calculate_sum",
    Description = "Adds two numbers",
    Parameters = new FunctionToolMainProperty()
        .AddPrimitive("number1", new FunctionToolPrimitiveProperty { Type = "integer" })
        .AddPrimitive("number2", new FunctionToolPrimitiveProperty { Type = "integer" })
        .AddRequired("number1")
        .AddRequired("number2")
};

var chat = openAiApi.Chat
    .AddUserMessage("Calculate the sum of 5 and 10.")
    .AddFunctionTool(functionTool);

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

### **Streaming with Stop Sequence**
**Description**: Streaming with an enforced stop condition.  

```csharp
await foreach (var chunk in openAiApi.Chat
    .AddUserMessage("Describe the universe.")
    .WithStopSequence("stop")
    .ExecuteAsStreamAsync())
{
    Console.Write(chunk.Choices?.FirstOrDefault()?.Delta?.Content);
}
```

### **Presence Penalty**
**Description**: Encouraging diverse topics in the response.  

```csharp
var chat = openAiApi.Chat
    .AddUserMessage("Tell me something new.")
    .WithPresencePenalty(1.5);

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

### **Frequency Penalty**
**Description**: Reducing repetitive phrases.  

```csharp
var chat = openAiApi.Chat
    .AddUserMessage("What is recursion?")
    .WithFrequencyPenalty(1.5);

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

### **JSON Response**
**Description**: Forcing the response to be in JSON format.  

```csharp
var chat = openAiApi.Chat
    .AddUserMessage("Summarize the book '1984'.")
    .ForceResponseAsJsonFormat();

var result = await chat.ExecuteAsync();
Console.WriteLine(result.Choices?.FirstOrDefault()?.Message?.Content);
```

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

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var response = await openAiApi.Chat
        .RequestWithUserMessage("What is the weather like in Boston?")
        .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
        .WithFunction(WeatherFunction.NameLabel)
        .ExecuteAndCalculateCostAsync(true);

    var content = response.Result.Choices[0].Message.Content;
```

## Function chaining
You may find the PlayFramework [here](https://github.com/KeyserDSoze/Rystem.OpenAi/tree/master/src/Rystem.PlayFramework)

## Images
[📖 Back to summary](#documentation)\
Given a prompt and/or an input image, the model will generate a new image.\
You may find more details [here](https://platform.openai.com/docs/api-reference/images),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/ImageEndpointTests.cs) samples from unit test.

The `IOpenAiImage` interface provides functionality for generating, editing, and varying images using OpenAI's image models. This document covers each method with explanations and includes **20 distinct examples** demonstrating their usage.

### 1. **Image Generation**
#### `GenerateAsync(string prompt, CancellationToken cancellationToken = default)`
- **Description**: Generates an image based on a textual prompt.
- **Usage**: Use for scenarios where a visual representation of an idea is required.
- **Returns**: `ImageResult` containing the generated image's details.

#### `GenerateAsBase64Async(string prompt, CancellationToken cancellationToken = default)`
- **Description**: Generates an image and returns it as a Base64 string.
- **Usage**: Ideal for embedding images directly into web or mobile applications without saving files.
- **Returns**: `ImageResultForBase64` with the image encoded as Base64.

### 2. **Image Editing**
#### `EditAsync(string prompt, Stream file, string fileName = "image", CancellationToken cancellationToken = default)`
- **Description**: Edits an image using a text prompt and an image file.
- **Usage**: Modify existing images based on creative or functional requirements.
- **Returns**: `ImageResult` with the edited image's details.

#### `EditAsBase64Async(string prompt, Stream file, string fileName = "image", CancellationToken cancellationToken = default)`
- **Description**: Edits an image and returns it as a Base64 string.
- **Usage**: Enables editing workflows with direct Base64 output for web integration.
- **Returns**: `ImageResultForBase64`.

### 3. **Image Variation**
#### `VariateAsync(Stream file, string fileName = "image", CancellationToken cancellationToken = default)`
- **Description**: Creates variations of an existing image.
- **Usage**: Generate alternate versions of an image for creative exploration.
- **Returns**: `ImageResult`.

#### `VariateAsBase64Async(Stream file, string fileName = "image", CancellationToken cancellationToken = default)`
- **Description**: Creates variations of an image and returns them as Base64 strings.
- **Usage**: Useful for embedding variations in platforms that consume Base64 directly.
- **Returns**: `ImageResultForBase64`.

### 4. **Additional Configurations**
#### `WithMask(Stream mask, string maskName = "mask.png")`
- **Description**: Adds a mask to guide image editing.
- **Usage**: Define specific areas of an image to be edited or preserved.

#### `WithNumberOfResults(int numberOfResults)`
- **Description**: Sets the number of images to generate (1 to 10).
- **Usage**: Control how many images are returned in a single operation.

#### `WithSize(ImageSize size)`
- **Description**: Specifies the size of generated images (e.g., `256x256`, `512x512`, `1024x1024`).
- **Usage**: Select resolution based on the intended use case.

#### `WithQuality(ImageQuality quality)`
- **Description**: Sets the quality of generated images.
- **Usage**: Choose between standard and high-quality outputs based on performance needs.

#### `WithStyle(ImageStyle style)`
- **Description**: Specifies the artistic style of generated images.
- **Usage**: Create images with specific aesthetic or thematic styles.

#### `WithUser(string user)`
- **Description**: Sets a unique identifier for tracking and abuse prevention.
- **Usage**: Helps monitor usage and identify specific user requests.

### Create Image
Creates an image given a prompt.

```csharp
    var openAiApi = _openAiFactory.Create(name)!;
    var response = await openAiApi.Image
        .WithSize(ImageSize.Large)
        .GenerateAsync("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.");
    var uri = response.Data?.FirstOrDefault();
```

Download directly and save as stream

```csharp
    var openAiApi = _openAiFactory.Create(name)!;

    var response = await openAiApi.Image
        .WithSize(ImageSize.Large)
        .GenerateAsBase64Async("Create a captive logo with ice and fire, and thunder with the word Rystem. With a desolated futuristic landscape.");

    var image = response.Data?.FirstOrDefault();
    var imageAsStream = image.ConvertToStream();
```

### Create Image Edit
Creates an edited or extended image given an original image and a prompt.

```csharp
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
```

### Create Image Variation
Creates a variation of a given image.

```csharp
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
```

## Embeddings
[📖 Back to summary](#documentation)\
Get a vector representation of a given input that can be easily consumed by machine learning models and algorithms.\
You may find more details [here](https://platform.openai.com/docs/api-reference/embeddings),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/EmbeddingEndpointTests.cs) samples from unit test.

The `IOpenAiEmbedding` interface provides methods to generate embeddings for text inputs, enabling downstream tasks such as similarity search, clustering, and machine learning model inputs. This documentation explains each method and includes **10 usage examples**.

### 1. **Input Management**
#### `WithInputs(params string[] inputs)`
- **Description**: Adds an array of strings to be processed for embeddings.
- **Usage**: Use when multiple inputs are provided simultaneously.

#### `ClearInputs()`
- **Description**: Removes all previously added inputs.
- **Usage**: Resets the input list, useful for reconfiguring the operation.

#### `AddPrompt(string input)`
- **Description**: Adds a single input string for embedding.
- **Usage**: Use when inputs are added incrementally or one at a time.

### 2. **User Identification**
#### `WithUser(string user)`
- **Description**: Adds a unique identifier for the user, aiding in monitoring and abuse detection.
- **Usage**: Helpful in multi-user applications or for logging purposes.

### 3. **Embedding Configuration**
#### `WithDimensions(int dimensions)`
- **Description**: Sets the desired dimensionality for the output embeddings.
- **Usage**: Supported only in specific models where dimension configuration is allowed.

#### `WithEncodingFormat(EncodingFormatForEmbedding encodingFormat)`
- **Description**: Specifies the encoding format of the embeddings (e.g., `Base64`, `Float`).
- **Usage**: Define the format based on downstream processing needs.

### 4. **Execution**
#### `ExecuteAsync(CancellationToken cancellationToken = default)`
- **Description**: Executes the embedding operation asynchronously and returns the result.
- **Usage**: Call after configuring inputs and parameters.

### Create Embedding
Creates an embedding vector representing the input text.

```csharp
    var openAiApi = name == "NoDI" ? OpenAiServiceLocatorLocator.Instance.Create(name) : _openAiFactory.Create(name)!;

    var results = await openAiApi.Embeddings
        .WithInputs("A test text for embedding")
        .ExecuteAsync();

    var resultOfCosineSimilarity = _openAiUtility.CosineSimilarity(results.Data.First().Embedding!, results.Data.First().Embedding!);
```

### Create Embedding with custom dimensions
Creates an embedding with custom dimensions vector representing the input text.
Only supported in text-embedding-3 and later models.

```csharp
    var openAiApi = name == "NoDI" ? OpenAiServiceLocatorLocator.Instance.Create(name) : _openAiFactory.Create(name)!;

    var results = await openAiApi.Embeddings
        .AddPrompt("A test text for embedding")
        .WithModel("text-embedding-3-large")
        .WithDimensions(999)
        .ExecuteAsync();
```

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
We recommend cosine similarity. The choice of distance function typically doesn't matter much.

OpenAI embeddings are normalized to length 1, which means that:

Cosine similarity can be computed slightly faster using just a dot product
Cosine similarity and Euclidean distance will result in the identical rankings

You may use the utility service in this repository to calculate in C# the distance with [Cosine similarity](#cosine-similarity)

## Audio
[📖 Back to summary](#documentation)\
You may find more details [here](https://platform.openai.com/docs/api-reference/audio),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/AudioEndpointTests.cs) samples from unit test.

The `IOpenAiAudio` interface provides methods to handle audio processing tasks such as transcription, translation, and customization of audio analysis. Below is a detailed breakdown of each method.

### 1. **Audio File Input**
#### `WithFile(byte[] file, string fileName = "default")`
- **Description**: Adds an audio file as a byte array for processing.
- **Parameters**:
  - `file`: Byte array representing the audio file.
  - `fileName`: Name of the audio file (default: "default").
- **Usage**: Useful when the audio file is loaded into memory as bytes.

#### `WithStreamAsync(Stream file, string fileName = "default")`
- **Description**: Adds an audio file as a stream asynchronously.
- **Parameters**:
  - `file`: Stream representing the audio file.
  - `fileName`: Name of the audio file (default: "default").
- **Usage**: Ideal for large files streamed directly without loading entirely into memory.

### 2. **Transcription**
#### `TranscriptAsync(CancellationToken cancellationToken = default)`
- **Description**: Transcribes the audio into the input language.
- **Returns**: An `AudioResult` containing the transcription details.
- **Usage**: Extract text content from audio in its original language.

#### `VerboseTranscriptAsSegmentsAsync(CancellationToken cancellationToken = default)`
- **Description**: Transcribes the audio into a verbose representation in the input language.
- **Returns**: A `VerboseSegmentAudioResult` with detailed transcription data.
- **Usage**: Suitable for scenarios requiring detailed transcriptions with additional context or metadata.

#### `VerboseTranscriptAsWordsAsync(CancellationToken cancellationToken = default)`
- **Description**: Transcribes the audio into a verbose representation in the input language.
- **Returns**: A `VerboseWordAudioResult` with detailed transcription data.
- **Usage**: Suitable for scenarios requiring detailed transcriptions with additional context or metadata.

### 3. **Translation**
#### `TranslateAsync(CancellationToken cancellationToken = default)`
- **Description**: Translates audio content into English.
- **Returns**: An `AudioResult` containing the translated text.
- **Usage**: Convert audio content from any supported language to English.

#### `VerboseTranslateAsSegmentsAsync(CancellationToken cancellationToken = default)`
- **Description**: Translates audio into a verbose representation in English.
- **Returns**: A `VerboseSegmentAudioResult` with detailed translation data.
- **Usage**: Obtain comprehensive translation output with additional metadata.

#### `VerboseTranslateAsWordsAsync(CancellationToken cancellationToken = default)`
- **Description**: Translates audio into a verbose representation in English.
- **Returns**: A `VerboseWordAudioResult` with detailed translation data.
- **Usage**: Obtain comprehensive translation output with additional metadata.

### 4. **Customization**
#### `WithPrompt(string prompt)`
- **Description**: Adds a text prompt to guide the model's transcription or translation style.
- **Parameters**:
  - `prompt`: Text to provide contextual guidance or continue a previous segment.
- **Usage**: Helps maintain consistency or tailor the model’s output style.

#### `WithTemperature(double temperature)`
- **Description**: Sets the sampling temperature (range: 0 to 1). Higher values increase randomness, while lower values make output more deterministic.
- **Parameters**:
  - `temperature`: Value for controlling randomness.
- **Usage**: Adjusts the balance between creativity and focus in the output.

#### `WithLanguage(Language language)`
- **Description**: Specifies the input audio's language using ISO-639-1 codes.
- **Parameters**:
  - `language`: Language code of the input audio.
- **Usage**: Improves transcription/translation accuracy and reduces latency by specifying the language explicitly.

#### `WithTranscriptionMinutes(int minutes)`
- **Description**: Sets the number of minutes allocated for transcription tasks.
- **Parameters**:
  - `minutes`: Duration in minutes.
- **Usage**: Controls the time allocation for transcription operations.

#### `WithTranslationMinutes(int minutes)`
- **Description**: Sets the number of minutes allocated for translation tasks.
- **Parameters**:
  - `minutes`: Duration in minutes.
- **Usage**: Controls the time allocation for translation operations.

### Create Transcription
Transcribes audio into the input language.

```csharp
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
```

example for verbose transcription in segments

```csharp
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
        .VerboseTranscriptAsSegmentsAsync();

    Assert.NotNull(results);
    Assert.True(results.Text?.Length > 100);
    Assert.StartsWith("Incidente tra due aerei di addestramento", results.Text);
    Assert.NotEmpty(results.Segments ?? []);
```

### Create Translation
Translates audio into English.

```csharp
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
```

example for verbose translation in segments

```csharp
    var openAiApi = _openAiFactory.Create(name)!;
    Assert.NotNull(openAiApi.Audio);

    var location = Assembly.GetExecutingAssembly().Location;
    location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
    using var readableStream = File.OpenRead($"{location}\\Files\\test.mp3");
    var editableFile = new MemoryStream();
    await readableStream.CopyToAsync(editableFile);
    editableFile.Position = 0;

    var results = await openAiApi.Audio
        .WithFile(editableFile.ToArray(), "default.mp3")
        .WithTemperature(1)
        .WithPrompt("sample")
        .VerboseTranslateAsSegmentsAsync();

    Assert.NotNull(results);
    Assert.True(results.Text?.Length > 100);
    Assert.NotEmpty(results.Segments ?? []);
```

## Speech

The `IOpenAiSpeech` interface enables text-to-speech synthesis by providing methods to generate audio in various formats, along with options for controlling voice style and playback speed. Below is a detailed description of each method.

### 1. **Audio Output Formats**

#### `Mp3Async(string input, CancellationToken cancellationToken = default)`
- **Description**: Converts the given text input into an MP3 audio stream.
- **Parameters**:
  - `input`: The text to be synthesized into audio.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Stream` containing the MP3 audio data.

#### `OpusAsync(string input, CancellationToken cancellationToken = default)`
- **Description**: Converts the given text input into an Opus audio stream.
- **Parameters**:
  - `input`: The text to be synthesized into audio.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Stream` containing the Opus audio data.

#### `AacAsync(string input, CancellationToken cancellationToken = default)`
- **Description**: Converts the given text input into an AAC audio stream.
- **Parameters**:
  - `input`: The text to be synthesized into audio.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Stream` containing the AAC audio data.

#### `FlacAsync(string input, CancellationToken cancellationToken = default)`
- **Description**: Converts the given text input into a FLAC audio stream.
- **Parameters**:
  - `input`: The text to be synthesized into audio.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Stream` containing the FLAC audio data.

#### `WavAsync(string input, CancellationToken cancellationToken = default)`
- **Description**: Converts the given text input into a WAV audio stream.
- **Parameters**:
  - `input`: The text to be synthesized into audio.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Stream` containing the WAV audio data.

#### `PcmAsync(string input, CancellationToken cancellationToken = default)`
- **Description**: Converts the given text input into a PCM audio stream.
- **Parameters**:
  - `input`: The text to be synthesized into audio.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Stream` containing the PCM audio data.

### 2. **Audio Configuration**

#### `WithSpeed(double speed)`
- **Description**: Adjusts the speed of the generated audio. The default speed is `1.0`.
- **Parameters**:
  - `speed`: A value between `0.25` and `4.0` to control playback speed.
- **Returns**: The current instance of `IOpenAiSpeech` for method chaining.
- **Exceptions**: Throws `ArgumentException` if the speed is out of the valid range.

#### `WithVoice(AudioVoice audioVoice)`
- **Description**: Specifies the voice to use for generating audio.
- **Parameters**:
  - `audioVoice`: The desired voice style. Supported values are `alloy`, `echo`, `fable`, `onyx`, `nova`, and `shimmer`.
- **Returns**: The current instance of `IOpenAiSpeech` for method chaining.

#### Notes

- The `IOpenAiSpeech` interface allows generating audio in multiple high-quality formats suitable for various applications, such as podcasts, presentations, and accessibility tools.
- You can customize the playback speed and voice style to suit your needs.
- It supports seamless integration with asynchronous workflows via `ValueTask<Stream>`.

This interface provides powerful capabilities for creating dynamic audio content from text, offering flexibility in format, speed, and voice customization.

```csharp
    var openAiApi = _openAiFactory.Create(name)!;
    var result = await openAiApi.Speech
        .WithVoice(AudioVoice.Fable)
        .WithSpeed(1.3d)
        .Mp3Async(text);
```

## File
[📖 Back to summary](#documentation)\
Files are used to upload documents that can be used with features like Fine-tuning.\
You may find more details [here](https://platform.openai.com/docs/api-reference/files),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/FileEndpointTests.cs) samples from unit test.

The `IOpenAiFile` interface provides functionality for managing files within the OpenAI platform. These files are typically used for tasks such as fine-tuning models or storing custom datasets. Below is a detailed explanation of each method in the interface.

### 1. **Retrieve All Files**
#### `AllAsync(CancellationToken cancellationToken = default)`
- **Description**: Retrieves a list of all files that belong to the user's organization.
- **Parameters**:
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `ValueTask<FilesDataResult>` containing metadata for all uploaded files.
- **Usage**: Use this method to get an overview of all available files and their statuses.
- **Exceptions**: Throws `HttpRequestException` if the request fails.

### 2. **Retrieve Specific File Information**
#### `RetrieveAsync(string fileId, CancellationToken cancellationToken = default)`
- **Description**: Fetches metadata about a specific file by its ID.
- **Parameters**:
  - `fileId`: The unique identifier of the file to retrieve.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `ValueTask<FileResult>` containing details about the specified file.
- **Usage**: Retrieve specific information about a file, such as its size, type, and upload time.

### 3. **Retrieve File Content**
#### `RetrieveFileContentAsStringAsync(string fileId, CancellationToken cancellationToken = default)`
- **Description**: Retrieves the content of a specified file as a string.
- **Parameters**:
  - `fileId`: The unique identifier of the file to retrieve.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Task<string>` containing the file content as a string.
- **Usage**: Use this to read the content of smaller text-based files, such as JSON or CSV files.

#### `RetrieveFileContentAsStreamAsync(string fileId, CancellationToken cancellationToken = default)`
- **Description**: Retrieves the content of a specified file as a stream.
- **Parameters**:
  - `fileId`: The unique identifier of the file to retrieve.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `Task<Stream>` containing the file content as a stream.
- **Usage**: Useful for reading large files or binary content incrementally.

### 4. **Delete File**
#### `DeleteAsync(string fileId, CancellationToken cancellationToken = default)`
- **Description**: Deletes a specified file by its ID.
- **Parameters**:
  - `fileId`: The unique identifier of the file to delete.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `ValueTask<FileResult>` indicating the result of the deletion operation.
- **Usage**: Use to remove files that are no longer needed, freeing up storage space.

### 5. **Upload a File**
#### `UploadFileAsync(Stream file, string fileName, string contentType = "application/json", PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default)`
- **Description**: Uploads a file to the OpenAI platform for use with various features, such as fine-tuning.
- **Parameters**:
  - `file`: A `Stream` representing the file to upload.
  - `fileName`: The name of the file to upload.
  - `contentType`: The MIME type of the file (default: "application/json").
  - `purpose`: The intended purpose of the file (e.g., "fine-tune").
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `ValueTask<FileResult>` containing details about the uploaded file.
- **Usage**: Use this to upload datasets or other documents required for OpenAI features.
- **Notes**: The total size of files for an organization is limited to 1 GB by default. Contact OpenAI support to increase the limit if necessary.

#### Notes

- This interface is designed to handle all file-related operations within the OpenAI ecosystem, including uploading, retrieving, and deleting files.
- It supports asynchronous operations to ensure scalability and responsiveness, especially when handling large files or network delays.
- The methods provide flexible options for retrieving file content, either as strings or streams, depending on the use case.

This interface is essential for managing files effectively in projects requiring fine-tuning or custom dataset handling.

### List files
Returns a list of files that belong to the user's organization.

```csharp
      var openAiApi = _openAiFactory.Create(name);
      var results = await openAiApi.File
                .AllAsync();
```

### Upload file
Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var uploadResult = await openAiApi.File
            .UploadFileAsync(editableFile, name);
```

### Delete file
Delete a file.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var deleteResult = await openAiApi.File
            .DeleteAsync(uploadResult.Id);
```

### Retrieve file
Returns information about a specific file.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var retrieve = await openAiApi.File
            .RetrieveAsync(uploadResult.Id);
```

### Retrieve file content
Returns the contents of the specified file

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var contentRetrieve = await openAiApi.File
            .RetrieveFileContentAsStringAsync(uploadResult.Id);
```

### Upload large files
You can upload large files by splitting them into parts.
Upload a file that can be used across various endpoints. Individual files can be up to 512 MB, and the size of all files uploaded by one organization can be up to 100 GB.
The Assistants API supports files up to 2 million tokens and of specific file types. See the Assistants Tools guide for details.
The Fine-tuning API only supports .jsonl files. The input also has certain required formats for fine-tuning chat or completions models.
The Batch API only supports .jsonl files up to 200 MB in size. The input also has a specific required format.

```csharp
     var upload = openAiApi.File
                .CreateUpload(fileName)
                .WithPurpose(PurposeFileUpload.FineTune)
                .WithContentType("application/json")
                .WithSize(editableFile.Length);

    var execution = await upload.ExecuteAsync();
    var partResult = await execution.AddPartAsync(editableFile);
    Assert.True(partResult.Id?.Length > 7);
    var completeResult = await execution.CompleteAsync();
```

## Fine-Tunes
[📖 Back to summary](#documentation)\
Manage fine-tuning jobs to tailor a model to your specific training data.\
You may find more details [here](https://platform.openai.com/docs/api-reference/fine-tunes),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/FineTuneEndpointTests.cs) samples from unit test.

The `IOpenAiFineTune` interface provides methods to manage fine-tuning operations, allowing customization of models with specific training data. Fine-tuning is useful for tailoring models to specialized tasks or datasets. Below is a detailed breakdown of the methods provided.

### 1. **Configuration Methods**

#### `WithFileId(string trainingFileId)`
- **Description**: Specifies the ID of the training file to be used for fine-tuning.
- **Parameters**:
  - `trainingFileId`: The unique identifier of the training dataset.
- **Returns**: The current instance of `IOpenAiFineTune` for method chaining.
- **Usage**: Set the file ID required for starting a fine-tune operation.

#### `WithValidationFile(string validationFileId)`
- **Description**: Sets the ID of a validation file to evaluate fine-tuning performance.
- **Parameters**:
  - `validationFileId`: The unique identifier of the validation dataset.
- **Returns**: The current instance of `IOpenAiFineTune` for method chaining.

#### `WithHyperParameters(Action<FineTuneHyperParameters> hyperParametersSettings)`
- **Description**: Configures hyperparameters for the fine-tuning operation.
- **Parameters**:
  - `hyperParametersSettings`: A delegate for configuring fine-tune hyperparameters.
- **Returns**: The current instance of `IOpenAiFineTune` for method chaining.

#### `WithSuffix(string value)`
- **Description**: Adds a suffix to the name of the fine-tuned model.
- **Parameters**:
  - `value`: The suffix string.
- **Returns**: The current instance of `IOpenAiFineTune` for method chaining.

#### `WithSeed(int seed)`
- **Description**: Sets a seed value for reproducibility during fine-tuning.
- **Parameters**:
  - `seed`: The seed value to ensure consistent results.
- **Returns**: The current instance of `IOpenAiFineTune` for method chaining.

#### `WithSpecificWeightAndBiasesIntegration(Action<WeightsAndBiasesFineTuneIntegration> integration)`
- **Description**: Configures integration with Weights and Biases for fine-tuning tracking.
- **Parameters**:
  - `integration`: A delegate for setting up Weights and Biases integration.
- **Returns**: The current instance of `IOpenAiFineTune` for method chaining.

#### `ClearIntegrations()`
- **Description**: Removes all integrations associated with the fine-tuning operation.
- **Returns**: The current instance of `IOpenAiFineTune` for method chaining.

### 2. **Execution Methods**

#### `ExecuteAsync(CancellationToken cancellationToken = default)`
- **Description**: Starts the fine-tuning operation with the configured parameters.
- **Parameters**:
  - `cancellationToken`: A token for cancelling the operation if needed.
- **Returns**: A `ValueTask<FineTuneResult>` representing the result of the operation.

### 3. **Retrieval Methods**

#### `ListAsync(int take = 20, int skip = 0, CancellationToken cancellationToken = default)`
- **Description**: Lists all fine-tuning jobs with pagination.
- **Parameters**:
  - `take`: The number of results to retrieve (default: 20).
  - `skip`: The number of results to skip (default: 0).
  - `cancellationToken`: A token for cancelling the operation.
- **Returns**: A `ValueTask<FineTuneResults>` containing a list of fine-tune jobs.

#### `RetrieveAsync(string fineTuneId, CancellationToken cancellationToken = default)`
- **Description**: Retrieves details of a specific fine-tune operation by its ID.
- **Parameters**:
  - `fineTuneId`: The unique identifier of the fine-tune operation.
  - `cancellationToken`: A token for cancelling the operation.
- **Returns**: A `ValueTask<FineTuneResult>` containing the fine-tune job details.

### 4. **Management Methods**

#### `CancelAsync(string fineTuneId, CancellationToken cancellationToken = default)`
- **Description**: Cancels a fine-tune operation by its ID.
- **Parameters**:
  - `fineTuneId`: The unique identifier of the fine-tune operation.
  - `cancellationToken`: A token for cancelling the operation.
- **Returns**: A `ValueTask<FineTuneResult>` indicating the cancellation status.

### 5. **Events and Checkpoints**

#### `CheckPointEventsAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default)`
- **Description**: Lists checkpoint events for a fine-tune operation with pagination.
- **Parameters**:
  - `fineTuneId`: The ID of the fine-tune operation.
  - `take`: The number of results to retrieve (default: 20).
  - `skip`: The number of results to skip (default: 0).
  - `cancellationToken`: A token for cancelling the operation.
- **Returns**: A `ValueTask<FineTuneCheckPointEventsResult>` containing the checkpoint events.

#### `ListEventsAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default)`
- **Description**: Retrieves a list of events related to a fine-tune operation.
- **Parameters**:
  - `fineTuneId`: The ID of the fine-tune operation.
  - `take`: The number of results to retrieve (default: 20).
  - `skip`: The number of results to skip (default: 0).
  - `cancellationToken`: A token for cancelling the operation.
- **Returns**: A `ValueTask<FineTuneEventsResult>` containing the event details.

### 6. **Streaming Methods**

#### `ListAsStreamAsync(int take = 20, int skip = 0, CancellationToken cancellationToken = default)`
- **Description**: Streams fine-tune job results asynchronously.
- **Parameters**:
  - `take`: The number of results to retrieve (default: 20).
  - `skip`: The number of results to skip (default: 0).
  - `cancellationToken`: A token for cancelling the operation.
- **Returns**: An `IAsyncEnumerable<FineTuneResult>` for processing results incrementally.

#### `ListEventsAsStreamAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default)`
- **Description**: Streams events for a fine-tune operation asynchronously.
- **Parameters**:
  - `fineTuneId`: The ID of the fine-tune operation.
  - `take`: The number of results to retrieve (default: 20).
  - `skip`: The number of results to skip (default: 0).
  - `cancellationToken`: A token for cancelling the operation.
- **Returns**: An `IAsyncEnumerable<FineTuneEvent>` for processing events incrementally.

#### Notes

- The `IOpenAiFineTune` interface provides a comprehensive API for managing fine-tuning operations, from configuration to execution and result retrieval.
- Hyperparameter configuration and event tracking enable fine-grained control and monitoring of the fine-tuning process.
- Asynchronous and streaming methods allow efficient handling of large datasets and operations.

### Create fine-tune
Creates a job that fine-tunes a specified model from a given dataset.
Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var createResult = await openAiApi.FineTune
                                .Create(fileId)
                                .ExecuteAsync();
```

### List fine-tunes
List your organization's fine-tuning jobs

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var allFineTunes = await openAiApi.FineTune
                        .ListAsync();
```

### Retrieve fine-tune
Gets info about the fine-tune job.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var retrieveFineTune = await openAiApi.FineTune
                            .RetrieveAsync(fineTuneId);
```

### Cancel fine-tune
Immediately cancel a fine-tune job.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var cancelResult = await openAiApi.FineTune
                            .CancelAsync(fineTuneId);
```

### List fine-tune events
Get fine-grained status updates for a fine-tune job.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var events = await openAiApi.FineTune
                        .ListEventsAsync(fineTuneId);
```

### List fine-tune events as stream
Get fine-grained status updates for a fine-tune job.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var events = await openAiApi.FineTune
                        .ListEventsAsStreamAsync(fineTuneId);
```

### Delete fine-tune model
Delete a fine-tuned model. You must have the Owner role in your organization.

```csharp
    var openAiApi = _openAiFactory.Create(name);
    var deleteResult = await openAiApi.FineTune
        .DeleteAsync(fineTuneModelId);
```

## Moderations
[📖 Back to summary](#documentation)\
Given a input text, outputs if the model classifies it as violating OpenAI's content policy.\
You may find more details [here](https://platform.openai.com/docs/api-reference/moderations),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.UnitTests/Endpoints/ModerationEndpointTests.cs) samples from unit test.

The `IOpenAiModeration` interface provides functionality for evaluating text against OpenAI's Content Policy, determining if the input violates any predefined guidelines. This interface is particularly useful for applications requiring automated content moderation to ensure safety and compliance.

### **1. Text Moderation**

#### `ExecuteAsync(string input, CancellationToken cancellationToken = default)`
- **Description**: Evaluates the provided text to determine if it violates OpenAI's Content Policy.
- **Parameters**:
  - `input`: The text to be analyzed for potential policy violations.
  - `cancellationToken`: Optional token for cancelling the operation.
- **Returns**: A `ValueTask<ModerationResult>` containing the moderation outcome.
- **Usage**:
  - The method classifies text for categories such as hate speech, violence, self-harm, or other potentially harmful content.
  - Useful for integrating automated moderation into platforms, such as user-generated content systems, messaging apps, or forums.

#### Notes

- The `ExecuteAsync` method processes the input text and provides a `ModerationResult` object that contains detailed classification results.
- It is an asynchronous operation, making it suitable for applications requiring high concurrency and responsiveness.
- This interface focuses solely on moderation tasks and is designed to integrate seamlessly with other OpenAI API functionalities.

The `IOpenAiModeration` interface is a vital tool for developers aiming to build applications that enforce content guidelines and promote a safe user environment.

### Create moderation
Classifies if text violates OpenAI's Content Policy

```csharp
    var openAiApi = _openAiFactory.Create(name)!;
    var results = await openAiApi.Moderation
        .WithModel("testModel")
        .WithModel(ModerationModelName.OmniLatest)
        .ExecuteAsync("I want to kill them and everyone else.");

    var categories = results.Results?.FirstOrDefault()?.Categories;
```

## Utilities
[📖 Back to summary](#documentation)\
Utilities for OpenAi, you can inject the interface IOpenAiUtility everywhere you need it.
In IOpenAiUtility you can find:

### Cosine Similarity
[📖 Back to embeddings](#embeddings)\
In data analysis, cosine similarity is a measure of similarity between two non-zero vectors defined in an inner product space. Cosine similarity is the cosine of the angle between the vectors; that is, it is the dot product of the vectors divided by the product of their lengths. It follows that the cosine similarity does not depend on the magnitudes of the vectors, but only on their angle. The cosine similarity always belongs to the interval [−1,1]. For example, two proportional vectors have a cosine similarity of 1, two orthogonal vectors have a similarity of 0, and two opposite vectors have a similarity of -1. In some contexts, the component values of the vectors cannot be negative, in which case the cosine similarity is bounded in [0,1].
Here an example from Unit test.

```csharp
    IOpenAiUtility _openAiUtility;
    var resultOfCosineSimilarity = _openAiUtility.CosineSimilarity(results.Data.First().Embedding, results.Data.First().Embedding);
    var resultOfEuclideanDinstance = _openAiUtility.EuclideanDistance(results.Data.First().Embedding, results.Data.First().Embedding);
    Assert.True(resultOfCosineSimilarity >= 1);
```

Without DI, you need to setup an OpenAiServiceLocator [without Dependency Injection](#without-dependency-injection) and after that you can use

```csharp
    IOpenAiUtility openAiUtility = OpenAiServiceLocator.Instance.Utility();
```

### Tokens
[📖 Back to summary](#documentation)\
You can think of tokens as pieces of words, where 1,000 tokens is about 750 words. You can calculate your request tokens with the Tokenizer service in Utility.

```csharp
    IOpenAiUtility _openAiUtility
    var encoded = _openAiUtility.Tokenizer
        .WithChatModel(ChatModelType.Gpt4)
        .Encode(value);
    Assert.Equal(numberOfTokens, encoded.NumberOfTokens);
    var decoded = _openAiUtility.Tokenizer.Decode(encoded.EncodedTokens);
    Assert.Equal(value, decoded);
```

### Cost
[📖 Back to summary](#documentation)\
You can think of tokens as pieces of words, where 1,000 tokens is about 750 words.

```csharp
    var openAiApi = _openAiFactory.Create(name)!;
    var results = await openAiApi.Chat
        .AddMessage(new ChatMessageRequest { Role = ChatRole.User, Content = "Hello!! How are you?" })
        .WithModel(ChatModelName.Gpt4_o)
        .WithTemperature(1)
        .ExecuteAsync();
    //calculate cost works only if you added the price during setup.
    var cost = openAiApi.Chat.CalculateCost();
```

### Setup price
[📖 Back to summary](#documentation)\
During setup of your OpenAi service you may add your custom price table with settings.PriceBuilder property.

```csharp
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
```

## Management
[📖 Back to summary](#documentation)\
In your openai dashboard you may get the billing usage, or users, or taxes, or similar. Here you have an api to retrieve this kind of data.

### Billing
[📖 Back to summary](#documentation)\
You may use the management endpoint to retrieve data for your usage. Here an example on how to get the usage for the month of April.

```csharp
    var management = _openAiFactory.CreateManagement(integrationName);
    var usages = await management
        .Billing
        .From(new DateTime(2023, 4, 1))
        .To(new DateTime(2023, 4, 30))
        .GetUsageAsync();
    Assert.NotEmpty(usages.DailyCosts);
```

### Deployments
[📖 Back to summary](#documentation)\
Only for Azure you have to deploy a model to use model in your application. You can configure Deployment during startup of your application.

```csharp
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
```

During startup you can configure other deployments on your application or on Azure.

```csharp
    var app = builder.Build();
    await app.Services.MapDeploymentsAutomaticallyAsync(true);
```

or a specific integration or list of integration that you setup previously.

```csharp
    await app.Services.MapDeploymentsAutomaticallyAsync(true, "Azure", "Azure2");
```

You can do this step with No dependency injection integration too.

MapDeploymentsAutomaticallyAsync is a extensions method for IServiceProvider, with true you can automatically install on Azure the deployments you setup on application.
In the other parameter you can choose which integration runs this automatic update. In the example it's running for the default integration.
With the Management endpoint you can programmatically configure or manage deployments on Azure.

You can create a new deployment

```csharp
    var createResponse = await openAiApi.Management.Deployment
        .Create(deploymentId)
        .WithCapacity(2)
        .WithDeploymentTextModel("ada", TextModelType.AdaText)
        .WithScaling(Management.DeploymentScaleType.Standard)
        .ExecuteAsync();
```

Get a deployment by Id

```csharp
    var deploymentResult = await openAiApi.Management.Deployment.RetrieveAsync(createResponse.Id);
```

List of all deployments by status

```csharp
    var listResponse = await openAiApi.Management.Deployment.ListAsync();
```

Update a deployment

```csharp
    var updateResponse = await openAiApi.Management.Deployment
        .Update(createResponse.Id)
        .WithCapacity(1)
        .WithDeploymentTextModel("ada", TextModelType.AdaText)
        .WithScaling(Management.DeploymentScaleType.Standard)
        .ExecuteAsync();
```

Delete a deployment by Id

```csharp
    var deleteResponse = await openAiApi.Management.Deployment
        .DeleteAsync(createResponse.Id);
```