# Cost Tracking Feature

## Overview

The PlayFramework now includes **automatic cost tracking** for all OpenAI API requests made during scene execution. This feature provides real-time visibility into the costs associated with each conversation.

## Features

### 1. **Per-Request Cost Tracking**
Each `AiSceneResponse` now includes:
- `Cost`: The cost of the specific OpenAI request (if applicable)
- `TotalCost`: The accumulated cost for the entire conversation

### 2. **Automatic Cost Calculation**
Costs are automatically calculated using:
- OpenAI's `CalculateCost()` method
- Token usage from each request
- Pricing configuration (setup during OpenAI service registration)

### 3. **Cost Accumulation**
The system tracks costs across:
- Scene selection requests
- Tool execution requests
- Planning requests (when deterministic planning is enabled)
- Continuation check requests
- Final response generation

## Usage

### Viewing Costs in Responses

```csharp
await foreach (var response in sceneManager.ExecuteAsync(
    "What's the weather in Milan?",
    settings => settings.WithKey(conversationKey),
    cancellationToken))
{
    if (response.Cost.HasValue)
    {
        Console.WriteLine($"Request cost: {response.Cost.Value:F6}");
    }
    
    if (response.TotalCost.HasValue)
    {
        Console.WriteLine($"Total cost so far: {response.TotalCost.Value:F6}");
    }
}
```

### Final Response with Total Cost

The last response in a conversation includes:
- The total accumulated cost
- A summary message (e.g., "Completed. Total cost: 0.001234")

## Cost Configuration

To enable cost tracking, configure pricing during OpenAI service setup:

```csharp
services.AddOpenAi(settings =>
{
    settings.ApiKey = apiKey;
    
    // Add pricing for models
    settings.PriceBuilder
        .AddModel("gpt-4",
            new OpenAiCost { Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.0000025m },
            new OpenAiCost { Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00000125m },
            new OpenAiCost { Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00001m });
});
```

### Using Default Pricing

Rystem.OpenAI includes default pricing for common models:

```csharp
services.AddOpenAi(settings =>
{
    settings.ApiKey = apiKey;
    // Default pricing is already included for:
    // - GPT-4o
    // - GPT-4o-mini
    // - GPT-3.5-turbo
    // - And more...
});
```

## Cost Breakdown

### Where Costs Are Tracked

1. **Scene Selection** (`RequestAsync`)
   - Initial request to determine which scene to execute
   
2. **Scene Execution** (`GetResponseFromSceneAsync`)
   - Each scene's OpenAI request
   
3. **Tool Calls** (`GetResponseAsync`)
   - Follow-up requests after tool execution
   
4. **Planning** (`DeterministicPlanner`)
   - Plan creation requests
   - Continuation check requests
   
5. **Final Response** (`GenerateFinalResponseAsync`)
   - Final synthesis of gathered information

### Example Cost Flow

```
Request: "What's the weather in Milan and Rome?"

1. Planning request        ? Cost: 0.000123  Total: 0.000123
2. Weather scene (Milan)   ? Cost: 0.000087  Total: 0.000210
3. Weather scene (Rome)    ? Cost: 0.000091  Total: 0.000301
4. Final response          ? Cost: 0.000145  Total: 0.000446
```

## JSON Structure

```json
{
  "rk": "conversation-key",
  "id": "response-id",
  "name": "Weather",
  "message": "The weather in Milan is sunny, 22°C",
  "status": "Running",
  "responseTime": "2024-01-15T10:30:00Z",
  "cost": 0.000087,      // Cost of this specific request
  "totalCost": 0.000210   // Accumulated cost so far
}
```

## Testing

### Unit Test Example

```csharp
[Fact]
public async Task Should_Track_Costs_Correctly()
{
    var responses = new List<AiSceneResponse>();
    
    await foreach (var response in _sceneManager.ExecuteAsync(
        "What's the weather?",
        settings => settings.WithKey(Guid.NewGuid().ToString()),
        CancellationToken.None))
    {
        responses.Add(response);
    }
    
    // Verify costs are being tracked
    var responsesWithCost = responses.Where(r => r.Cost.HasValue).ToList();
    Assert.NotEmpty(responsesWithCost);
    
    // Verify total cost is accumulating
    var lastResponse = responses.Last();
    Assert.True(lastResponse.TotalCost > 0, "Total cost should be greater than 0");
    
    // Verify total equals sum of individual costs
    var sumOfCosts = responses.Where(r => r.Cost.HasValue).Sum(r => r.Cost!.Value);
    Assert.Equal(sumOfCosts, lastResponse.TotalCost.Value);
}
```

## Benefits

1. **Transparency**: Know exactly how much each conversation costs
2. **Monitoring**: Track costs in real-time during execution
3. **Optimization**: Identify expensive operations
4. **Budgeting**: Calculate costs for specific use cases
5. **Debugging**: Understand which requests are being made

## Notes

- Costs are calculated based on the pricing configuration
- If no pricing is configured, costs will be `null`
- Costs are accumulated per conversation (using the conversation key)
- Cached conversations may have lower costs due to cached context
- Cost tracking works with both planning and non-planning modes
- **No currency symbols**: Cost values are numeric only, allowing users to apply their own currency formatting

## Related

- See [Rystem.OpenAI Cost Documentation](../README.md#cost) for more details on pricing
- See [OpenAI Pricing](https://openai.com/pricing) for current rates
