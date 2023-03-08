# C#/.NET SDK for accessing the OpenAI GPT-3 API

A simple C# .NET wrapper library to use with [OpenAI](https://openai.com/)'s GPT-3 API.

> This repository is available to transfer to the OpenAI organization if they choose to accept it.


## Help the project

Reach out us on [Discord](https://discord.gg/wUh2fppr)

### Contribute: https://www.buymeacoffee.com/keyserdsoze



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

- [Dependency Injection](#dependency-injection)
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
[📖 Back to summary](#documentation)\
List and describe the various models available in the API. You can refer to the [Models documentation](https://platform.openai.com/docs/models/overview) to understand what models are available and the differences between them.\
You may find more details [here](https://platform.openai.com/docs/api-reference/models),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ModelEndpointTests.cs) samples from unit test.

### List Models
Lists the currently available models, and provides basic information about each one such as the owner and availability.

    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Model.ListAsync();

### Retrieve Models
Retrieves a model instance, providing basic information about the model such as the owner and permissioning.

    IOpenAiApi _openAiApi;
    var result = await _openAiApi.Model.RetrieveAsync(TextModelType.DavinciText3.ToModelId());


## Completions
[📖 Back to summary](#documentation)\
Given a prompt, the model will return one or more predicted completions, and can also return the probabilities of alternative tokens at each position.\
You may find more details [here](https://platform.openai.com/docs/api-reference/completions),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/CompletionEndpointTests.cs) samples from unit test

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

## Chat
[📖 Back to summary](#documentation)\
Given a chat conversation, the model will return a chat completion response.\
You may find more details [here](https://platform.openai.com/docs/api-reference/chat),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ChatEndpointTests.cs) samples from unit test.

    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Chat
            .Request(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
            .WithModel(ChatModelType.Gpt35Turbo0301)
            .WithTemperature(1)
            .ExecuteAsync();

### Chat Streaming 

    IOpenAiApi _openAiApi;
    var results = new List<ChatResult>();
        await foreach (var x in _openAiApi.Chat
            .Request(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
            .WithModel(ChatModelType.Gpt35Turbo)
            .WithTemperature(1)
            .ExecuteAsStreamAsync())
            {
                results.Add(x);
            }

## Edits
Given a prompt and an instruction, the model will return an edited version of the prompt.
You may find more details [here](https://platform.openai.com/docs/api-reference/edits),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/EditEndpointTests.cs) samples from unit test.

    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Edit
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

    IOpenAiApi _openAiApi;
    var response = await _openAiApi.Image
        .Generate("A cute baby sea otter")
        .WithSize(ImageSize.Small)
        .ExecuteAsync();    

Download directly and save as stream

    IOpenAiApi _openAiApi;
    var streams = new List<Stream>();
    await foreach (var image in _openAiApi.Image
        .Generate("A cute baby sea otter")
        .WithSize(ImageSize.Small)
        .DownloadAsync())
    {
        streams.Add(image);
    }

### Create Image Edit
Creates an edited or extended image given an original image and a prompt.

    IOpenAiApi _openAiApi;
    var response = await _openAiApi.Image
        .Generate("A cute baby sea otter wearing a beret")
        .EditAndTrasformInPng(editableFile, "otter.png")
        .WithSize(ImageSize.Small)
        .WithNumberOfResults(2)
        .ExecuteAsync();

### Create Image Variation
Creates a variation of a given image.

    IOpenAiApi _openAiApi;
    var response = await _openAiApi.Image
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

    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Embedding
        .Request("A test text for embedding")
        .ExecuteAsync();

## Audio
[📖 Back to summary](#documentation)\
You may find more details [here](https://platform.openai.com/docs/api-reference/audio),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/AudioEndpointTests.cs) samples from unit test.

### Create Transcription
Transcribes audio into the input language.
    
    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Audio
        .Request(editableFile, "default.mp3")
        .TranscriptAsync();

### Create Translation
Translates audio into into English.

    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Audio
        .Request(editableFile, "default.mp3")
        .TranslateAsync();

## File
[📖 Back to summary](#documentation)\
Files are used to upload documents that can be used with features like Fine-tuning.\
You may find more details [here](https://platform.openai.com/docs/api-reference/files),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/FileEndpointTests.cs) samples from unit test.

### List files
Returns a list of files that belong to the user's organization.
    
      IOpenAiApi _openAiApi;
      var results = await _openAiApi.File
                .AllAsync();

### Upload file
Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.

    IOpenAiApi _openAiApi;
    var uploadResult = await _openAiApi.File
            .UploadFileAsync(editableFile, name);

### Delete file
Delete a file.
    
    IOpenAiApi _openAiApi;
    var deleteResult = await _openAiApi.File
            .DeleteAsync(uploadResult.Id);

### Retrieve file
Returns information about a specific file.

    IOpenAiApi _openAiApi;
    var retrieve = await _openAiApi.File
            .RetrieveAsync(uploadResult.Id);

### Retrieve file content
Returns the contents of the specified file

    IOpenAiApi _openAiApi;
    var contentRetrieve = await _openAiApi.File
            .RetrieveFileContentAsStringAsync(uploadResult.Id);

## Fine-Tunes
[📖 Back to summary](#documentation)\
Manage fine-tuning jobs to tailor a model to your specific training data.\
You may find more details [here](https://platform.openai.com/docs/api-reference/fine-tunes),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/FineTuneEndpointTests.cs) samples from unit test.

### Create fine-tune
Creates a job that fine-tunes a specified model from a given dataset.
Response includes details of the enqueued job including job status and the name of the fine-tuned models once complete.

    IOpenAiApi _openAiApi;
    var createResult = await _openAiApi.FineTune
                                .Create(fileId)
                                .ExecuteAsync();

### List fine-tunes
List your organization's fine-tuning jobs
    
    IOpenAiApi _openAiApi;
    var allFineTunes = await _openAiApi.FineTune
                        .ListAsync();

### Retrieve fine-tune
Gets info about the fine-tune job.

    IOpenAiApi _openAiApi;
    var retrieveFineTune = await _openAiApi.FineTune
                            .RetrieveAsync(fineTuneId);

### Cancel fine-tune
Immediately cancel a fine-tune job.

    IOpenAiApi _openAiApi;
    var cancelResult = await _openAiApi.FineTune
                            .CancelAsync(fineTuneId);

### List fine-tune events
Get fine-grained status updates for a fine-tune job.

    IOpenAiApi _openAiApi;
    var events = await _openAiApi.FineTune
                        .ListEventsAsync(fineTuneId);

### List fine-tune events as stream
Get fine-grained status updates for a fine-tune job.

    IOpenAiApi _openAiApi;
    var events = await _openAiApi.FineTune
                        .ListEventsAsStreamAsync(fineTuneId);

### Delete fine-tune model
Delete a fine-tuned model. You must have the Owner role in your organization.

    IOpenAiApi _openAiApi;
    var deleteResult = await _openAiApi.File
        .DeleteAsync(fileId);

## Moderations
[📖 Back to summary](#documentation)\
Given a input text, outputs if the model classifies it as violating OpenAI's content policy.\
You may find more details [here](https://platform.openai.com/docs/api-reference/moderations),
and [here](https://github.com/KeyserDSoze/Rystem.OpenAi/blob/master/src/Rystem.OpenAi.Test/Tests/ModerationEndpointTests.cs) samples from unit test.

### Create moderation
Classifies if text violates OpenAI's Content Policy
    
    IOpenAiApi _openAiApi;
    var results = await _openAiApi.Moderation
            .Create("I want to kill them.")
            .WithModel(ModerationModelType.TextModerationStable)
            .ExecuteAsync();