### [What is Rystem?](https://github.com/KeyserDSoze/Rystem)

# Play Framework

## [Docs](https://rystem.net)

## Introduction

The `Rystem.PlayFramework` library enables developers to integrate multi-agent systems and OpenAI into their .NET applications. It provides tools to define and orchestrate complex agent-based scenarios using OpenAI's API. In this guide, we'll cover how to install, configure, and use the `UseAiEndpoints` middleware and PlayFramework scenes within a .NET application.

## Prerequisites

To use this library, you need:

- .NET 9.0 SDK or later
- An OpenAI API key and endpoint
- Access to an HTTP API (if you're using scenes with HTTP clients)
- A database or service to handle identity management (optional)

## Installation

Follow these steps to set up `Rystem.PlayFramework`:

1. Clone or download the repository from [GitHub](https://github.com/KeyserDSoze/Rystem/tree/master/src/PlayFramework).
2. Add the package to your project using the NuGet package manager:
   ```bash
   dotnet add package Rystem.PlayFramework
   ```

## Project Setup

The `.csproj` file is already configured for building the package, including symbol generation and source embedding for debugging purposes. The most important parts are:

- **TargetFramework**: net9.0
- **Package Information**: Contains metadata like authorship, versioning, repository URL, and licensing.

Ensure that your `csproj` file contains the necessary framework and package references.

## Configuration

In your `.NET` application, you will need to configure the services and middlewares for `Rystem.PlayFramework`. This is done primarily within two extension methods:

1. **`AddServices(IServiceCollection services, IConfiguration configuration)`**:
   This method sets up the necessary services, such as OpenAI configuration, HTTP client, identity management, and PlayFramework scenes.

   Example setup:
   ```csharp
   //setup OpenAi client
   services.AddOpenAi(x =>
    {
        x.ApiKey = configuration["OpenAi2:ApiKey"]!;
        x.Azure.ResourceName = configuration["OpenAi2:ResourceName"]!;
        x.Version = "2024-08-01-preview";
        x.DefaultRequestConfiguration.Chat = chatClient =>
        {
            chatClient.WithModel(configuration["OpenAi2:ModelName"]!);
        };
        x.PriceBuilder
        .AddModel(ChatModelName.Gpt4_o,
        new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00000125m },
        new OpenAiCost { Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00001m });
    }, "playframework");
    //setup http client to use during play framework integration to call external services
    services.AddHttpClient("apiDomain", x =>
    {
        x.BaseAddress = new Uri(configuration["Api:Uri"]!);
    });
    //setup for Play Framework
    services.AddPlayFramework(scenes =>
    {
        scenes.Configure(settings =>
        {
            settings.OpenAi.Name = "playframework";
        })
        .AddMainActor((context) => $"Oggi è {DateTime.UtcNow}.", true)
        .AddScene(scene =>
        {
            scene
                .WithName("Weather")
                .WithDescription("Get information about the weather")
                .WithHttpClient("apiDomain")
                .WithOpenAi("playframework")
                .WithApi(pathBuilder =>
                {
                    pathBuilder
                        .Map(new Regex("Country/*"))
                        .Map(new Regex("City/*"))
                        .Map("Weather/");
                })
                    .WithActors(actors =>
                    {
                        actors
                            .AddActor("Nel caso non esistesse la città richiesta potresti aggiungerla con il numero dei suoi abitanti.")
                            .AddActor("Ricordati che va sempre aggiunta anche la nazione, quindi se non c'è la nazione aggiungi anche quella.")
                            .AddActor("Non chiamare alcun meteo prima di assicurarti che tutto sia stato popolato correttamente.")
                            .AddActor<ActorWithDbRequest>();
                    });
        })
        .AddScene(scene =>
        {
            scene
            .WithName("Identity")
            .WithDescription("Get information about the user")
            .WithOpenAi("openai")
            .WithService<IdentityManager>(builder =>
            {
                builder.WithMethod(x => x.GetNameAsync);
            });
        });
    });
   ```

2. **`UseMiddlewares(IApplicationBuilder app)`**:
   This method enables middleware components for routing, authorization, HTTPS redirection, and AI endpoints. It also maps OpenAPI and Scalar API routes.

   Example middleware setup:
   ```csharp
   app.UseRouting();
   app.UseEndpoints(endpoints =>
   {
       endpoints.MapOpenApi();
       endpoints.MapScalarApiReference();
       endpoints.MapControllers();
   });
   app.UseHttpsRedirection();
   app.UseAuthorization();
   app.UseAiEndpoints(); // Enables AI endpoints for your application
   ```

### Configuration Options

- **OpenAI Configuration**: The OpenAI settings are fetched from the `appsettings.json` file using configuration keys such as `OpenAi:ApiKey`, `OpenAi:Endpoint`, and `OpenAi:ModelName`.
- **HTTP Client**: A named HTTP client (`apiDomain`) is used for interacting with external APIs.
- **Scenes**: You can define multiple scenes within PlayFramework, each tied to specific functionality (e.g., `Weather` and `Identity` in this example).

### Example `appsettings.json`

```json
{
  "OpenAi": {
    "ApiKey": "your-openai-api-key",
    "Endpoint": "https://api.openai.com",
    "ModelName": "gpt-4"
  },
  "Api": {
    "Uri": "https://api.example.com/"
  }
}
```

## Usage

### Defining a Scene

A scene in PlayFramework represents a unit of interaction. Each scene can have:
- **Name**: A descriptive name of the scene.
- **Description**: A short description of the scene's purpose.
- **HttpClient**: The HTTP client to use when making API requests within the scene.
- **Actors**: Define behaviors or actions that should be executed as part of the scene.

#### Example Scene: Weather

```csharp
scene.WithName("Weather")
    .WithDescription("Get information about the weather")
    .WithHttpClient("apiDomain")
    .WithOpenAi("openai")
    .WithApi(pathBuilder =>
    {
        pathBuilder.Map(new Regex("Country/*"))
            .Map(new Regex("City/*"))
            .Map("Weather/");
    })
    .WithActors(actors =>
    {
        actors.AddActor("Ensure that the requested city exists, or add it with its population.")
              .AddActor<ActorWithDbRequest>(); 
    });
```

### Example Scene: Identity Management

The `IdentityManager` service is used to manage user identities. It can be configured and used as follows:

```csharp
scene.WithName("Identity")
    .WithDescription("Get information about the user")
    .WithOpenAi("openai")
    .WithService<IdentityManager>(builder =>
    {
        builder.WithMethod(x => x.GetNameAsync);
    });
```

This setup allows you to fetch information about a user, possibly using OpenAI in the process.

## Middleware

The `UseAiEndpoints()` middleware is essential for enabling AI-powered interactions in your API. Once registered, it allows your application to respond to AI-driven requests via OpenAI or custom scenes.

## How AI Endpoints Work

### Endpoint Definition

The primary endpoint that gets mapped by `UseAiEndpoints` is `api/ai/message`. This endpoint receives a query string parameter `m` (which represents the user's message or request) and uses the `ISceneManager` to handle the message. The AI service or scene logic processes the message and returns a result.

#### Example of a GET Request:

```http
GET https://yourdomain.com/api/ai/message?m=What%20is%20the%20weather%20today?
```

This will trigger the scene manager to process the message (e.g., fetching weather information using an AI service or scene logic).

### Endpoint Logic

- **Input**: The endpoint expects a query string parameter `m` (message), which will be processed by the scene manager.
- **Service Dependency**: The `ISceneManager` service is injected and handles the business logic of processing the message.
- **Response**: The result of the scene or AI interaction is returned as a response to the client.

### Authorization

The endpoints can be configured to require authorization either by default (using the `isAuthorized` parameter) or by specifying custom authorization policies.

Example of enforcing authorization:
```csharp
app.UseAiEndpoints("AdminPolicy", "UserPolicy"); // Only users matching these policies can access AI endpoints
```

## AI Endpoint Response (JSON Format)

When a request is made to the AI endpoint (`api/ai/message`), the response is returned in JSON format. The structure of the response is defined by the `AiSceneResponse` class.

### JSON Response Structure

The JSON response will contain the following fields:

- **Id**: A unique identifier for the request, generated as a `GUID`.
- **Name**: The name of the scene that handled the request (optional).
- **FunctionName**: The name of the specific function or action executed by the scene (optional).
- **Message**: The original message or query that was sent to the AI endpoint.
- **Arguments**: Any arguments that were passed to the function (optional).
- **Response**: The result or output generated by the AI or scene.

### Example JSON Response

```json
{
  "Id": "123e4567-e89b-12d3-a456-426614174000",
  "Name": "Weather",
  "FunctionName": "GetWeatherInfo",
  "Message": "What is the weather in New York?",
  "Arguments": "City: New York, Country: USA",
  "Response": "The weather in New York is sunny with a temperature of 25°C."
}
```

### Field Descriptions

- **Id**: A unique request identifier.
- **Name**: Name of the scene that handled the request, e.g., "Weather".
- **FunctionName**: The name of the specific function invoked by the scene, e.g., "GetWeatherInfo".
- **Message**: The original query made by the user, e.g., "What is the weather in New York?".
- **Arguments**: Any relevant parameters that were processed as part of the function, e.g., "City: New York, Country: USA".
- **Response**: The result of the AI's or scene's execution, e.g., "The weather in New York is sunny with a temperature of 25°C.".

This structured response allows clients to interpret and use the output of the AI or multi-agent system in a consistent manner.

## Example of a weather scene

```json
[
    {
        "id": "18b0628a-5202-47cc-813d-099702be3153",
        "name": "Weather",
        "functionName": null,
        "message": "Starting",
        "arguments": null,
        "response": null
    },
    {
        "id": "55f4754f-f372-40a2-a805-ce50c5d1c88d",
        "name": "Weather",
        "functionName": "Country_CityExists",
        "message": null,
        "arguments": "\"{\n  \u0022city\u0022: \u0022milan\u0022\n}\"",
        "response": "false"
    },
    {
        "id": "455e0384-a52e-4ee3-81d1-e0f9478a8484",
        "name": "Weather",
        "functionName": "Country_AddCity",
        "message": null,
        "arguments": "\"{\n  \u0022city\u0022: {\n    \u0022Name\u0022: \u0022milan\u0022,\n    \u0022Country\u0022: \u0022Italy\u0022,\n    \u0022Population\u0022: 1366180\n  }\n}\"",
        "response": "\"true\""
    },
    {
        "id": "aca48a7a-bb8d-4352-9d79-eb0535a2f6ed",
        "name": "Weather",
        "functionName": "Country_Exists",
        "message": null,
        "arguments": "\"{\n  \u0022country\u0022: \u0022Italia\u0022\n}\"",
        "response": "false"
    },
    {
        "id": "cdaf0ae2-3663-4287-94cf-edd1062e6bd8",
        "name": "Weather",
        "functionName": "Country_AddCountry",
        "message": null,
        "arguments": "\"{\n  \u0022country\u0022: {\n    \u0022Name\u0022: \u0022Italy\u0022,\n    \u0022Population\u0022: 60461826\n  }\n}\"",
        "response": "\"true\""
    },
    {
        "id": "0d369878-cc5f-4563-86a9-0714d909b85f",
        "name": "Weather",
        "functionName": "WeatherForecast_Get",
        "message": null,
        "arguments": "\"{\n  \u0022city\u0022: \u0022milan\u0022\n}\"",
        "response": "[{\"date\":\"2024-10-19\",\"temperatureC\":20,\"temperatureF\":67,\"summary\":\"Cool\"},{\"date\":\"2024-10-20\",\"temperatureC\":20,\"temperatureF\":67,\"summary\":\"Hot\"},{\"date\":\"2024-10-21\",\"temperatureC\":20,\"temperatureF\":67,\"summary\":\"Cool\"},{\"date\":\"2024-10-22\",\"temperatureC\":20,\"temperatureF\":67,\"summary\":\"Freezing\"},{\"date\":\"2024-10-23\",\"temperatureC\":20,\"temperatureF\":67,\"summary\":\"Hot\"}]"
    },
    {
        "id": "13192a15-e4c6-44d4-aec9-d38253c2dfe4",
        "name": "Weather",
        "functionName": null,
        "message": "Il tempo oggi a Milano è fresco con una temperatura di 20 gradi Celsius (67 gradi Fahrenheit).",
        "arguments": null,
        "response": null
    }
]
```

## Building and Testing

To build the project, run:
```bash
dotnet build
```

To run tests, add a test project or directly execute requests against your API to validate the scene and actor configurations.

## Conclusion

By following this guide, you can successfully install, configure, and use `Rystem.PlayFramework` with OpenAI integration. The key steps are setting up the services, defining scenes, and utilizing the AI middleware to handle interactions in your .NET application.