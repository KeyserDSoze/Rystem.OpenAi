# AiResponseStatus.ToolSkipped

## ?? Purpose

The `ToolSkipped` status explicitly marks when a tool or scene is skipped during execution to prevent duplicate operations.

## ?? Status Definition

```csharp
[Flags]
public enum AiResponseStatus
{
    // ... other statuses
    
    /// <summary>
    /// Indicates that a tool/function was skipped because it was already executed in the current scene
    /// </summary>
    ToolSkipped = 2048,
}
```

## ?? When It's Used

### 1. Duplicate Tool Execution Prevention

When the same tool is called multiple times in the same scene:

```csharp
// First call - executes normally
Status: AiResponseStatus.FunctionRequest
FunctionName: "GetWeather"
Message: null

// Second call - skipped
Status: AiResponseStatus.ToolSkipped
FunctionName: "GetWeather"
Message: "Tool 'GetWeather' already executed in scene 'MeteoScene', skipping duplicate execution"
```

### 2. Duplicate Scene Execution Prevention

When a scene is already executed in the current plan:

```csharp
// First execution
Status: AiResponseStatus.SceneRequest
Name: "MeteoScene"
Message: "Executing step 1: Get weather information"

// Duplicate attempt
Status: AiResponseStatus.ToolSkipped
Name: "MeteoScene"
Message: "Scene 'MeteoScene' already executed, skipping step 2"
```

## ?? Benefits

### Before (Using `Running` status)

```csharp
yield return new AiSceneResponse
{
    Status = AiResponseStatus.Running,  // ? Generic, unclear
    Message = "Tool already executed, skipping"
};
```

**Problems**:
- ? Not explicit that tool was skipped
- ? Harder to filter/analyze logs
- ? Mixing running and skipped operations

### After (Using `ToolSkipped` status)

```csharp
yield return new AiSceneResponse
{
    Status = AiResponseStatus.ToolSkipped,  // ? Explicit
    FunctionName = functionName,
    Message = $"Tool '{functionName}' already executed in scene '{sceneName}', skipping duplicate execution"
};
```

**Benefits**:
- ? **Explicit**: Clear that operation was skipped
- ? **Filterable**: Easy to find skipped operations
- ? **Traceable**: Detailed message explains why
- ? **Debuggable**: Can identify duplicate call patterns

## ?? Usage in Code

### Checking for Skipped Tools

```csharp
// Get all skipped tools
var skippedTools = responses
    .Where(r => r.Status == AiResponseStatus.ToolSkipped)
    .ToList();

// Count unique vs duplicate calls
var executedTools = responses
    .Where(r => r.Status == AiResponseStatus.FunctionRequest)
    .Count();

var skippedDuplicates = responses
    .Where(r => r.Status == AiResponseStatus.ToolSkipped)
    .Count();

Console.WriteLine($"Executed: {executedTools}, Skipped duplicates: {skippedDuplicates}");
```

### Filtering Out Skipped Operations

```csharp
// Get only actually executed tools
var executedTools = responses
    .Where(r => 
        r.FunctionName != null && 
        r.Status != AiResponseStatus.ToolSkipped)
    .Select(r => r.FunctionName)
    .ToList();
```

### Validation in Tests

```csharp
[Fact]
public async Task NoDuplicateToolExecutionTest()
{
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

    // Verify skipped tools are properly marked
    var skippedTools = responses
        .Where(r => r.Status == AiResponseStatus.ToolSkipped)
        .ToList();

    foreach (var skipped in skippedTools)
    {
        Assert.NotNull(skipped.FunctionName);
        Assert.NotNull(skipped.Message);
        Assert.Contains("skipping", skipped.Message, StringComparison.OrdinalIgnoreCase);
    }
}
```

## ?? Workflow Example

### Scenario: Weather Request with Duplicate Call

```
User: "Tell me the weather in Milan and then tell me the weather in Milan again"

Plan Created:
  Step 1: Get weather for Milan (MeteoScene)
  Step 2: Get weather for Milan (MeteoScene) // Duplicate!

Execution Flow:
  
  1. [SceneRequest] Executing step 1: Get weather information
     Scene: MeteoScene
  
  2. [FunctionRequest] GetWeather
     Arguments: {"city": "Milan"}
     Response: {"temp": 20, "condition": "Sunny"}
  
  3. [Running] Weather retrieved successfully
  
  4. [SceneRequest] Executing step 2: Get weather information  
     Scene: MeteoScene
  
  5. [ToolSkipped] ?? Tool 'GetWeather' already executed in scene 'MeteoScene'
     Skipping duplicate execution
     Cost: null (no API call made)
  
  6. [FinishedOk] Plan execution completed
```

### Key Points

- ? **Step 2**: Uses cached result from Step 1
- ? **Status**: `ToolSkipped` makes it clear what happened
- ? **Cost**: No additional cost for duplicate
- ? **Message**: Explains why it was skipped

## ?? Status Comparison

| Status | Purpose | Cost | API Call | Message Required |
|--------|---------|------|----------|------------------|
| `FunctionRequest` | Tool executed | ? Yes | ? Yes | Optional |
| `Running` | Processing/responding | Maybe | Maybe | Optional |
| `ToolSkipped` | Duplicate prevented | ? No | ? No | ? Yes |

## ?? Design Rationale

### Why a Separate Status?

1. **Clarity**: Makes log analysis trivial
2. **Metrics**: Easy to track duplicate prevention efficiency
3. **Debugging**: Identify when/why tools are skipped
4. **Billing**: Clear that no cost was incurred
5. **Testing**: Explicit assertions on skip behavior

### Why Not Reuse `Running`?

- `Running` indicates active processing
- `ToolSkipped` indicates no processing (cached/prevented)
- Different semantic meaning
- Different cost implications

