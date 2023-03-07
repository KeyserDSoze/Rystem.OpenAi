# C#/.NET SDK for accessing the OpenAI GPT-3 API

A simple C# .NET wrapper library to use with [OpenAI](https://openai.com/)'s GPT-3 API.

> This repository is available to transfer to the OpenAI organization if they so choose to accept it.

## Requirements

This library targets .NET standard 2.0 and above.

### Advertising
Watch out my Rystem framework to be able to do .Net webapp faster (easy integration with repository pattern or CQRS for your Azure services).
### [What is Rystem?](https://github.com/KeyserDSoze/Rystem)

## Setup

Install package [`Rystem.OpenAi` from Nuget](https://github.com/KeyserDSoze/Rystem.OpenAi).  
Here's how via command line:

```powershell
Install-Package Rystem.OpenAi
```

## Documentation

### Table of Contents

- [Dependency Injection](#dependency-injection)
- [Models](#models)
  - [List Models](#list-models)
  - [Retrieve Models](#retrieve-model)
- [Completions](#completions)
  - [Streaming](#streaming)
- [Edits](#edits)
  - [Create Edit](#create-edit)
- [Embeddings](#embeddings)
  - [Create Embedding](#create-embeddings)
- [Images](#images)
  - [Create Image](#create-image)
  - [Edit Image](#edit-image)
  - [Create Image Variation](#create-image-variation)
- [Files](#files)
  - [List Files](#list-files)
  - [Upload File](#upload-file)
  - [Delete File](#delete-file)
  - [Retrieve File Info](#retrieve-file-info)
  - [Download File Content](#download-file-content)
- [Fine Tuning](#fine-tuning)
  - [Create Fine Tune Job](#create-fine-tune-job)
  - [List Fine Tune Jobs](#list-fine-tune-jobs)
  - [Retrieve Fine Tune Job Info](#retrieve-fine-tune-job-info)
  - [Cancel Fine Tune Job](#cancel-fine-tune-job)
  - [List Fine Tune Events](#list-fine-tune-events)
  - [Stream Fine Tune Events](#stream-fine-tune-events)
- [Moderations](#moderations)
  - [Create Moderation](#create-moderation)

## Dependency Injection

### Add to service collection the OpenAi service in your DI

    var apiKey = configuration["Azure:ApiKey"];
    services.AddOpenAi(settings =>
    {
        settings.ApiKey = apiKey;
    });

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

## Models
List and describe the various models available in the API. You can refer to the [Models documentation](https://platform.openai.com/docs/models/overview) to understand what models are available and the differences between them.
You may find more details [here](https://platform.openai.com/docs/api-reference/models),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/ModelEndpointTests.cs) samples from unit test.

### List Models
Lists the currently available models, and provides basic information about each one such as the owner and availability.

    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Model.ListAsync();

### Retrieve Models
Retrieves a model instance, providing basic information about the model such as the owner and permissioning.

    IOpenAiApi _openAiApi;
    var result = await _openAiApi.Model.RetrieveAsync(TextModelType.DavinciText3.ToModelId());


## Completions
Given a prompt, the model will return one or more predicted completions, and can also return the probabilities of alternative tokens at each position.
You may find more details [here](https://platform.openai.com/docs/api-reference/completions),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/CompletionEndpointTests.cs) samples from unit test

    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Completion
        .Request("One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight")
        .WithModel(TextModelType.CurieText)
        .WithTemperature(0.1)
        .SetMaxTokens(5)
        .ExecuteAsync();

### Streaming

    IOpenAiApi _openAiApi;
    var results = new List<CompletionResult>();
            await foreach (var x in _openAiApi.Completion
               .Request("Today is Monday, tomorrow is", "10 11 12 13 14")
               .WithTemperature(0)
               .SetMaxTokens(3)
               .ExecuteAsStreamAsync())
            {
                results.Add(x);
            }
