# AiResponseStatus.Summarizing Usage

## ?? Purpose

The `Summarizing` status indicates when the system is condensing conversation history to prevent context overflow and reduce costs.

## ?? When It's Used

### Automatic Summarization

When conversation history exceeds configured thresholds:

```csharp
// Configuration in Startup.cs
settings.Summarization.Enabled = true;
settings.Summarization.ResponseThreshold = 20;    // Summarize after 20 responses
settings.Summarization.CharacterThreshold = 3000; // Or 3000 characters
```

### Response Flow

```
User: [Turn 21 - exceeds threshold]
    ?
[Summarizing] Summarizing 20 previous responses...
    ?
[Request] Processing new request...
    ?
... normal execution ...
```

## ?? Implementation

### In SceneManager.ExecuteAsync

```csharp
// Check if summarization will happen BEFORE GetChatClientAsync
if (requestSettings.Key != null && !requestSettings.KeyHasStartedAsNull && 
    !requestSettings.CacheIsAvoidable && _cacheService != null && _summarizer != null)
{
    if (!_cacheValue.ContainsKey(requestSettings.Key))
    {
        var oldValue = await _cacheService.GetAsync(requestSettings.Key, cancellationToken);
        
        if (oldValue != null && _summarizer.ShouldSummarize(oldValue))
        {
            // Emit Summarizing status
            yield return new AiSceneResponse
            {
                RequestKey = requestSettings.Key!,
                Name = "Summarization",
                Message = $"Summarizing {oldValue.Count} previous responses...",
                ResponseTime = DateTime.UtcNow,
                Status = AiResponseStatus.Summarizing,  // ? Used here!
                Cost = null,
                TotalCost = 0
            };
        }
    }
}

// Then continue with GetChatClientAsync which performs actual summarization
var chatClient = await GetChatClientAsync(message, requestSettings, cancellationToken);
```

### In DefaultSummarizer

```csharp
public bool ShouldSummarize(List<AiSceneResponse> responses)
{
    var responseThreshold = _settings?.Summarization.ResponseThreshold ?? 50;
    var characterThreshold = _settings?.Summarization.CharacterThreshold ?? 10000;

    // Check response count
    if (responses.Count >= responseThreshold)
        return true;

    // Check total characters
    var totalCharacters = responses.Sum(r =>
        (r.Message?.Length ?? 0) +
        (r.Response?.Length ?? 0) +
        (r.FunctionName?.Length ?? 0));

    return totalCharacters >= characterThreshold;
}
```

## ?? Benefits

### Before (No Summarizing Status)

```
? User doesn't know summarization is happening
? Looks like a delay or hang
? Can't track summarization in logs
? Testing is harder
```

### After (With Summarizing Status)

```
? [Summarizing] Summarizing 20 previous responses...
? Clear feedback to user
? Visible in logs and metrics
? Testable behavior
? Cost tracking accurate (null cost)
```

## ?? Example Workflow

### Scenario: Long Conversation

```
Turn 1-19: Normal conversation
  [Request] User: What's the weather?
  [SceneRequest] MeteoScene
  [FunctionRequest] GetWeather
  [Running] Weather data retrieved
  [FinishedOk] Here's the weather...

Turn 20: Triggers summarization
  [Summarizing] Summarizing 19 previous responses...  ?? NEW!
  [Request] User: And tomorrow?
  [SceneRequest] MeteoScene
  [FunctionRequest] GetWeather
  [Running] Weather data retrieved
  [FinishedOk] Tomorrow's weather...

Turn 21+: Uses summary instead of full history
  [Request] User: Thanks!
  [FinishedNoTool] You're welcome!
```

## ?? Testing

### Test for Summarization Status

```csharp
[Fact]
public async Task SummarizationThresholdTest()
{
    var conversationKey = Guid.NewGuid().ToString();

    // Generate many turns to exceed threshold
    for (int i = 1; i <= 15; i++)
    {
        await ExecuteTurnAsync($"Question {i}", conversationKey);
    }

    // Next turn should trigger summarization
    var responses = await ExecuteTurnAsync("Final question", conversationKey);

    // Verify Summarizing status was used
    var summarizingResponses = responses
        .Where(r => r.Status == AiResponseStatus.Summarizing)
        .ToList();

    if (summarizingResponses.Any())
    {
        Assert.NotEmpty(summarizingResponses);
        
        foreach (var summary in summarizingResponses)
        {
            Assert.NotNull(summary.Message);
            Assert.Contains("summarizing", summary.Message, 
                StringComparison.OrdinalIgnoreCase);
            Assert.Null(summary.Cost); // No cost for summarization
        }
    }
}
```

### Filtering Summarizing Responses

```csharp
// Get only actual execution responses (exclude metadata)
var executionResponses = responses
    .Where(r => r.Status != AiResponseStatus.Summarizing &&
                r.Status != AiResponseStatus.Planning &&
                r.Status != AiResponseStatus.ToolSkipped)
    .ToList();
```

## ?? Monitoring

### Track Summarization Frequency