### Why Not Create a New Field?

```csharp
// Alternative (NOT chosen):
public bool WasSkipped { get; set; }

// Why status is better:
// ? Part of the standard workflow tracking
// ? Filterable with existing status checks
// ? Consistent with other status values
// ? Supports [Flags] combinations if needed
```

## ?? Monitoring & Analytics

### Duplicate Detection Rate

```csharp
public class ExecutionMetrics
{
    public int TotalToolCalls { get; set; }
    public int ExecutedTools { get; set; }
    public int SkippedDuplicates { get; set; }
    
    public decimal DuplicateRate => 
        TotalToolCalls > 0 
            ? (decimal)SkippedDuplicates / TotalToolCalls 
            : 0;
}

var metrics = new ExecutionMetrics
{
    ExecutedTools = responses.Count(r => r.Status == AiResponseStatus.FunctionRequest),
    SkippedDuplicates = responses.Count(r => r.Status == AiResponseStatus.ToolSkipped),
};
metrics.TotalToolCalls = metrics.ExecutedTools + metrics.SkippedDuplicates;

Console.WriteLine($"Duplicate rate: {metrics.DuplicateRate:P2}");
// Output: "Duplicate rate: 33.33%" (1 duplicate out of 3 calls)
```

### Cost Savings

```csharp
public decimal CalculateCostSavings(List<AiSceneResponse> responses)
{
    var skippedTools = responses
        .Where(r => r.Status == AiResponseStatus.ToolSkipped)
        .ToList();
    
    // Each skipped tool saves an average API call cost
    const decimal avgToolCost = 0.002m; // $0.002 per tool call
    
    return skippedTools.Count * avgToolCost;
}
```

## ?? Test Examples

### Test 1: Verify No Duplicates Executed

```csharp
[Fact]
public async Task NoDuplicateToolExecutionTest()
{
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

    // Get executed tools (excluding skipped)
    var executedTools = responses
        .Where(r => r.FunctionName != null && r.Status != AiResponseStatus.ToolSkipped)
        .Select(r => $"{r.Name}.{r.FunctionName}")
        .ToList();

    // Verify no duplicates in executed tools
    var duplicates = executedTools.GroupBy(x => x)
        .Where(g => g.Count() > 1)
        .Select(g => g.Key)
        .ToList();

    Assert.Empty(duplicates);
}
```

### Test 2: Verify Skipped Tools Are Marked

```csharp
[Fact]
public async Task SkippedToolsAreProperlyMarkedTest()
{
    var responses = await ExecuteTurnAsync(duplicateRequest, conversationKey);

    var skippedTools = responses
        .Where(r => r.Status == AiResponseStatus.ToolSkipped)
        .ToList();

    Assert.NotEmpty(skippedTools); // Should have skipped some

    foreach (var skipped in skippedTools)
    {
        Assert.NotNull(skipped.FunctionName);
        Assert.NotNull(skipped.Message);
        Assert.Contains("already executed", skipped.Message, 
            StringComparison.OrdinalIgnoreCase);
    }
}
```

### Test 3: Verify Cost Not Charged for Skipped

```csharp
[Fact]
public async Task SkippedToolsHaveNoCostTest()
{
    var responses = await ExecuteTurnAsync(userQuestion, conversationKey);

    var skippedTools = responses
        .Where(r => r.Status == AiResponseStatus.ToolSkipped)
        .ToList();

    foreach (var skipped in skippedTools)
    {
        Assert.Null(skipped.Cost); // No cost for skipped tools
    }
}
```

## ?? Migration Guide

### Before

```csharp
if (context.HasExecutedTool(sceneName, functionName))
{
    yield return new AiSceneResponse
    {
        Status = AiResponseStatus.Running,  // Generic
        Message = "Tool already executed, skipping"
    };
    continue;
}
```

### After

```csharp
if (context.HasExecutedTool(sceneName, functionName))
{
    yield return new AiSceneResponse
    {
        Status = AiResponseStatus.ToolSkipped,  // Explicit
        FunctionName = functionName,  // Include details
        Name = sceneName,
        Message = $"Tool '{functionName}' already executed in scene '{sceneName}', skipping duplicate execution"
    };
    continue;
}
```

## ? Best Practices

### 1. Always Include Context in Message

```csharp
// ? Bad
Message = "Skipped"

// ? Good
Message = $"Tool '{functionName}' already executed in scene '{sceneName}', skipping duplicate execution"
```

### 2. Set Cost to null

```csharp
// Skipped tools don't incur costs
Cost = null,
TotalCost = context.TotalCost  // Keep total cost accurate
```

### 3. Include Tool/Scene Names

```csharp
FunctionName = functionName,  // What was skipped
Name = sceneName,             // Where it was skipped
```

### 4. Use in Filters

```csharp
// Exclude skipped from metrics
var actualExecutions = responses
    .Where(r => r.Status != AiResponseStatus.ToolSkipped)
    .ToList();
```

## ?? Summary

The `ToolSkipped` status provides:

? **Explicit** marking of skipped operations  
? **Clear** messaging about why skipped  
? **Easy** filtering and analysis  
? **Accurate** cost tracking (no cost for skipped)  
? **Better** debugging and monitoring  
? **Consistent** with existing status patterns  

It replaces the generic use of `Running` status with a dedicated, semantic value that makes the system behavior transparent and traceable.

---

**Related Documentation**:
- [AiResponseStatus.cs](../src/Rystem.PlayFramework/Models/AiResponseStatus.cs)
- [SceneManager.cs](../src/Rystem.PlayFramework/Manager/SceneManager.cs)
- [COST_TRACKING.md](./COST_TRACKING.md)