```csharp
public class SummarizationMetrics
{
    public int TotalConversations { get; set; }
    public int SummarizationCount { get; set; }
    public int AverageResponsesBeforeSummary { get; set; }
    
    public decimal SummarizationRate => 
        TotalConversations > 0 
            ? (decimal)SummarizationCount / TotalConversations 
            : 0;
}

var metrics = new SummarizationMetrics
{
    TotalConversations = allResponses.Select(r => r.RequestKey).Distinct().Count(),
    SummarizationCount = allResponses.Count(r => r.Status == AiResponseStatus.Summarizing)
};

Console.WriteLine($"Summarization rate: {metrics.SummarizationRate:P2}");
```

### Cost Savings

```csharp
// Summarization reduces token usage significantly
public decimal CalculateCostSavings(List<AiSceneResponse> responses)
{
    // Without summarization: Send all N responses every time
    // With summarization: Send 1 condensed summary
    
    var summarizations = responses.Count(r => r.Status == AiResponseStatus.Summarizing);
    
    // Average cost per full context vs summary
    const decimal fullContextCost = 0.010m;  // $0.01 per full context
    const decimal summaryCost = 0.002m;      // $0.002 per summary
    
    var savings = summarizations * (fullContextCost - summaryCost);
    return savings;
}
```

## ?? Configuration

### Tuning Thresholds

```csharp
services.AddPlayFramework(scenes =>
{
    scenes.Configure(settings =>
    {
        settings.Summarization.Enabled = true;
        
        // For testing (low thresholds)
        settings.Summarization.ResponseThreshold = 20;
        settings.Summarization.CharacterThreshold = 3000;
        
        // For production (higher thresholds)
        // settings.Summarization.ResponseThreshold = 50;
        // settings.Summarization.CharacterThreshold = 10000;
    });
});
```

### Disable Summarization

```csharp
settings.Summarization.Enabled = false;
// No Summarizing status will ever be emitted
```

## ?? Status Comparison

| Status | Purpose | Cost | When Used | User Visible |
|--------|---------|------|-----------|--------------|
| `Summarizing` | Condensing history | ? No | Start of turn after threshold | ? Yes |
| `Planning` | Creating execution plan | Maybe | Start of turn if enabled | ? Yes |
| `ToolSkipped` | Preventing duplicates | ? No | During execution | ?? Optional |
| `FunctionRequest` | Executing tool | ? Yes | During execution | ? Yes |

## ?? UI Representation

### Console Output

```csharp
switch (response.Status)
{
    case AiResponseStatus.Summarizing:
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[SUMMARY] {response.Message}");
        Console.ResetColor();
        break;
        
    case AiResponseStatus.Planning:
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[PLANNING] {response.Message}");
        Console.ResetColor();
        break;
        
    case AiResponseStatus.Request:
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"User: {response.Message}");
        Console.ResetColor();
        break;
}
```

### Web UI

```typescript
function renderResponse(response: AiSceneResponse) {
  switch (response.status) {
    case 'Summarizing':
      return (
        <div className="status-summarizing">
          <SpinnerIcon />
          {response.message}
        </div>
      );
      
    case 'Planning':
      return (
        <div className="status-planning">
          <PlanningIcon />
          {response.message}
        </div>
      );
      
    // ... other statuses
  }
}
```

## ? Best Practices

### 1. Always Set Cost to null

```csharp
Status = AiResponseStatus.Summarizing,
Cost = null,  // Summarization is internal, no API cost
TotalCost = 0 // Or context.TotalCost if mid-conversation
```

### 2. Include Response Count in Message

```csharp
Message = $"Summarizing {oldValue.Count} previous responses..."
```

### 3. Emit Before Actual Work

```csharp
// ? Good: Emit status BEFORE summarization
yield return new AiSceneResponse { Status = AiResponseStatus.Summarizing };
var summary = await _summarizer.SummarizeAsync(...);

// ? Bad: Emit AFTER summarization (user sees delay first)
var summary = await _summarizer.SummarizeAsync(...);
yield return new AiSceneResponse { Status = AiResponseStatus.Summarizing };
```

### 4. Use in Filters

```csharp
// Get only content responses (exclude metadata)
var contentOnly = responses
    .Where(r => 
        r.Status != AiResponseStatus.Summarizing &&
        r.Status != AiResponseStatus.Planning)
    .ToList();
```

## ?? Related

- **[AiResponseStatus.Planning](./PLANNING_STATUS.md)** - Planning phase status
- **[AiResponseStatus.ToolSkipped](./TOOL_SKIPPED_STATUS.md)** - Duplicate prevention status
- **[DefaultSummarizer.cs](../src/Rystem.PlayFramework/DefaultServices/Summarizer/DefaultSummarizer.cs)** - Implementation
- **[SceneManagerSettings.cs](../src/Rystem.PlayFramework/Models/SceneManager/SceneManagerSettings.cs)** - Configuration

## ?? Summary

The `Summarizing` status:

? **Explicit** feedback when condensing history  
? **Transparent** operation visibility  
? **Testable** behavior  
? **Cost-aware** (null cost)  
? **User-friendly** (explains delays)  
? **Monitorable** (track frequency)  

It was previously defined but **never used** - now it's properly integrated into the execution flow! ??

---

**Status**: ? Now implemented and used  
**Added in**: This update  
**Previously**: Defined but unused
